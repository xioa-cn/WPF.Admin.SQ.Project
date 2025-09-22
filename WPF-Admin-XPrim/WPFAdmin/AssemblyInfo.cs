using System.Security.Permissions;
using System.Windows;
// 允许跳过验证，即允许运行未经验证的代码
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
// 允许执行非托管代码，即允许调用本机（native）代码
[assembly: SecurityPermission(SecurityAction.RequestMinimum, UnmanagedCode = true)]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page,
                                              // app, or any theme specific resource dictionaries)
)]