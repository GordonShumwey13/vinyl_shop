namespace VinylShop.Models
{
    public class OrderItemDto
    {
        public int? Id { get; set; }
        public int AlbumId { get; set; }
        public string AlbumTitle { get; set; } = "";
        public string ArtistName { get; set; } = "";
        public int Quantity { get; set; }
    }

    public class OrderEditDto
    {
        public int Id { get; set; }
        public string Address { get; set; } = "";
        public string City { get; set; } = "";
        public string Phone { get; set; } = "";
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
