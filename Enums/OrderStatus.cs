using System.ComponentModel.DataAnnotations;
namespace VinylShop.Models.Enums
{
    public enum OrderStatus
    {
        [Display(Name = "Очікування підтвердження")]
        Pending = 1,

        [Display(Name = "Замовлення успішне")]
        Success = 2,

        [Display(Name = "Замовлення відхилине")]
        Declined = 3
    }
}
