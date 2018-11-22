using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Management;                //ManagementEventWatcher
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
using System.Runtime.InteropServices;   //DllImport

// TODO add facility for grabbing the list of currently running processes and setting the appropriate state
//      for when the program has just been started or if user wants to manually un-fuck

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
        const UInt32 SPI_GETMOUSESPEED = 0x0070;
        const UInt32 SPI_SETMOUSESPEED = 0x0071;
        const UInt32 SPIF_SENDCHANGE = 0x02;

        //import the SystemParametersInfo function we need to use to get/set mouse speed
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, IntPtr pvParam, UInt32 fWinIni);

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

        private ManagementBaseObject lookForNewProcess()
        {
            //construct query using the (event class name, polling interval, condition) constructor
            WqlEventQuery qry = new WqlEventQuery(  "__InstanceCreationEvent",
                                                    new TimeSpan(0,0,1),                        //1 second
                                                    "TargetInstance isa \"Win32_Process\"");    //<propertyname> <operator> <value>

            return startWatcher(qry);
        }

        private ManagementBaseObject lookForClosingProcess()
        {
            //construct query using the (event class name, polling interval, condition) constructor
            WqlEventQuery qry = new WqlEventQuery("__InstanceDeletionEvent",
                                                    new TimeSpan(0, 0, 1),                        //1 second
                                                    "TargetInstance isa \"Win32_Process\"");    //<propertyname> <operator> <value>

            return startWatcher(qry);
        }

        private ManagementBaseObject startWatcher(WqlEventQuery query)
        {
            //initialize an event watcher and subscribe to events matching our query
            ManagementEventWatcher watcher = new ManagementEventWatcher();
            watcher.Query = query;
            watcher.Options.Timeout = new TimeSpan(0, 0, 90);    //wait at most n seconds then give up

            //block until event occurs
            ManagementBaseObject e = watcher.WaitForNextEvent();

            //display information from the event
            tbLog.Text += "Process: ";
            tbLog.Text += ((ManagementBaseObject)e["TargetInstance"])["Name"] + "\n";

            tbLog.Text += "Path: ";
            tbLog.Text += ((ManagementBaseObject)e["TargetInstance"])["ExecutablePath"] + "\n";

            watcher.Stop();

            return e;
        }


        private unsafe int getMouseSpeed()
        {
            int mouseSpeed = 0;
            bool res = SystemParametersInfo(SPI_GETMOUSESPEED,          //get mouse speed
                                            0,                          //unused
                                            new IntPtr(&mouseSpeed),    //pointer to the address to store the mousespeed
                                            0 );                        //unused here
            if (res) return mouseSpeed;
            return 0;
        }

        private unsafe bool setMouseSpeed(int mouseSpeed)
        { 
            if( (mouseSpeed > MAXSPEED) || (mouseSpeed < MINSPEED) )
            {
                return false;
            }

            bool res = SystemParametersInfo(SPI_SETMOUSESPEED,          //set mouse speed
                                            0,                          //unused
                                            new IntPtr(mouseSpeed),     //pointer to the desired mousespeed
                                            SPIF_SENDCHANGE);           //Broadcasts the WM_SETTINGCHANGE message after updating the user profile. 
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
            setMouseSpeed(int.Parse(tbCurrentSense.Text));
        }

        private void btWaitForProcessStart_Click(object sender, RoutedEventArgs e)
        {
            ManagementBaseObject prc = lookForNewProcess();
            if ( (string)((ManagementBaseObject)prc["TargetInstance"])["Name"] == "moggySleep.exe" )
            {
                setMouseSpeed(14);
            }
        }

        private void btWaitForProcessEnd_Click(object sender, RoutedEventArgs e)
        {
            ManagementBaseObject prc = lookForClosingProcess();
            if ((string)((ManagementBaseObject)prc["TargetInstance"])["Name"] == "moggySleep.exe")
            {
                setMouseSpeed(10);
            }
        }
    }
}
