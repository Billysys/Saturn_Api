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
            try
            {
                Api.Initialize();
                _isInjected = true;
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Injection Error: " + ex.Message);
                _isInjected = false;
            }
        }
        public static bool IsInjected() => _isInjected;

        public static bool IsRobloxOpen()
        {
            try
            {
                return Process.GetProcessesByName("RobloxPlayerBeta").Any();
            }
            catch (Exception ex)
            {
                Console.WriteLine("IsRobloxOpen Error: " + ex.Message);
                return false;
            }
        }
        public static void Execute(string scriptSource)
        {
            if (!_isInjected)
            {
                Console.WriteLine("Uninjected Api");
                return;
            }
            try
            {
                var clients = GetClientsList();
                if (clients.Count == 0)
                {
                    Console.WriteLine("No clients found");
                    return;
                }
                string[] clientNames = clients.Select(c => c.name).ToArray();
                byte[] scriptBytes = Encoding.UTF8.GetBytes(scriptSource);
                Execute(scriptBytes, clientNames, clientNames.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Execution Error: " + ex.Message);
            }
        }

        public static void KillRoblox()
        {
            try
            {
                if (IsRobloxOpen())
                {
                    foreach (Process process in Process.GetProcessesByName("RobloxPlayerBeta"))
                    {
                        process.Kill();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("KillRoblox Error: " + ex.Message);
            }
        }

        public static List<ClientInfo> GetClientsList()
        {
            List<ClientInfo> clients = new List<ClientInfo>();
            try
            {
                IntPtr ptr = GetClients();
                int size = Marshal.SizeOf<ClientInfo>();
                int counter = 0;
                const int maxClients = 1000;
                while (counter < maxClients)
                {
                    ClientInfo client = Marshal.PtrToStructure<ClientInfo>(ptr);
                    if (string.IsNullOrEmpty(client.name))
                    {
                        break;
                    }
                    clients.Add(client);
                    ptr = IntPtr.Add(ptr, size);
                    counter++;
                }
                if (counter == maxClients)
                {
                    Console.WriteLine("Maximum number of clients reached");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("etClientsList Error: " + ex.Message);
            }
            return clients;
        }


        public struct ClientInfo
        {
            public string version;

            public string name;

            public int id;
        }
    }
}
