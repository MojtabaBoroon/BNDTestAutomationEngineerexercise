using System.ComponentModel;
using System.Runtime.Serialization;

namespace Internal.Services.Movements.Data.Models.Enums
{
    public enum EnumMovementType
    {
        [Description("Unknown"), EnumMember(Value = "Unknown")]
        Unknown = 0,
        [Description("Fee"), EnumMember(Value = "Fee")]
        Fee = 1,
        [Description("Interest"), EnumMember(Value = "Interest")]
        Interest = 2,
        [Description("Tax"), EnumMember(Value = "Tax")]
        Tax = 3,
        [Description("Incoming"), EnumMember(Value = "Incoming")]
        Incoming = 4,
        [Description("Outgoing"), EnumMember(Value = "Outgoing")]
        Outgoing = 5,
        [Description("FiscalTransfer"), EnumMember(Value = "FiscalTransfer")]
        FiscalTransfer = 6
    }
}
