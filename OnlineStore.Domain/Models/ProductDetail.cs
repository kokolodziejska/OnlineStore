using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace OnlineStore.Domain.Models
{
    public class ProductDetail
    {
        public int ProductDetailId { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Specifications are required.")]
        public string Specifications { get; set; }

        [ValidateNever]
        public int ProductId { get; set; }

        [ValidateNever]
        public Product Product { get; set; }
    }
}
