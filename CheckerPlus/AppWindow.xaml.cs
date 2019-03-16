// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using FastSearchLibrary;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.IO.Compression;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CheckerPlus.Steam;
using System.Drawing;
using System.Windows.Interop;
using BespokeFusion;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using System.Reflection;

namespace CheckerPlus
{
    /// <summary>
    /// Логика взаимодействия для AppWindow.xaml
    /// </summary>
    public partial class AppWindow : Window
    {

        public AppWindow(string dir, Startup str)
        {
            InitializeComponent();
            this.dir = dir;
            this.str = str;
        }

        Startup str = null;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SteamProfile();
            //Task tsk = Task.Run((Action)SteamProfile);
            Thread th = new Thread(LoadOther)
            {
                IsBackground = true,
                Priority = ThreadPriority.Normal
            };
            th.Start();
            th.Join();
            str.Close();
            GC.Collect();
        }

        void LoadOther()
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                SteamAccounts();
                generateicon();
                LoadHookKeys();
                CheckBansOtherProject();
            }));
        }

        void CheckBansOtherProject()
        {
            if (!string.IsNullOrEmpty((string)profile_steamid.Content))
            {
                banned = CheckBans.GetBanMagicow((string)profile_steamid.Content);
                if (banned != null)
                    MaterialMessageBox.Show("Игрок получил бан за " + banned.reason + ". Дата получения бана" + banned.time);
            }
        }

        #region ForLoad

        void SteamProfile()
        {
            var steam = SteamGetProfile.GetProfile(SteamProfiles.GetSteamID());
            if (steam == null)
            {
                Maked.Visibility = Visibility.Visible;
                return;
            }
            profile_steamid.Content = steam.id;
            profile_name.Content = steam.name;
            profile_vac.Content = steam.ban ? "VAC: YES" : "VAC NO";
            ImageBrush myBrush = new ImageBrush();
            myBrush.ImageSource = SteamGetImage(steam.url);
            newprofile_img.Fill = myBrush;
        }

        void SteamAccounts()
        {
            List<SteamGetProfile.FullState> fullStates = SteamGetProfile.GetProfiles();
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                if (fullStates.Count() > 0)
                {
                    foreach (var d in fullStates)
                    {
                        DataAccounts.Items.Add(new SteamA()
                        {
                            personaname = d.name,
                            steamid = d.id,
                            VACBanned = d.ban ? "Yes" : "No"
                        });
                    }
                }
                else
                {
                    DataAccounts.Visibility = Visibility.Hidden;
                    Label_NoAccounts.Visibility = Visibility.Visible;
                    // нет аккаунтов
                }
            }));
        }

        #region Keys

        bool keys = false;
        CheckerPlus.Hooks.Main hook;

        void LoadHookKeys()
        {
            //LoadDll();
            hook = new CheckerPlus.Hooks.Main();
            hook.Start();
        }

        void LoadDll()
        {
            //AppDomain.CurrentDomain.AssemblyResolve += Load_Hooks;
        }

        byte[] dll_hopks = null;

        private Assembly Load_Hooks(object sender, ResolveEventArgs args)
        {
            return Assembly.Load(dll_hopks);
        }

        #endregion

        #endregion

        #region Dannie

        CheckBans.User_Banned banned = null;

        public string dir = string.Empty;

        private List<Process> prces = new List<Process>();

        List<string> sites = new List<string>()
        {
            "https://codhacks.ru/forum/120",
            "https://blacksector.solutions/",
            "https://vk.com/uberhatchet",
            "https://royalhack.net/",
            "https://forum.virtual-advantage.com/",
            "https://vk.com/culrust",
            "http://oplata.info/info/"
        };

        #endregion

        #region Classes

        public class foundedfile
        {
            public string path { get; set; }
            public string namefile { get; set; }
            public string lastchange { get; set; }
            public double height { get; set; }
            public int danger { get; set; }
        }

        public class SteamA
        {
            public string personaname { get; set; }
            public string steamid { get; set; }
            public string VACBanned { get; set; }
        }

        #endregion

        #region Hooks

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        #region gridFind

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (DataFiles.CurrentCell != null)
                {
                    string path = (DataFiles.CurrentCell.Item as foundedfile).path;
                    path = path.Remove(path.LastIndexOf(@"\"));
                    opendir(path);
                }

            }
            catch { }
        }

        private void dataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }

        #endregion

        #region Buttons

        private void button_exit(object sender, RoutedEventArgs e)
        {
            var mbResult = MaterialMessageBox.ShowWithCancel("Вы хотите завершить работу программы?", "CheckerPlus");
            if (mbResult == MessageBoxResult.OK)
            {
                ni.Visible = false;
                ni.Dispose();
                exitapp();
                Environment.Exit(0);
            }
        }

        #region Menu

        Grid prev = null;
        Grid next = null;

        void ChangeMenu(Grid prev, Grid next)
        {
            this.prev = prev;
            this.next = next;
            next.Opacity = 0;
            next.Visibility = Visibility.Visible;
            prev.Visibility = Visibility.Hidden;
            Storyboard strprev = new Storyboard();
            Storyboard strnext = new Storyboard();

            DoubleAnimation animfro = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 50)));
            DoubleAnimation animto = new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 150)));

            strnext.Children.Add(animto);
            Storyboard.SetTargetProperty(animto, new PropertyPath("Opacity"));
            Storyboard.SetTarget(animto, next);

            strprev.Children.Add(animfro);
            Storyboard.SetTargetProperty(animfro, new PropertyPath("Opacity"));
            Storyboard.SetTarget(animfro, prev);

            strprev.Begin();
            strnext.Begin();
        }

        private void button_Copy2_Click(object sender, RoutedEventArgs e)
        {
            Grid ad = getGrid(Main);
            if (ad == null)
                return;

            fon.Width = 528;
            ChangeMenu(ad, Main);
            //if (!Main.IsVisible)
            //  Main.Visibility = Visibility.Visible;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Grid ad = getGrid(FindWindow);
            if (ad == null)
                return;

            fon.Width = 570;
            ChangeMenu(ad, FindWindow);
            //if (!FindWindow.IsVisible)
            //   FindWindow.Visibility = Visibility.Visible;
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            Grid ad = getGrid(Soft);
            if (ad == null)
                return;
            fon.Width = 528;
            ChangeMenu(ad, Soft);
            //if (!Soft.IsVisible)
            //    Soft.Visibility = Visibility.Visible;
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            Grid ad = getGrid(Other);
            if (ad == null)
                return;
            fon.Width = 528;
            ChangeMenu(ad, Other);
            //if (!Other.IsVisible)
            //    Other.Visibility = Visibility.Visible;
        }

        private void Button_Copy2_Click_1(object sender, RoutedEventArgs e)
        {
            Grid ad = getGrid(Accounts);
            if (ad == null)
                return;
            fon.Width = 528;
            ChangeMenu(ad, Accounts);
            //if (!Accounts.IsVisible)
            //    Accounts.Visibility = Visibility.Visible;
        }

        Grid getGrid(Grid exclude)
        {
            var grid = new List<Grid>()
            {
                FindWindow,
                Soft,
                Other,
                Accounts,
                Main
            }.First(s => s.Visibility == Visibility.Visible);
            if (grid == exclude)
                return null;
            return grid;
        }

        #endregion

        #region Soft

        private void historydeviews_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process a = null;
                a = Process.Start(dir + @"\USBLogView.exe");
                prces.Add(a);
            }
            catch { }
        }

        private void historybrowsers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process a = null;
                a = Process.Start(dir + @"\BrowsingHistoryView.exe");
                prces.Add(a);
            }
            catch { }
        }

        private void Recuva_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process a = null;
                a = Process.Start(dir + @"\recuva.exe");
                prces.Add(a);
            }
            catch { }
        }

        private void Everything_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process a = null;
                a = Process.Start(dir + @"\Everything.exe");
                prces.Add(a);
            }
            catch { }
        }

        private void lastactivity_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                Process a = null;
                a = Process.Start(dir + @"\LastActivityView.exe");
                prces.Add(a);
            }
            catch { }
        }

        #endregion

        #region Other

        private void checkpress_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Buttons bt = new Buttons();
                bt.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void checkmouse_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MouseCheck pn = new MouseCheck();
                pn.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        #endregion

        private void button_startfind_Click(object sender, RoutedEventArgs e)
        {
            startfind();
        }

        private void Button_sites_Click(object sender, RoutedEventArgs e)
        {
            foreach (var d in sites)
            {
                try
                {
                    Process.Start(d);
                }
                catch { }

            }

        }

        private void Ni_MouseClick(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = WindowState.Normal;
            ni.Visible = false;
        }

        private void hideapp(object sender, RoutedEventArgs e)
        {
            this.Hide();
            this.WindowState = WindowState.Minimized;
            ni.Visible = true;
        }

        private void grid_totxt_Click(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(dir + @"\Report"))
                Directory.CreateDirectory(dir + @"\Report");
            string file = DateTime.Now.ToString("dd.MM.yy") + ".txt";
            if (!File.Exists(dir + @"\Report\" + file))
            {
                try
                {
                    File.Delete(dir + @"\Report\" + file);
                }
                catch
                {
                    MaterialMessageBox.ShowError("Закройте открытый экземпляр отчета!");
                    return;
                }
            }
            if (DataFiles.Items.Count == 0)
            {
                MaterialMessageBox.ShowError("Нет результата поиска!");
                return;
            }
            try
            {
                string text = string.Empty;
                for (int a = 0; a < DataFiles.Items.Count; a++)
                {
                    var fls = DataFiles.Items.GetItemAt(a) as foundedfile;
                    text += "Название файла: "
                        + fls.namefile
                        + "\nПодозрение: "
                        + fls.danger
                        + "\n Вес файла(Кб.): "
                        + fls.height
                        + "\n Последнее изменение: "
                        + fls.lastchange
                        + "\n Путь: "
                        + fls.path
                        + "\n--------\n";
                }
                //File.WriteAllText(temp + $@"\{dir}\Report\" + file, text);
                byte[] array = Encoding.UTF8.GetBytes(text);
                using (FileStream fstream = new FileStream(dir + $@"\{dir}\Report\" + file, FileMode.OpenOrCreate))
                {
                    fstream.Write(array, 0, array.Length);
                }
            }
            catch (Exception ex)
            {
                MaterialMessageBox.Show(ex.ToString());
                return;
            }
            opendir(dir + @"\Report");
        }

        private void Button_cancelsearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void hiddenfiles_Click(object sender, RoutedEventArgs e)
        {
            string cm = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            DirectoryInfo directoryInfo = new DirectoryInfo(cm);
            List<string> ag = new List<string>();
            int fi = 0;
            foreach (var dir in directoryInfo.GetDirectories())
            {
                if (dir.Attributes.HasFlag(FileAttributes.Hidden))
                {
                    ag.Add(dir.Name);
                    dir.Attributes = FileAttributes.Normal;
                    fi++;
                }
            }
            if (fi == 0)
            {
                MaterialMessageBox.Show($"Скрытых папок не было найдено", "CheckerPlus");
                return;
            }
            string text = string.Empty;
            foreach (string dd in ag)
                text += $" {dd}\n";
            MaterialMessageBox.Show("Скрытые папки и файлы:" + text);
            MessageBoxResult msgb = MaterialMessageBox.ShowWithCancel($"Они стали видимыми. Открыть рабочий стол?", "CheckerPlus");
            if (msgb == MessageBoxResult.OK)
            {
                opendir(cm);
            }
        }

        #region HookKeys

        private void HookKeys_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{


            if (!keys)
            {
                HookKeys.Content = "Включено";
                keys = true;
                hook.Install();
            }
            else
            {
                HookKeys.Content = "Выключено";
                keys = false;
                hook.Uninstall();
            }
            //}
            //catch(Exception ex) { MessageBox.Show(ex.ToString()); }
        }

        #endregion

        #region Author

        private void Steam_Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://steamcommunity.com/id/ka1doz/");
        }

        private void Tgraph_Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("http://t.me/kaidoz");
        }

        private void Vk_Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start("https://vk.com/kaidoz");
        }

        #endregion

        #endregion

        #endregion


        #region Helper

        MessageBoxResult ShowMessage(string message, string title, MessageBoxButton button, MessageBoxImage img)
        {
            return MessageBox.Show(message, title, button, img);
        }

        void opendir(string dir)
        {
            Process.Start(new ProcessStartInfo()
            {
                FileName = "explorer",
                Arguments = dir,
                UseShellExecute = true,
                Verb = "open"
            });
        }

        void MessageError(Exception ex)
        {
            ShowMessage("Произошла ошибка: " + ex.ToString(), "CheckerPlus", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        void direxists(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                File.SetAttributes(dir, FileAttributes.Normal);
            }
        }

        void exitapp()
        {
            foreach (Process a in prces)
            {
                try
                {
                    if (a != null)
                        a.Kill();
                }
                catch
                {
                }
            }
            try
            {
                new FileInfo(dir).Delete();
            }
            catch
            {

            }

        }

        void startfind()
        {
            button_startfind.Visibility = Visibility.Hidden;

            DataFiles.Visibility = Visibility.Visible;

            grid_totxt.Visibility = Visibility.Visible;

            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;

                string[] Drives = Environment.GetLogicalDrives();
                foreach (string s in Drives)
                {
                    try
                    {
                        findfind(s);
                    }
                    catch { }
                }

            }).Start();
            CheckExtDll();
        }

        System.Windows.Forms.NotifyIcon ni = new System.Windows.Forms.NotifyIcon();

        void generateicon()
        {
            ni.Icon = Properties.Resources.new_ico;
            ni.MouseClick += Ni_MouseClick;
        }

        #endregion

        #region FindFile

        private FileSearcher searcher;
        private object locker = new object();
        public List<FileInfo> files = new List<FileInfo>();

        void findfind(string directory)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            // create tokenSource to get stop search process possibility

            searcher = new FileSearcher(directory, (f) =>
            {
                return CheckFile(f);
            }, tokenSource);  // give tokenSource in constructor

            searcher.FilesFound += Searcher_FilesFound;

            searcher.SearchCompleted += (sender, arg) => // subscribe on SearchCompleted event
            {
                if (arg.IsCanceled) // check whether StopSearch() called
                    MessageBox.Show("Поиск завершен.", "CheckerPlus", MessageBoxButton.OK);
            };

            searcher.StartSearchAsync();
        }

        private void Searcher_FilesFound(object sender, FileEventArgs e)
        {
            lock (locker) // using a lock is obligatorily
            {
                /*Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    e.Files.ForEach((f) =>
                    {
                        DataFiles.Items.Add(new foundedfile() { namefile = f.Name, path = f.FullName, lastchange = f.CreationTime.ToString() });
                    });
                }));*/
            }
        }

        List<FileInfo> dllexxs = new List<FileInfo>();

        void CheckExtDll()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.Priority = ThreadPriority.Normal;
                Thread.CurrentThread.IsBackground = true;
                while (true)
                {
                    Thread.Sleep(100);
                _A:
                    if (dllexxs.Count() > 0)
                    {
                        if (Find_.CheckFileInfo(dllexxs[0]) == 3)
                            addfile(dllexxs[0], 3);
                        dllexxs.RemoveAt(0);
                        goto _A;
                    }
                }
            }).Start();
        }

        public bool CheckFile(FileInfo file)
        {
            int d = 0;
            foreach (var w_ in CheckerPlus.Find_.warringsearchfile)
            {
                if (file.Name.ToLower().Contains(w_))
                {
                    switch (w_)
                    {
                        case ".ini":
                            if (Find_.CheckText(file.FullName, Find_.warringtext))
                            {
                                d = 2;
                                goto addf;
                            }
                            break;
                        case ".bat":
                            if (Find_.CheckText(file.FullName, Find_.warringbat))
                            {
                                d = 2;
                                goto addf;
                            }
                            break;
                        default:
                            d = 1;
                            goto addf;
                    }
                }
            }
            if (Find_.checkext(file.FullName) && include(file))
                dllexxs.Add(file);
            return false;
        addf:

            addfile(file, d);

            return true;
        }

        bool include(FileInfo file)
        {
            string dir = file.FullName;
            if (file.Length / 1024 < 10000 && file.Length > 50 && Find_.checkinclude(file.FullName))
            {
                return true;
            }

            return false;
        }

        BitmapSource SteamGetImage(string url)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(url);
            Bitmap bitmap; bitmap = new Bitmap(stream);
            stream.Close();
            return Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }

        public void addfile(FileInfo file, int d)
        {
            Dispatcher.BeginInvoke(new ThreadStart(delegate
            {
                DataFiles.Items.Add(new foundedfile() { namefile = file.Name, height = file.Length / 1024, danger = d, lastchange = file.CreationTime.ToString(), path = file.FullName });
            }));
        }

        #endregion
    }
}
