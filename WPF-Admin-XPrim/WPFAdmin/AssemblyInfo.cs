using System.Security.Permissions;
using System.Windows;
// ����������֤������������δ����֤�Ĵ���
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
// ����ִ�з��йܴ��룬��������ñ�����native������
[assembly: SecurityPermission(SecurityAction.RequestMinimum, UnmanagedCode = true)]

[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //where theme specific resource dictionaries are located
                                     //(used if a resource is not found in the page,
                                     // or application resource dictionaries)
    ResourceDictionaryLocation.SourceAssembly //where the generic resource dictionary is located
                                              //(used if a resource is not found in the page,
                                              // app, or any theme specific resource dictionaries)
)]