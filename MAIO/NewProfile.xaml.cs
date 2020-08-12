using System;
using System.Collections.Generic;
using System.Text;
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

        private void save_Click(object sender, RoutedEventArgs e)
        {
          bool duplicate = false;
           string key = "";
            Firstname fs = new Firstname();
           string firt= fs.name();
           string last=fs.name();
            string fullname = firt + "" + last;
            if ((bool)isssb.IsChecked)
            {
                string profile = "[{\"FirstName\":\"" + firstname.Text + "\",\"LastName\":\"" + lastname.Text + "\"," +
 "\"EmailAddress\":\"" + email.Text + "\",\"Address1\":\"" + address1.Text + "\",\"Address2\":\"" + address2.Text + "\"," +
 "\"Tel\":\"" + tel.Text + "\",\"City\":\"" + city.Text + "\",\"Zipcode\":\"" + zip.Text + "\",\"State\":\"" + state.
 Text + "\",\"Country\":\"" + country.SelectedItem.ToString() + "\",\"Cardnum\":\"" + cardnumber.Text + "\",\"MMYY\":\"" + MMYY.Text + "\"," +
 "\"NameonCard\":\"" + nameoncard.Text + "\",\"Cvv\":\"" + CVV.Text + "\",\"ProfileName\":\"" + profilename.Text + "\"}]";
            }
            else
            {
                
            }

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
    }
}
