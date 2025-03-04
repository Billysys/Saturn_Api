using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace SaturnApi
{
    public class Api
    {
        private static bool _isInjected = false;

        [DllImport("dll\\Nezur.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Initialize();

        [DllImport("dll\\Nezur.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Attach();

        [DllImport("dll\\Nezur.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetClients();

        [DllImport("dll\\Nezur.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void Execute(byte[] scriptSource, string[] clientUsers, int numUsers);

        public static void Inject()
        {
            Api.Initialize();
            _isInjected = true;
            Thread.Sleep(1000);
            string s = "game:GetService(\"StarterGui\"):SetCore(\"SendNotification\",{\r\n\tTitle = \"Saturn X\",\r\n\tText = \"Saturn X has been injected!\",\r\n})";
            string[] array = (from c in Api.GetClientsList()
                              select c.name).ToArray<string>();
            Api.Execute(Encoding.UTF8.GetBytes(s), array, array.Length);
        }
        public static bool IsInjected()
        {
            return _isInjected;
        }

        public static bool IsRobloxOpen()
        {
            return Process.GetProcessesByName("RobloxPlayerBeta").Length != 0;
        }
        public static void Execute(string scriptSource)
        {
            string[] array = (from c in Api.GetClientsList()
                              select c.name).ToArray<string>();
            Api.Execute(Encoding.UTF8.GetBytes(scriptSource), array, array.Length);
        }

        public static void KillRoblox()
        {
            bool flag = !Api.IsRobloxOpen();
            if (!flag)
            {
                foreach (Process process in Process.GetProcessesByName("RobloxPlayerBeta"))
                {
                    process.Kill();
                }
            }
        }

        public static List<Api.ClientInfo> GetClientsList()
        {
            List<Api.ClientInfo> list = new List<Api.ClientInfo>();
            IntPtr intPtr = Api.GetClients();
            for (; ; )
            {
                Api.ClientInfo clientInfo = Marshal.PtrToStructure<Api.ClientInfo>(intPtr);
                bool flag = clientInfo.name != null;
                if (!flag)
                {
                    break;
                }
                list.Add(clientInfo);
                intPtr += Marshal.SizeOf<Api.ClientInfo>();
            }
            return list;
        }

        public struct ClientInfo
        {
            public string version;

            public string name;

            public int id;
        }
    }
}