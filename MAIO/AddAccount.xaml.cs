using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
    /// AddAccount.xaml 的交互逻辑
    /// </summary>
    public partial class AddAccount : Window
    {
        public AddAccount()
        {
            InitializeComponent();
        }
        public delegate void GetTextHandler(bool st, string name,bool account); //声明委托
        public GetTextHandler getTextHandler;
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
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
            }
            catch
            {

            }
        }
        private void save(object sender, RoutedEventArgs e)
        {
            if (giftaccount.Text != "")
            {
                if (giftaccountbox.Text.Contains("@"))
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "account.json";
                    string[] saveaccount = giftaccountbox.Text.Split("\n");
                    JObject ja = new JObject();
                    JObject jo = null;
                    try
                    {
                        for (int i = 0; i < saveaccount.Length; i++)
                        {
                            if (saveaccount[i] != "")
                            {
                                if (saveaccount[i].Contains("-"))
                                {
                                    var sp = saveaccount[i].Split("-");
                                    ja.Add(sp[0], sp[1]);
                                }
                                else if (saveaccount[i].Contains(":"))
                                {
                                    var sp = saveaccount[i].Split(":");
                                    ja.Add(sp[0], sp[1]);
                                }
                            }
                        }
                       jo = new JObject(
                          new JProperty(giftaccount.Text,
                          new JObject(ja))
                         );
                    }
                    catch
                    {
                        MessageBox.Show("Account Input Error");
                    }
                    string sValue = "";
                    if (Mainwindow.account.TryGetValue(giftaccount.Text, out sValue))
                    {
                        Mainwindow.account[giftaccount.Text] = ja.ToString();
                        updatedetail(path,jo, giftaccount.Text);
                        getTextHandler(true, giftaccount.Text,true);
                    }
                    else
                    {
                        Mainwindow.account.Add(giftaccount.Text, ja.ToString());
                        updatedetail(path, jo,giftaccount.Text);
                        getTextHandler(false, giftaccount.Text,true);
                    }
                }
                else
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "giftcard.json";
                    string[] savegiftcard = giftaccountbox.Text.Split("\n");
                    JObject ja = new JObject();
                    JObject jo = null;
                    try
                    {
                        for (int i = 0; i < savegiftcard.Length; i++)
                        {
                            if (savegiftcard[i] != "")
                            {
                                var sp = savegiftcard[i].Split("-");
                                ja.Add(sp[0], sp[1]);
                            }
                        }
                        jo = new JObject(
                         new JProperty(giftaccount.Text,
                         new JObject(ja))
                        );
                     
                    }
                    catch
                    {
                        MessageBox.Show("Giftcard Input Error");
                    }
                    string sValue = "";
                    if (Mainwindow.giftcardlist.TryGetValue(giftaccount.Text, out sValue))
                    {
                        Mainwindow.giftcardlist[giftaccount.Text] = ja.ToString();
                        updatedetail(path, jo, giftaccount.Text);
                        getTextHandler(true, giftaccount.Text, false);
                    }
                    else
                    {
                        Mainwindow.giftcardlist.Add(giftaccount.Text, ja.ToString());
                        updatedetail(path, jo, giftaccount.Text);
                        getTextHandler(false, giftaccount.Text, false);
                    }
                }
            }    
        }
        public void updatedetail(string path,JObject jo,string name)
        {
            try
            {
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
                        if (matchkey == name.Replace(" ", ""))
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
            catch
            {
                MessageBox.Show("Save Error");
            }
        }
    }
}
