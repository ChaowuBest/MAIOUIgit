using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Input.Manipulations;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MAIO
{
    /// <summary>
    /// Proxy.xaml 的交互逻辑
    /// </summary>
    public partial class Proxy : Page
    {
        string[] sArray = new string[4];
        Dictionary<string, string> dicproxytest = new Dictionary<string, string>();
        public Proxy()
        {   
            InitializeComponent();
            if (Mainwindow.proxylist != null)
            {
            }
            proxylistview.ItemsSource = Mainwindow.proxy;
        }
        public class Proxyclass : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            private string index;
            public string Index
            {
                get { return index; }
                set
                {
                    index = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Index"));
                    }
                }
            }
            private string name;
            public string Name
            {
                get { return name; }
                set
                {
                    name = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                    }
                }
            }

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Writecoookie.write();
            Application.Current.Shutdown();
        }
        private void test_Click(object sender, RoutedEventArgs e)
        {
            /*  testbox.Document.Blocks.Clear();
              try
              {
                  if (Mainwindow.proxypool.Count != 0)
                  {
                      for (int i = 0; i < Mainwindow.proxypool.Count; i++)
                      {
                          Thread thread = new Thread(new ParameterizedThreadStart(testproxy));
                          thread.Start(Mainwindow.proxypool[i]);
                      }
                  }
              }
              catch (Exception)
              {
                  MessageBox.Show("Test failed please check your input");
              }*/

        }
        public void testproxy(object pro)
        {
            /*   WebProxy wp = new WebProxy();
               sArray = pro.ToString().Split(':');
               if (sArray.Length == 2)
               {
                   wp.Address = new Uri("http://" + sArray[0] + ":" + sArray[1] + "/");
               }
               else
               {
                   wp.Address = new Uri("http://" + sArray[0] + ":" + sArray[1] + "/");
                   wp.Credentials = new NetworkCredential(sArray[2], sArray[3]);
               }
               bool bad = false;
               string url = "https://www.nike.com";
               HiPerfTimer hi = new HiPerfTimer();
               HttpWebRequest test = (HttpWebRequest)WebRequest.Create(url);
               test.Proxy = wp;
               hi.Start();
               test.Timeout = 5000;
               test.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
               test.Method = "GET";
               test.ContentType = "application/json; charset=UTF-8";
               test.Accept = "application/json";
               test.Headers.Add("Accept-Encoding", "gzip, deflate, br");
               test.Headers.Add("appid", "com.nike.commerce.nikedotcom.web");
               test.Headers.Add("Accept-Language", "en-US, en; q=0.9");
               try
               {
                   HttpWebResponse resptest = (HttpWebResponse)test.GetResponse();
                   hi.Stop();
               }
               catch (WebException)
               {
                   try
                   {
                       dicproxytest.Add(wp.Address.ToString().Replace("http://", "").Replace("/", ""), "failed");
                   }
                   catch (ArgumentException)
                   {

                   }
                   testbox.Dispatcher.Invoke(new Action(
                          delegate
                          {
                              testbox.AppendText(wp.Address.ToString().Replace("http://", "").Replace("/", "") + "\t\t\t\tfailed");
                              testbox.AppendText("\r\n");
                          }
                     ));
                   bad = true;
               }
               if (bad == false)
               {
                   int dua = (int)hi.Duration;
                   try
                   {
                       dicproxytest.Add(wp.Address.ToString().Replace("http://", "").Replace("/", ""), dua.ToString());
                   }
                   catch (ArgumentException)
                   {

                   }
                   testbox.Dispatcher.Invoke(new Action(
                           delegate
                           {
                               testbox.AppendText(wp.Address.ToString().Replace("http://", "").Replace("/", "") + "\t\t\t\t" + dua.ToString() + "");
                               testbox.AppendText("\r\n");
                           }
                      ));
               }*/
        }
        private void clearfailed_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dicproxytest.Count != 0)
                {
                    string path2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "proxy.txt";
                    FileStream fs0 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    StreamWriter sw = new StreamWriter(fs0);
                    fs0.SetLength(0);
                    var keys = dicproxytest.Where(q => q.Value == "failed").Select(q => q.Key);  //get all keys  
                    List<string> keyList = (from q in dicproxytest
                                            where q.Value == "failed"
                                            select q.Key).ToList<string>();
                    for (int i = 0; i < Mainwindow.proxypool.Count; i++)
                    {
                        for (int n = 0; n < keyList.Count; n++)
                        {
                            if (Mainwindow.proxypool[i].ToString().Contains(keyList[n]))
                            {
                                Mainwindow.proxypool.RemoveAt(i);
                            }
                        }
                    }
                    for (int i = 0; i < Mainwindow.proxypool.Count; i++)
                    {
                        if (Mainwindow.proxypool[i].ToString() != "")
                        {
                            sw.WriteLine(Mainwindow.proxypool[i].ToString());
                        }
                    }
                    sw.Close();
                    fs0.Close();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Clear fail failed");
            }
        }
        private void saveproxy(object sender, RoutedEventArgs e)
        {
            if (proxybox.Text != "")
            {
                bool duplicate = false;
                string key = null;
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "proxy.json";
                string[] saveproxy = proxybox.Text.Split("\n");
                JObject ja = new JObject();
                for (int i = 0; i < saveproxy.Length; i++)
                {
                    if (saveproxy[i] != "")
                    {
                        ja.Add(i.ToString(), saveproxy[i]);
                    }
                }
                JObject jo = new JObject(
                  new JProperty(proxylistname.Text,
                  new JObject(ja))
                 );
                for (int i = 0; i < Mainwindow.proxylist.Count; i++)
                {
                    KeyValuePair<string, string> kv = Mainwindow.proxylist.ElementAt(i);
                    if (kv.Key == proxylistname.Text)
                    {
                        duplicate = true;
                        key = kv.Key;
                        break;
                    }
                }
                if (duplicate)
                {
                    Mainwindow.proxylist[key] = ja.ToString();
                }
                else
                {
                    if (proxylistname.Text == "")
                    {
                        MessageBox.Show("Please enter proxylistname ");
                    }
                }
                try
                {
                    if (duplicate == false)
                    {
                        Mainwindow.proxylist.Add(proxylistname.Text, ja.ToString());
                        Mainwindow.proxy.Add(new Proxyclass { Index = (Mainwindow.proxy.Count + 1).ToString(), Name = proxylistname.Text });
                    }
                    FileInfo fi = new FileInfo(path);
                    if (fi.Length == 0)
                    {
                        var jot = jo.ToString().Insert(0, "[").Insert(jo.ToString().Length + 1, "]");
                        FileStream fs0 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        StreamWriter sw = new StreamWriter(fs0);
                        sw.Write(jot.ToString().Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", ""));
                        sw.Close();
                        fs0.Close();
                    }
                    else
                    {
                        FileStream fs1 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        StreamReader sw2 = new StreamReader(fs1);
                        var read = sw2.ReadToEnd();
                        JArray ja2 = JArray.Parse(read);
                        for (int i = 0; i < ja2.Count; i++)
                        {
                            Regex rex = new Regex("\"(.*)\"");
                            var matchkey = rex.Match(ja2[i].ToString()).Value.Replace("\"", "");
                            if (matchkey == proxylistname.Text.Replace(" ", ""))
                            {
                                ja2.RemoveAt(i);
                            }
                        }
                        ja2.Add(jo);
                        var wu = ja2.ToString();
                        FileStream fs0 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        StreamWriter sw = new StreamWriter(fs0);
                        fs1.SetLength(0);
                        sw.Write(ja2.ToString().Replace("\r", "").Replace("\n", "").Replace("\t", "").Replace(" ", ""));
                        sw.Close();
                        fs0.Close();
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("Please check your input");
                }
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var del = (Proxyclass)((Button)sender).DataContext;
            string needdel = Mainwindow.proxylist[del.Name];
            Mainwindow.proxylist.Remove(del.Name);
            Mainwindow.proxy.Remove(del);
            string path2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "proxy.json";
           updateproxy(del.Name, path2);

        }
        private void updateproxy(string gft, string path2)
        {
            FileStream fs0 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs0);
            string pro = sr.ReadToEnd();
            JArray ja = JArray.Parse(pro);
            var test = ja.ToString();
            sr.Close();
            fs0.Close();
            foreach (var item in ja)
            {
                if (item[gft] != null)
                {
                    item.Remove();
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
    }
}
