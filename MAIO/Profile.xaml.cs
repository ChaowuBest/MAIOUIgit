using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
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
                Thread.Sleep(1);
                KeyValuePair<string, string> kv = Mainwindow.allprofile.ElementAt(i);
                profilelist.Items.Add(kv.Key);
            }
            countrylist.ItemsSource = Countrycode.countrycode;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Writecoookie.write();
            Application.Current.Shutdown();
            try
            {
                Process[] ps = Process.GetProcesses();
                foreach (Process process in ps)
                {
                    if (process.ProcessName.Contains("CheckoutHelper") || process.ProcessName.Contains("chromedriver"))
                    {
                        process.Kill();
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        private void save(object sender, RoutedEventArgs e)
        {
            bool duplicate = false;
            string key = null;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "profile.json";
            string profile = "[{\"FirstName\":\"" + firstname.Text + "\",\"LastName\":\"" + lastname.Text + "\"," +
              "\"EmailAddress\":\"" + email.Text + "\",\"Address1\":\"" + address1.Text + "\",\"Address2\":\"" + address2.Text + "\"," +
              "\"Tel\":\"" + tel.Text + "\",\"City\":\"" + city.Text + "\",\"Zipcode\":\"" + zipcode.Text + "\",\"State\":\"" + state.
              Text + "\",\"Country\":\"" + countrylist.SelectedItem.ToString() + "\",\"Cardnum\":\"" + cardnumber.Text + "\",\"MMYY\":\"" + MMYY.Text + "\"," +
              "\"NameonCard\":\"" + nameoncard.Text + "\",\"Cvv\":\"" + CVV.Text + "\",\"ProfileName\":\"" + profilename.Text + "\"}]";
            string svalue = null;
            if (Mainwindow.allprofile.TryGetValue(profilename.Text, out svalue))
            {
                duplicate = true;
                key = profilename.Text;
            }
            if (duplicate)
            {
                Mainwindow.allprofile[key] = profile.Replace("[", "").Replace("]", "").Replace("\r", "").Replace("\n", "");
                profilewrite(profile, path);
            }
            else
            {
                Mainwindow.allprofile.Add(profilename.Text, profile.Replace("[", "").Replace("]", "").Replace("\r", "").Replace("\n", ""));
                profilewrite(profile, path);
                profilelist.Items.Add(profilename.Text);
            }
        }
        public void profilewrite(string profile,string path2)
        {
            JArray ja2 = JArray.Parse(profile);
            try
            {
             //   string path2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "profile.json";
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
            if (del.Contains("Profilelist"))
            {
                updateprofilegroup(del);
            }
            else
            {
                string needdel = Mainwindow.allprofile[del];
                Mainwindow.allprofile.Remove(del);
                profilelist.Items.Refresh();
                string path2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "profile.json";
                updateprofile(needdel, path2);
            }
        }
        public void updateprofile(string profile,string path2)
        {
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
            sw.Write(ja.ToString().Replace("\n", ""));
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
        public void updateprofilegroup(string profilegroupname)
        {
            string path2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "profilegroup.json";
            FileStream fs0 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs0);
            string pro = sr.ReadToEnd();
            JArray ja = JArray.Parse(pro);
            sr.Close();
            fs0.Close();
            for (int i = 0; i < ja.Count; i++)
            {
                if (ja[i]["ProfileName"].ToString() == profilegroupname.ToString())
                {
                    ja.RemoveAt(i);
                    break;
                }
            }
            FileStream fs1 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamWriter sw = new StreamWriter(fs1);
            fs1.SetLength(0);
            sw.Write(ja.ToString().Replace("\n", ""));
            sw.Close();
            fs1.Close();
            FileInfo fi = new FileInfo(path2);
            FileStream fs2 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            if (fi.Length == 2)
            {
                fs2.SetLength(0);
            }
            fs2.Close();
            Mainwindow.allprofile.Remove(profilegroupname);
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
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            IDataObject iData = Clipboard.GetDataObject();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "profile.json";
            if (iData.GetDataPresent(DataFormats.Text))
            {
                bool duplicate = false;
                string key = null;
                string profile = (string)iData.GetData(DataFormats.UnicodeText);
                string[] profile_Arrary = profile.Split("\r\n");
                Dictionary<string, string> importprofile = new Dictionary<string, string>();
                try
                {
                    for (int i = 1; i < profile_Arrary.Length; i++)
                    {
                        var ay = new List<string>();
                        for (int y = 0; y < 15; y++)
                        {
                            ay.Add(profile_Arrary[i].Split("\t")[y]);
                        }
                        string profiles = "[{\"FirstName\":\"" + ay[2] + "\",\"LastName\":\"" + ay[3] + "\"," +
    "\"EmailAddress\":\"" + ay[1] + "\",\"Address1\":\"" + ay[4] + "\",\"Address2\":\"" + ay[5] + "\"," +
    "\"Tel\":\"" + ay[7] + "\",\"City\":\"" + ay[8] + "\",\"Zipcode\":\"" + ay[6] + "\",\"State\":\"" + ay[9]
    + "\",\"Country\":\"" + ay[10] + "\",\"Cardnum\":\"" + ay[11] + "\",\"MMYY\":\"" + ay[12] + "\"," +
    "\"NameonCard\":\"" + ay[14] + "\",\"Cvv\":\"" + ay[13] + "\",\"ProfileName\":\"" + ay[0] + "\"}]";
                        importprofile.Add(ay[0].ToString(), profiles);
                    }
                    for (int i = 0; i < importprofile.Count; i++)
                    {
                        string svalue = null;
                        KeyValuePair<string, string> kv = importprofile.ElementAt(i);
                        if (Mainwindow.allprofile.TryGetValue(kv.Key, out svalue))
                        {
                            duplicate = true;
                            key = kv.Key;
                        }
                        if (duplicate)
                        {
                            Mainwindow.allprofile[key] = kv.Value.Replace("[", "").Replace("]", "").Replace("\r", "").Replace("\n", "");
                            profilewrite(kv.Key, path);
                        }
                        else
                        {
                            Mainwindow.allprofile.Add(kv.Key, kv.Value.Replace("[", "").Replace("]", "").Replace("\r", "").Replace("\n", ""));
                            profilewrite(kv.Value, path);
                            profilelist.Items.Add(kv.Key);
                        }
                    }

                }
                catch
                {
                    MessageBox.Show("Import Error");
                }

            }
            else
            {
                MessageBox.Show("Import Format Error");
            }

        }
        private void savegroup(object sender, RoutedEventArgs e)
        {
            string profiles = profilegroup.Text.ToString().Replace(" ", "");
            bool error = false;
            string profilesvalue = null;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "profilegroup.json";
            if (profiles.Contains("+"))
            {
                var profiles_name = profiles.Split("+");
                for (int i = 0; i < profiles_name.Length; i++)
                {
                    Thread.Sleep(1);
                    if (Mainwindow.allprofile.ContainsKey(profiles_name[i]) == false)
                    {
                        error = true;
                        break;
                    }
                    else
                    {
                        profilesvalue += profiles_name[i] + ";";
                    }
                }
                if (error)
                {
                    MessageBox.Show("Add Profile Groups Error");
                }
                else
                {
                    int count = Mainwindow.allprofile.Count + 1;
                    string profile = "[{\"ProfileName\":\"Profilelist" + count + "\",\"ProfileValue\":\"" + profilesvalue.Replace("[", "").Replace("]", "").Replace("\r", "").Replace("\n", "") + "\"}]";
                    Mainwindow.allprofile.Add("Profilelist" +count + "", profile.Replace("[", "").Replace("]", "").Replace("\r", "").Replace("\n", ""));
                    profilewrite(profile,path);
                    profilelist.Items.Add("Profilelist" + count);
                }

            }
            else if (profiles.Contains("|"))
            {
                try
                {
                    var profils_name = profiles.Split("|");
                    if (profils_name.Length == 2)
                    {
                        bool start = false;
                        foreach (var i in Mainwindow.allprofile)
                        {
                            if (i.Key == profils_name[0])
                            {
                                start = true;
                            }
                            else if (i.Key == profils_name[1])
                            {
                                profilesvalue += profils_name[1];
                                break;
                            }
                            if (start)
                            {
                                profilesvalue += i.Key + ";";
                            }
                        }
                    }
                    int count=Mainwindow.allprofile.Count + 1;
                    string profile = "[{\"ProfileName\":\"Profilelist" + count + "\",\"ProfileValue\":\"" + profilesvalue.Replace("[", "").Replace("]", "").Replace("\r", "") + "\"}]";
                    Mainwindow.allprofile.Add("Profilelist" + count + "", profile.Replace("[", "").Replace("]", "").Replace("\r", "").Replace("\n", ""));                
                    profilewrite(profile,path);
                    profilelist.Items.Add("Profilelist" + count);
                }
                catch
                {
                    MessageBox.Show("Add Profile Groups Error");
                }
            }
        }
    }
}
