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
            OrderStatus.Pending => "status-pending",
            OrderStatus.Success => "status-success",
            OrderStatus.Declined => "status-cancelled",
            _ => ""
        };

        private string GetText(OrderStatus status) => status switch
        {
            OrderStatus.Pending => "Очікування",
            OrderStatus.Success => "Успішний",
            OrderStatus.Declined => "Відхилений",
            _ => status.ToString()
        };
    }
}
