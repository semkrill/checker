// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Threading;
using System.Diagnostics;

namespace CheckerPlus
{
    /// <summary>
    /// Interaction logic for Buttons.xaml
    /// </summary>
    public partial class Buttons : Window
    {
        public Buttons()
        {
            InitializeComponent();
        }

        private void Button3_Copy_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
            Close();
        }

        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("USER32.DLL")]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        private void button3_Copy1_Click(object sender, RoutedEventArgs e)
        {
            if (Process.GetProcessesByName("RustClient").Length == 0)
            {
                label1.Visibility = Visibility.Visible;
                label1.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label1.Foreground = new SolidColorBrush(Colors.LimeGreen);
                Process p = Process.GetProcessesByName("RustClient")[0];
                SetForegroundWindow(p.MainWindowHandle);
                KeyCheck key = new KeyCheck();
                key.startfind(true);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Process.GetProcessesByName("RustClient").Length == 0)
            {
                label1.Visibility = Visibility.Visible;
                label1.Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                label1.Foreground = new SolidColorBrush(Colors.LimeGreen);
                Process p = Process.GetProcessesByName("RustClient")[0];
                SetForegroundWindow(p.MainWindowHandle);
                KeyCheck key = new KeyCheck();
                key.StartCheck(true);
            }
        }
    }
}
