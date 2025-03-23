using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VinylShop.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AlbumId { get; set; }

        [ForeignKey("AlbumId")]
        public Album Album { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Required]
        public int BuyerId { get; set; }

        [ForeignKey("BuyerId")]
        public Buyer Buyer { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    }
}
