using System.ComponentModel.DataAnnotations;

namespace OnlineStore.Domain.Models;

public class OrderProduct
{
    [Key]
    public int OrderId { get; set; }
    public Order Order { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; }

    public int Quantity { get; set; }
}
