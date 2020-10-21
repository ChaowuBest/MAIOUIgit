using System;
using System.Collections.Generic;
using System.IO;
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

namespace MAIO
{
    /// <summary>
    /// CookieGen.xaml 的交互逻辑
    /// </summary>
    public partial class CookieGen : Window
    {
        public CookieGen()
        {
            InitializeComponent();
            if (Mainwindow.proxypool != null)
            {
                for (int i = 0; i < Mainwindow.cookieproxypool.Count; i++)
                {
                    monitorproxy.AppendText(Mainwindow.cookieproxypool[i].ToString());
                    monitorproxy.AppendText("\r\n");
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings settings = new Settings();
                Task task1 = new Task(() =>settings.ws("NIKE"));
                task1.Start();
                Task task2 = new Task(() => settings.ws("NIKE"));
                task2.Start();
            }
            catch
            {
                MessageBox.Show("You must have chrome");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings settings = new Settings();
                Task task1 = new Task(() => settings.ws("TNF"));
                task1.Start();
                Task task2 = new Task(() => settings.ws("TNF"));
                task2.Start();
            }
            catch
            {
                MessageBox.Show("You must have chrome");
            }
        }

        private void del_Copy_Click(object sender, RoutedEventArgs e)
        {
            string path2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "cookieproxy.txt";
            FileStream fs0 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            fs0.SetLength(0);
            fs0.Close();
            Mainwindow.cookieproxypool.Clear();
            monitorproxy.Document.Blocks.Clear();
        }

        private void save_Copy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (new TextRange(monitorproxy.Document.ContentStart, monitorproxy.Document.ContentEnd).Text == "")
                {
                    MessageBox.Show("No proxy");
                }
                else
                {
                    string path2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "cookieproxy.txt";
                    string[] saveproxy = new TextRange(monitorproxy.Document.ContentStart, monitorproxy.Document.ContentEnd).Text.Split("\r\n");
                    FileStream fs0 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                    StreamWriter sw = new StreamWriter(fs0);
                    fs0.SetLength(0);
                    Mainwindow.cookieproxypool.Clear();
                    for (int i = 0; i < saveproxy.Length - 1; i++)
                    {
                        if (saveproxy[i] != "")
                        {
                            sw.WriteLine(saveproxy[i]);
                            Mainwindow.cookieproxypool.Add(saveproxy[i]);
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
    }
}
