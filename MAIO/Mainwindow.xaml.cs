using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static MAIO.Main;
using static MAIO.Mainwindow;

namespace MAIO
{
    /// <summary>
    /// Mainwindow.xaml 的交互逻辑
    /// </summary>
    public partial class Mainwindow : Window
    {
        public static Dictionary<string, string> allprofile = new Dictionary<string, string>();
        public static Dictionary<string, string> refreshtoken = new Dictionary<string, string>();
        public static Dictionary<string, string> giftcardlist = new Dictionary<string, string>();
        public static Dictionary<string, string> account = new Dictionary<string, string>();
        public static ObservableCollection<taskset> task = new ObservableCollection<taskset>();
        public static ObservableCollection<Main.Monitor> Advancemonitortask = new ObservableCollection<Main.Monitor>();
        public static ArrayList proxypool = new ArrayList();
        public static ArrayList monitorproxypool = new ArrayList();
        public static List<string> codepool = new List<string>();
        public static List<string> listproxy;
        public static Dictionary<string, string> tasklist = new Dictionary<string, string>();
        public static Dictionary<long, string> cookiewtime = new Dictionary<long, string>();
        public static List<string> lines = new List<string>();
        public static bool iscookielistnull = false;
        string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + "MAIO";
        string path2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "profile.json";
        string path3 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "proxy.txt";
        string path4 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "account.json";
        string path5 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "giftcard.json";
        string path6 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "codelist.txt";
        string path7 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "monitorproxy.txt";
        string path8 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "profilegroup.json";
        public Mainwindow()
        {
            InitializeComponent();
            Task task1 = Task.Run(() => Initialprofile());
            Task task2 = Task.Run(() => Initialproxy());
            Task task3 = Task.Run(() => Initialaccount());
            Task task4 = Task.Run(() => Initialgiftcard());
            Task task5 = Task.Run(() => Initialtask());
            Task task6 = Task.Run(() => InitialCookie());
            Task task8 = Task.Run(() => Initialcode());
            Task.WaitAll(task1, task2, task3, task4, task5, task6, task8);
            Main mn = new Main();
            Config.mn = mn;
            maingrid.Children.Add(mn);
        }
        string[] sArray = new string[4];
        public void Initialproxy()
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                FileStream fs1 = new FileStream(path3, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs1.Close();
            }
            else
            {
                if (!File.Exists(path3))
                {
                    FileStream fs1 = new FileStream(path3, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fs1.Close();
                }
                if (!File.Exists(path7))
                {
                    FileStream fs1 = new FileStream(path7, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fs1.Close();
                }
            }
            FileInfo fi = new FileInfo(path3);
            FileInfo fi2 = new FileInfo(path7);
            if (fi.Length != 0)
            {
                listproxy = new List<string>(File.ReadAllLines(path3));
                for (int i = 0; i < listproxy.Count; i++)
                {
                    Thread.Sleep(1);
                    proxypool.Add(listproxy[i]);
                }
                listproxy.Clear();
            }
            if (fi2.Length != 0)
            {
                listproxy = new List<string>(File.ReadAllLines(path7));
                for (int i = 0; i < listproxy.Count; i++)
                {
                    Thread.Sleep(1);
                    monitorproxypool.Add(listproxy[i]);
                }
            }
        }
        public void Initialprofile()
        {
            if (!File.Exists(path2))
            {
                FileStream fs1 = new FileStream(path2, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs1.Close();
            }
            else
            {
                FileInfo fi = new FileInfo(path2);
                if (fi.Length != 0)
                {
                    FileStream fs3 = new FileStream(path2, FileMode.Open, FileAccess.Read, FileShare.Read);
                    StreamReader sw = new StreamReader(fs3);
                    string read = sw.ReadToEnd();
                    JArray ja = JArray.Parse(read);
                    for (int i = 0; i < ja.Count; i++)
                    {
                        Thread.Sleep(1);
                        allprofile.Add(ja[i]["ProfileName"].ToString(), ja[i].ToString().Replace("\n", "").Replace("\t", ""));
                    }
                }
            }
            if (!File.Exists(path8))
            {
                FileStream fs1 = new FileStream(path8, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs1.Close();
            }
            else
            {
                FileInfo fi = new FileInfo(path8);
                if (fi.Length != 0)
                {
                    FileStream fs3 = new FileStream(path8, FileMode.Open, FileAccess.Read, FileShare.Read);
                    StreamReader sw = new StreamReader(fs3);
                    string read = sw.ReadToEnd();
                    JArray ja = JArray.Parse(read);
                    for (int i = 0; i < ja.Count; i++)
                    {
                        Thread.Sleep(1);
                        allprofile.Add(ja[i]["ProfileName"].ToString(), ja[i].ToString().Replace("\n", "").Replace("\t", ""));
                    }
                }
            }
        }
        public void Initialaccount()
        {
            if (!File.Exists(path4))
            {
                FileStream fs1 = new FileStream(path4, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs1.Close();
            }
            try
            {
                FileInfo fi = new FileInfo(path4);
                if (fi.Length != 0)
                {
                    FileStream fs2 = new FileStream(path4, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    StreamReader sr = new StreamReader(fs2);
                    var srread = sr.ReadToEnd();
                    if (srread.Contains("["))
                    {
                        JArray ja = JArray.Parse(srread);
                       var wu=ja.ToString();
                        foreach (var i in ja)
                        {
                            Thread.Sleep(1);
                            var jo = JObject.Parse(i.ToString());
                            foreach (var n in jo)
                            {
                                Thread.Sleep(1);
                                account.Add(n.Key, n.Value.ToString());
                            }
                        }
                    }
                    else
                    {
                        JObject jo = JObject.Parse(srread);
                        foreach (var i in jo)
                        {
                            Thread.Sleep(1);
                            account.Add(i.Key, i.Value.ToString());
                            var chao = i.Value.ToString();
                        }
                    }
                    sr.Close();
                    fs2.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Read accountinfo failed");
            }

        }
        public void Initialgiftcard()
        {
            if (!File.Exists(path5))
            {
                FileStream fs1 = new FileStream(path5, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs1.Close();
            }
            try
            {
                FileInfo fi = new FileInfo(path5);
                if (fi.Length != 0)
                {
                    FileStream fs2 = new FileStream(path5, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    StreamReader sr = new StreamReader(fs2);
                    var srread = sr.ReadToEnd();
                    if (srread.Contains("["))
                    {
                        JArray ja = JArray.Parse(srread);
                        foreach (var i in ja)
                        {
                            Thread.Sleep(1);
                            var jo = JObject.Parse(i.ToString());
                            foreach (var n in jo)
                            {
                                Thread.Sleep(1);
                                giftcardlist.Add(n.Key, n.Value.ToString());
                            }
                        }

                    }
                    else
                    {
                        JObject jo = JObject.Parse(srread);
                        foreach (var i in jo)
                        {
                            Thread.Sleep(1);
                            giftcardlist.Add(i.Key, i.Value.ToString());
                            var chao = i.Value.ToString();
                        }

                    }
                    sr.Close();
                    fs2.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Read giftcardlinfo failed");
            }

        }
        public void Initialtask()
        {
            string path6 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "task.json";
            if (!File.Exists(path6))
            {
                FileStream fs1 = new FileStream(path6, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs1.Close();
            }
            try
            {
                FileInfo fi = new FileInfo(path6);
                if (fi.Length == 0)
                {
                }
                else
                {
                    FileStream fs2 = new FileStream(path6, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    StreamReader sr = new StreamReader(fs2);
                    string read = sr.ReadToEnd();
                    JArray ja = JArray.Parse(read);
                    for (int i = 0; i < ja.Count; i++)
                    {
                        Thread.Sleep(1);
                        tasklist.Add(ja[i]["Taskid"].ToString(), ja[i].ToString().Replace("\n", ""));
                    }
                    sr.Close();
                    fs2.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Read task error");
            }
        }
        public void InitialCookie()
        {
            string path7 = Environment.CurrentDirectory + "\\" + "cookie.json";
            if (!File.Exists(path7))
            {
                FileStream fs1 = new FileStream(path7, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs1.Close();
            }
            try
            {
                FileInfo fi = new FileInfo(Environment.CurrentDirectory + "\\" + "cookie.json");
                if (fi.Length == 0)
                {
                    iscookielistnull = true;
                }
                else
                {
                    FileStream fs3 = new FileStream(path7, FileMode.Open, FileAccess.Read, FileShare.Read);
                    StreamReader sw = new StreamReader(fs3);
                    string read = sw.ReadToEnd();
                    JArray ja = JArray.Parse(read);
                    for (int i = 0; i < ja.Count; i++)
                    {
                        Thread.Sleep(1);
                        JObject jo = JObject.Parse(ja[i].ToString());
                        var chao = jo.ToString();
                        if (jo["site"].ToString() == "NIKE")
                        {
                            lines.Add(jo["cookie"].ToString());
                            cookiewtime.Add(long.Parse(jo["time"].ToString()), jo["cookie"].ToString());
                        }
                    }
                    sw.Close();
                    fs3.Close();
                }

            }
            catch
            {
                MessageBox.Show("Error read cookie");
            }
            try
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "refreshtoken.json"))
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "refreshtoken.json";
                    FileInfo fi = new FileInfo(path);
                    if (fi.Length != 0)
                    {
                        FileStream fs1 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        StreamReader sr = new StreamReader(fs1);
                        string read = sr.ReadToEnd();
                        JArray ja = JArray.Parse(read);
                        foreach (var i in ja)
                        {
                            Thread.Sleep(1);
                            refreshtoken.Add(i["Account"].ToString(),i["Token"].ToString());
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Check Refreshtoken File");
            }
        }
        public void Initialcode()
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                FileStream fs1 = new FileStream(path6, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs1.Close();
            }
            else
            {
                if (!File.Exists(path6))
                {
                    FileStream fs1 = new FileStream(path6, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fs1.Close();
                }
            }
            FileInfo fi = new FileInfo(path6);
            if (fi.Length != 0)
            {
                codepool = new List<string>(File.ReadAllLines(path6));
            }
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Thread.Sleep(1);
                this.DragMove();
            }
            catch
            {

            }
        }
        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Rectangle_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Writecoookie.write();
            try
            {
                Process[] ps = Process.GetProcesses();
                foreach (Process process in ps)
                {
                    if (process.ProcessName.Contains("CheckoutHelper") || process.ProcessName.Contains("chromedriver"))
                    {
                        process.Kill();
                    }
                }
            }
            catch (Exception)
            {
            }
            Application.Current.Shutdown();
        }
        private void New_Task(object sender, RoutedEventArgs e)
        {

        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                //     datagrid.Items.Add(new Task { Size = "123", Sku = "123", TaskID = "1234" });
            }
            catch (Exception)
            {

            }
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("123");
        }

        private void ProxyClick(object sender, RoutedEventArgs e)
        {
            maingrid.Visibility = Visibility.Hidden;
            this.frmMain.Navigate(new Uri("proxy.xaml", UriKind.Relative));
        }

        private void AccountClick(object sender, RoutedEventArgs e)
        {
            maingrid.Visibility = Visibility.Hidden;
            this.frmMain.Navigate(new Uri("Account.xaml", UriKind.Relative));
        }

        private void ProfileClick(object sender, RoutedEventArgs e)
        {
            maingrid.Visibility = Visibility.Hidden;
            this.frmMain.Navigate(new Uri("Profile.xaml", UriKind.Relative));
        }

        private void SettingClick(object sender, RoutedEventArgs e)
        {
            maingrid.Visibility = Visibility.Hidden;
            this.frmMain.Navigate(new Uri("Settings.xaml", UriKind.Relative));

        }
        private void MenuClick(object sender, RoutedEventArgs e)
        {

            maingrid.Visibility = Visibility.Visible;
            //  this.frmMain.Navigate(new Uri("Main.xaml", UriKind.Relative));           

        }
    }
}
