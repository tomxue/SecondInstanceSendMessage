using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SingleInstanceSendMessage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private IntPtr WindowInteropHandle = IntPtr.Zero;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // MSGFLT_ADD: 1, Adds the message to the filter. This has the effect of allowing the message to be received.
            //_ = NativeHelper.ChangeWindowMessageFilter(NativeHelper.WM_COPYDATA, 1);
        }

        public MainWindow(string args) : this()
        {
            _ = Task.Run(() =>
            {
                // case: boot from task bar or double click
                HandleCommandlineArgs(args);
            });
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            WindowInteropHandle = new WindowInteropHelper(this).Handle;
            HwndSource hs = HwndSource.FromHwnd(WindowInteropHandle);
            hs?.AddHook(WindowProc);
        }

        private void HandleCommandlineArgs(string arg)
        {
            if (arg == "-search")
            {
                MessageBox.Show("Get Search Command!");
            }
        }

        private IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeHelper.WM_COPYDATA)
            {
                string arg = NativeHelper.PraseMessage(lParam);

                if (string.IsNullOrEmpty(arg))
                {
                    return IntPtr.Zero;
                }

                _ = Task.Run(() =>
                {
                    HandleCommandlineArgs(arg);
                });
            }

            return IntPtr.Zero;
        }

        private void ExitApp()
        {
            if (WindowInteropHandle != IntPtr.Zero)
            {
                HwndSource hs = HwndSource.FromHwnd(WindowInteropHandle);
                hs?.RemoveHook(WindowProc);
            }
        }
    }
}
