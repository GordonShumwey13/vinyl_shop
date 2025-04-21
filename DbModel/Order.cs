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

    [Required]
    public string PaymentMethod { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Phone { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]   
    public string Address { get; set; } = string.Empty;

    [StringLength(500)]
    public string Comment { get; set; } = string.Empty;
    
    public decimal TotalPrice { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

}
