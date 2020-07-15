using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
///3123123123213213
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
            if (Mainwindow.listaccount != null)
            {
                for (int i = 0; i < Mainwindow.listaccount.Count; i++)
                {
                    accountbox.AppendText(Mainwindow.listaccount[i]);
                    accountbox.AppendText("\r\n");
                }
            }
            if (Mainwindow.giftcardlist != null)
            {
                for (int i = 0; i < Mainwindow.giftcardlist.Count; i++)
                {
                    KeyValuePair<string, string> kv = Mainwindow.giftcardlist.ElementAt(i);
                    giftlist.Items.Add(kv.Key);
                }
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Writecoookie.write();
            Application.Current.Shutdown();
        }
        private void saveaccount_Click(object sender, RoutedEventArgs e)
        {
            if (new TextRange(accountbox.Document.ContentStart, accountbox.Document.ContentEnd).Text == "")
            {
                MessageBox.Show("No account");
            }
            else
            {
                string[] saveaccount = new TextRange(accountbox.Document.ContentStart, accountbox.Document.ContentEnd).Text.Split("\r\n");
                string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "account.txt";
                FileStream fs0 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamWriter sw = new StreamWriter(fs0);
                fs0.SetLength(0);
                Mainwindow.listaccount.Clear();
                for (int i = 0; i < saveaccount.Length - 1; i++)
                {
                    if (saveaccount[i] != "")
                    {
                        sw.WriteLine(saveaccount[i]);
                        Mainwindow.listaccount.Add(saveaccount[i]);
                    }
                }
                sw.Close();
                fs0.Close();
            }
        }
        private void del_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "account.txt";
            FileStream fs0 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            fs0.SetLength(0);
            Mainwindow.listaccount.Clear();
            accountbox.Document.Blocks.Clear();
        }
        private void savegiftcard(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "giftcard.json";
            if (giftcardname.Text != "")
            {
                string[] savegiftcard = new TextRange(giftcardbox.Document.ContentStart, giftcardbox.Document.ContentEnd).Text.Split("\r\n");
                JObject ja = new JObject();
                try
                {
                    for (int i = 0; i < savegiftcard.Length; i++)
                    {
                        if (savegiftcard[i] != "")
                        {
                            var sp = savegiftcard[i].Split("-");
                            dic.Add(sp[0], sp[1]);
                            ja.Add(sp[0], sp[1]);
                        }
                    }
                    JObject jo = new JObject(
                      new JProperty(giftcardname.Text,
                      new JObject(ja))
                     );
                    Mainwindow.giftcardlist.Add(giftcardname.Text, ja.ToString());
                    giftlist.Items.Add(giftcardname.Text);               
                    FileInfo fi = new FileInfo(path);
                    if (fi.Length == 0)
                    {
                        var jot= jo.ToString().Insert(0, "[").Insert(jo.ToString().Length+1,"]");
                        FileStream fs0 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        StreamWriter sw = new StreamWriter(fs0);
                        sw.Write(jot.ToString().Replace("\r", "").Replace("\n","").Replace("\t", "").Replace(" ", ""));
                        sw.Close();
                        fs0.Close();
                    }
                    else
                    {
                        FileStream fs1 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        StreamReader sw2 = new StreamReader(fs1);
                        var read= sw2.ReadToEnd();
                        JArray ja2 = JArray.Parse(read);
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
                catch (Exception ex)
                {
                    MessageBox.Show("Please check your input");
                }

            }
            else
            {
                MessageBox.Show("Please enter giftcardlist name");
            }
        }
        private void giftlist_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                giftcardbox.Document.Blocks.Clear();
                string selectdata = Mainwindow.giftcardlist["" + giftlist.SelectedItem.ToString() + ""];
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
                
            }

        }
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var del = ((Button)sender).DataContext.ToString();
            giftlist.Items.Remove(del);
            string needdel = Mainwindow.giftcardlist[del];
            Mainwindow.giftcardlist.Remove(del);
            giftlist.Items.Refresh();
            updategiftcard(del);

        }
        public void updategiftcard(string gft)
        {
            string path2 = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "giftcard.json";
            FileStream fs0 = new FileStream(path2, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            StreamReader sr = new StreamReader(fs0);
            string pro = sr.ReadToEnd();
            JArray ja = JArray.Parse(pro);
            var test =ja.ToString();
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

        private void giftlist_SizeChanged_1(object sender, SizeChangedEventArgs e)
        {
              ListView listView = sender as ListView;
                  GridView gridView = listView.View as GridView;
                var width = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
               var Profiles2 = 0;
              var Profiles1 = 0.5;
              gridView.Columns[0].Width = width * Profiles2;
               gridView.Columns[1].Width = width * Profiles1;
            UpdateColumnsWidth(sender as ListView);
        }
        private void ListView_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateColumnsWidth(sender as ListView);
        }
        private void UpdateColumnsWidth(ListView listView)
        {
            int autoFillColumnIndex = (listView.View as GridView).Columns.Count - 1;
            if (listView.ActualWidth == Double.NaN)
                listView.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            double remainingSpace = listView.ActualWidth;
            for (int i = 0; i < (listView.View as GridView).Columns.Count; i++)
                if (i != autoFillColumnIndex)
                    remainingSpace -= (listView.View as GridView).Columns[i].ActualWidth;
            (listView.View as GridView).Columns[autoFillColumnIndex].Width = remainingSpace >= 0 ? remainingSpace : 0;
        }
    }
}
