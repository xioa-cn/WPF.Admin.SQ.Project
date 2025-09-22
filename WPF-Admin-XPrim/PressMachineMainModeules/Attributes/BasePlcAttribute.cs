

namespace PressMachineMainModeules.Attributes
{
    public class BasePlcAttribute : Attribute
    {
        public string PlcPropertyName { get; set; }

        public BasePlcAttribute(string plcPropertyName)
        {
            this.PlcPropertyName = plcPropertyName;
        }
    }
}
