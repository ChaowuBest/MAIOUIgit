using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
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
        public string Key { get; set; }
        public string webhook { get; set; }
        public string cid { get; set; }
        public string cjevent { get; set; }
        public static Dictionary<string, string> allprofile = new Dictionary<string, string>();
        public static Dictionary<string,Button> profiles = new Dictionary<string, Button>();
        public static Dictionary<string, string> giftcardlist = new Dictionary<string, string>();
        public static Dictionary<string, string> account = new Dictionary<string, string>();
        public static ObservableCollection<taskset> task = new ObservableCollection<taskset>();
        public static ObservableCollection<Monitor> Advancemonitortask = new ObservableCollection<Monitor>();     
        public static ArrayList proxypool = new ArrayList();
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
        public Mainwindow()
        {
            InitializeComponent();
            Task task1 = Task.Run(()=> Initialprofile());
            Task task2 = Task.Run(() => Initialproxy());
            Task task3 = Task.Run(() => Initialaccount());
            Task task4 = Task.Run(() => Initialgiftcard());
            Task task5 = Task.Run(() => Initialtask());
            Task task6 = Task.Run(() => InitialCookie());
            Task task7 = Task.Run(() => InitialAdvancemode());   
            Task.WaitAll(task1,task2,task3,task4,task5,task6,task7);
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
            }
            FileInfo fi = new FileInfo(path3);
            if (fi.Length == 0)
            {
            }
            else
            {
                listproxy = new List<string>(File.ReadAllLines(path3));
                for (int i = 0; i < listproxy.Count; i++)
                {
                    proxypool.Add(listproxy[i]);
                }
            }
        }
        public void Initialprofile()
        {

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                FileStream fs1 = new FileStream(path2, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs1.Close();
            }
            else
            {
                if (!File.Exists(path2))
                {
                    FileStream fs1 = new FileStream(path2, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    fs1.Close();
                }
            }
            FileInfo fi = new FileInfo(path2);
            if (fi.Length != 0)
            {
                FileStream fs3 = new FileStream(path2, FileMode.Open, FileAccess.Read, FileShare.Read);
                StreamReader sw = new StreamReader(fs3);
                string read = sw.ReadToEnd();
                JArray ja = JArray.Parse(read);
                for (int i = 0; i < ja.Count; i++)
                {
                    allprofile.Add(ja[i]["ProfileName"].ToString(), ja[i].ToString().Replace("\n", "").Replace("\t", ""));
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
                if (fi.Length == 0)
                {
                }
                else
                {
                    FileStream fs2 = new FileStream(path4, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    StreamReader sr = new StreamReader(fs2);
                    var srread = sr.ReadToEnd();
                    if (srread.Contains("["))
                    {
                        JArray ja = JArray.Parse(srread);
                        foreach (var i in ja)
                        {
                            var jo = JObject.Parse(i.ToString());
                            foreach (var n in jo)
                            {
                                account.Add(n.Key, n.Value.ToString());
                            }

                        }
                    }
                    else
                    {
                        JObject jo = JObject.Parse(srread);
                        foreach (var i in jo)
                        {
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
                if (fi.Length == 0)
                {
                }
                else
                {
                    FileStream fs2 = new FileStream(path5, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    StreamReader sr = new StreamReader(fs2);
                    var srread = sr.ReadToEnd();
                    if (srread.Contains("["))
                    {
                        JArray ja = JArray.Parse(srread);
                        foreach (var i in ja)
                        {
                            var jo = JObject.Parse(i.ToString());
                            foreach (var n in jo)
                            {
                                giftcardlist.Add(n.Key, n.Value.ToString());
                            }

                        }

                    }
                    else
                    {
                        JObject jo = JObject.Parse(srread);
                        foreach (var i in jo)
                        {
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
                        tasklist.Add(ja[i]["Taskid"].ToString(), ja[i].ToString().Replace("\n", "").Replace("\t", ""));
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
                        JObject jo = JObject.Parse(ja[i].ToString());
                        var chao=jo.ToString();
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
        }
        public void InitialAdvancemode()
        {
            string path7 = Environment.CurrentDirectory + "\\" + "advancecookie.txt";
            if (!File.Exists(path7))
            {
                FileStream fs1 = new FileStream(path7, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                fs1.Close();
            }
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
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
