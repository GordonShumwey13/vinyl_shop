using VinylShop.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Order
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int BuyerId { get; set; }

    [ForeignKey("BuyerId")]
    public Buyer Buyer { get; set; }

    [Required]
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public string DeliveryMethod { get; set; } = "";
    public string Packaging { get; set; } = "";
    public string PaymentMethod { get; set; } = "";
    public string City { get; set; } = "";
    public string Address { get; set; } = "";
    public string Comment { get; set; } = "";
    public string PromoCode { get; set; } = "";
    public decimal TotalPrice { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

}
