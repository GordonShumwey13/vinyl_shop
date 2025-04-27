using System.ComponentModel.DataAnnotations;
namespace VinylShop.Models.Enums
{
    public enum OrderStatus
    {
        [Display(Name = "Очікування підтвердження")]
        Очікування = 1,

        [Display(Name = "Замовлення успішне")]
        Успішний = 2,

        [Display(Name = "Замовлення відхилине")]
        Відхилений = 3
    }
}
