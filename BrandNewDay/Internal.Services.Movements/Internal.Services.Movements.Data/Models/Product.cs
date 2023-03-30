using System.ComponentModel.DataAnnotations;
using Internal.Services.Movements.Data.Models.Enums;

namespace Internal.Services.Movements.Data.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public EnumProductType ProductType { get; set; }
        public string? ExternalAccount { get; set; }
        public virtual ICollection<ProductCustomer>? ProductCustomers { get; set; }
    }
}
