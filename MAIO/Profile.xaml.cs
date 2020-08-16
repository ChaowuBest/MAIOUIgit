using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MAIO
{
    /// <summary>
    /// Profile.xaml 的交互逻辑
    /// </summary>
    public partial class Profile : UserControl
    {
        public Profile()
        {
            InitializeComponent();
            for (int i = 0; i < Mainwindow.allprofile.Count; i++)
            {
                KeyValuePair<string, string> kv = Mainwindow.allprofile.ElementAt(i);
                Addbilling(false, kv.Key, kv.Value);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Writecoookie.write();
            Application.Current.Shutdown();
        }
        public void updateprofile(string profile)
        {
            string path2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "profile.json";
            FileStream fs0 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs0);
            string pro = sr.ReadToEnd();
            JArray ja = JArray.Parse(pro);
            sr.Close();
            fs0.Close();
            JObject jo = JObject.Parse(profile);
            for (int i = 0; i < ja.Count; i++)
            {
                if (ja[i]["ProfileName"].ToString() == jo["ProfileName"].ToString())
                {
                    ja.RemoveAt(i);
                    break;
                }
            }
            FileStream fs1 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fs1);
            fs1.SetLength(0);
            sw.Write(ja.ToString().Replace("\n", "").Replace("\t", ""));
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
            NewProfile np = new NewProfile();
            np.getTextHandler = Addbilling;
            np.Show();
        }
        private void Addbilling(bool st, string profilename, string profile)
        {
            if (st)
            {
            }
            else
            {
                SolidColorBrush formyBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
                SolidColorBrush delbrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 84, 57));
                Button btn = new Button();
                btn.Margin = new Thickness(25, 20, 5, 10);
                btn.Click += new RoutedEventHandler(check);
                btn.Width = 200;
                btn.Height = 150;
                btn.Content = profilename;
                btn.Name =profilename.Replace(" ", "");
                SolidColorBrush myBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(37, 41, 53));
                btn.FontFamily = new System.Windows.Media.FontFamily("PingFangSC-Semibold");
                btn.FontSize = 16;
                btn.Background = myBrush;
                btn.Foreground = formyBrush;
                panel.Children.Add(btn);
                panel.RegisterName(profilename.Replace(" ", ""), btn);

                Button btndel = new Button();
                HandyControl.Controls.BorderElement.SetCornerRadius(btndel, new CornerRadius(12, 12, 12, 12));
                HandyControl.Controls.IconElement.SetGeometry(btndel, (Geometry)this.FindResource("CloseGeometry"));
                btndel.Margin = new Thickness(-25, -128, 22, 10);
                Style myStyle = (Style)this.FindResource("ButtonIcon");
                btndel.Style = myStyle;
                btndel.Click += new RoutedEventHandler(delprofile);
                btndel.Background = delbrush;
                btndel.Padding = new Thickness(5);
                btndel.Height = 24;
                btndel.Width = 24;
                btndel.Name = "del" + profilename.Replace(" ", "");
                btndel.Foreground = formyBrush;
                panel.Children.Add(btndel);
                panel.RegisterName("del" + profilename.Replace(" ", ""), btndel);
                EditeProfile.edit = false;
            }
        } 
        public void check(object sender, RoutedEventArgs e)
        {
            SolidColorBrush delbrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 84, 57));
            Button btn = (Button)sender;
            string profiles = null;
            for (int i = 0; i < Mainwindow.allprofile.Count; i++)
            {
                KeyValuePair<string, string> kv = Mainwindow.allprofile.ElementAt(i);
                if (kv.Key.Replace(" ", "") == btn.Name)
                {
                    profiles = Mainwindow.allprofile[kv.Key];
                    break;
                }
            }
            JObject jo = JObject.Parse(profiles);
         
            EditeProfile.FirstName = jo["FirstName"].ToString();
            EditeProfile.LastName = jo["LastName"].ToString();
            EditeProfile.EmailAddress = jo["EmailAddress"].ToString();
            EditeProfile.Address1 = jo["Address1"].ToString();
            EditeProfile.Address2 = jo["Address2"].ToString();
            EditeProfile.Tel = jo["Tel"].ToString();
            EditeProfile.City = jo["City"].ToString();
            EditeProfile.Zipcode = jo["Zipcode"].ToString();
            EditeProfile.State = jo["State"].ToString();
            EditeProfile.Country = jo["Country"].ToString();

            EditeProfile.Cardnum = jo["Cardnum"].ToString();
            EditeProfile.MMYY = jo["MMYY"].ToString();
            EditeProfile.NameonCard = jo["NameonCard"].ToString();
            EditeProfile.Cvv = jo["Cvv"].ToString();
            EditeProfile.ProfileName = jo["ProfileName"].ToString();           
            EditeProfile.BillingFirstName = jo["BillingFirstName"].ToString();
            EditeProfile.BillingLastName = jo["BillingLastName"].ToString();
            EditeProfile.BillingAddress1 = jo["BillingAddress1"].ToString();
            EditeProfile.BillingAddress2 = jo["BillingAddress2"].ToString();
            EditeProfile.BillingTel = jo["BillingTel"].ToString();
            EditeProfile.BillingCity = jo["BillingCity"].ToString();
            EditeProfile.Billingstate = jo["Billingstate"].ToString();
            EditeProfile.Billingzipcode = jo["Billingzipcode"].ToString();
            EditeProfile.BillingCountry = jo["BillingCountry"].ToString();
            EditeProfile.edit = true;
            NewProfile np = new NewProfile();
            np.getTextHandler = Addbilling;
            np.Show();
        }
        public void delprofile(object sender, RoutedEventArgs e)
        {
            string needdel = null;
            Button btn = (Button)sender;
            panel.Children.Remove((Button)sender);
            panel.UnregisterName(btn.Name);
            panel.Children.Remove((Button)panel.FindName(btn.Name.Replace("del", "")));
            panel.UnregisterName(btn.Name.Replace("del", ""));
            for (int i = 0; i < Mainwindow.allprofile.Count; i++)
            {
                KeyValuePair<string, string> kv = Mainwindow.allprofile.ElementAt(i);
                if (kv.Key.Replace(" ", "") == btn.Name.Replace("del", ""))
                {
                    needdel = Mainwindow.allprofile[kv.Key];
                    Mainwindow.allprofile.Remove(kv.Key);
                    break;
                }
            }
            updateprofile(needdel);

        }
        private void panel_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var eventArg = new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta);
            eventArg.RoutedEvent = UIElement.MouseWheelEvent;
            eventArg.Source = sender;
            panel.RaiseEvent(eventArg);
        }
    }
}
