

namespace WPF.Admin.Models.Models
{
    public class ApplicationAuthTaskFactory
    {
        public static bool AuthFlag
        {
            get
            {
                if (ApplicationAuthModule.AuthTaskFlag)
                    return (DateTime.Now - ApplicationAuthModule.StartTime).TotalHours >= ApplicationAuthModule._Interval ? true : false;
                return false;
            }
        }
    }
}
