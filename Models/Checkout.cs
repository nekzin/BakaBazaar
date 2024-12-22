using System.ComponentModel.DataAnnotations;

namespace BBC.Models
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
    }
    public class CheckoutViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public List<CartItemViewModel> CartItems { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public class CartItemViewModel
    {

        public int ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

}
