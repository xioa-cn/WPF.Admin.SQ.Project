using Xioa.Admin.Request.Tools.NetAxios;

namespace WPF.Admin.Service.Services.Login;

public class ApplicationAxios
{
    private IAxios? _axios;

    public IAxios Axios
    {
        get
            => _axios ??= new NAxios(AxiosConfig, IgnoreSslErrorsSslError);
    }

    public NAxiosConfig? _AxiosConfig;

    public NAxiosConfig? AxiosConfig
    {
        get
        {
            if (_AxiosConfig is null)
                throw new NullReferenceException();
            return _AxiosConfig;
        }
    }

    public bool IgnoreSslErrorsSslError { get; set; }

    public static ApplicationAxios Instance = new ApplicationAxios();
    
    public static void SetAxiosConfig(NAxiosConfig? nAxiosConfig, bool sslError = false)
    {
        Instance.IgnoreSslErrorsSslError = sslError;
        Instance._AxiosConfig = nAxiosConfig;
    }
}