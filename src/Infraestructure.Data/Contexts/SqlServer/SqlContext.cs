using Domain.Entities;
using Infraestructure.Data.Contexts.SqlServer.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infraestructure.Data.Contexts.SqlServer
{
    sealed class SqlContext: DbContext
    {
        public DbSet<Order> Order { get; set; }

        public SqlContext(DbContextOptions contextOptions) : base(contextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
        }
    }
}
