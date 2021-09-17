using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using static DemoService.Settings;

namespace DemoService
{
    public partial class Service1 : ServiceBase
    {
        private string _str_inject = Settings.STR_INJECT;
        string logPath = "D:\\SystemLogs-Demo.txt";
        [DllImport("Wtsapi32.dll")]
        private static extern bool WTSQuerySessionInformation(IntPtr hServer, int sessionId, WtsInfoClass wtsInfoClass, out IntPtr ppBuffer, out int pBytesReturned);
        [DllImport("Wtsapi32.dll")]
        private static extern void WTSFreeMemory(IntPtr pointer);
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            if (!File.Exists(logPath))
            {
                File.Create(logPath).Close();
            }

            File.AppendAllText(logPath, Environment.NewLine + ">>>> Datetime:" + DateTime.Now + ">>> Service Started \t" + "\t" + _str_inject + "\t" + Environment.NewLine);


        }


        private enum WtsInfoClass
        {
            WTSUserName = 5,
            WTSDomainName = 7,
        }

        private static string GetUsername(int sessionId, bool prependDomain = true)
        {
            IntPtr buffer;
            int strLen;
            string username = "SYSTEM";
            if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WtsInfoClass.WTSUserName, out buffer, out strLen) && strLen > 1)
            {
                username = Marshal.PtrToStringAnsi(buffer);
                WTSFreeMemory(buffer);
                if (prependDomain)
                {
                    if (WTSQuerySessionInformation(IntPtr.Zero, sessionId, WtsInfoClass.WTSDomainName, out buffer, out strLen) && strLen > 1)
                    {
                        username = Marshal.PtrToStringAnsi(buffer) + "\\" + username;
                        WTSFreeMemory(buffer);
                    }
                }
            }
            return username;
        }
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            if (!File.Exists(logPath))
            {
                File.Create(logPath).Close();
            }
            string username;
            switch (changeDescription.Reason)
            {
                case SessionChangeReason.SessionLogon:
                    username = GetUsername(changeDescription.SessionId);
                    File.AppendAllText(logPath, ">>> UserName: " + username + " System Log On Time: \t " + DateTime.Now + "\t" + _str_inject + "\t" + Environment.NewLine);
                    break;
                case SessionChangeReason.SessionLogoff:
                    username = GetUsername(changeDescription.SessionId);
                    File.AppendAllText(logPath, ">>> UserName: " + username + " System Log Off Time: \t " + DateTime.Now + "\t" + _str_inject + "\t" + Environment.NewLine);
                    break;
                case SessionChangeReason.RemoteConnect:
                    username = GetUsername(changeDescription.SessionId);
                    File.AppendAllText(logPath, ">>> UserName: " + username + " System Remote Connect Time: \t " + DateTime.Now + "\t" + _str_inject + "\t" + Environment.NewLine);
                    break;
                case SessionChangeReason.RemoteDisconnect:
                    username = GetUsername(changeDescription.SessionId);
                    File.AppendAllText(logPath, ">>> UserName: " + username + " System Remote Disconnect Time: \t " + DateTime.Now + "\t" + _str_inject + "\t" + Environment.NewLine);
                    break;
                case SessionChangeReason.SessionLock:
                    username = GetUsername(changeDescription.SessionId);
                    File.AppendAllText(logPath, ">>> UserName: " + username + " System Locked Time: \t" + DateTime.Now + "\t" + _str_inject + "\t" + Environment.NewLine);
                    break;
                case SessionChangeReason.SessionUnlock:
                    username = GetUsername(changeDescription.SessionId);
                    File.AppendAllText(logPath, ">>> UserName: " + username + " System Unlocked Time: \t " + DateTime.Now + "\t" + _str_inject + "\t" + Environment.NewLine);
                    break;
                default:
                    break;
            }
        }
        protected override void OnShutdown()
        {
            //string username;
            if (!File.Exists(logPath))
            {
                File.Create(logPath).Close();
            }
            //username = GetUsername(changeDescription.SessionId);
            File.AppendAllText(logPath, ">>> System Turn Off Time: \n " + DateTime.Now + "\t" + _str_inject + "\t" + Environment.NewLine);
        }

        protected override void OnStop()
        {
            if (!File.Exists(logPath))
            {
                File.Create(logPath).Close();
            }

            File.AppendAllText(logPath, Environment.NewLine + ">>>> Datetime:" + DateTime.Now + "\t" + _str_inject + "\t" + ">>> Service Stopped");

        }
    }
}
