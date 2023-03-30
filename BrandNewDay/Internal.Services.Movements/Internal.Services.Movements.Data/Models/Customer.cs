using System.ComponentModel.DataAnnotations;

namespace Internal.Services.Movements.Data.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }
        public string? CustomerFirstName { get; set; }
        public string? CustomerLastName { get; set; }
        public string? CustomerEmail { get; set; }
        public virtual ICollection<ProductCustomer>? ProductCustomers { get; set; }
    }
}
