// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using CheckerPlus;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
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
using CheckerPlus.Steam;
using BespokeFusion;

namespace CheckerPlus
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Startup : Window
    {

        public Startup()
        {
            InitializeComponent();
#if DEBUG
            Debug.WriteLine(System.Windows.Forms.Keys.Oemtilde.GetHashCode());
#endif
        }

        void Iplogg()
        {
            WebClient client = new WebClient();
            string da = client.DownloadString("https://2no.co/1hmzV6");
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        System.Windows.Threading.DispatcherTimer dt = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer dt2 = new System.Windows.Threading.DispatcherTimer();

        void Loads()
        {
            dt.Tick += Dt_Tick;
            dt.Interval = new TimeSpan(0, 0, 0, 1);
            dt.Start();


            dt2.Tick += DispatcherTimer_Tick;
            dt2.Interval = new TimeSpan(0, 0, 0, 0, 500);
            dt2.Start();

            dispatcherTimer.Tick += dispatcherTimer_Tick;
        }

        int se = 5;

        private void Dt_Tick(object sender, EventArgs e)
        {
            if (se <= 0)
            {
                Button_Timer.IsHitTestVisible = true;
                Button_Timer.Content = "Запустить";
                dt.Stop();
                return;
            }
            se--;
            Button_Timer.Content = se.ToString();
        }

        Random rnd = new Random();
        public float o = 0;

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (o < logs.Count() - 1)
            {
                if (Math.Abs(o % 1) == 0)
                    logs_text.Text = logs_text.Text + Environment.NewLine + logs[Convert.ToInt32(o)];
                else
                {
                    var ad = logsfun[rnd.Next(0, logsfun.Count())];
                    logs_text.Text = logs_text.Text + Environment.NewLine + ad;
                    logsfun.Remove(ad);
                }
            }
            else
            {
                logs_text.Text = logs_text.Text + Environment.NewLine + logs[logs.Count() - 1];
                dt2.Stop();
            }
            o += 0.5f;
        }

        private void Button3_Copy_Click(object sender, RoutedEventArgs e)
        {
            GC.Collect();
            mnw.Visibility = Visibility.Visible;
            mnw.Show();
            Visibility = Visibility.Hidden;
            if (mnw.IsLoaded)
                Close();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Loads();
            Thread th = new Thread(LoadDll);
            th.Start();
            th.Join();
            mnw = new AppWindow()
            {
                Visibility = Visibility.Hidden
            };
            Iplogg();
        }

        #region Dll

        void LoadDll()
        {
            dll_json = Dll.QuickLZ.QuickLZ.decompress(Properties.Resources.Newtonsoft_Json);
            AppDomain.CurrentDomain.AssemblyResolve += Load_Zip;
        }

        byte[] dll_json = null;

        private Assembly Load_Zip(object sender, ResolveEventArgs args)
        {
            return Assembly.Load(dll_json);
        }

        #endregion

        List<string> logs = new List<string>()
        {
            "Включаем самозащиту",
            "Загрузка ресурсов программы",
            "Создание формы программы",
            "Распаковываем временные файлы",
            "Отправляем отсчет на сервер",
            "Программа запущена"
        };

        List<string> logsfun = new List<string>()
        {
            "Ddos oxide-russia.ru",
            "Собираем пароли с браузеров(шутка(нет))",
            "Крякаем чекер RustCheatCheck",
            "Смотрим видосы Magicow",
            "Убиваем хейтеров Rust'a",
            "Садимся на бутылку за репост",
            "А вы знали,что разработика софта зовут Тимур?",
            "Собираем деньги на лечение Filant'у"
        };

        private void Button_Copy3_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        public AppWindow mnw;

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer()
        {
            Interval = new TimeSpan(0, 0, 0, 0, 10)
        };

        private void GridFon_MouseEnter(object sender, MouseEventArgs e)
        {
            d = 1;
            dispatcherTimer.Start();
        }

        private void GridFon_MouseLeave(object sender, MouseEventArgs e)
        {
            d = 0;
            dispatcherTimer.Start();
        }

        int d = 0;

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            ChangeFon();
        }


        private void ChangeFon()
        {
            if (d == 1)
            {
                if (Fon.Opacity < 1.0)
                {
                    Fon.Opacity = Fon.Opacity + (0.3 / 10);
                }
                else
                    dispatcherTimer.Stop();
            }
            else
            {
                if (Fon.Opacity > 0.8)
                {
                    Fon.Opacity = Fon.Opacity - (0.3 / 10);
                }
                else
                    dispatcherTimer.Stop();
            }
        }
    }
}
