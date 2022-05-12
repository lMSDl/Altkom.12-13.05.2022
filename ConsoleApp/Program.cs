using DAL;
using Microsoft.EntityFrameworkCore;
using System;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var contextOptions = new DbContextOptionsBuilder<Context>()
                .UseSqlServer(@"Server=(local);Database=EFC;Integrated security=true");
            using var context = new Context(contextOptions.Options);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();


        }
    }
}
