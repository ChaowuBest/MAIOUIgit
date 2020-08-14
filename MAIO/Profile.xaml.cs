using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

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
                profilelist.Items.Add(kv.Key);
            }
            countrylist.ItemsSource = Countrycode.countrycode;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Writecoookie.write();
            Application.Current.Shutdown();
        }
        private void save(object sender, RoutedEventArgs e)
        {
            bool duplicate = false;
            string key = "";
            string profile = "[{\"FirstName\":\"" + firstname.Text + "\",\"LastName\":\"" + lastname.Text + "\"," +
              "\"EmailAddress\":\"" + email.Text + "\",\"Address1\":\"" + address1.Text + "\",\"Address2\":\"" + address2.Text + "\"," +
              "\"Tel\":\"" + tel.Text + "\",\"City\":\"" + city.Text + "\",\"Zipcode\":\"" + zipcode.Text + "\",\"State\":\"" + state.
              Text + "\",\"Country\":\"" + countrylist.SelectedItem.ToString() + "\",\"Cardnum\":\"" + cardnumber.Text + "\",\"MMYY\":\"" + MMYY.Text + "\"," +
              "\"NameonCard\":\"" + nameoncard.Text + "\",\"Cvv\":\"" + CVV.Text + "\",\"ProfileName\":\"" + profilename.Text + "\"}]";
            for (int i = 0; i < Mainwindow.allprofile.Count; i++)
            {
                KeyValuePair<string, string> kv = Mainwindow.allprofile.ElementAt(i);
                if (kv.Key == profilename.Text)
                {
                    duplicate = true;
                    key=kv.Key;
                    break;
                }              
            }
            if(duplicate)
            {
                Mainwindow.allprofile[key] = profile.Replace("[", "").Replace("]", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");
                profilewrite(profile);              
            }
            else
            {
                Mainwindow.allprofile.Add(profilename.Text, profile.Replace("[", "").Replace("]", "").Replace("\r", "").Replace("\n", "").Replace("\t", ""));
                profilewrite(profile);
                profilelist.Items.Add(profilename.Text);
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
                        var wuds=rex.Matches(ja[i].ToString());
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
        private void profilelist_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gridView = listView.View as GridView;
            var width = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
            var Profiles = 0.8;
            gridView.Columns[0].Width = width * Profiles;
        }
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var del = ((Button)sender).DataContext.ToString();
            profilelist.Items.Remove(del);
            string needdel = Mainwindow.allprofile[del];
            Mainwindow.allprofile.Remove(del);
            profilelist.Items.Refresh();
            updateprofile(needdel);
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

        private void profilelist_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {

                string selectdata = Mainwindow.allprofile["" + profilelist.SelectedItem.ToString() + ""];
                JObject jo = JObject.Parse(selectdata);
                firstname.Text = jo["FirstName"].ToString();
                lastname.Text = jo["LastName"].ToString();
                email.Text = jo["EmailAddress"].ToString();
                address1.Text = jo["Address1"].ToString();
                address2.Text = jo["Address2"].ToString();
                tel.Text = jo["Tel"].ToString();
                zipcode.Text = jo["Zipcode"].ToString();
                city.Text = jo["City"].ToString();
                state.Text = jo["State"].ToString();
                countrylist.Text = jo["Country"].ToString();
                cardnumber.Text = jo["Cardnum"].ToString();
                CVV.Text = jo["Cvv"].ToString();
                MMYY.Text = jo["MMYY"].ToString();
                nameoncard.Text = jo["NameonCard"].ToString();
                profilename.Text = jo["ProfileName"].ToString();
            }
            catch
            { 

            }
        }
    }
}
