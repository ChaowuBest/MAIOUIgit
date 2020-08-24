﻿using Newtonsoft.Json;
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
        private void giftlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            /*  try
              {
                  giftcardbox.Document.Blocks.Clear();
                  string selectdata = Mainwindow.giftcardlist["" + giftlist.SelectedItem.ToString() + ""];
                  giftcardname.Text = giftlist.SelectedItem.ToString();
                  var gl = selectdata.Replace("\"", "").Replace("{", "").Replace("}", "").Replace(" ", "").Replace(",", "").Replace(":", "-").Split("\r\n");
                  for (int i = 0; i < gl.Length; i++)
                  {
                      if (gl[i] != "")
                      {
                          giftcardbox.AppendText(gl[i]);
                          giftcardbox.AppendText("\r\n");
                      }
                  }
              }
              catch
              {

              }*?

          }
          private void BtnDelete_Click(object sender, RoutedEventArgs e)
          {
           /*   var del = ((Button)sender).DataContext.ToString();
              giftlist.Items.Remove(del);
              string needdel = Mainwindow.giftcardlist[del];
              Mainwindow.giftcardlist.Remove(del);
              giftlist.Items.Refresh();
              string path2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "giftcard.json";
              updategiftcard(del, path2);*/

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
        private void accountlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            /*  try
              {
                  accountbox.Document.Blocks.Clear();
                  string selectdata = Mainwindow.account["" + accountlist.SelectedItem.ToString() + ""];
                  var gl = selectdata.Replace("\"", "").Replace("{", "").Replace("}", "").Replace(" ", "").Replace(",", "").Replace(":", "-").Split("\r\n");
                  for (int i = 0; i < gl.Length; i++)
                  {
                      if (gl[i] != "")
                      {
                          accountbox.AppendText(gl[i]);
                          accountbox.AppendText("\r\n");
                      }
                  }
              }
              catch
              {

              }*/
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
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
    }
}
