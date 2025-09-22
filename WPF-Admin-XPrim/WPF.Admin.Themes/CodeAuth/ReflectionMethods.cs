using System.Reflection;
using System.Runtime.CompilerServices;
using WPF.Admin.Models.Utils;

namespace WPF.Admin.Themes.CodeAuth {
    public static class Temp
    {
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static bool asdhuasdgawydaduasdgu()
        {
            try
            {
                Console.WriteLine("Using replaced method");
                return true;
            }
            catch
            {
                return true; // 即使出错也返回 true
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static string nasduabwduadawdb(string code)
        {
            return ApplicationConfigConst.Code;
        }
    }

    public class ReflectionMethods
    {
        public static void getMethod<T>(string method = "asdhuasdgawydaduasdgu")
        {
            try
            {
                Type targetType = typeof(T);
                Console.WriteLine($"Searching in type: {targetType.FullName}");

                // 1. 首先搜索当前类型
                var targetMethod = targetType.GetMethod(method, 
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static | 
                    BindingFlags.Instance | BindingFlags.FlattenHierarchy);

                if (targetMethod != null)
                {
                    Console.WriteLine($"Found method in main type: {targetMethod.Name}");
                    Console.WriteLine($"Method declaring type: {targetMethod.DeclaringType}");
                    Console.WriteLine($"Method attributes: {targetMethod.Attributes}");

                    var replacementMethod = typeof(Temp).GetMethod(method,
                        BindingFlags.NonPublic | BindingFlags.Static);

                    if (replacementMethod != null)
                    {
                        Console.WriteLine("Found replacement method, attempting to replace...");
                        ReflectionTools.ReplaceMethod(targetMethod, replacementMethod);
                        Console.WriteLine("Method replacement completed");
                        return;
                    }
                }
                else
                {
                    Console.WriteLine("Method not found in main type, this shouldn't happen!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during method replacement: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner error: {ex.InnerException.Message}");
                }
            }
        }

        private static MethodInfo SearchMethodInType(Type type)
        {
            // 搜索当前类型中的方法
            var method = type.GetMethod("asdhuasdgawydaduasdgu",
                BindingFlags.NonPublic | BindingFlags.Public | 
                BindingFlags.Static | BindingFlags.Instance | 
                BindingFlags.DeclaredOnly);

            if (method != null)
            {
                return method;
            }

            // 递归搜索嵌套类型
            var nestedTypes = type.GetNestedTypes(
                BindingFlags.NonPublic | BindingFlags.Public | 
                BindingFlags.Static | BindingFlags.Instance);

            foreach (var nestedType in nestedTypes)
            {
                var nestedMethod = SearchMethodInType(nestedType);
                if (nestedMethod != null)
                {
                    return nestedMethod;
                }
            }

            return null;
        }

        private static bool FindAndReplaceMethod(Type type)
        {
            try
            {
                // 首先在当前类型中查找方法
                Console.WriteLine($"\nSearching in type: {type.FullName}");
                
                // 修改绑定标志，包含所有可能的标志
                BindingFlags allFlags = BindingFlags.NonPublic | BindingFlags.Public | 
                                      BindingFlags.Static | BindingFlags.Instance | 
                                      BindingFlags.DeclaredOnly;

                var currentTypeMethods = type.GetMethods(allFlags);
                foreach (var method in currentTypeMethods)
                {
                    if (method.Name == "asdhuasdgawydaduasdgu")
                    {
                        return ReplaceTargetMethod(method);
                    }
                }

                // 获取所有嵌套类型，包括私有的
                var nestedTypes = type.GetNestedTypes(allFlags);
                
                foreach (var nestedType in nestedTypes)
                {
                    // 获取嵌套类型中的所有方法
                    var methods = nestedType.GetMethods(allFlags);
                    
                    foreach (var method in methods)
                    {
                        if (method.Name == "asdhuasdgawydaduasdgu")
                        {
                            return ReplaceTargetMethod(method);
                        }
                    }

                    // 递归检查更深层的嵌套类型
                    if (FindAndReplaceMethod(nestedType))
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FindAndReplaceMethod: {ex.Message}");
                return false;
            }
        }

        private static bool ReplaceTargetMethod(MethodInfo method)
        {
            try
            {
                var replacementMethod = typeof(Temp).GetMethod("asdhuasdgawydaduasdgu",
                    BindingFlags.NonPublic | BindingFlags.Static);

                if (replacementMethod != null)
                {
                    Console.WriteLine($"Found method to replace: {method.Name} in {method.DeclaringType}");
                    
                    // 确保方法已准备好
                    RuntimeHelpers.PrepareMethod(method.MethodHandle);
                    RuntimeHelpers.PrepareMethod(replacementMethod.MethodHandle);

                    ReflectionTools.ReplaceMethod(method, replacementMethod);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ReplaceTargetMethod: {ex.Message}");
                return false;
            }
        }

        // 调用被替换的方法的示例
        //public static bool CallReplacedMethod<T>()
        //{
        //    var method = typeof(T).GetMethod("asdhuasdgawydaduasdgu", 
        //        BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

        //    // 获取可能被替换的方法
        //    var del = MethodHelper.GetReplacementOrOriginal<BoolMethodDelegate>(method);

        //    // 调用方法
        //    return del();
        //}
    }
}