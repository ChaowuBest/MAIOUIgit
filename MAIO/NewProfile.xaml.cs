using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// NewProfile.xaml 的交互逻辑
    /// </summary>
    public partial class NewProfile : Window
    {
        public NewProfile()
        {
            InitializeComponent();
            country.ItemsSource= Countrycode.countrycode;
            billingcountry.ItemsSource = Countrycode.countrycode;
        }

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
        public delegate void GetTextHandler(bool st,string profilename); //声明委托
        public GetTextHandler getTextHandler;
        private void save_Click(object sender, RoutedEventArgs e)
        {
            bool duplicate = false;
            string key = "";
            string profile = "[{\"FirstName\":\"" + firstname.Text + "\",\"LastName\":\"" + lastname.Text + "\"," +
"\"EmailAddress\":\"" + email.Text + "\",\"Address1\":\"" + address1.Text + "\",\"Address2\":\"" + address2.Text + "\"," +
"\"Tel\":\"" + tel.Text + "\",\"City\":\"" + city.Text + "\",\"Zipcode\":\"" + zipcode.Text + "\",\"State\":\"" + state.
Text + "\",\"Country\":\"" + country.SelectedItem.ToString() + "\",\"Cardnum\":\"" + cardnum.Text + "\",\"MMYY\":\"" + MMYY.Text + "\"," +
"\"NameonCard\":\"" + nameoncard.Text + "\",\"Cvv\":\"" + cvv.Text + "\",\"ProfileName\":\"" + profilename.Text + "\",\"BillingFirstName\":\"" + billingfirst.Text + "\",\"BillingLastName\":\"" + billinglast.Text + "\"," +
"\"BillingAddress1\":\"" + billingaddress1.Text + "\",\"BillingAddress2\":\"" + billingaddress2.Text + "\",\"BillingTel\":\"" + billingtel.Text + "\"," +
"\"BillingCity\":\"" + billingcity.Text + "\",\"Billingstate\":\"" + billingstate.Text + "\",\"Billingzipcode\":\"" + billingzipcode.Text + "\",\"BillingCountry\":\"" + billingcountry.
Text + "\"}]";
            for (int i = 0; i < Mainwindow.allprofile.Count; i++)
            {
                KeyValuePair<string, string> kv = Mainwindow.allprofile.ElementAt(i);
                if (kv.Key == profilename.Text)
                {
                    duplicate = true;
                    key = kv.Key;
                    break;
                }
            }
            if (duplicate)
            {
                Mainwindow.allprofile[key] = profile.Replace("[", "").Replace("]", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");
                profilewrite(profile);
                getTextHandler(duplicate,profilename.Text);
            }
            else
            {
                Mainwindow.allprofile.Add(profilename.Text, profile.Replace("[", "").Replace("]", "").Replace("\r", "").Replace("\n", "").Replace("\t", ""));
                profilewrite(profile);
                getTextHandler(duplicate, profilename.Text);
            }
        }
        public void profilewrite(string profile)
        {
            JArray ja2 = JArray.Parse(profile);
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + "MAIO";
                string path2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "profile.json";
                FileStream fs0 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs0);
                FileInfo fi = new FileInfo(path2);
                FileStream fs1 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                if (fi.Length == 0)
                {
                    JArray ja = JArray.Parse(profile);
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
                        Regex rex = new Regex("\"(.*)\"");
                        var wuds = rex.Matches(ja[i].ToString());
                        if (ja[i]["ProfileName"].ToString() == ja2[0]["ProfileName"].ToString())
                        {
                            ja.RemoveAt(i);
                            break;
                        }
                    }
                    ja.Add(JObject.Parse(profile.Replace("[", "").Replace("]", "")));
                    fs1.SetLength(0);
                    StreamWriter sw = new StreamWriter(fs1);
                    sw.Write(ja.ToString().Replace("\n", "").Replace("\t", ""));
                    sw.Close();
                    fs1.Close();
                }
            }
            catch
            {
                MessageBox.Show("Error to save");
            }
        }
        private void isssb_Checked(object sender, RoutedEventArgs e)
        {
            if ((bool)isssb.IsChecked)
            {
                billingfirst.Text = firstname.Text;
                billinglast.Text = lastname.Text;
                billingaddress1.Text = address1.Text;
                billingaddress2.Text = address2.Text;
                billingcity.Text = city.Text;
                billingcountry.Text = country.Text;
                billingstate.Text = state.Text;
                billingtel.Text = tel.Text;
                billingzipcode.Text = zipcode.Text;
            }
        }
    }
}
