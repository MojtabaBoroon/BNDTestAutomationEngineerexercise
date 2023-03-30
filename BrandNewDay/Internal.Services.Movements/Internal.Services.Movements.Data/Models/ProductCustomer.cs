using System.ComponentModel.DataAnnotations;

namespace Internal.Services.Movements.Data.Models
{
    public class ProductCustomer
    {
        [Key]
        public int ProductCustomerId { get; set; }
        public int CustomerId { get; set; }
        public int ProductId { get; set; }

        public virtual Customer? Customer { get; set; }
        public virtual Product? Product { get; set; }
    }
}
