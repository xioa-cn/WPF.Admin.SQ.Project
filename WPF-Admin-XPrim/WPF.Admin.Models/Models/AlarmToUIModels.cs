namespace WPF.Admin.Models.Models {
    public enum ToUIEnum
    {
        Normal,
        Remove,
        Add,
    }
    public class AlarmToUIModels
    {
        public ToUIEnum ToUIType { get; set; }

        public List<string> Alarms { get; set; }

        public AlarmToUIModels(ToUIEnum e, List<string> alarms)
        {
            this.Alarms = alarms;
            this.ToUIType = e;
        }

        public static AlarmToUIModels CreateAddAlarm(List<string> strings)
        {
            return new AlarmToUIModels(ToUIEnum.Add, strings);
        }

        public static AlarmToUIModels CreateNormalAlarm(List<string> strings)
        {
            return new AlarmToUIModels(ToUIEnum.Normal, strings);
        }

        public static AlarmToUIModels CreateRemoveAlarm(List<string> strings)
        {
            return new AlarmToUIModels(ToUIEnum.Remove, strings);
        }


    }
}