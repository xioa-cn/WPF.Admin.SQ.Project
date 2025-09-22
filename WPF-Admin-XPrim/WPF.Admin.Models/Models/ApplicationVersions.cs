namespace WPF.Admin.Models.Models {
    public enum ApplicationVersions {
        None,
        ListenAuthorization,
        NoAuthorization,
    }

    public static class ListenApplicationVersions {
        public static ApplicationVersions NormalVersion { get; set; } = ApplicationVersions.None;
    }
}