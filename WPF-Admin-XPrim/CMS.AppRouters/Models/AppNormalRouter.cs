namespace CMS.AppRouters.Models {
    public static class AppNormalRouter {
        public static BaseRouters PlotView = new BaseRouters() {
            Key = "曲线界面",
            Content = "曲线界面",
            Icon = "Visualization",
            Page = "AutoHome"
        };

        public static BaseRouters OperateView = new BaseRouters() {
            Key = "操作界面",
            Content = "操作界面",
            Icon = "FlowChart",
            Page = "Manual2"
        };

        public static BaseRouters IOView = new BaseRouters() {
            Key = "监控界面",
            Content = "监控界面",
            Icon = "Charts",
            Page = "IoManager"
        };

        public static BaseRouters ParameterView = new BaseRouters() {
            Key = "配方管理",
            Content = "配方管理",
            Icon = "DataList",
            Page = "Params"
        };

        public static BaseRouters SearchDataView = new BaseRouters() {
            Key = "数据查询",
            Content = "数据查询",
            Icon = "Carousel",
            Page = "Table"
        };

        public static BaseRouters PlotHisView = new BaseRouters() {
            Key = "曲线查看",
            Content = "曲线查看",
            Icon = "Carousel",
            Page = "HisPlot"
        };

        public static BaseRouters AlarmHisView = new BaseRouters() {
            Key = "报警记录",
            Content = "报警记录",
            Icon = "Carousel",
            Page = "SystemAlarmLog"
        };

        public static BaseRouters SystemView = new BaseRouters() {
            Key = "系统设置",
            Content = "系统设置",
            Icon = "SystemInfo",
            Page = "PreManager",
            LoginAuth = 4
        };

        public static BaseRouters UserView = new BaseRouters() {
            Key = "用户管理",
            Content = "用户管理",
            Icon = "License",
            Page = "SystemUser",
            LoginAuth = 1
        };

        public static BaseRouters MesView = new BaseRouters() {
            Key = "Mes配置",
            Content = "Mes配置",
            Icon = "Cloud",
            Page = "AutoMes",
            LoginAuth = 4
        };
        
        public static BaseRouters CheckCodeView = new BaseRouters() {
            Key = "工装配置",
            Content = "工装配置",
            Icon = "CheckCode",
            Page = "AutoCheckCode",
            LoginAuth = 4
        };
        
        public static BaseRouters BarcodeCharacteristicsView = new BaseRouters() {
            Key = "条码特征",
            Content = "条码特征",
            Icon = "CodeCont",
            Page = "BarcodeCharacteristics",
            LoginAuth = 4
        };

        public static void Initialized() {
            Router.Instance.Routers.Add(PlotView);
            Router.Instance.Routers.Add(OperateView);
            Router.Instance.Routers.Add(IOView);
            Router.Instance.Routers.Add(ParameterView);
            Router.Instance.Routers.Add(SearchDataView);
            Router.Instance.Routers.Add(PlotHisView);
            Router.Instance.Routers.Add(AlarmHisView);
            Router.Instance.Routers.Add(SystemView);
            Router.Instance.Routers.Add(UserView);
            Router.Instance.Routers.Add(MesView);
            Router.Instance.Routers.Add(CheckCodeView);
            Router.Instance.Routers.Add(BarcodeCharacteristicsView);
        }
    }
}