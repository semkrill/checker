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
        }

        string dir = string.Empty;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Loads();
            LoadDll();
            dir = System.IO.Path.GetTempPath() + $@"{System.Guid.NewGuid().ToString()}";
            Directory.CreateDirectory(dir);
            LoadFile();
        }

        void LoadFile()
        {
            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                     WebClient web = new WebClient();
                     web.DownloadProgressChanged += Web_DownloadProgressChanged;
                     web.DownloadFileCompleted += Web_DownloadFileCompleted;
                     web.DownloadFileAsync(new Uri(@"https://github.com/Kaidoz/CheckerPlus/raw/master/CheckerPlus/packapps.exe"), dir + @"/packapps.exe");
            }
            catch (WebException)
            {
                MaterialMessageBox.ShowError("Нет подключения к интернету!");
            }
            catch (Exception ex)
            {
                MaterialMessageBox.ShowError("Произошла ошибка! " + ex);
            }
        }

        void UnPackApps()
        {
            Process prc = new Process();
            prc.StartInfo.FileName = dir + @"/packapps.exe";
            prc.StartInfo.Arguments = "-y";
            prc.StartInfo.CreateNoWindow = true;
            prc.Start();
            if (prc.HasExited)
                File.Delete(dir + @"/packapps.exe");
        }

        private void Web_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (dt2.IsEnabled)
                dt2.Stop();
            ChangeProgress(10, "Распаковка файлов");
            UnPackApps();
            ChangeProgress(5, "Обработка данных");
            mnw = new AppWindow(dir, this);
            mnw.Loaded += Mnw_Loaded;
            mnw.Show();
            ChangeProgress(10);
        }

        private void Mnw_Loaded(object sender, RoutedEventArgs e)
        {
            ChangeProgress(5, "Открытие окна");
        }

        private void Web_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }

        System.Windows.Threading.DispatcherTimer dt2 = new System.Windows.Threading.DispatcherTimer();

        void Loads()
        {
            dt2.Tick += DispatcherTimer_Tick;
            dt2.Interval = new TimeSpan(0, 0, 0, 0, 800);
            dt2.Start();
        }

        Random rnd = new Random();
        public float o = 0;

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (logsfun.Count() == 0)
            {
                dt2.Stop();
                return;
            }

            var ad = logsfun[rnd.Next(0, logsfun.Count())];
            logs_text.Text = logs_text.Text + Environment.NewLine + ad;
            logsfun.Remove(ad);
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

        void ChangeProgress(double value, string load = null)
        {
            progressBar.Value += value;
            if (load != null)
                Button_Load.Content = load;
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Copy3_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        public AppWindow mnw;
    }
}
