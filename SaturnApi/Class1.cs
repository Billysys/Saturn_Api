using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace SaturnApi
{
    public static class Api
    {
        private const string DllName = "Xeno.dll";

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Initialize(bool useConsole);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Attach();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetClients();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        private static extern void Execute(byte[] scriptSource, int[] PIDs, int numUsers);

        private static bool _isInjected;
        private static bool _initialized;

        public static void Inject()
        {
            if (!_initialized)
            {
                Initialize(false);
                _initialized = true;
            }

            Attach();
            WaitForClients();

            _isInjected = GetClientsList().Count > 0;
        }

        public static bool IsInjected() => _isInjected;

        public static bool IsRobloxOpen() =>
            Process.GetProcessesByName("RobloxPlayerBeta").Any();

        public static void KillRoblox()
        {
            foreach (var proc in Process.GetProcessesByName("RobloxPlayerBeta"))
            {
                try { proc.Kill(); } catch { }
            }
        }

        public static void Execute(string scriptSource)
        {
            var clients = GetClientsList();
            if (clients.Count == 0) return;

            int[] ids = clients.Select(c => c.Id).ToArray();
            byte[] scriptBytes = Encoding.UTF8.GetBytes(scriptSource + "\0");
            Execute(scriptBytes, ids, ids.Length);
        }

        public static List<ClientInfo> GetClientsList()
        {
            var result = new List<ClientInfo>();
            IntPtr ptr = GetClients();

            if (ptr == IntPtr.Zero) return result;

            string raw = Marshal.PtrToStringAnsi(ptr);
            if (string.IsNullOrWhiteSpace(raw)) return result;

            var matches = Regex.Matches(raw, "\\[\\s*(\\d+),\\s*\"(.*?)\",\\s*\"(.*?)\",\\s*(\\d+)\\s*\\]");
            foreach (Match match in matches)
            {
                result.Add(new ClientInfo
                {
                    Id = int.Parse(match.Groups[1].Value),
                    Name = match.Groups[2].Value,
                    Version = match.Groups[3].Value,
                    State = int.Parse(match.Groups[4].Value)
                });
            }

            return result;
        }

        private static void WaitForClients(int timeoutMs = 3000, int pollInterval = 100)
        {
            int waited = 0;
            while (waited < timeoutMs)
            {
                if (GetClientsList().Count > 0) break;
                System.Threading.Thread.Sleep(pollInterval);
                waited += pollInterval;
            }
        }

        private static string EscapeLuaString(string input) =>
            input.Replace("\\", "\\\\").Replace("'", "\\'");

        public struct ClientInfo
        {
            public int Id;
            public string Name;
            public string Version;
            public int State;
        }
    }
}
