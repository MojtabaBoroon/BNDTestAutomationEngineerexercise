using System.ComponentModel;
using System.Runtime.Serialization;

namespace Internal.Services.Movements.Data.Models.Enums
{
    public enum EnumProductType
    {
        [Description("Unknown"), EnumMember(Value = "Unknown")]
        Unknown = 0,
        [Description("SavingsRetirement"), EnumMember(Value = "SavingsRetirement")]
        SavingsRetirement = 1
    }
}
