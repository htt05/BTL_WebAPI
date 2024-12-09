using back_end.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace back_end.Models.BusinessModels
{
    public class back_endContext : DbContext
    {
        public back_endContext()
        {

        }
        public back_endContext(DbContextOptions<back_endContext> options) : base(options)
        {

        }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImg> ProductImgs { get; set; }
    }
}
