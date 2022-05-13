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
    public abstract class EntityConfiguration<T> : IEntityTypeConfiguration<T> where T : Entity
    {
        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            //builder.Property(x => x.Created).HasDefaultValueSql("getdate()");
            //konfiguracja shadow property
            builder.Property<DateTime>("Created").HasDefaultValueSql("getdate()"); 
            builder.Property(x => x.Modified).ValueGeneratedOnUpdate();
        }
    }
}
