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
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            //Token współbieżności
            //builder.Property(x => x.Name).IsConcurrencyToken();

            //Znacznik czasowy (token)
            builder.Property(x => x.Timestamp).IsRowVersion();
        }
    }
}
