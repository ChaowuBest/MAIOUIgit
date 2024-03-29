﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MAIO
{
    /// <summary>
    /// NewTask.xaml 的交互逻辑
    /// </summary>
    public partial class NewTask : Window
    {
        public NewTask()
        {
            InitializeComponent();
            Quantity.ItemsSource = Config.qual;
            account.ItemsSource = Mainwindow.account.Keys;
            tasknumber.Document.Blocks.Clear();
            Run run4 = new Run("1");
            Paragraph p4 = new Paragraph();
            p4.Inlines.Add(run4);
            tasknumber.Document.Blocks.Add(p4);
            discount.ItemsSource = Mainwindow.codepool;
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void mygrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (Midtransfer.edit)
            {
                giftcard.ItemsSource = Mainwindow.giftcardlist.Keys;
            }
            else
            {
                giftcard.ItemsSource = Mainwindow.giftcardlist.Keys;
            }


        }
        private void profiles_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (Midtransfer.edit)
            {
                profiles.ItemsSource = Mainwindow.allprofile.Keys;
                size.Document.Blocks.Clear();
                Run run = new Run(Midtransfer.sizeid);
                Paragraph p = new Paragraph();
                p.Inlines.Add(run);
                size.Document.Blocks.Add(p);

                sku.Document.Blocks.Clear();
                Run run2 = new Run(Midtransfer.pid);
                Paragraph p2 = new Paragraph();
                p2.Inlines.Add(run2);
                sku.Document.Blocks.Add(p2);

                discount.Text = Midtransfer.code;

                tasknumber.Document.Blocks.Clear();
                Run run4 = new Run("1");
                Paragraph p4 = new Paragraph();
                p4.Inlines.Add(run4);
                tasknumber.Document.Blocks.Add(p4);

                int profile = 0;
                foreach (var i in Mainwindow.allprofile)
                {

                    if (i.Key == Midtransfer.profilesel.ToString())
                    {
                        profiles.SelectedIndex = profile;
                        profile = 0;
                        break;
                    }
                    profile++;
                }
                int gift = 0;
                foreach (var i in Mainwindow.giftcardlist)
                {
                    if (i.Key == Midtransfer.giftcard.ToString())
                    {
                        giftcard.SelectedIndex = gift;
                        gift = 0;
                        break;
                    }
                    gift++;
                }

            }
            else
            {
                profiles.ItemsSource = Mainwindow.allprofile.Keys;
            }

        }
        public delegate void GetTextHandler(string[] st); //声明委托
        public GetTextHandler getTextHandler;
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string productid = new TextRange(sku.Document.ContentStart, sku.Document.ContentEnd).Text.Replace("\r\n", "");
            string sizeid = new TextRange(size.Document.ContentStart, size.Document.ContentEnd).Text.Replace("\r\n", "");
            string code = discount.Text;
            string taskNumber = new TextRange(tasknumber.Document.ContentStart, tasknumber.Document.ContentEnd).Text.Replace("\r\n", "");
            string user = null;
            if (assingaccount.Text != "" && assingaccount.Text != null)
            {
                user = assingaccount.Text;
            }
            else
            {
                user = account.Text;
            }
            string[] setup = new string[10];
            try
            {
                int tasklength = 1;

                List<string> profilevalues = null;
                if (profiles.SelectedItem!= null)
                {
                    if (profiles.SelectedItem.ToString().Contains("Profilelist"))
                    {
                        var w2u = Mainwindow.allprofile[profiles.SelectedItem.ToString()];
                        JObject jo = JObject.Parse(Mainwindow.allprofile[profiles.SelectedItem.ToString()]);
                        profilevalues = jo["ProfileValue"].ToString().Split(";").ToList();
                        foreach (var i in profilevalues)
                        {
                            Thread.Sleep(1);
                            if (i == "")
                            {
                                profilevalues.Remove(i);
                            }
                        }
                        tasklength = profilevalues.Count;
                    }
                }
                if (taskNumber != null)
                {
                    for (int z = 0; z < tasklength; z++)
                    {
                        for (int i = 0; i < int.Parse(taskNumber); i++)
                        {
                            if (Midtransfer.edit)
                            {
                                JObject jo = JObject.Parse(Mainwindow.tasklist[Midtransfer.taskid].ToString());
                                string profile = "[{\"Taskid\":\"" + jo["Taskid"].ToString() + "\",\"Tasksite\":\"" + site.SelectedItem.ToString().Replace("System.Windows.Controls.ComboBoxItem: ", "") + "\",\"Sku\":\"" + productid + "\"," + "\"Size\":\"" + sizeid + "\"," +
                                        "\"Profile\":\"" + profiles.Text + "\",\"Proxies\":\"Default\"," + "\"Status\":\"IDLE\",\"giftcard\":\"" + giftcard.Text + "\",\"Code\":\"" + code + "\",\"Quantity\":\"" + Quantity.Text + "\"," +
                                        "\"AdvanceMonitor\":\"False\",\"Account\":\"" + user + "\"}]";

                                Midtransfer.tk.Tasksite = site.SelectedItem.ToString().Replace("System.Windows.Controls.ComboBoxItem: ", "");
                                Midtransfer.tk.Sku = productid.Replace("\r\n", "");
                                Midtransfer.tk.Size = sizeid.Replace("\r\n", "");
                                Midtransfer.tk.Profile = profiles.Text.Replace("\r\n", "");
                                Mainwindow.tasklist[Midtransfer.taskid] = profile.Replace("[", "").Replace("]", "");
                                Midtransfer.tk.Account = user;
                                Main.taskwrite(profile);
                                this.Close();
                            }
                            else
                            {
                                if (site.SelectedItem.ToString() != null)
                                {
                                    if (site.SelectedItem.ToString().Contains("NikeUK") == false && site.SelectedItem.ToString().Contains("NikeUS") == false)
                                    {
                                       
                                        if ((site.SelectedItem != null) && (profiles.SelectedItem != null))
                                        {
                                            setup[0] = site.SelectedItem.ToString();
                                            if (profilevalues != null)
                                            {
                                                setup[2] = profilevalues[z];
                                            }
                                            else
                                            {
                                                setup[2] = profiles.SelectedItem.ToString();
                                            }
                                        }
                                        else if ((site.SelectedItem != null) && (profiles.SelectedItem == null))
                                        {
                                            setup[0] = site.SelectedItem.ToString();
                                            for (int y = 0; y < Mainwindow.allprofile.Count; y++)
                                            {
                                                KeyValuePair<string, string> kv = Mainwindow.allprofile.ElementAt(y);
                                                setup[2] = kv.Key;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if ((site.SelectedItem != null) && (profiles.SelectedItem != null))
                                        {
                                            setup[0] = site.SelectedItem.ToString();
                                            if (profilevalues != null)
                                            {
                                                setup[2] = profilevalues[z];
                                            }
                                            else
                                            {
                                                setup[2] = profiles.SelectedItem.ToString();
                                            }
                                        }
                                    }
                                    if (giftcard.SelectedItem != null)
                                    {
                                        setup[1] = giftcard.SelectedItem.ToString();
                                    }
                                    setup[3] = productid;
                                    setup[4] = sizeid;
                                    setup[5] = code;
                                    setup[6] = Quantity.SelectedItem.ToString();
                                    setup[8] = advancemonitor.IsChecked.ToString();
                                    setup[9] = user;
                                    getTextHandler(setup);
                                }
                            }
                        }
                    }

                }
                else
                {
                    MessageBox.Show("Check your input");
                }

            }
            catch (Exception)
            {
                MessageBox.Show("Check your input");
            }
        }
        private void site_Loaded(object sender, RoutedEventArgs e)
        {
            if (Midtransfer.edit)
            {
                site.Text = Midtransfer.sitesel;
            }
        }
        private void Quantity_Loaded(object sender, RoutedEventArgs e)
        {
            if (Midtransfer.edit)
            {
                advancemonitor.Visibility = Visibility.Hidden;
                Quantity.Text = Midtransfer.Quantity;
                num.Visibility = Visibility.Hidden;
                tasknumber.Visibility = Visibility.Hidden;
                account.Text = Midtransfer.tk.Account;
            }
            else
            {

                Quantity.Text = "1";
            }

        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
        }
        private void advance_Click(object sender, RoutedEventArgs e)
        {
        }

        private void account_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var chao = Mainwindow.account[account.SelectedItem.ToString()];
            Dictionary<string, string> dic = new Dictionary<string, string>();
            JObject jo = JObject.Parse(chao);
            foreach (var i in jo)
            {
                dic.Add(i.Key, i.Value.ToString().Replace("{", "").Replace("}", ""));
            }
            assingaccount.ItemsSource = dic;
        }
    }
}
