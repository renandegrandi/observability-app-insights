using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.Inputs.V1
{
    public class OrderUpdateInput
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        [Required]
        public decimal Total { get; set; }
    }
}