using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace WPF.Admin.Themes.CodeAuth {
    public class ReflectionTools
    {
        public static BindingFlags BindingFlags = 
            BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        public static T GetControl<T>(Type type, Object typeInstance, string fieldname)
        {
            var fieldInfo = type.GetField(fieldname, BindingFlags);
            var control = (T)fieldInfo.GetValue(typeInstance);
            return control;
        }

        public static void SetControlValue(Type type, Object typeInstance, string fieldname, object value)
        {
            var fieldInfo = type.GetField(fieldname, BindingFlags);
            fieldInfo.SetValue(typeInstance, value);
        }

        public static EventHandler GetEventHandler(Type type, Object typeInstance, string eventname)
        {
            var method = type.GetMethod(eventname, BindingFlags);
            Delegate del = Delegate.CreateDelegate(typeof(EventHandler), typeInstance, method);
            return del as EventHandler;
        }


        public static void InvokeMethod(Type type, Object typeInstance, string eventname, object[] paramters)
        {
            var method = type.GetMethod(eventname, BindingFlags);
            method.Invoke(typeInstance, paramters);
        }

        public static void ReplaceMethod(MethodInfo methodToReplace, MethodInfo methodToInject) {
            // 强制 CLR 立即编译这个方法
            RuntimeHelpers.PrepareMethod(methodToReplace.MethodHandle);
            RuntimeHelpers.PrepareMethod(methodToInject.MethodHandle);

            // 获取方法的实际地址
            IntPtr methodStartAddr = GetMethodStartAddress(methodToReplace);
            IntPtr injectStartAddr = GetMethodStartAddress(methodToInject);

            if (methodStartAddr == IntPtr.Zero || injectStartAddr == IntPtr.Zero)
            {
                throw new InvalidOperationException("无法获取方法地址");
            }

            // 计算需要修改的字节数
            int size = IntPtr.Size == 8 ? 13 : 6;

            // 修改内存保护
            if (!VirtualProtect(methodStartAddr, (UIntPtr)size, 0x40, out uint oldProtect))
            {
                throw new InvalidOperationException("无法修改内存保护");
            }

            try
            {
                if (IntPtr.Size == 8) // 64-bit
                {
                    CreateJump64(methodStartAddr, injectStartAddr);
                    // // 创建跳转指令
                    // byte* jumpStub = (byte*)methodStartAddr.ToPointer();
                    //
                    // // mov r11, target
                    // *jumpStub = 0x49;
                    // *(jumpStub + 1) = 0xBB;
                    // *((ulong*)(jumpStub + 2)) = (ulong)injectStartAddr.ToInt64();
                    //
                    // // jmp r11
                    // *(jumpStub + 10) = 0x41;
                    // *(jumpStub + 11) = 0xFF;
                    // *(jumpStub + 12) = 0xE3;
                }
                else // 32-bit
                {
                    CreateJump32(methodStartAddr, injectStartAddr);
                    // // 创建跳转指令
                    // byte* jumpStub = (byte*)methodStartAddr.ToPointer();
                    //
                    // // jmp <addr>
                    // *jumpStub = 0xE9;
                    // *((int*)(jumpStub + 1)) = (int)injectStartAddr.ToInt32() - 
                    //     (int)methodStartAddr.ToInt32() - 5;
                }
            }
            finally
            {
                // 恢复内存保护
                VirtualProtect(methodStartAddr, (UIntPtr)size, oldProtect, out _);
            }
        }
        private static unsafe void CreateJump64(IntPtr methodStartAddr, IntPtr injectStartAddr)
        {
            byte* jumpStub = (byte*)methodStartAddr.ToPointer();
    
#if DEBUG
            // Debug模式: mov r11, target + jmp r11
            *jumpStub = 0x49;                // REX.WB prefix
            *(jumpStub + 1) = 0xBB;          // MOV r11, imm64
            *((ulong*)(jumpStub + 2)) = (ulong)injectStartAddr.ToInt64();  // target address
            *(jumpStub + 10) = 0x41;         // REX.B prefix
            *(jumpStub + 11) = 0xFF;         // JMP
            *(jumpStub + 12) = 0xE3;         // ModR/M byte
#else
    // Release模式: 直接使用相对跳转
    *jumpStub = 0xE9;    // JMP rel32
    *((int*)(jumpStub + 1)) = (int)(injectStartAddr.ToInt64() - 
        methodStartAddr.ToInt64() - 5);
#endif
        }

        private static unsafe void CreateJump32(IntPtr methodStartAddr, IntPtr injectStartAddr)
        {
            byte* jumpStub = (byte*)methodStartAddr.ToPointer();
    
#if DEBUG
            // Debug模式: mov eax, target + jmp eax
            *jumpStub = 0xB8;    // MOV eax, imm32
            *((int*)(jumpStub + 1)) = injectStartAddr.ToInt32();
            *(jumpStub + 5) = 0xFF;    // JMP
            *(jumpStub + 6) = 0xE0;    // ModR/M byte
#else
    // Release模式: 直接使用相对跳转
    *jumpStub = 0xE9;    // JMP rel32
    *((int*)(jumpStub + 1)) = injectStartAddr.ToInt32() - 
        methodStartAddr.ToInt32() - 5;
#endif
        }
        private static IntPtr GetMethodStartAddress(MethodInfo method)
        {
            // 获取方法句柄
            RuntimeMethodHandle handle = method.MethodHandle;

            // 获取方法入口点
            IntPtr ptr = handle.GetFunctionPointer();

            return ptr;
        }

        [DllImport("kernel32.dll")]
        private static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize,
            uint flNewProtect, out uint lpflOldProtect);
    }

    public static class MethodHelper
    {
        private static readonly Dictionary<MethodInfo, Delegate> _replacements = new Dictionary<MethodInfo, Delegate>();

        public static void ReplaceMethod<T>(MethodInfo original, MethodInfo replacement)
        {
            // 创建委托
            var originalDelegate = Delegate.CreateDelegate(typeof(T), original);
            var replacementDelegate = Delegate.CreateDelegate(typeof(T), replacement);
            
            // 存储替换
            _replacements[original] = replacementDelegate;
            
            // 当原始方法被调用时，将使用替换方法
            // 注意：这需要在调用点使用 GetReplacementOrOriginal 方法
        }

        public static T GetReplacementOrOriginal<T>(MethodInfo method) where T : Delegate
        {
            if (_replacements.TryGetValue(method, out var replacement))
            {
                return (T)replacement;
            }
            return (T)Delegate.CreateDelegate(typeof(T), method);
        }
    }
}