using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CheckerPlus.Dll.MessageKey
{
    /// <summary>
    /// Логика взаимодействия для MessageKeyWindow.xaml
    /// </summary>
    public partial class MessageKeyWindow : Window
    {
        public string textkey = "";

        public MessageKeyWindow()
        {
            InitializeComponent();
        }

        System.Windows.Threading.DispatcherTimer dt = new System.Windows.Threading.DispatcherTimer();

        KeyCheck key = new KeyCheck();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LabelKey.Content = textkey;
            dt.Tick += Dt_Tick;
            dt.Interval = new TimeSpan(0, 0, 0, 0, 12);
            dt.Start();
        }

        private void Dt_Tick(object sender, EventArgs e)
        {
          //  new Thread(() =>
          //  {
          //      Thread.CurrentThread.IsBackground = true;
          //      Thread.CurrentThread.Priority = ThreadPriority.Highest;
            //    Dispatcher.BeginInvoke(new ThreadStart(delegate
             //   {
                    Opacity = Opacity - 0.2;
                    LabelKey.Opacity = LabelKey.Opacity - 0.1;
                    if (Opacity <= 0.2)
                    {
                        GC.Collect();
                        Close();
                    }
                //}));
            ////}).Start();
        }
    }
}
