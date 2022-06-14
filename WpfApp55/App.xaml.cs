using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp55
{
    public class Startup2
    {
        [STAThread]
        public static void Main(string[] args)
        {
            SingleInstanceApplicationWrapper wrapper = new SingleInstanceApplicationWrapper();
            wrapper.Run(args);
        }
    }

    public class SingleInstanceApplicationWrapper : Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase
    {
        private App _app;

        public SingleInstanceApplicationWrapper()
        {
            IsSingleInstance = true;
        }
        protected override bool OnStartup(Microsoft.VisualBasic.ApplicationServices.StartupEventArgs e)
        {
            _app = new App();
            _app.DispatcherUnhandledException += App_DispatcherUnhandledException;

            try
            {
                //ResourceDictionary resource = new ResourceDictionary();
                //resource.Source = new Uri(@"pack://application:,,,/SingleInstanceWpfApp;component/Themes/ResourceDictionaries.xaml", UriKind.RelativeOrAbsolute);
                //_app.Resources.MergedDictionaries.Add(resource);

                _app.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                _app.Run();
            }
            catch (Exception ex)
            {
                Debug.Print($"Exception: {ex.Message}");
            }

            return false;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Debug.Print($"DispatcherUnhandledException: {e.Exception}");
        }

        protected override void OnStartupNextInstance(Microsoft.VisualBasic.ApplicationServices.StartupNextInstanceEventArgs eventArgs)
        {
            _app.OnSecondInstanceRun(eventArgs.CommandLine);
        }
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private MainWindow _mainWindow;
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                _mainWindow = new MainWindow();
                _mainWindow.Show();
            }
            catch (Exception ex)
            {
                Debug.Print($"Startup exception: {ex.Message}");
            }

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            e.ApplicationExitCode = 1;
            base.OnExit(e);
        }

        public void OnSecondInstanceRun(ReadOnlyCollection<string> args)
        {
            foreach (var arg in args)
            {
                Debug.WriteLine($".....................{args.Count}, {arg}....................");
                MessageBox.Show(arg, "test");
            }
        }
    }
}
