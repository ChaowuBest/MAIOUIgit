using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Channels;
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
namespace MAIO
{
    /// <summary>
    /// Account.xaml 的交互逻辑
    /// </summary>
    public partial class Account : UserControl
    {
        public Account()
        {
            InitializeComponent();
            accountlist.ItemsSource = Mainwindow.Acccountclass;
            giftcardlist.ItemsSource = Mainwindow.Giftcardclass;
        }
        public class AccountClass : INotifyPropertyChanged
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
        public class GiftcardClass : INotifyPropertyChanged
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
        public void updategiftcard(string gft, string path2)
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
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            EditAccount.editaccount = false;
            Editgiftcard.editgiftcard = false;
            AddAccount ad = new AddAccount();
            ad.getTextHandler = Addaccountorgiftcard;
            ad.Show();
        }
        public void Addaccountorgiftcard(bool st, string name, bool account)
        {
            if (account == true && st == false)
            {
                Mainwindow.Acccountclass.Add(new AccountClass { Index = (Mainwindow.Acccountclass.Count + 1).ToString(), Name = name });
            }
            else if (account == false && st == false)
            {
                Mainwindow.Giftcardclass.Add(new GiftcardClass { Index = (Mainwindow.Giftcardclass.Count + 1).ToString(), Name = name });
            }
        }
        private void delaccount(object sender, RoutedEventArgs e)
        {
            var del = (AccountClass)((Button)sender).DataContext;
            Mainwindow.account.Remove(del.Name);
            Mainwindow.Acccountclass.Remove(del);
            string path2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "account.json";
            updateaccount(del.Name, path2);
        }
        private void updateaccount(string gft, string path2)
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
        private void delgiftcard(object sender, RoutedEventArgs e)
        {
            var del = (GiftcardClass)((Button)sender).DataContext;
            Mainwindow.giftcardlist.Remove(del.Name);
            Mainwindow.Giftcardclass.Remove(del);
            string path2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "giftcard.json";
            updateaccount(del.Name, path2);
        }
        private void accountlist_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            var account =(AccountClass)accountlist.SelectedItem;
            try
            {
                string selectdata = Mainwindow.account[account.Name];
                var gl = selectdata.Replace("\"", "").Replace("{", "").Replace("}", "").Replace(" ", "").Replace(",", "").Replace(":", "-").Split("\r\n");
                EditAccount.account= gl;
                EditAccount.editaccount = true;
                EditAccount.accountname = account.Name;
                AddAccount ad = new AddAccount();
                ad.getTextHandler = Addaccountorgiftcard;
                ad.Show();       
            }
            catch
            {
            }
        }
        private void giftcardlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var giftcard = (GiftcardClass)giftcardlist.SelectedItem;
            try
            {
                string selectdata = Mainwindow.giftcardlist[giftcard.Name];
                var gl = selectdata.Replace("\"", "").Replace("{", "").Replace("}", "").Replace(" ", "").Replace(",", "").Replace(":", "-").Split("\r\n");
                Editgiftcard.giftcardlist = gl;
                Editgiftcard.editgiftcard = true;
                Editgiftcard.giftcardlistname = giftcard.Name;
                AddAccount ad = new AddAccount();
                ad.getTextHandler = Addaccountorgiftcard;
                ad.Show();
            }
            catch
            {
            }
        }
    }
}
