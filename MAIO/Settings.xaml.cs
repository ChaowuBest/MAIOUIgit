using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static MAIO.LoginWindow;

namespace MAIO
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : UserControl
    {
        //public Mainwindow ParentWindow { get; set; }
        public Settings()
        {
            InitializeComponent();
            Cid.Text = Config.cid;
            Cjevent.Text = Config.cjevent;
            discordwebhook.Text = Config.webhook;
            delay2.Text = Config.delay;
            if (Config.Usemonitor.Contains("True"))
            {
                Usemonitor.IsChecked = true;
            }
            else
            {
                Usemonitor.IsChecked = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Deactive_Click(object sender, RoutedEventArgs e)
        {
            string md5checkdoublekey = MD5Helper.EncryptString(Config.Key);
            Settings ss = new Settings();
            bool resetstatus = ss.KeyRest(md5checkdoublekey);
            if (resetstatus == true)
            {
                Thread.Sleep(5000);
                Application.Current.Shutdown();
            }
            else
            {
                Thread.Sleep(5000);
                Application.Current.Shutdown();
            }
        }
        public bool KeyRest(string md5key)
        {
            var binding = new BasicHttpBinding();
            var endpoint = new EndpointAddress(@"http://100.26.188.137:8090/WebService1.asmx");
            var factory = new ChannelFactory<ServiceReference2.WebService1Soap>(binding, endpoint);
            var callClient = factory.CreateChannel();
            var resetkey = callClient.KeyresetAsync(md5key);
            return resetkey.Result;
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Config.webhook = discordwebhook.Text;
                Config.cid = Cid.Text;
                Config.cjevent = Cjevent.Text;
                Config.Usemonitor=Usemonitor.IsChecked.ToString();
                Random ra = new Random();
                if (delay2.Text == "")
                {
                    MessageBox.Show("Please fill all information");
                }
                else
                {
                    Config.delay=delay2.Text;
                } 
                string config = "{\"webhook\":\"" + Config.webhook + "\",\"key\":\"" + Config.Key + "\",\"cid\":\"" + Config.cid + "\",\"cjevent\":\"" + Config.cjevent + "\",\"delay\":\"" + Config.delay + "\",\"Usemonitor\":\"" + Usemonitor.IsChecked.ToString() + "\"}";
                File.WriteAllText(Environment.CurrentDirectory + "\\" + "config.json", config);
                MessageBox.Show("Save success");
            }
            catch
            {
                MessageBox.Show("fail save");
            }
        }

        private void Check_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string cardurl = "https://api.nike.com/payment/giftcard_balance/v1";
                ArrayList checkdcardlist = new ArrayList();
                CheckCard cc = new CheckCard();
                string[] card = new TextRange(Balancebox.Document.ContentStart, Balancebox.Document.ContentEnd).Text.Split("\r\n");
                for (int i = 0; i < card.Length - 1; i++)
                {
                    var sp = card[0].Split("-");
                    string cardinfo = "{\"accountNumber\":\"" + sp[0] + "\",\"pin\":\"" + sp[1] + "\",\"currency\":\"USD\"}";
                    checkdcardlist.Add(cc.Postcardinfo(cardurl, cardinfo));              
                }
                Balancebox.Document.Blocks.Clear();
                for (int i = 0; i < checkdcardlist.Count; i++)
                {
                    Run run = new Run(checkdcardlist[i].ToString());
                    Paragraph p = new Paragraph();
                    p.Inlines.Add(run);
                    Balancebox.Document.Blocks.Add(p);
                }
            }
            catch (Exception)
            {
               
                Run run = new Run("Error Input");
                Paragraph p = new Paragraph();
                p.Inlines.Add(run);
                Balancebox.Document.Blocks.Add(p);
            }
        }
    }
}
