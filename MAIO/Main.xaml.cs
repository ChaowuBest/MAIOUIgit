﻿using Fleck;
using MAIO.ASOS;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
using System.Windows.Threading;

namespace MAIO
{
    /// <summary>
    /// Main.xaml 的交互逻辑
    /// </summary>
    public partial class Main : UserControl
    {
        public static Dictionary<string, CancellationTokenSource> dic = new Dictionary<string, CancellationTokenSource>();
        public static Dictionary<string, int> randomdic = new Dictionary<string, int>();
        private static DateTime timeStampStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static Dictionary<string, JObject> returnstatus = new Dictionary<string, JObject>();
        public static List<IWebSocketConnection> allSockets = new List<IWebSocketConnection>();
        public static Dictionary<string, bool> share_dog = new Dictionary<string, bool>();
        public static Dictionary<string, ArrayList> share_dog_skuid = new Dictionary<string, ArrayList>();
        public static Dictionary<string, string> localsize = new Dictionary<string, string>();
        public static ArrayList tokenlist = new ArrayList();
        public static int i = 0;
        public Main()
        {
            InitializeComponent();
            updatelable("123", true);
            for (int i = 0; i < Mainwindow.tasklist.Count; i++)
            {
                Thread.Sleep(1);
                KeyValuePair<string, string> kv = Mainwindow.tasklist.ElementAt(i);
                JObject jo = JObject.Parse(kv.Value);
                if (kv.Value.Contains("\"AdvanceMonitor\": \"False\"") || kv.Value.Contains("AdvanceMonitor") == false)
                {
                    string monitortask = "True";
                    if (kv.Value.Contains("\"monitortask\": \"False\""))
                    {
                        monitortask = "False";
                    }
                    string account = null;
                    if (kv.Value.Contains("Account"))
                    {
                        account = jo["Account"].ToString();
                        if (account == "")
                        {
                            account = null;
                        }
                    }
                    else
                    {
                        account = null;
                    }
                    Mainwindow.task.Add(new taskset { Tasksite = jo["Tasksite"].ToString(), Sku = jo["Sku"].ToString(), Size = jo["Size"].ToString(), Profile = jo["Profile"].ToString(), Proxies = jo["Proxies"].ToString(), Status = jo["Status"].ToString(), Taskid = jo["Taskid"].ToString(), Quantity = jo["Quantity"].ToString(), monitortask = monitortask, Account = account });
                }
            }
            datagrid.ItemsSource = Mainwindow.task;
            cookienum.Dispatcher.Invoke(new Action(
              delegate
            {
                cookienum.Content = Mainwindow.lines.Count;
            }));
            Process.Start(Environment.CurrentDirectory + "\\" + "Checkouthelper.exe");
            var cts = new CancellationTokenSource();
            var ct = cts.Token;
            Task task6 = Task.Run(() => checkusemonitor(ct, cts));
            Task task7 = Task.Run(() => writetoken());

        }
        public void writetoken()
        {
        A: if (tokenlist.Count != 0)
            {
                JArray ja = JArray.Parse(tokenlist[0].ToString());
                writerefreshtoken(tokenlist[0].ToString(), ja[0]["Account"].ToString());
                tokenlist.RemoveAt(0);
                goto A;
            }
            else
            {
                Thread.Sleep(1);
                goto A;
            }
        }
        public void writerefreshtoken(string token, string account)
        {
            Thread.Sleep(1);
            try
            {
                if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "refreshtoken.json"))
                {
                    FileStream fs1 = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "refreshtoken.json", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    StreamWriter sw = new StreamWriter(fs1);
                    JArray ja = JArray.Parse(token);
                    sw.Write(ja.ToString().Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace(" ", ""));
                    sw.Close();
                    fs1.Close();
                }
                else
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "refreshtoken.json";
                    FileInfo fi = new FileInfo(path);
                    if (fi.Length == 0)
                    {
                        FileStream fs1 = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "refreshtoken.json", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                        StreamWriter sw = new StreamWriter(fs1);
                        JArray ja = JArray.Parse(token);
                        sw.Write(ja.ToString().Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace(" ", ""));
                        sw.Close();
                        fs1.Close();
                    }
                    else
                    {
                        FileStream fs1 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        StreamReader sr = new StreamReader(fs1);
                        string read = sr.ReadToEnd();
                        JArray ja = JArray.Parse(read);
                        foreach (var i in ja)
                        {
                            Thread.Sleep(1);
                            if (i.ToString().Contains(account))
                            {
                                ja.Remove(i);
                                break;
                            }
                        }
                        ja.Add(JObject.Parse(token.Replace("[", "").Replace("]", "")));
                        fs1.SetLength(0);
                        FileStream fs0 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        StreamWriter sw = new StreamWriter(fs0);
                        sw.Write(ja.ToString().Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace(" ", ""));
                        sw.Close();
                        fs1.Close();
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        bool alreadystartbrowser = false;
        public void checkusemonitor(CancellationToken ct, CancellationTokenSource cts)
        {
        A: if (Config.UseAdvancemode == "True")
            {
                alreadystartbrowser = true;
                if (alreadystartbrowser)
                {
                    openbrowser();
                    cts.Cancel();
                    if (ct.IsCancellationRequested)
                    {
                        try
                        {
                            ct.ThrowIfCancellationRequested();
                        }
                        catch
                        {

                        }
                    }
                }
            }
            Thread.Sleep(1);
            if (alreadystartbrowser != true)
            {
                goto A;
            }

        }
        public static string checkchrome()
        {
            try
            {
                string str = "chrome";
                string empty = string.Empty;
                string str2 = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\";
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(str2 + str + ".exe", false);
                object value = registryKey.GetValue(empty);
                if (registryKey.GetValueKind(empty) == RegistryValueKind.String)
                {
                    return value.ToString();
                }
            }
            catch
            {
                return null;
            }
            return null;
        }
        public static void openbrowser()
        {
            string ChromePath = Environment.CurrentDirectory + "\\" + "checkouthelp";
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + "cookiedata" + "\\" + Guid.NewGuid().ToString();
            string argument1 = "--user-data-dir=\"" + path + "\"";
            string argument2 = "--no-default-browser-check";
            string argument3 = "--no-first-run";
            string argument4 = "--disable-default-apps";
            string argument5 = "--autoplay-policy=no-user-gesture-required";
            string argument6 = "--enable-automation";
            string argument7 = "--disable-infobars";
            string argument8 = "--load-extension=\"" + ChromePath + "\"";
            string argument9 = "--disable-web-security";
            Process process = new Process();
            process.StartInfo.FileName = checkchrome();
            process.StartInfo.Arguments = argument1 + " " + argument2 + " " + argument3 + " " + argument4 + " " + argument5 + " " + argument6 + " " + argument7 + " " + argument8 + " " + argument9;
            process.StartInfo.UseShellExecute = true;
            process.Start();
            FleckLog.Level = LogLevel.Debug;
            new WebSocketServer("ws://127.0.0.1:64525", true).Start(delegate (IWebSocketConnection socket)
              {
                  socket.OnOpen = delegate ()
                  {
                      allSockets.Add(socket);
                  };
                  socket.OnClose = delegate ()
                  {
                      allSockets.Remove(socket);
                      openbrowser();
                  };
                  socket.OnMessage = delegate (string message)// 接收客户端发送过来的信息
                  {

                      if (message.IndexOf("updatetab") != -1)
                      {
                          if (i == 0)
                          {
                              allSockets.ToList().ForEach(s => s.Send("{\n  \"type\": \"updatetab\",\n  \"proxy\": \"\"\n}"));
                              i++;
                          }

                          return;
                      }
                      if (message.IndexOf("response") != -1)
                      {
                          try
                          {
                              JObject jo = JObject.Parse(message);
                              returnstatus.Add(jo["id"].ToString(), jo);
                          }
                          catch
                          {

                          }
                      }


                  };
              });
        }
        #region
        public void check()
        {
        A:
            Thread.Sleep(1);
            Process[] ps = Process.GetProcesses();
            foreach (Process p in ps)
            {
                try
                {
                    if (p.ProcessName.ToString().Contains("Fiddler") || p.ProcessName.ToString().ToString().Contains("wireshark") || p.ProcessName.ToString().ToString().Contains("Charles") || p.ProcessName.ToString().ToString().Contains("dnSpy"))
                    {
                        string pd2 = "{\"username\":\"MAIO\",\"avatar_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\",\"embeds\":[{\"title\":\"Exception\",\"color\":3329330,\"footer\":{\"text\":\"" + "MAIO" + DateTime.Now.ToLocalTime().ToString() + "\",\"icon_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\"},\"fields\":[{\"name\":\"Key ban\",\"value\":\"" + Config.hwid + "\\t\\t\\t\\tKey:" + Config.hwid + "\\t\\t\\t\\tIp:" + Config.ip + "\",\"inline\":false}]}]}";
                        Http("https://discordapp.com/api/webhooks/517871792677847050/qry12HP2IqJQb2sAfSNBmpUmFPOdPsVXUYY2_yhDgckgznpeVtRpNbwvO1Oma6nMGeK9", pd2);
                        Environment.Exit(0);
                    }
                }
                catch (Exception)
                {
                    Environment.Exit(0);
                }
            }
            goto A;
        }
        #endregion
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
                    var firstKey = Mainwindow.cookiewtime.FirstOrDefault(q => q.Value == cookie).Key;
                    Mainwindow.cookiewtime.Remove(firstKey);
                }
            }
        }
        public class taskset : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

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
            private string tasksite;
            public string Tasksite
            {
                get { return tasksite; }
                set
                {
                    tasksite = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Tasksite"));
                    }
                }
            }
            private string sku;
            public string Sku
            {
                get { return sku; }
                set
                {
                    sku = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Sku"));
                    }
                }
            }
            private string size;
            public string Size
            {
                get { return size; }
                set
                {
                    size = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Size"));
                    }
                }
            }
            private string profile;
            public string Profile
            {
                get { return profile; }
                set
                {
                    profile = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Profile"));
                    }
                }
            }
            private string proxies;
            public string Proxies
            {
                get { return proxies; }
                set
                {
                    proxies = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Proxies"));
                    }
                }
            }
            public string Taskid { get; set; }
            public string monitortask { get; set; }
            public string Quantity { get; set; }
            public string Account { get; set; }

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
        private void createtask_Click(object sender, RoutedEventArgs e)
        {
            Midtransfer.edit = false;
            NewTask nt = new NewTask();
            nt.getTextHandler = Ctask;
            nt.Show();
        }
        public static MonitorProduct mp = new MonitorProduct();
        public class Monitor : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            public string Sku { get; set; }
            public string Region { get; set; }
            public string Taskid { get; set; }
            private string size;
            public string Size
            {
                get { return size; }
                set
                {
                    size = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Size"));
                    }
                }
            }
            private string status;
            public string Status
            {
                get { return status; }
                set
                {
                    status = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("status"));
                    }
                }
            }
            public string imageurl { get; set; }
        }
        private void Ctask(string[] st)
        {
            string taskid = Guid.NewGuid().ToString();
            if (st[4].Replace("\r\n", "") == "")
            {
                st[4] = "RA";
            }
            string profile = "[{\"Taskid\":\"" + taskid + "\",\"Tasksite\":\"" + st[0].Replace("System.Windows.Controls.ComboBoxItem: ", "") + "\",\"Sku\":\"" + st[3].Replace("\r\n", "") + "\"," +
         "\"Size\":\"" + st[4].Replace("\r\n", "") + "\",\"Profile\":\"" + st[2] + "\",\"Proxies\":\"Default\"," +
        "\"Status\":\"IDLE\",\"giftcard\":\"" + st[1] + "\",\"Code\":\"" + st[5] + "\",\"Quantity\":\"" + st[6].Replace("System.Windows.Controls.ComboBoxItem: ", "") + "\",\"AdvanceMonitor\":\"" + st[8] + "\",\"Account\":\"" + st[9] + "\"}]";
            if (st[8] == "True")
            {
                Mainwindow.Advancemonitortask.Add(new Monitor { Region = st[0].Replace("System.Windows.Controls.ComboBoxItem: ", ""), Sku = st[3].Replace("\r\n", ""),Size = st[4].Replace("\r\n", ""), Taskid = taskid});
            }
            else
            {
                Mainwindow.tasklist.Add(taskid, profile.Replace("[", "").Replace("]", ""));
                Mainwindow.task.Add(new taskset { Taskid = taskid, Tasksite = st[0].Replace("System.Windows.Controls.ComboBoxItem: ", ""), Sku = st[3].Replace("\r\n", ""), Size = st[4].Replace("\r\n", ""), Profile = st[2], Proxies = "Default", Status = "IDLE", Quantity = st[6].Replace("System.Windows.Controls.ComboBoxItem: ", ""),Account = st[9] });
            }
            taskwrite(profile);
        }
        public void updatediscount(string discount)
        {
            ArrayList ar = new ArrayList();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "codelist.txt";
            ar = new ArrayList(File.ReadAllLines(path));
            for (int i = 0; i < ar.Count; i++)
            {
                if ((string)ar[i] == discount)
                {
                    ar.RemoveAt(i);
                }
            }
            FileStream fs0 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fs0);
            fs0.SetLength(0);
            for (int i = 0; i < ar.Count; i++)
            {
                if (ar[i].ToString() != "")
                {
                    sw.WriteLine(ar[i]);
                }
            }
            sw.Close();
            fs0.Close();
        }
        public static void taskwrite(string task)
        {
            JArray ja2 = JArray.Parse(task);
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
                    for (int i = 0; i < ja.Count; i++)
                    {
                        if (ja[i]["Taskid"].ToString() == ja2[0]["Taskid"].ToString())
                        {
                            ja.RemoveAt(i);
                            break;
                        }
                    }
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
        private async void start_Click(object sender, RoutedEventArgs e)
        {
            int count = datagrid.SelectedItems.Count;
            taskset tk;
            try
            {
                for (int y = 0; y < count; y++)
                {
                    await Task.Delay(220);
                    tk = (taskset)datagrid.SelectedItems[y];
                    new Task(delegate ()
                    {
                        this.startsta(tk);
                    }).Start();
                }
            }
            catch
            { 

            }
        }
        public void startsta(taskset tk)
        {
            string taskid = Guid.NewGuid().ToString();
            bool monitortask = false;
            if (tk.monitortask == "True")
            {
                monitortask = true;
            }
            if (dic.Keys.Contains(tk.Taskid))
            {
               // MessageBox.Show("Please stop task first");
            }
            else
            {
                tk.Status = "Starting";
                try
                {
                    string giftcard = "";
                    string code = "";
                    if (tk.Tasksite == "NikeCA" || tk.Tasksite == "NikeAU" || tk.Tasksite == "NikeMY" || tk.Tasksite == "NikeNZ" || tk.Tasksite == "NikeSG")
                    {
                        NikeAUCA NA = new NikeAUCA();
                        NA.tk = tk;
                        NA.profile = Mainwindow.allprofile[tk.Profile];
                        NA.pid = tk.Sku;
                        NA.size = tk.Size;
                        NA.Quantity = int.Parse(tk.Quantity);
                        NA.monitortask = monitortask;
                        if (tk.Size == "RA" || tk.Size == "ra" || tk.Size == "" || tk.Size == null || tk.Size == " ")
                        {
                            tk.Size = "RA";
                            NA.randomsize = true;
                        }
                        var cts = new CancellationTokenSource();
                        var ct = cts.Token;
                        Task task1 = new Task(() => { NA.StartTask(ct, cts); }, ct, TaskCreationOptions.LongRunning);
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
                        int random = ran.Next(0, Mainwindow.account.Count);
                        try
                        {
                        A: string[] account = null;
                            bool guest = false;
                            if (tk.Account != null && tk.Account != "")
                            {
                                string sValue = "";
                                if (Mainwindow.account.TryGetValue(tk.Account, out sValue))
                                {
                                    JObject jo = JObject.Parse(sValue);
                                    ArrayList ar = new ArrayList();
                                    foreach (var i in jo)
                                    {
                                        Thread.Sleep(1);
                                        ar.Add(i.ToString().Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace(" ", ""));
                                    }
                                    try
                                    {
                                        randomdic.Add(tk.Account, 0);
                                        account = ar[0].ToString().Split(",");
                                    }
                                    catch (Exception)
                                    {
                                        randomdic[tk.Account] = randomdic[tk.Account] + 1;
                                        if (ar.Count == randomdic[tk.Account])
                                        {
                                            randomdic.Remove(tk.Account);
                                            goto A;
                                        }
                                        if (randomdic[tk.Account] >= ar.Count)
                                        {
                                            account = ar[ar.Count - 1].ToString().Split(",");
                                        }
                                        else
                                        {
                                            account = ar[randomdic[tk.Account]].ToString().Split(",");
                                        }
                                    }
                                }
                                else
                                {
                                    account = tk.Account.Replace(" ", "").Replace("[", "").Replace("]", "").Split(",");
                                }
                            }
                            else
                            {
                                guest = true;
                            }
                            NikeUSUK NSK = new NikeUSUK();
                            if (guest)
                            {
                                NSK.guest = true;
                            }
                            else
                            {
                                NSK.username = account[0];
                                NSK.password = account[1];
                            }
                            NSK.giftcard = giftcard;
                            NSK.pid = tk.Sku;
                            NSK.size = tk.Size;
                            NSK.code = code;
                            NSK.monitortask = monitortask;
                            NSK.profile = Mainwindow.allprofile[tk.Profile];
                            NSK.tk = tk;

                            if (tk.Size == "RA" || tk.Size == "ra" || tk.Size == "" || tk.Size == null || tk.Size == " ")
                            {
                                tk.Size = "RA";
                                NSK.randomsize = true;
                            }
                            var cts = new CancellationTokenSource();
                            var ct = cts.Token;
                            Task task2 = new Task(() => { NSK.StartTask(ct, cts); }, ct, TaskCreationOptions.LongRunning);
                            dic.Add(tk.Taskid, cts);
                            task2.Start();

                        }
                        catch (Exception)
                        {
                            tk.Status = "No Account";
                        }
                    }
                    else if (tk.Tasksite == "Footasylum")
                    {
                        try
                        {
                            Footasylum fasy = new Footasylum();
                            if (tk.Size == "RA" || tk.Size == "ra" || tk.Size == "" || tk.Size == null || tk.Size == " ")
                            {
                                tk.Size = "RA";
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
                        TheNorthFaceUSUK tnfus = new TheNorthFaceUSUK();
                        tnfus.link = tk.Sku;
                        tnfus.profile = Mainwindow.allprofile[tk.Profile];
                        tnfus.size = tk.Size;
                        if (tk.Size == "RA" || tk.Size == "ra" || tk.Size == "" || tk.Size == null || tk.Size == " ")
                        {
                            tk.Size = "RA";
                            tnfus.randomsize = true;
                        }
                        tnfus.tasksite = tk.Tasksite;
                        tnfus.tk = tk;
                        var cts = new CancellationTokenSource();
                        var ct = cts.Token;
                        Task tnftask = new Task(() => { tnfus.StartTask(ct, cts); }, ct);
                        dic.Add(tk.Taskid, cts);
                        tnftask.Start();
                    }
                    else if (tk.Tasksite == "TheNorthFaceUK")
                    {
                        TheNorthFaceUSUK tnfus = new TheNorthFaceUSUK();
                        tnfus.link = tk.Sku;
                        tnfus.profile = Mainwindow.allprofile[tk.Profile];
                        tnfus.size = tk.Size;
                        tnfus.tasksite = tk.Tasksite;
                        if (tk.Size == "RA" || tk.Size == "ra" || tk.Size == "" || tk.Size == null || tk.Size == " ")
                        {
                            tk.Size = "RA";
                            tnfus.randomsize = true;
                        }
                        tnfus.tk = tk;
                        var cts = new CancellationTokenSource();
                        var ct = cts.Token;
                        Task tnftask = new Task(() => { tnfus.StartTask(ct, cts); }, ct);
                        dic.Add(tk.Taskid, cts);
                        tnftask.Start();
                    }
                    else if (tk.Tasksite == "ASOS")
                    {
                        ASOS.ASOS asos = new ASOS.ASOS();
                        asos.link = tk.Sku;
                        asos.profile = Mainwindow.allprofile[tk.Profile];
                        asos.size = tk.Size;
                        if (tk.Size == "RA" || tk.Size == "ra" || tk.Size == "" || tk.Size == null || tk.Size == " ")
                        {
                            tk.Size = "RA";
                            asos.randomsize = true;
                        }
                        asos.tk = tk;
                        var cts = new CancellationTokenSource();
                        var ct = cts.Token;
                        Task asostask = new Task(() => { asos.StartTask(ct, cts); }, ct);
                        dic.Add(tk.Taskid, cts);
                        asostask.Start();
                    }
                    else if (tk.Tasksite == "JDUS")
                    {
                        JDUS.JDUS jdus = new JDUS.JDUS();
                        jdus.link = tk.Sku;
                        jdus.profile = Mainwindow.allprofile[tk.Profile];
                        jdus.size = tk.Size;
                        if (tk.Size == "RA" || tk.Size == "ra" || tk.Size == "" || tk.Size == null || tk.Size == " ")
                        {
                            tk.Size = "RA";
                            jdus.randomsize = true;
                        }
                        jdus.tk = tk;
                        var cts = new CancellationTokenSource();
                        var ct = cts.Token;
                        Task jdustask = new Task(() => { jdus.StartTask(ct, cts); }, ct);
                        dic.Add(tk.Taskid, cts);
                        jdustask.Start();
                    }
                }
                catch (Exception)
                {
                    tk.Status = "Task Error";
                }
            }
            // }

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
            try
            {
                taskset del;
                int count = datagrid.SelectedItems.Count;
                ArrayList ary = new ArrayList();
                for (int y = 0; y < count; y++)
                {
                    del = (taskset)datagrid.SelectedItems[y];
                    ary.Add(del);
                    if (dic.Keys.Contains(del.Taskid))
                    {
                        dic[del.Taskid].Cancel();
                        dic.Remove(del.Taskid);
                    }
                    for (int i = 0; i < Mainwindow.tasklist.Count; i++)
                    {
                        Thread.Sleep(1);
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
                }
                for (int i = 0; i < ary.Count; i++)
                {
                    Mainwindow.task.Remove((taskset)ary[i]);
                }
            }
            catch { }
        }
        private void button1_Copy3_Click(object sender, RoutedEventArgs e)
        {
            string path2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "task.json";
            FileStream fs1 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            fs1.SetLength(0);
            Mainwindow.tasklist.Clear();
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
        }
        private void datagrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var content = (taskset)datagrid.SelectedItem;
            try
            {
                NewTask nt = new NewTask();
                Midtransfer.sitesel = content.Tasksite;
                Midtransfer.pid = content.Sku;
                Midtransfer.taskid = content.Taskid;
                Midtransfer.sizeid = content.Size;
                Midtransfer.profilesel = content.Profile;
                if (Mainwindow.tasklist[content.Taskid] != "")
                {
                    JObject jo = JObject.Parse(Mainwindow.tasklist[content.Taskid]);
                    Midtransfer.giftcard = jo["giftcard"].ToString();
                    Midtransfer.code = jo["Code"].ToString();
                    Midtransfer.Quantity = jo["Quantity"].ToString();
                    Midtransfer.tk = content;
                    for (int i = 0; i < Mainwindow.task.Count; i++)
                    {
                        if (Mainwindow.task[i].monitortask == "True" && Mainwindow.task[i].Taskid == content.Taskid)
                        {
                            Midtransfer.monitor = true;
                            break;
                        }
                        else if (Mainwindow.task[i].Taskid == content.Taskid && Mainwindow.task[i].monitortask == "False")
                        {
                            Midtransfer.monitor = false;
                            break;
                        }
                    }
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
        private async void startall_Click(object sender, RoutedEventArgs e)
        {
            new Task(delegate ()
            {
                this.startsta(Mainwindow.task.ElementAt(0));
            }).Start();
            try
            {
                for (int i = 0; i < Mainwindow.task.Count-1; i++)
                {
                    await Task.Delay(220);
                    new Task(delegate ()
                    {
                        this.startsta(Mainwindow.task.ElementAt(i));
                    }).Start();
                }
            }
            catch { }
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
            var content = (taskset)datagrid.SelectedItem;
            try
            {
                NewTask nt = new NewTask();
                Midtransfer.sitesel = content.Tasksite;
                Midtransfer.pid = content.Sku;
                Midtransfer.taskid = content.Taskid;
                Midtransfer.sizeid = content.Size;
                Midtransfer.profilesel = content.Profile;
                if (Mainwindow.tasklist[content.Taskid] != "")
                {
                    JObject jo = JObject.Parse(Mainwindow.tasklist[content.Taskid]);
                    Midtransfer.giftcard = jo["giftcard"].ToString();
                    Midtransfer.code = jo["Code"].ToString();
                    Midtransfer.Quantity = jo["Quantity"].ToString();
                    Midtransfer.tk = content;
                    if (Mainwindow.tasklist[content.Taskid].Contains("\"monitortask\": \"True\""))
                    {
                        Midtransfer.monitor = true;
                    }
                    else
                    {
                        Midtransfer.monitor = false;
                    }
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
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                mp.Show();
            }
            catch
            { }
        }
        public static void autorestock(taskset tk)
        {
            if (dic.Keys.Contains(tk.Taskid) == false)
            {
            }
            else
            {
                dic[tk.Taskid].Cancel();
                dic.Remove(tk.Taskid);
            }
            bool monitortask = false;
            if (tk.monitortask == "True")
            {
                monitortask = true;
            }
            if (dic.Keys.Contains(tk.Taskid))
            {
                try
                {
                    dic[tk.Taskid].Cancel();
                    dic.Remove(tk.Taskid);
                }
                catch { }
            }
            else
            {
                tk.Status = "Starting";
                try
                {
                    string giftcard = "";
                    string code = "";
                    if (tk.Tasksite == "NikeCA" || tk.Tasksite == "NikeAU" || tk.Tasksite == "NikeNZ" || tk.Tasksite == "NikeSG" || tk.Tasksite == "NikeMY")
                    {
                        NikeAUCA NA = new NikeAUCA();
                        NA.tk = tk;
                        NA.profile = Mainwindow.allprofile[tk.Profile];
                        NA.pid = tk.Sku;
                        NA.size = tk.Size;
                        NA.Quantity = int.Parse(tk.Quantity);
                        NA.monitortask = monitortask;
                        if (tk.Size == "RA" || tk.Size == "ra" || tk.Size == "" || tk.Size == null || tk.Size == " ")
                        {
                            tk.Size = "RA";
                            NA.randomsize = true;
                        }
                        var cts = new CancellationTokenSource();
                        var ct = cts.Token;
                        Task task1 = new Task(() => { NA.StartTask(ct, cts); }, ct, TaskCreationOptions.LongRunning);
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
                        int random = ran.Next(0, Mainwindow.account.Count);
                        try
                        {
                        A: string[] account = null;
                            bool guest = false;
                            if (tk.Account != null && tk.Account != "")
                            {
                                string sValue = "";
                                if (Mainwindow.account.TryGetValue(tk.Account, out sValue))
                                {
                                    JObject jo = JObject.Parse(sValue);
                                    ArrayList ar = new ArrayList();
                                    foreach (var i in jo)
                                    {
                                        Thread.Sleep(1);
                                        ar.Add(i.ToString().Replace("{", "").Replace("}", "").Replace("[", "").Replace("]", "").Replace(" ", ""));
                                    }
                                    try
                                    {
                                        randomdic.Add(tk.Account, 0);
                                        account = ar[0].ToString().Split(",");
                                    }
                                    catch (Exception)
                                    {
                                        randomdic[tk.Account] = randomdic[tk.Account] + 1;
                                        if (ar.Count == randomdic[tk.Account])
                                        {
                                            randomdic.Remove(tk.Account);
                                            goto A;
                                        }
                                        if (randomdic[tk.Account] >= ar.Count)
                                        {
                                            account = ar[ar.Count - 1].ToString().Split(",");
                                        }
                                        else
                                        {
                                            account = ar[randomdic[tk.Account]].ToString().Split(",");
                                        }
                                    }
                                }
                                else
                                {
                                    account = tk.Account.Replace(" ", "").Replace("[", "").Replace("]", "").Split(",");
                                }
                            }
                            else
                            {
                                guest = true;
                            }
                            NikeUSUK NSK = new NikeUSUK();
                            if (guest)
                            {
                                NSK.guest = true;
                            }
                            else
                            {
                                NSK.username = account[0];
                                NSK.password = account[1];
                            }
                            NSK.giftcard = giftcard;
                            NSK.pid = tk.Sku;
                            NSK.size = tk.Size;
                            NSK.code = code;
                            NSK.monitortask = monitortask;
                            NSK.profile = Mainwindow.allprofile[tk.Profile];
                            NSK.tk = tk;
                            if (tk.Size == "RA" || tk.Size == "ra" || tk.Size == "" || tk.Size == null || tk.Size == " ")
                            {
                                tk.Size = "RA";
                                NSK.randomsize = true;
                            }
                            var cts = new CancellationTokenSource();
                            var ct = cts.Token;
                            Task task2 = new Task(() => { NSK.StartTask(ct, cts); }, ct, TaskCreationOptions.LongRunning);
                            dic.Add(tk.Taskid, cts);
                            task2.Start();
                        }
                        catch (Exception)
                        {

                            tk.Status = "No Account";
                        }
                    }
                }
                catch
                {

                }
            }
        }
        public static void Http(string url, string postDataStr)
        {
        Retry: Random ra = new Random();
            int sleeptime = ra.Next(0, 3000);
            Thread.Sleep(sleeptime);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "post";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            byte[] bytes = Encoding.UTF8.GetBytes(postDataStr);
            request.ContentLength = bytes.Length;
            Stream webstream = request.GetRequestStream();
            webstream.Write(bytes, 0, bytes.Length);
            webstream.Close();
            try
            {
                HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8);
                string result = streamReader.ReadToEnd();
            }
            catch (WebException ex)
            {
                goto Retry;
            }

        }
    }
}
