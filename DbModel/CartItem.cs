namespace VinylShop.Models
{
    public class CartItem
    {
        public int AlbumId { get; set; }
        public int Quantity { get; set; }
        public string Price { get; set; } = string.Empty;
    }
}
