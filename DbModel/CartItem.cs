using System.Text.Json.Serialization;

namespace VinylShop.Models
{
    public class CartItem
    {
        public int Id { get; set; }  
        
        [JsonPropertyName("id")]
        public int AlbumId { get; set; } 

        public string Title { get; set; } = "";
        public string Artist { get; set; } = "";
        public string Image { get; set; } = "";

        [JsonPropertyName("qty")]
        public int Quantity { get; set; }   

        public decimal Price { get; set; } 
    }
}