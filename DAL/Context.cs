using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DAL
{
    public class Context : DbContext
    {
        public Context()
        {
        }
        public Context([NotNullAttribute] DbContextOptions options) : base(options)
        {
        }

        public static Func<Context, DateTime, DateTime, IEnumerable<Order>> GetOrdersRange { get; } =
            EF.CompileQuery((Context context, DateTime from, DateTime to) => 
                context.Set<Order>().AsNoTracking().Include(x => x.Products)
                       .Where(x => x.DateTime >= from).Where(x => x.DateTime <= to));

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if(!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);

            modelBuilder.Model.GetEntityTypes().SelectMany(x => x.GetProperties())
                .Where(x => x.PropertyInfo?.Name == "Key").ToList()
                .ForEach(property => {
                    property.IsNullable = false;
                    property.DeclaringEntityType.SetPrimaryKey(property); });

            //modelBuilder.Model.GetEntityTypes().SelectMany(x => x.GetProperties())
            //    .Where(x => x.PropertyInfo?.PropertyType == typeof(DateTime)).ToList()
            //    .ForEach(property => property.SetColumnType("datetime"));

            modelBuilder.HasSequence<int>("ProductPrice", "sequences")
                .StartsAt(100)
                .HasMax(999)
                .HasMin(10)
                .IncrementsBy(33)
                .IsCyclic();

        }

        public bool RandomFail { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            if(RandomFail)
            {
                if(new Random().Next(1, 5) == 1)
                {
                    throw new Exception();
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
