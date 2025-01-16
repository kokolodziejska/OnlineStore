using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Domain.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        [Required(ErrorMessage = "The CustomerName field is required.")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "The OrderDate field is required.")]
        public DateTime OrderDate { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();
    }
}
