using System.ComponentModel.DataAnnotations;

namespace VinylShop.Enums
{
    public enum UserRoleEnum
    {
        [Display(Name = "Адміністратор")]
        Admin,

        [Display(Name = "Менеджер продажів")]
        SalesManager
    }
}
