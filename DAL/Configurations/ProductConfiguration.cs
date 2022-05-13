using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Configurations
{
    public class ProductConfiguration : EntityConfiguration<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);

            //Token współbieżności
            //builder.Property(x => x.Name).IsConcurrencyToken();

            //Znacznik czasowy (token)
            builder.Property(x => x.Timestamp).IsRowVersion();

            builder.HasIndex(x => x.Name).IsUnique();

            //builder.Property(x => x.Price).HasDefaultValue(0.01);
            builder.Property(x => x.Price).HasDefaultValueSql("NEXT VALUE FOR sequences.ProductPrice");


            builder.Property(x => x.Description).HasComputedColumnSql("[Name] + ' ' + STR([Price]) + 'zł'", stored: true);

        }
    }
}
