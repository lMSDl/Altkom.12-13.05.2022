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
    public class OrderConfiguration : EntityConfiguration<Order>
    {
        public override void Configure(EntityTypeBuilder<Order> builder)
        {
            base.Configure(builder);
            builder.HasQueryFilter(x => !x.IsDeleted);
            builder.Property(x => x.OrderType).HasDefaultValue(OrderType.Unknown)
                .HasConversion<string>();
                /*.HasConversion(x => x.ToString(),
                               x => (OrderType)Enum.Parse(typeof(OrderType), x));*/
                //.HasConversion(new Microsoft.EntityFrameworkCore.Storage.ValueConversion.EnumToStringConverter<OrderType>());
        }
    }
}
