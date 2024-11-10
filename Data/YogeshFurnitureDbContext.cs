using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using YogeshFurnitureAPI.Model;
using YogeshFurnitureAPI.Model.Account;

namespace YogeshFurnitureAPI.Data
{


    public class YogeshFurnitureDbContext : IdentityDbContext<YogeshFurnitureUsers>
    {
        public YogeshFurnitureDbContext(DbContextOptions<YogeshFurnitureDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<YogeshFurnitureUsers>(entity =>
            {
                entity.ToTable("YogeshFurnitureUsers");

                // Profile information
                entity.Property(e => e.Name).HasMaxLength(100);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.Pincode).HasMaxLength(10);
                entity.Property(e => e.Street).HasMaxLength(200);
                entity.Property(e => e.Landmark).HasMaxLength(200);
                entity.Property(e => e.City).HasMaxLength(100);
                entity.Property(e => e.State).HasMaxLength(100);
                entity.Property(e => e.ProfileImage).HasMaxLength(500);

                // Identity properties
                entity.Property(e => e.Email).HasMaxLength(100).IsRequired(false); ;
                entity.Property(e => e.PhoneNumber).HasMaxLength(15).IsRequired(false); ;

                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasIndex(u => u.PhoneNumber).IsUnique();

                // Audit information
                entity.Property(e => e.CreatedDate).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.LastModifyDate).IsConcurrencyToken();
            });

            // Product and Category configurations
            builder.Entity<Product>(entity =>
            {
                entity.HasKey(p => p.ProductId);
                entity.Property(p => p.ProductName).IsRequired().HasMaxLength(100);
                entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
                entity.Property(p => p.Description).HasMaxLength(500);
                entity.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId);
            });

            builder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.CategoryId);
                entity.Property(c => c.CategoryName).IsRequired().HasMaxLength(50);
            });
        }
    }
}
    
