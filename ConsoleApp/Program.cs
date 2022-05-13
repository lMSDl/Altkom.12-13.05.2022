using DAL;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var contextOptions = new DbContextOptionsBuilder<Context>()
                .LogTo(x => Debug.WriteLine(x))
                .UseSqlServer(@"Server=(local);Database=EFC;Integrated security=true");
            await ChangeTracking(contextOptions);
            await ConcurrencyToken(contextOptions);
            await Transactions(contextOptions);
            await LoadRelatedData(contextOptions);
            CompileQuery(contextOptions);
            await GlobalFilters(contextOptions);


        }

        private static async Task GlobalFilters(DbContextOptionsBuilder<Context> contextOptions)
        {
            using var context = new Context(contextOptions.Options);
            var orders = await context.Set<Order>().Where(x => x.Id % 2 == 0).ToListAsync();
            foreach (var order in orders)
            {
                order.IsDeleted = true;
            }
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            orders = await context.Set<Order>()/*.Where(x => !x.IsDeleted)*/.ToListAsync();
            context.ChangeTracker.Clear();

            orders = await context.Set<Product>().SelectMany(x => x.Orders).Distinct().ToListAsync();
        }

        private static void CompileQuery(DbContextOptionsBuilder<Context> contextOptions)
        {
            using var context = new Context(contextOptions.Options);

            var orders = Context.GetOrdersRange(context, DateTime.Now.AddDays(-1), DateTime.Now);
        }

        private static async Task LoadRelatedData(DbContextOptionsBuilder<Context> contextOptions)
        {
            using (var context = new Context(contextOptions.Options))
            {
                //Lazy loading z wykorzystaniem ILazyLoader
                var order = await context.Set<Order>().FirstAsync();

                context.ChangeTracker.Clear();

                //Eager loading
                order = await context.Set<Order>().Include(x => x.Products).ThenInclude(x => x.Orders).Where(x => x.Products.Count > 1).FirstAsync();

                context.ChangeTracker.Clear();
                order = await context.Set<Order>().FirstAsync();

                //Explicit loading
                //await context.Set<Product>().LoadAsync();
                await context.Entry(order).Collection(x => x.Products).LoadAsync();
                //await context.Entry(order).Reference(x => x.<>).LoadAsync();
            }



            //LazyLoading z wykorzystanie proxy
            //var contextOptionsProxies = new DbContextOptionsBuilder<Context>()
            //    .LogTo(x => Debug.WriteLine(x))
            //    .UseSqlServer(@"Server=(local);Database=EFC;Integrated security=true")
            //    .UseLazyLoadingProxies();

            //Order myOrder = null;
            //using (var context = new Context(contextOptionsProxies.Options))
            //{
            //    myOrder = await context.Set<Order>().FirstAsync();
            //}
        }

        private static async Task Transactions(DbContextOptionsBuilder<Context> contextOptions)
        {
            using var context = new Context(contextOptions.Options);

            var products = Enumerable.Range(100, 4).Select(x => new Product { Name = $"Product {x}", Price = 2.33f * x }).ToList();

            using (var transaction = await context.Database.BeginTransactionAsync())
            {
                var counter = 0;
                try
                {
                    context.RandomFail = true;

                    foreach (var item in products)
                    {
                        counter++;
                        await context.AddAsync(item);
                        await context.SaveChangesAsync();

                        await transaction.CreateSavepointAsync($"Product{counter}");
                    }

                    await transaction.CommitAsync();
                }
                catch
                {
                    //await transaction.RollbackToSavepointAsync($"Product{counter-1}");
                    if (counter > 2)
                    {
                        await transaction.RollbackToSavepointAsync($"Product2");
                        await transaction.CommitAsync();
                    }
                }


                context.RandomFail = false;
                //await transaction.RollbackAsync();
            }
        }

        private static async Task ConcurrencyToken(DbContextOptionsBuilder<Context> contextOptions)
        {
            using var context = new Context(contextOptions.Options);

            var product = await context.Set<Product>().FirstAsync();

            product.Price = 12.11f;

            var saved = false;
            while (!saved)
            {
                try
                {
                    await context.SaveChangesAsync();
                    saved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        //wartości jakie my chcemy wprowadzić do encji (stan jaki chcemy zapisać)
                        var currentValues = entry.CurrentValues;
                        //pobieramy wartości, które są aktualnie w bazie danych
                        var databaseValues = entry.GetDatabaseValues();

                        if (entry.Entity is Product)
                        {
                            var nameProperty = currentValues.Properties.SingleOrDefault(x => x.Name == nameof(Product.Name));
                            var currentNamePropertyValue = currentValues[nameProperty];
                            var databaseNamePropertyValue = databaseValues[nameProperty];
                            //jeśli nazwa produktu uległa zmianie, to w bazie danych nie wprowadzany zmian
                            //(przepisujemy do currentValues wartości z bazy danych (databaseValues))
                            if (!currentNamePropertyValue.Equals(databaseNamePropertyValue))
                            {
                                foreach (var property in currentValues.Properties)
                                {
                                    currentValues[property] = databaseValues[property];
                                }
                            }
                        }

                        entry.OriginalValues.SetValues(databaseValues);
                    }

                }
            }

        }

        private static async Task ChangeTracking(DbContextOptionsBuilder<Context> contextOptions)
        {
            var products = Enumerable.Range(1, 10).Select(x => new Product { Name = $"Product {x}", Price = 2.33f * x }).ToList();
            using (var context = new Context(contextOptions.Options))
            {

                context.Database.EnsureDeleted();
                context.Database.Migrate();

                for (int i = 0; i < 10; i++)
                {
                    var order = new Order() { DateTime = DateTime.Now.AddMinutes(-i * 452) };
                    order.OrderType = (OrderType)(i % 3);
                    var random = new Random(i);
                    var randomValue1 = random.Next(0, 9);
                    var randomValue2 = random.Next(0, 9);
                    order.Products = Enumerable.Range(Math.Min(randomValue1, randomValue2), Math.Max(randomValue1, randomValue2) - Math.Min(randomValue1, randomValue2)).Select(x => products[x]).ToList();

                    Console.WriteLine($"Stan zamówienia przed dodaniem do contekstu: {context.Entry(order).State}");
                    Console.WriteLine($"Stan jednego z produktów przed dodaniem do contekstu: {context.Entry(order.Products.First()).State}");

                    await context.AddAsync(order);

                    Console.WriteLine($"Stan zamówienia po dodaniu do contekstu: {context.Entry(order).State}");
                    Console.WriteLine($"Stan jednego z produktów po dodaniu do contekstu: {context.Entry(order.Products.First()).State}");
                }

                ShowChangeTrackerDebugView(context);

                context.SaveChanges();

                context.ChangeTracker.Entries().Where(x => x.Entity is Order).ToList()
                    .ForEach(x =>
                    {
                        Console.WriteLine($"Stan zamówienia po zapisie do bazy: {x.State}");
                        Console.WriteLine($"Stan jednego z produktów po zapisie do bazy: {context.Entry(((Order)x.Entity).Products.First()).State}");
                    });


                var product = products.First();

                context.ChangeTracker.AutoDetectChangesEnabled = false;

                //context.Entry(product).State = EntityState.Detached;
                product.Price = 19.99f;
                Console.WriteLine($"Stan jednego z produktów po edycji: {context.Entry(product).State}");

                Console.WriteLine($"Stan modyfikacji ceny z produktów po edycji: {context.Entry(product).Property(x => x.Price).IsModified}");
                Console.WriteLine($"Stan modyfikacji nazy z produktów po edycji: {context.Entry(product).Property(x => x.Name).IsModified}");

                //context.Entry(product).State = EntityState.Unchanged;
                //context.Entry(product).Property(x => x.Price).IsModified = false;

                ShowChangeTrackerDebugView(context);

                try
                {
                    context.SaveChanges();
                }
                finally
                {
                    context.ChangeTracker.AutoDetectChangesEnabled = true;
                }
            }
        }

        private static void ShowChangeTrackerDebugView(Context context)
        {
            context.ChangeTracker.DetectChanges();
            Console.WriteLine("=====");
            Console.WriteLine();
            Console.WriteLine(context.ChangeTracker.DebugView.ShortView);
            Console.WriteLine("-----");
            Console.WriteLine();
            Console.WriteLine(context.ChangeTracker.DebugView.LongView);
            Console.WriteLine("=====");
        }
    }
}
