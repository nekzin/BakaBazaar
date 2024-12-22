using BBC.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BBC.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Delivery> Deliveries { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Настройка типов данных для SQLite
            modelBuilder.Entity<IdentityUser>(entity =>
            {
                entity.Property(m => m.Id).HasColumnType("TEXT");
                entity.Property(m => m.UserName).HasColumnType("TEXT").HasMaxLength(256);
                entity.Property(m => m.NormalizedUserName).HasColumnType("TEXT").HasMaxLength(256);
                entity.Property(m => m.Email).HasColumnType("TEXT").HasMaxLength(256);
                entity.Property(m => m.NormalizedEmail).HasColumnType("TEXT").HasMaxLength(256);
                entity.Property(m => m.PasswordHash).HasColumnType("TEXT");
                entity.Property(m => m.SecurityStamp).HasColumnType("TEXT");
                entity.Property(m => m.ConcurrencyStamp).HasColumnType("TEXT");
                entity.Property(m => m.PhoneNumber).HasColumnType("TEXT");
            });

            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.Property(m => m.Id).HasColumnType("TEXT");
                entity.Property(m => m.Name).HasColumnType("TEXT").HasMaxLength(256);
                entity.Property(m => m.NormalizedName).HasColumnType("TEXT").HasMaxLength(256);
                entity.Property(m => m.ConcurrencyStamp).HasColumnType("TEXT");
            });

            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.Property(m => m.UserId).HasColumnType("TEXT");
                entity.Property(m => m.LoginProvider).HasColumnType("TEXT").HasMaxLength(128);
                entity.Property(m => m.Name).HasColumnType("TEXT").HasMaxLength(128);
                entity.Property(m => m.Value).HasColumnType("TEXT");
            });

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.Property(m => m.LoginProvider).HasColumnType("TEXT").HasMaxLength(128);
                entity.Property(m => m.ProviderKey).HasColumnType("TEXT").HasMaxLength(128);
                entity.Property(m => m.ProviderDisplayName).HasColumnType("TEXT");
                entity.Property(m => m.UserId).HasColumnType("TEXT");
            });

            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.Property(m => m.UserId).HasColumnType("TEXT");
                entity.Property(m => m.RoleId).HasColumnType("TEXT");
            });

            modelBuilder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.Property(m => m.Id).HasColumnType("INTEGER");
                entity.Property(m => m.UserId).HasColumnType("TEXT");
                entity.Property(m => m.ClaimType).HasColumnType("TEXT");
                entity.Property(m => m.ClaimValue).HasColumnType("TEXT");
            });

            modelBuilder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.Property(m => m.Id).HasColumnType("INTEGER");
                entity.Property(m => m.RoleId).HasColumnType("TEXT");
                entity.Property(m => m.ClaimType).HasColumnType("TEXT");
                entity.Property(m => m.ClaimValue).HasColumnType("TEXT");
            });

            // Настройка связей между таблицами
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ProductImage>()
                .HasOne(pi => pi.Product)  // Указываем, что ProductImage имеет один Product
                .WithMany(p => p.Images)   // Указываем, что Product имеет несколько изображений
                .HasForeignKey(pi => pi.ProductId)  // Настроим внешний ключ
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Order)
                .WithOne(o => o.Delivery)
                .HasForeignKey<Delivery>(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Review>()
            .HasOne(r => r.Product)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProductId);

            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany(u => u.WishlistItems)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.Product)
                .WithMany()
                .HasForeignKey(w => w.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Cart>()
            .HasMany(c => c.Items)
            .WithOne(ci => ci.Cart)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

            // Связь CartItem и Product
            modelBuilder.Entity<CartItem>()
                .HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CartItem>()
                .Property(c => c.RowVersion)
                .IsRowVersion();
        }
    }
}
