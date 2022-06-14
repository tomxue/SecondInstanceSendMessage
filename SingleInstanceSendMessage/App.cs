using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SecondInstanceSendMessage
{
    public class EntryPoint
    {
        private static readonly string ClassName = nameof(EntryPoint);

        [STAThread]
        public static void Main(string[] args)
        {
            try
            {
                string commandArgs = GetCommandLineArgs();

                (bool ret, Process instance) = GetRunningInstance();
                if (!ret)
                    return;

                if (instance == null)
                {
                    App app = new App(commandArgs)
                    {
                        ShutdownMode = ShutdownMode.OnExplicitShutdown
                    };

                    app.Run();
                }
                else
                {
                    HandleRunningInstance(instance, commandArgs);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ClassName + " : " + nameof(Main) + $", exception occurs, {ex.StackTrace}");
            }
        }

        public static (bool, Process) GetRunningInstance()
        {
            try
            {
                Process current = Process.GetCurrentProcess();

                Process[] processes = Process.GetProcessesByName(current.ProcessName);
                foreach (Process process in processes)
                {
                    if (process.Id != current.Id)
                    {
                        if (process.MainModule.FileName == current.MainModule.FileName)
                        {
                            return (true, process);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, null);
            }

            return (true, null);
        }

        public static void HandleRunningInstance(Process instance, string arg)
        {
            //IntPtr hWnd = instance.MainWindowHandle;
            //IntPtr hWnd = NativeHelper.FindWindow(null, NativeHelper.GetTitle("SingleInstanceSendMessage"));
            IntPtr hWnd = NativeHelper.FindWindow(null, "MainWindow_TomTest");
            if (hWnd != IntPtr.Zero)
            {
                NativeHelper.SendMessage((int)hWnd, arg);
            }
        }

        public static string GetCommandLineArgs()
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length == 2)
                return args[1];
            else
                return "";
        }
    }

    class App : Application
    {
        private readonly string _commandLineArg;

        public App(string arg)
        {
            _commandLineArg = arg;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Create and show the MainWindow / UI
            MainWindow main = new MainWindow(_commandLineArg);
            main.Show();

            base.OnStartup(e);
        }
    }
}
