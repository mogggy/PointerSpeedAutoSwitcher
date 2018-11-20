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
    }
}
