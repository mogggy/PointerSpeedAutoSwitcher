using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Management;                // ManagementEventWatcher/ObjectSearcher
using System.Text;
using System.Threading.Tasks;
using System.Windows;                   // NotifyIcon
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;   // DllImport

// TODO run on startup

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

        // import the SystemParametersInfo function we need to use to get/set mouse speed
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SystemParametersInfo(UInt32 uiAction, UInt32 uiParam, IntPtr pvParam, UInt32 fWinIni);

        private static ManagementObjectSearcher searcher = null;
        private static ManagementEventWatcher watcher = null;
        private static bool watcherTypeIsCreation = true;

        public MainWindow()
        {
            InitializeComponent();

            ni = new System.Windows.Forms.NotifyIcon();

            Stream iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/images/cheeseGray.ico")).Stream;
            redIcon = new System.Drawing.Icon(iconStream);      //off icon

            iconStream = Application.GetResourceStream(new Uri("pack://application:,,,/images/cheese.ico")).Stream;
            greenIcon = new System.Drawing.Icon(iconStream);    //on icon
            iconStream.Dispose();

            ni.Icon = redIcon;

            ni.Visible = true;
            ni.DoubleClick +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;

                };

            // load settings
            tbDefaultSense.Text = Properties.Settings.Default.DefaultMouseSpeed;
            tbProcessName.Text = Properties.Settings.Default.ProcessName;
            tbProcessSense.Text = Properties.Settings.Default.ProcessSense;

            tbLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " :: Started watching.\n");

            hotStart(); 
        }

        private void hotStart()
        {
            childLock(true); // lock all the text boxes

            // check if process is already running
            if (checkAlreadyRunning() > 0)
            {
                // process already running, set speeed and switch to closing watcher
                setMouseSpeed(int.Parse(tbProcessSense.Text));
                watcherTypeIsCreation = false;
                tbLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " :: "
                    + tbProcessName.Text + " already running.\n");
                lookForClosingProcess();
            }
            else
            {
                lookForNewProcess();
            }
        }

        private int checkAlreadyRunning()
        {
            string procName = "";
            this.Dispatcher.Invoke(() =>
            {
                procName = tbProcessName.Text;
            });

            WqlObjectQuery qry = new WqlObjectQuery("SELECT Name FROM Win32_Process WHERE Name=\"" + procName + "\"");
            searcher = new ManagementObjectSearcher(qry);

            ManagementObjectCollection res = searcher.Get();

            return res.Count;
        }

        private void lookForNewProcess()
        {
            string procName = "";
            this.Dispatcher.Invoke(() => 
            {
                procName = tbProcessName.Text;
                ni.Icon = redIcon;
            });

            // construct query using the (event class name, polling interval, condition) constructor
            WqlEventQuery qry = new WqlEventQuery(  "__InstanceCreationEvent",
                                                    new TimeSpan(0,0,1),                        //1 second
                                                    "TargetInstance isa \"Win32_Process\" and TargetInstance.Name=\""
                                                        + procName + "\"");           //<propertyname> <operator> <value>

            watcherTypeIsCreation = true; // watching for a process creation
            startWatcher(qry);
        }

        private void lookForClosingProcess()
        {
            string procName = "";
            this.Dispatcher.Invoke(() =>
            {
                procName = tbProcessName.Text;
                ni.Icon = greenIcon;
            });

            // construct query using the (event class name, polling interval, condition) constructor
            WqlEventQuery qry = new WqlEventQuery("__InstanceDeletionEvent",
                                                    new TimeSpan(0, 0, 1),                        //1 second
                                                    "TargetInstance isa \"Win32_Process\" and TargetInstance.Name=\""
                                                        + procName + "\"");           //<propertyname> <operator> <value>

            watcherTypeIsCreation = false; // watching for a process deletion
            startWatcher(qry);

        }

        private void handleEvent(object sender, EventArrivedEventArgs e)
        {
            int newSpeed = 10;
            // EventArrivedEventArgs has a property named NewEvent and in this case this 
            // property will hold a reference to the instance of __InstanceCreationEvent that triggered the event.
            // NewEvent type is ManagementBaseObject, but NewEvent[“TargetInstance”] is Object so you need to cast it to ManagementBaseObject. 
            ManagementBaseObject targetInstance = (ManagementBaseObject)e.NewEvent["TargetInstance"];

            // Whenever you update your UI elements from a thread other than the main thread, you need to use:
            this.Dispatcher.Invoke(() =>
            {
                tbLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " :: " + targetInstance["Name"]);
            });

            // stop the old watcher, set appropriate sense, start new watcher
            // if its a deletionevent and theres still other instances of the process running
            // then we dont stop the watcher and we dont change pointer speed
            if (watcherTypeIsCreation)
            {
                watcher.Stop();
                this.Dispatcher.Invoke(() =>
                {
                    tbLog.AppendText(" launched.\n");
                    newSpeed = int.Parse(tbProcessSense.Text);
                });
                setMouseSpeed(newSpeed);
                lookForClosingProcess();                
            }
            else
            {
                int runningInstances = checkAlreadyRunning();
                this.Dispatcher.Invoke(() =>
                {
                    tbLog.AppendText(" closed.\n");
                    newSpeed = int.Parse(tbDefaultSense.Text);
                });

                if(runningInstances == 0)
                {
                    watcher.Stop();
                    setMouseSpeed(newSpeed);
                    lookForNewProcess();
                }
                else
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        tbLog.AppendText("\t" + runningInstances.ToString() + " instances remain open.\n");
                    });
                }
            }
        }


        private void startWatcher(WqlEventQuery query)
        {
            watcher = new ManagementEventWatcher(query);
            watcher.EventArrived += new EventArrivedEventHandler(handleEvent);

            watcher.Start();    //start receiving events
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

        private void childLock(bool state)
        {
            tbProcessName.IsReadOnly    = state;
            tbProcessSense.IsReadOnly   = state;
            tbDefaultSense.IsReadOnly   = state;
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }

            base.OnStateChanged(e);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ni.Icon.Dispose();
            ni.Dispose();

            // save settings
            Properties.Settings.Default.DefaultMouseSpeed = tbDefaultSense.Text;
            Properties.Settings.Default.ProcessName = tbProcessName.Text;
            Properties.Settings.Default.ProcessSense = tbProcessSense.Text;
            Properties.Settings.Default.Save();
        }

        private void btGetCurrent_Click(object sender, RoutedEventArgs e)
        {
            tbDefaultSense.Text = getMouseSpeed().ToString();
        }

        private void btSetCurrent_Click(object sender, RoutedEventArgs e)
        {
            setMouseSpeed(int.Parse(tbDefaultSense.Text));
        }

        private void btStart_Click(object sender, RoutedEventArgs e)
        {
            // lock all the text boxes
            childLock(true);

            tbLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " :: Started watching.\n");
            btStart.IsEnabled = false;
            btEnd.IsEnabled = true;
            hotStart();
        }

        private void btEnd_Click(object sender, RoutedEventArgs e)
        {
            tbLog.AppendText(DateTime.Now.ToString("HH:mm:ss") + " :: Stopped watching.\n");
            btStart.IsEnabled = true;
            btEnd.IsEnabled = false;
            watcher.Stop();

            // set default speed
            setMouseSpeed(int.Parse(tbDefaultSense.Text));

            // unlock all the text boxes
            childLock(false); 
        }

        private void tbLog_TextChanged(object sender, TextChangedEventArgs e)
        {
            tbLog.ScrollToEnd();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //DragMove();
        }

        private void tbLog_KeyDown(object sender, KeyEventArgs e)
        {
        }
    }
}
