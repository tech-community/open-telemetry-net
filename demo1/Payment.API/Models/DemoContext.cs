using System;
using Microsoft.EntityFrameworkCore;

namespace Payment.API.Models
{
    public class DemoContext : DbContext
    {
        public DemoContext(DbContextOptions<DemoContext> options)
            : base(options)
        { }
        public DbSet<Order> Orders { get; set; }

        public DbSet<OrderDetail> OrderDetail { get; set; }
    }
}