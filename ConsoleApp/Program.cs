using DAL;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var contextOptions = new DbContextOptionsBuilder<Context>()
                .UseSqlServer(@"Server=(local);Database=EFC;Integrated security=true");


            var products = Enumerable.Range(1, 10).Select(x => new Product { Name = $"Product {x}", Price = 2.33f * x }).ToList();


            using (var context = new Context(contextOptions.Options))
            {

                context.Database.EnsureDeleted();
                context.Database.Migrate();

                for (int i = 0; i < 10; i++)
                {
                    var order = new Order() { DateTime = DateTime.Now.AddMinutes(-i * 452) };

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
