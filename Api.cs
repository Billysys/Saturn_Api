using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SaturnApi
{
    public static class Api
    {
        private static System.Windows.Forms.Timer time1 = new System.Windows.Forms.Timer();
        private static SaturnApiEngine saturn = null;
        private static bool isUserAgentSet = false;

        static Api()
        {
            CreateSaturnEngine();
            time1.Tick += new EventHandler(ticktimer3125);
            time1.Start();
        }

        private static void CreateSaturnEngine() => saturn = new SaturnApiEngine();

        public static void Attach()
        {
            if (!isUserAgentSet)
            {
                MessageBox.Show("Error Config", "Saturn X");
            }
            else
            {
                saturn?.InjectSaturn();
            }
        }

        public static void KillRoblox() => saturn?.KillRoblox();

        public static bool IsInjected()
        {
            return saturn != null && saturn.IsInjected();
        }

        public static bool IsRobloxOpen() => Process.GetProcessesByName("RobloxPlayerBeta").Length != 0;

        public static string[] GetActiveClientNames() => saturn?.GetActiveClientNames();

        public static void Execute(string script)
        {
            try
            {
                string scriptPath = "setup.lua";
                scriptPath = Path.GetFullPath(scriptPath);

                string defaultScript = "WebSocket=WebSocket or {} ;WebSocket.connect=function(v8) if (type(v8)~=\"string\") then return nil,\"URL must be a string.\";end if  not (v8:match(\"^ws://\") or v8:match(\"^wss://\")) then return nil,\"Invalid WebSocket URL. Must start with 'ws://' or 'wss://'.\";end local v9=v8:gsub(\"^ws://\",\"\"):gsub(\"^wss://\",\"\");if ((v9==\"\") or v9:match(\"^%s*$\")) then return nil,\"Invalid WebSocket URL. No host specified.\";end return {Send=function(v51) end,Close=function() end,OnMessage={},OnClose={}};end;local v1={};local v2=setmetatable;function setmetatable(v10,v11) local v12=v2(v10,v11);v1[v12]=v11;return v12;end function getrawmetatable(v14) return v1[v14];end function setrawmetatable(v15,v16) local v17=getrawmetatable(v15);table.foreach(v16,function(v52,v53) v17[v52]=v53;end);return v15;end local v3={};function sethiddenproperty(v18,v19,v20) if ( not v18 or (type(v19)~=\"string\")) then error(\"Failed to set hidden property '\"   .. tostring(v19)   .. \"' on the object: \"   .. tostring(v18) );end v3[v18]=v3[v18] or {} ;v3[v18][v19]=v20;return true;end function gethiddenproperty(v23,v24) if ( not v23 or (type(v24)~=\"string\")) then error(\"Failed to get hidden property '\"   .. tostring(v24)   .. \"' from the object: \"   .. tostring(v23) );end local v25=(v3[v23] and v3[v23][v24]) or nil ;local v26=true;return v25 or ((v24==\"size_xml\") and 5) ,v26;end function hookmetamethod(v27,v28,v29) assert((type(v27)==\"table\") or (type(v27)==\"userdata\") ,\"invalid argument #1 to 'hookmetamethod' (table or userdata expected, got \"   .. type(v27)   .. \")\" ,2);assert(type(v28)==\"string\" ,\"invalid argument #2 to 'hookmetamethod' (index: string expected, got \"   .. type(v27)   .. \")\" ,2);assert(type(v29)==\"function\" ,\"invalid argument #3 to 'hookmetamethod' (function expected, got \"   .. type(v27)   .. \")\" ,2);local v30=v27;local v31=Xeno.debug.getmetatable(v27);v31[v28]=v29;v27=v31;return v30;end function hookmetamethod(v33,v34,v35) local v36=getgenv().getrawmetatable(v33);local v37=v36[v34];v36[v34]=v35;return v37;end debug.getproto=function(v39,v40,v41) local v42=function() return true;end;if v41 then return {v42};else return v42;end end;debug.getconstant=function(v43,v44) local v45={[1]=\"print\",[2]=nil,[3]=\"Hello, world!\"};return v45[v44];end;debug.getupvalues=function(v46) local v47;setfenv(v46,{print=function(v55) v47=v55;end});v46();return {v47};end;debug.getupvalue=function(v48,v49) local v50;setfenv(v48,{print=function(v56) v50=v56;end});v48();return v50;end;\r\n\r\nlocal file = readfile(\"configs/Config.txt\") \r\nif file then\r\n    local ua = file:match(\"([^\\r\\n]+)\") \r\n    if ua then\r\n        local uas = ua .. \"/SaturnApi\" \r\n        local oldr = request \r\n        getgenv().request = function(options)\r\n            if options.Headers then\r\n                options.Headers[\"User-Agent\"] = uas\r\n            else\r\n                options.Headers = {[\"User-Agent\"] = uas}\r\n            end\r\n            local response = oldr(options)\r\n            return response\r\n        end\r\n \r\n    else\r\n        error(\"failed to load config\")\r\n    end\r\nelse\r\n    error(\"Failed to open config\")\r\nend\r\nfunction printidentity()\r\n\tprint(\"Current identity is 6\")\r\n \r\nend\r\nfunction getexecutorname()\r\n\tprint(\"Saturn X\")\r\n \r\nend\r\n";

                if (!File.Exists(scriptPath))
                {
                    File.WriteAllText(scriptPath, defaultScript);
                }
                string scriptContent = File.ReadAllText(scriptPath);
                saturn?.Execute(scriptContent + script);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }

        public static void UserAgent(string ua, int ver)
        {
            string userag = ua + ":" + ver;
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string configDir = Path.Combine(appPath, "workspace", "configs");
            string configFile = Path.Combine(configDir, "Config.txt");

            Directory.CreateDirectory(configDir);
            File.WriteAllText(configFile, userag);

            isUserAgentSet = true;
        }

        private static void CheckForUpdates()
        {
            using (var client = new WebClient())
            {
                string versionUrl = "https://raw.githubusercontent.com/saturnExecutor/webb/refs/heads/main/saturnxapi.version";
                string latestVersion = client.DownloadString(versionUrl).Trim();
                string currentVersion = "1.0.0";

                if (latestVersion != currentVersion)
                {
                    MessageBox.Show("Your Saturn X Api is outdated. Please update to the latest version.", "Saturn X Api");
                }
            }
        }

        private static void ticktimer3125(object sender, EventArgs e)
        {
            if (!IsRobloxOpen())
            {
                if (saturn == null)
                    return;
                saturn.Deject();
                saturn = null;
            }
            else
            {
                if (saturn != null)
                    return;
                CreateSaturnEngine();
            }
        }

        public static void SetAutoInject(bool value) => saturn?.AutoInject(value);
    }
}

namespace SaturnApi
{
    public class SaturnApiEngine
    {
        public static string SaturnVersion = "1.0.0";
        private bool isInjected;
        private System.Timers.Timer time;
        private bool autoinject;

        [DllImport("dlls\\Xeno.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Initialize();

        [DllImport("dlls\\Xeno.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr GetClients();

        [DllImport("dlls\\Xeno.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern void Execute(byte[] scriptSource, string[] clientUsers, int numUsers);

        [DllImport("dlls\\Xeno.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Compilable(byte[] scriptSource);

        [DllImport("dlls\\Xeno.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Attach();

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);


        public SaturnApiEngine()
        {
            Initialize();
            time = new System.Timers.Timer();
            time.Elapsed += timertick;
            time.AutoReset = true;
            Task.Run(async () =>
            {
                while (true)
                {
                    if (IsRobloxOpen() && autoinject && !isInjected)
                        InjectSaturn();
                    await Task.Delay(1000);
                }
            });
        }

        public void KillRoblox()
        {
            if (!IsRobloxOpen())
                return;
            foreach (Process process in Process.GetProcessesByName("RobloxPlayerBeta"))
                process.Kill();
        }

        public void AutoInject(bool value) => autoinject = value;

        public bool IsInjected() => isInjected;

        public bool IsRobloxOpen() => Process.GetProcessesByName("RobloxPlayerBeta").Length != 0;

        public string[] GetActiveClientNames()
        {
            return GetClientsFromDll().Select(c => c.name).ToArray();
        }

        public void InjectSaturn()
        {
            if (!IsRobloxOpen())
                return;
            try
            {
                if (GetModuleHandle("Xeno.dll") == IntPtr.Zero)
                    LoadLibrary("dlls\\Xeno.dll");
                Attach();
                isInjected = true;
                if (!time.Enabled)
                    time.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to attach Saturn X: " + ex.Message, "Saturn X");
                isInjected = false;
            }
        }

        public void Deject()
        {
            isInjected = false;
            IntPtr moduleHandle = GetModuleHandle("Xeno.dll");
            if (moduleHandle != IntPtr.Zero)
                FreeLibrary(moduleHandle);
            Reload();
        }

        public void Reload()
        {
            if (isInjected)
                return;
            LoadLibrary("dlls\\Xeno.dll");
            isInjected = true;
        }

        private void timertick(object sender, EventArgs e)
        {
            if (IsRobloxOpen())
                return;
            isInjected = false;
            if (time.Enabled)
                time.Stop();
        }

        public void Execute(string script)
        {
            try
            {
                if (!IsInjected() || !IsRobloxOpen())
                    return;
                List<ClientInfo> clientsFromDll = GetClientsFromDll();
                if (clientsFromDll == null || clientsFromDll.Count == 0)
                    return;
                string[] clientNames = clientsFromDll
                                        .GroupBy(c => c.id)
                                        .Select(g => g.First().name)
                                        .ToArray();
                if (clientNames.Length == 0)
                    return;
                Execute(Encoding.UTF8.GetBytes(script), clientNames, clientNames.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error executing script: " + ex.Message);
            }
        }

        public string GetCompilableStatus(string script)
        {
            IntPtr ptr = Compilable(Encoding.ASCII.GetBytes(script));
            string status = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeCoTaskMem(ptr);
            return status;
        }

        private List<ClientInfo> GetClientsFromDll()
        {
            List<ClientInfo> clientsFromDll = new List<ClientInfo>();
            IntPtr clients = GetClients();
            while (true)
            {
                ClientInfo info = Marshal.PtrToStructure<ClientInfo>(clients);
                if (!string.IsNullOrEmpty(info.name))
                {
                    clientsFromDll.Add(info);
                    clients += Marshal.SizeOf<ClientInfo>();
                }
                else
                    break;
            }
            return clientsFromDll;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private struct ClientInfo
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string version;
            [MarshalAs(UnmanagedType.LPStr)]
            public string name;
            public int id;
        }
    }
}