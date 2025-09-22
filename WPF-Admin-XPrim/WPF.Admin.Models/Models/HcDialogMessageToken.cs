namespace WPF.Admin.Models.Models;

public class HcDialogMessageToken
{
    public static string SignalInteractionViewToken { get; } = nameof(SignalInteractionViewToken);
    public static string DialogMainToken { get; } = nameof(DialogMainToken);
    public static string DialogEjectToken { get; } = nameof(DialogEjectToken);
    public static string DialogPressMachineParametersToken { get; } = nameof(DialogPressMachineParametersToken);
    public static string DialogPressMachineSystemToken { get; } = nameof(DialogPressMachineSystemToken);
    public static string DialogMesKeyValueToken { get; } = nameof(DialogMesKeyValueToken);
    public static string DialogCheckCodeToken { get; } = nameof(DialogCheckCodeToken);
    public static string DialogAutoEnvelopeLineToken { get; } = nameof(DialogAutoEnvelopeLineToken);

    public static string DialogPartialCodeToken
    {
        get { return nameof(DialogPartialCodeToken); }
    }
}