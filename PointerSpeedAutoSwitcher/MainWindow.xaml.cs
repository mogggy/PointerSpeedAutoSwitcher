using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;



namespace PointerSpeedAutoSwitcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Windows.Forms.NotifyIcon ni;
        System.Drawing.Icon redIcon;
        System.Drawing.Icon greenIcon;

        const int MINSPEED = 1;
        const int MAXSPEED = 20;
        const uint SPI_GETMOUSESPEED = 0x0070;
        const uint SPI_SETMOUSESPEED = 0x0071;
        const uint SPIF_SENDCHANGE = 0x02;

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref uint pvParam, uint fWinIni);

        public MainWindow()
        {
            InitializeComponent();

            ni = new System.Windows.Forms.NotifyIcon();

            Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/images/redicon.ico")).Stream;
            redIcon = new System.Drawing.Icon(iconStream);      //red icon

            iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/images/greenicon.ico")).Stream;
            greenIcon = new System.Drawing.Icon(iconStream);    //green icon
            iconStream.Dispose();

            ni.Icon = redIcon;

            ni.Visible = true;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    ni.Icon = greenIcon;
                    this.Show();
                    this.WindowState = WindowState.Normal;

                };
        }

        private uint getMouseSpeed()
        {
            uint mouseSpeed = 0;
            bool res = SystemParametersInfo(SPI_GETMOUSESPEED,  
                                            0,
                                            ref mouseSpeed,
                                            0 );
            if (res) return mouseSpeed;
            return 0;
        }

        private bool setMouseSpeed(uint mouseSpeed)
        { 
            bool res = SystemParametersInfo(SPI_SETMOUSESPEED,
                                            0,
                                            ref mouseSpeed,
                                            SPIF_SENDCHANGE);
            return res;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                ni.Icon = greenIcon;
                Hide();
            }

            base.OnStateChanged(e);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ni.Icon.Dispose();
            ni.Dispose();
        }

        private void btGetCurrent_Click(object sender, RoutedEventArgs e)
        {
            tbCurrentSense.Text = getMouseSpeed().ToString();
        }

        private void btSetCurrent_Click(object sender, RoutedEventArgs e)
        {
            setMouseSpeed(16);
        }
    }
}
