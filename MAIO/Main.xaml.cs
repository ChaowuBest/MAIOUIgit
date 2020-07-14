using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MAIO
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : UserControl
    {
        public static Dictionary<string, CancellationTokenSource> dic = new Dictionary<string, CancellationTokenSource>();
        private static DateTime timeStampStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public Main()
        {
            InitializeComponent();
            updatelable("123",true);
            for (int i = 0; i < Mainwindow.tasklist.Count; i++)
            {
                KeyValuePair<string, string> kv = Mainwindow.tasklist.ElementAt(i);
                JObject jo = JObject.Parse(kv.Value);
                Mainwindow.task.Add(new taskset { Tasksite = jo["Tasksite"].ToString(), Sku = jo["Sku"].ToString(), Size = jo["Size"].ToString(), Profile = jo["Profile"].ToString(), Proxies = jo["Proxies"].ToString(), Status = jo["Status"].ToString(), Taskid = jo["Taskid"].ToString(), Quantity = jo["Quantity"].ToString() });
            }
            datagrid.ItemsSource = Mainwindow.task;
            cookienum.Dispatcher.Invoke(new Action(
                delegate
            {
                cookienum.Content = Mainwindow.lines.Count;
            }));
            Task task2 = new Task(() => clearcookie());
            task2.Start();
        }
        public void clearcookie()
        {
        A: foreach (var i in Mainwindow.cookiewtime.ToArray())
            {
                Thread.Sleep(1000);
                long timest = (long)(DateTime.Now.ToUniversalTime() - timeStampStartTime).TotalMilliseconds;
                var cookitime = ConvertStringToDateTime(i.Key.ToString());
                var nowtime = ConvertStringToDateTime(timest.ToString());
                var difference = nowtime - cookitime;
                if (difference.Hours >= 1)
                {
                    Mainwindow.cookiewtime.Remove(long.Parse(i.Key.ToString()));
                    Mainwindow.lines.Remove(i.Value);
                    updatelable(i.Value, false);
                }
            }
            goto A;
        }
        private DateTime ConvertStringToDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        public static int counter = 0;
        public static void updatelable(string cookie, bool addcookie)
        {
            if (counter == 0)
            {
                counter++;
            }
            else
            {
                Config.mn.cookienum.Dispatcher.Invoke(new Action(
                    delegate
                    {
                        Config.mn.cookienum.Content = Mainwindow.lines.Count;
                    }));
                if (addcookie == false)
                {
                    for (int i = 0; i < Mainwindow.cookiewtime.Count; i++)
                    {
                        KeyValuePair<long, string> kv = Mainwindow.cookiewtime.ElementAt(i);
                        if (kv.Value == cookie)
                        {
                            Mainwindow.cookiewtime.Remove(kv.Key);
                            break;
                        }
                    }
                }
            }
        }
      
        public class taskset : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public string Tasksite { get; set; }
            public string Sku { get; set; }
            public string Size { get; set; }
            public string Profile { get; set; }
            public string Proxies { get; set; }
            private string status;
            public string Status
            {
                get { return status; }
                set
                {
                    status = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Status"));
                    }
                }
            }
            public string Taskid { get; set; }
            public string Quantity { get; set; }

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Writecoookie.write();
            Application.Current.Shutdown();
        }
        private void createtask_Click(object sender, RoutedEventArgs e)
        {
            string[] setup = new string[6];
            NewTask nt = new NewTask();
            nt.getTextHandler = Ctask;
            nt.ShowDialog();
        }
        private void Ctask(string[] st)
        {
            string taskid = Guid.NewGuid().ToString();
            string profile = "[{\"Taskid\":\"" + taskid + "\",\"Tasksite\":\"" + st[0].Replace("System.Windows.Controls.ComboBoxItem: ", "") + "\",\"Sku\":\"" + st[3].Replace("\r\n", "") + "\"," +
             "\"Size\":\"" + st[4].Replace("\r\n", "") + "\",\"Profile\":\"" + st[2] + "\",\"Proxies\":\"Default\"," +
            "\"Status\":\"IDLE\",\"giftcard\":\"" + st[1] + "\",\"Code\":\"" + st[5] + "\",\"Quantity\":\"" + st[6].Replace("System.Windows.Controls.ComboBoxItem: ", "") + "\"}]";
            Mainwindow.tasklist.Add(taskid, profile.Replace("[", "").Replace("]", ""));
            Mainwindow.task.Add(new taskset { Taskid = taskid, Tasksite = st[0].Replace("System.Windows.Controls.ComboBoxItem: ", ""), Sku = st[3].Replace("\r\n", ""), Size = st[4].Replace("\r\n", ""), Profile = st[2], Proxies = "Default", Status = "IDLE", Quantity = st[6].Replace("System.Windows.Controls.ComboBoxItem: ", "") });
            taskwrite(profile);
        }
        public void taskwrite(string task)
        {
            try
            {
                string path2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "task.json";
                FileStream fs0 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs0);
                FileInfo fi = new FileInfo(path2);
                FileStream fs1 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                if (fi.Length == 0)
                {
                    JArray ja = JArray.Parse(task);
                    StreamWriter sw = new StreamWriter(fs1);
                    sw.Write(ja.ToString().Replace("\n", "").Replace("\t", ""));
                    sw.Close();
                    fs1.Close();
                }
                else
                {
                    string read = sr.ReadToEnd();
                    JArray ja = JArray.Parse(read);
                    ja.Add(JObject.Parse(task.Replace("[", "").Replace("]", "")));
                    fs1.SetLength(0);
                    StreamWriter sw = new StreamWriter(fs1);
                    sw.Write(ja.ToString().Replace("\n", "").Replace("\t", ""));
                    sw.Close();
                    fs1.Close();
                }
            }
            catch (Exception)
            {
            }
        }
        private void start_Click(object sender, RoutedEventArgs e)
        {
            string taskid = Guid.NewGuid().ToString();
            int row = datagrid.SelectedIndex;
            taskset tk;
            tk = Mainwindow.task.ElementAt(row);
            if (dic.Keys.Contains(tk.Taskid))
            {
                MessageBox.Show("Please stop task first");
            }
            else
            {
                tk.Status = "Starting";
                try
                {
                    string giftcard = "";
                    string code = "";
                    if (tk.Tasksite == "NikeCA" || tk.Tasksite == "NikeAU")
                    {
                        NikeAUCA NA = new NikeAUCA();
                        NA.tk = tk;
                        NA.profile = Mainwindow.allprofile[tk.Profile];
                        NA.pid = tk.Sku;
                        NA.size = tk.Size;
                        NA.Quantity = tk.Quantity;
                        if (tk.Size == "RA" || tk.Size == "ra")
                        {
                            NA.randomsize = true;
                        }
                        var cts = new CancellationTokenSource();
                        var ct = cts.Token;
                        Task task1 = new Task(() => { NA.StartTask(ct, cts); }, ct);
                        dic.Add(tk.Taskid, cts);
                        task1.Start();
                    }
                    else if (tk.Tasksite == "NikeUS" || tk.Tasksite == "NikeUK")
                    {
                        if (Mainwindow.tasklist[tk.Taskid] != "")
                        {
                            JObject jo = JObject.Parse(Mainwindow.tasklist[tk.Taskid]);
                            giftcard = jo["giftcard"].ToString();
                            code = jo["Code"].ToString().Replace("\r\n", "");
                        }
                        Random ran = new Random();
                        int random = ran.Next(0, Mainwindow.listaccount.Count);
                        try
                        {
                            string[] account = null;
                            if (Mainwindow.listaccount[random].Contains("-"))
                            {
                                account = Mainwindow.listaccount[random].Split("-");
                            }
                            else if (Mainwindow.listaccount[random].Contains(":"))
                            {
                                account = Mainwindow.listaccount[random].Split(":");
                            }
                            NikeUSUK NSK = new NikeUSUK();
                            NSK.giftcard = giftcard;
                            NSK.pid = tk.Sku;
                            NSK.size = tk.Size;
                            NSK.code = code;
                            NSK.profile = Mainwindow.allprofile[tk.Profile];
                            NSK.tk = tk;
                            NSK.username = account[0];
                            NSK.password = account[1];
                            if (tk.Size == "RA" || tk.Size == "ra")
                            {
                                NSK.randomsize = true;
                            }
                            var cts = new CancellationTokenSource();
                            var ct = cts.Token;
                            Task task2 = new Task(() => { NSK.StartTask(ct); }, ct);
                            dic.Add(tk.Taskid, cts);
                            task2.Start();
                        }
                        catch (Exception ex)
                        {

                            tk.Status = "No Account";
                        }
                    }
                    else if (tk.Tasksite == "Footasylum")
                    {
                        try
                        {
                            Footasylum fasy = new Footasylum();
                            if (tk.Size == "RA" || tk.Size == "ra")
                            {
                                fasy.randomsize = true;
                            }
                            fasy.link = tk.Sku;
                            fasy.profile = Mainwindow.allprofile[tk.Profile];
                            fasy.size = tk.Size;
                            fasy.tk = tk;
                            var cts = new CancellationTokenSource();
                            var ct = cts.Token;
                            Task fasytask = new Task(() => { fasy.StartTask(ct, cts); }, ct);
                            dic.Add(tk.Taskid, cts);
                            fasytask.Start();
                        }
                        catch
                        {

                        }
                    }
                    else if (tk.Tasksite == "TheNorthFaceUS")
                    {
                        TheNorthFaceUS tnfus = new TheNorthFaceUS();
                        tnfus.link = tk.Sku;
                        tnfus.profile = Mainwindow.allprofile[tk.Profile];
                        tnfus.size = tk.Size;
                        tnfus.tk = tk;
                        var cts = new CancellationTokenSource();
                        var ct = cts.Token;
                        Task tnftask = new Task(() => { tnfus.StartTask(ct, cts); }, ct);
                        dic.Add(tk.Taskid, cts);
                        tnftask.Start();
                    }
                }
                catch (Exception ex)
                {
                    tk.Status = "Task Error";
                }
            }
        }
        public void updatetask(string task)
        {
            string path2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "task.json";
            FileStream fs0 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs0);
            string pro = sr.ReadToEnd();
            JArray ja = JArray.Parse(pro);
            sr.Close();
            fs0.Close();
            JObject jo = JObject.Parse(task);
            for (int i = 0; i < ja.Count; i++)
            {
                if (ja[i]["Taskid"].ToString() == jo["Taskid"].ToString())
                {
                    ja.RemoveAt(i);
                    break;
                }
            }
            FileStream fs1 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fs1);
            fs1.SetLength(0);
            sw.Write(ja.ToString().Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", ""));
            sw.Close();
            fs1.Close();
            FileInfo fi = new FileInfo(path2);
            FileStream fs2 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            if (fi.Length == 2)
            {
                fs2.SetLength(0);
            }
            fs2.Close();
        }
        private void BtnDelete_Click_1(object sender, RoutedEventArgs e)
        {
            taskset del = (taskset)((Button)sender).DataContext;
            //  var chao=dic[del.Taskid];
            if (dic.Keys.Contains(del.Taskid))
            {
                dic[del.Taskid].Cancel();
                dic.Remove(del.Taskid);
            }
            else
            {

            }
            int n = datagrid.SelectedIndex;
            for (int i = 0; i < Mainwindow.tasklist.Count; i++)
            {
                KeyValuePair<string, string> kv = Mainwindow.tasklist.ElementAt(i);
                JObject jo = JObject.Parse(kv.Value);
                if (del.Taskid == jo["Taskid"].ToString())
                {
                    string needdel = Mainwindow.tasklist[jo["Taskid"].ToString()];
                    Mainwindow.tasklist.Remove(jo["Taskid"].ToString());
                    updatetask(needdel);
                    break;
                }
            }
            Mainwindow.task.Remove(del);
        }
        private void button1_Copy3_Click(object sender, RoutedEventArgs e)
        {
            string path2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "task.json";
            FileStream fs1 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            fs1.SetLength(0);
            Mainwindow.tasklist.Clear();
            // datagrid.Items.Clear();
            Mainwindow.task.Clear();
            try
            {
                foreach (var i in dic)
                {
                    i.Value.Cancel();
                }
            }
            catch
            {
            }

            dic.Clear();
            //  dic2.Clear();
        }
        private void datagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var content = (taskset)datagrid.SelectedItem;

            try
            {
                NewTask nt = new NewTask();
                Midtransfer.sitesel = content.Tasksite;
                Midtransfer.pid = content.Sku;
                Midtransfer.sizeid = content.Size;
                Midtransfer.profilesel = content.Profile;
                if (Mainwindow.tasklist[content.Taskid] != "")
                {
                    JObject jo = JObject.Parse(Mainwindow.tasklist[content.Taskid]);
                    Midtransfer.giftcard = jo["giftcard"].ToString();
                    Midtransfer.code = jo["Code"].ToString();
                    Midtransfer.Quantity = jo["Quantity"].ToString();
                }
                Midtransfer.edit = true;
                nt.getTextHandler = Ctask;
                nt.ShowDialog();
            }
            catch
            {
                MessageBox.Show("Error Edit");
            }


        }
        private void startall_Click(object sender, RoutedEventArgs e)
        {
            for (int n = 0; n < Mainwindow.task.Count; n++)
            {
                Thread.Sleep(10);
                string taskid = Guid.NewGuid().ToString();
                taskset tk = Mainwindow.task.ElementAt(n);
                if (dic.Keys.Contains(tk.Taskid))
                {
                }
                else
                {
                    tk.Status = "Starting";
                    try
                    {
                        string giftcard = "";
                        string code = "";
                        if (tk.Tasksite == "NikeCA" || tk.Tasksite == "NikeAU")
                        {
                            NikeAUCA NA = new NikeAUCA();
                            NA.tk = tk;
                            NA.profile = Mainwindow.allprofile[tk.Profile];
                            NA.pid = tk.Sku;
                            NA.size = tk.Size;
                            NA.Quantity = tk.Quantity;
                            if (tk.Size == "RA" || tk.Size == "ra")
                            {
                                NA.randomsize = true;
                            }
                            var cts = new CancellationTokenSource();
                            var ct = cts.Token;
                            Task task1 = new Task(() => { NA.StartTask(ct, cts); }, ct);
                            dic.Add(tk.Taskid, cts);
                            task1.Start();
                        }
                        else if (tk.Tasksite == "NikeUS" || tk.Tasksite == "NikeUK")
                        {
                            if (Mainwindow.tasklist[tk.Taskid] != "")
                            {
                                JObject jo = JObject.Parse(Mainwindow.tasklist[tk.Taskid]);
                                giftcard = jo["giftcard"].ToString();
                                code = jo["Code"].ToString().Replace("\r\n", "");
                            }
                            Random ran = new Random();
                            int random = ran.Next(0, Mainwindow.listaccount.Count);
                            try
                            {
                                string[] account = null;
                                if (Mainwindow.listaccount[random].Contains("-"))
                                {
                                    account = Mainwindow.listaccount[random].Split("-");
                                }
                                else if (Mainwindow.listaccount[random].Contains(":"))
                                {
                                    account = Mainwindow.listaccount[random].Split(":");
                                }
                                NikeUSUK NSK = new NikeUSUK();
                                NSK.giftcard = giftcard;
                                NSK.pid = tk.Sku;
                                NSK.size = tk.Size;
                                NSK.code = code;
                                NSK.profile = Mainwindow.allprofile[tk.Profile];
                                NSK.tk = tk;
                                NSK.username = account[0];
                                NSK.password = account[1];
                                if (tk.Size == "RA" || tk.Size == "ra")
                                {
                                    NSK.randomsize = true;
                                }
                                var cts = new CancellationTokenSource();
                                var ct = cts.Token;
                                Task task2 = new Task(() => { NSK.StartTask(ct); }, ct);
                                dic.Add(tk.Taskid, cts);
                                task2.Start();
                            }
                            catch
                            {
                                tk.Status = "No Account";
                            }
                        }
                    }
                    catch
                    {
                        tk.Status = "Task Error";
                    }
                }

            }
        }
        private void button1_Copy1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var i in dic)
                {
                    i.Value.Cancel();
                }
            }
            catch
            {
            }
            dic.Clear();
        }
        private void stop_Click(object sender, RoutedEventArgs e)
        {

            taskset stop = (taskset)((Button)sender).DataContext;
            if (dic.Keys.Contains(stop.Taskid) == false)
            {
            }
            else
            {
                dic[stop.Taskid].Cancel();
                dic.Remove(stop.Taskid);
            }


        }
        private void button1_Copy2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var content = (taskset)datagrid.SelectedItem;
                if (content == null)
                {

                }
                else
                {
                    NewTask nt = new NewTask();
                    if (Mainwindow.tasklist[content.Taskid] != "")
                    {
                        JObject jo = JObject.Parse(Mainwindow.tasklist[content.Taskid]);
                        Midtransfer.giftcard = jo["giftcard"].ToString();
                        Midtransfer.code = jo["Code"].ToString();
                        Midtransfer.Quantity = jo["Quantity"].ToString();
                    }
                    Midtransfer.sitesel = content.Tasksite;
                    Midtransfer.pid = content.Sku;
                    Midtransfer.sizeid = content.Size;
                    Midtransfer.profilesel = content.Profile;
                    Midtransfer.edit = true;
                    nt.getTextHandler = Ctask;
                    nt.ShowDialog();
                }
            }
            catch
            {
                MessageBox.Show("Error Edit");
            }

        }
    }
}
