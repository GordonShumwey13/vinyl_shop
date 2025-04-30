using Microsoft.AspNetCore.Razor.TagHelpers;
using VinylShop.Models.Enums;

namespace VinylShop.TagHelpers
{
    [HtmlTargetElement("order-status")]
    public class OrderStatusTagHelper : TagHelper
    {
        public OrderStatus Status { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "span";
            output.Attributes.SetAttribute("class", $"status-badge {GetClass(Status)}");
            output.Content.SetContent(GetText(Status));
        }

        private string GetClass(OrderStatus status) => status switch
        {
            OrderStatus.Очікування => "status-pending",
            OrderStatus.Успішний => "status-success",
            OrderStatus.Відхилений => "status-cancelled",
            _ => ""
        };

        private string GetText(OrderStatus status) => status switch
        {
            OrderStatus.Очікування => "Очікування",
            OrderStatus.Успішний => "Успішний",
            OrderStatus.Відхилений => "Відхилений",
            _ => status.ToString()
        };
    }
}
