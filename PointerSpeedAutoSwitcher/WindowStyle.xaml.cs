using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PointerSpeedAutoSwitcher
{
    public partial class WindowStyle : ResourceDictionary
    {
        public WindowStyle()
        {
            InitializeComponent();
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            if (window.WindowState == System.Windows.WindowState.Normal)
            {
                window.WindowState = System.Windows.WindowState.Maximized;
            }
            else
            {
                window.WindowState = System.Windows.WindowState.Normal;
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            window.Close();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            var window = (Window)((FrameworkElement)sender).TemplatedParent;
            window.WindowState = System.Windows.WindowState.Minimized;
        }
    }
}
