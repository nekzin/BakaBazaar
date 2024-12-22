using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace BBC.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "The Category name is required")]
        public string Name { get; set; }

        [BindNever]
        public ICollection<Product> Products { get; set; }

        public Category()
        {
            Products = new HashSet<Product>();
        }
    }

    public class Product
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "CategoryId is required")]
        public int CategoryId { get; set; }
        public ICollection<Review> Reviews { get; set; }
        [BindNever]
        public virtual Category Category { get; set; }
        [BindNever]
        [NotMapped]
        public IFormFile Image { get; set; }
        [BindNever]
        public virtual ICollection<ProductImage> Images { get; set; }
    }
    public class User : IdentityUser
    {
        // Добавьте свои поля сюда
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Role { get; set; } = "user";
        public string FullName { get; set; }

        // Навигационные свойства
        public List<Order> Orders { get; set; }
        public List<Review> Reviews { get; set; }
        public List<Wishlist> WishlistItems { get; set; }
    }

    public class ProductImage
    {
        [Key]
        public int ImageId { get; set; }
        public string ImageUrl { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
    public class Cart
    {
        public int CartId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public byte[] RowVersion { get; set; }
    }

    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public string UserId { get; set; }  // Явный внешний ключ
        public User? User { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public Payment? Payment { get; set; }
        public Delivery? Delivery { get; set; }

    }

    public class Review
    {
        [Key]
        public int ReviewId { get; set; }
        public string UserId { get; set; }  // Явный внешний ключ
        public User User { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class Wishlist
    {
        [Key]
        public int WishlistId { get; set; }
        public string UserId { get; set; }  // Явный внешний ключ
        public User User { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }


    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public string PaymentMethod { get; set; }
        public PaymentStatus Status { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; } = DateTime.Now;
    }

    public class Delivery
    {
        [Key]
        public int DeliveryId { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public string Address { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public string Status { get; set; }
    }



    public enum OrderStatus
    {
        Processing,
        Shipped,
        Delivered,
        Canceled
    }

    public enum PaymentStatus
    {
        Success,
        Failed
    }
}

