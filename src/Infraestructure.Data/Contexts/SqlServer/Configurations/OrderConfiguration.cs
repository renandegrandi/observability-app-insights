using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infraestructure.Data.Contexts.SqlServer.Configurations
{
    sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder
                .HasKey((b) => b.Id);

            builder
                .Property((b) => b.Id)
                .ValueGeneratedNever();

            builder
                .Property((b) => b.Total)
                .HasColumnType("decimal");
        }
    }
}
