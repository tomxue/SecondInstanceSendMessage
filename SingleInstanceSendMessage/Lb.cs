using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SingleInstanceSendMessage
{
    public struct COPYDATASTRUCT
    {
        public IntPtr dwData; //可以是任意值
        public int cbData;    //指定lpData内存区域的字节数
        [MarshalAs(UnmanagedType.LPStr)]
        public string lpData; //发送给目录窗口所在进程的数据
    }

    public static class NativeHelper
    {
        public const int WM_COPYDATA = 0x004A;

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public extern static IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd, out int ID);


        public static string GetTitle(string title)
        {
            return title + " - " + "VoiceAddinSearch" + " - " + AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        }

        #region SendMessage
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern int SendMessage(int hWnd, int Msg, int wParam, ref COPYDATASTRUCT lParam);

        public static int SendMessage(int hWnd, string message)
        {
            try
            {
                if (message == null)
                    //return 0;
                    ////if user click the startmenu shortcut, the args is null and should message first instance to show contextmenu
                    message = "";
                byte[] sarr = Encoding.UTF8.GetBytes(message);
                int len = sarr.Length;
                COPYDATASTRUCT cds;
                cds.dwData = (IntPtr)0;
                cds.cbData = len + 1;
                cds.lpData = message;

                return SendMessage(hWnd, WM_COPYDATA, 0, ref cds);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static string PraseMessage(IntPtr lParam)
        {
            COPYDATASTRUCT cds = new COPYDATASTRUCT();
            cds = (COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(COPYDATASTRUCT));
            if (cds.cbData > 0)
            {
                return cds.lpData;
            }
            return null;
        }
        #endregion


        #region SendNotifyMessage
        public const int HWND_BROADCAST = 0xffff;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern int RegisterWindowMessage(string lpString);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern bool SendNotifyMessage(int hWnd, int Msg, int wParam, int lParam);
        #endregion

        [DllImport("user32")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern bool ChangeWindowMessageFilter(uint msg, int flags);


        #region WinRT
        [DllImport("FirewallAPI.dll")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        internal static extern uint NetworkIsolationEnumAppContainers(uint Flags, out uint pdwCntPublicACs, out IntPtr ppPublicACs);
        [DllImport("FirewallAPI.dll")]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        internal static extern void NetworkIsolationFreeAppContainers(IntPtr pACs);

        [StructLayout(LayoutKind.Sequential)]
        internal struct INET_FIREWALL_APP_CONTAINER
        {
            internal IntPtr appContainerSid;
            internal IntPtr userSid;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string appContainerName;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string displayName;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string description;
            internal INET_FIREWALL_AC_CAPABILITIES capabilities;
            internal INET_FIREWALL_AC_BINARIES binaries;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string workingDirectory;
            [MarshalAs(UnmanagedType.LPWStr)]
            internal string packageFullName;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct INET_FIREWALL_AC_CAPABILITIES
        {
            public uint count;
            public IntPtr capabilities;
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct INET_FIREWALL_AC_BINARIES
        {
            public uint count;
            public IntPtr binaries;
        }
        #endregion
    }

    internal class Lb
    {
    }
}
