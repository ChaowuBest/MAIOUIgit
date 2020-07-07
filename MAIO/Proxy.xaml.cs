using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
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
            if (Mainwindow.proxypool != null)
            {
                for (int i = 0; i < Mainwindow.proxypool.Count; i++)
                {
                    proxybox.AppendText(Mainwindow.proxypool[i].ToString());
                    proxybox.AppendText("\r\n");
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Writecoookie.write();
            Application.Current.Shutdown();
        }
        private void save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (new TextRange(proxybox.Document.ContentStart, proxybox.Document.ContentEnd).Text == "")
                {
                    MessageBox.Show("No proxy");
                }
                else
                {
                    string path2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "proxy.txt";
                    string[] saveproxy = new TextRange(proxybox.Document.ContentStart, proxybox.Document.ContentEnd).Text.Split("\r\n");
                    FileStream fs0 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    StreamWriter sw = new StreamWriter(fs0);
                    fs0.SetLength(0);
                    Mainwindow.proxypool.Clear();
                    for (int i = 0; i < saveproxy.Length - 1; i++)
                    {
                        if (saveproxy[i] != "")
                        {
                            sw.WriteLine(saveproxy[i]);
                            Mainwindow.proxypool.Add(saveproxy[i]);
                        }
                    }
                    sw.Close();
                    fs0.Close();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Save proxy failed, please Check your input");
            }

        }

        private void del_Click(object sender, RoutedEventArgs e)
        {
            string path2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "proxy.txt";
            FileStream fs0 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            fs0.SetLength(0);
            fs0.Close();
            Mainwindow.proxypool.Clear();
            proxybox.Document.Blocks.Clear();
        }

        private void test_Click(object sender, RoutedEventArgs e)
        {
            testbox.Document.Blocks.Clear();
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
            }

        }
        public void testproxy(object pro)
        {
            WebProxy wp = new WebProxy();
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
            }
        }
        private void proxttest_SizeChanged(object sender, SizeChangedEventArgs e)
        {
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
    }
}
