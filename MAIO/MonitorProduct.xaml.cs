using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static MAIO.Main;

namespace MAIO
{
    /// <summary>
    /// MonitorProduct.xaml 的交互逻辑
    /// </summary>
    public partial class MonitorProduct : Window
    {

        public MonitorProduct()
        {
            InitializeComponent();
        
            for (int i = 0; i < Mainwindow.tasklist.Count; i++)
            {
                KeyValuePair<string, string> kv = Mainwindow.tasklist.ElementAt(i);
                JObject jo = JObject.Parse(kv.Value);
                if (kv.Value.Contains("\"AdvanceMonitor\": \"True\""))
                {
                    Mainwindow.Advancemonitortask.Add(new Main.Monitor { Region = jo["Tasksite"].ToString(), Sku = jo["Sku"].ToString(), Size = jo["Size"].ToString(), Taskid = jo["Taskid"].ToString() });
                }
            }
            monitorproduct.ItemsSource = Mainwindow.Advancemonitortask;
        }
        private void Window_Closing_1(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        private void Grid_IsVisibleChanged_1(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                for (int i = 0; i < Mainwindow.Advancemonitortask.Count; i++)
                {
                    if (Mainwindow.Advancemonitortask[i].Region == "NikeAU" || Mainwindow.Advancemonitortask[i].Region == "NikeCA"|| Mainwindow.Advancemonitortask[i].Region == "NikeMY"|| Mainwindow.Advancemonitortask[i].Region == "NikeNZ"|| Mainwindow.Advancemonitortask[i].Region == "NikeSG")
                    {
                        var cts = new CancellationTokenSource();
                        var ct = cts.Token;
                        try
                        {
                            dic.Add(Mainwindow.Advancemonitortask[i].Taskid, cts);
                        }
                        catch (ArgumentException)
                        {
                            break;
                        }
                        NikeMonitorProduct NMP = new NikeMonitorProduct();
                        NMP.mn = Mainwindow.Advancemonitortask[i];


                        if (Mainwindow.Advancemonitortask[i].Size.Contains("ra") || Mainwindow.Advancemonitortask[i].Size.Contains("RA"))
                        {
                            NMP.randomsize = true;
                        }
                        Task task1 = new Task(() => { NMP.start(ct); }, ct);
                        task1.Start();
                    }
                    else if (Mainwindow.Advancemonitortask[i].Region == "NikeUS" || Mainwindow.Advancemonitortask[i].Region == "NikeUK")
                    {
                        var cts = new CancellationTokenSource();
                        var ct = cts.Token;
                        try
                        {
                            dic.Add(Mainwindow.Advancemonitortask[i].Taskid, cts);
                        }
                        catch (ArgumentException)
                        {
                            continue;
                        }
                        NikeMonitorProduct NMP = new NikeMonitorProduct();
                        NMP.mn = Mainwindow.Advancemonitortask[i];
                        if (Mainwindow.Advancemonitortask[i].Size.Contains("ra") || Mainwindow.Advancemonitortask[i].Size.Contains("RA"))
                        {
                            NMP.randomsize = true;
                        }
                        Task task1 = new Task(() => { NMP.start(ct); }, ct);
                        task1.Start();
                    }

                }
            }
            else
            {

            }
        }
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            Main.Monitor del = (Main.Monitor)((Button)sender).DataContext;
            if (Main.dic.Keys.Contains(del.Taskid))
            {
                Main.dic[del.Taskid].Cancel();
                Main.dic.Remove(del.Taskid);
            }
            else
            {

            }
            int n = monitorproduct.SelectedIndex;
            for (int i = 0; i < Mainwindow.tasklist.Count; i++)
            {
                KeyValuePair<string, string> kv = Mainwindow.tasklist.ElementAt(i);
                JObject jo = JObject.Parse(kv.Value);
                if (del.Taskid == jo["Taskid"].ToString())
                {
                    string needdel = Mainwindow.tasklist[jo["Taskid"].ToString()];
                    Mainwindow.tasklist.Remove(jo["Taskid"].ToString());
                    Config.mn.updatetask(needdel);
                    break;
                }
            }
            Mainwindow.Advancemonitortask.Remove(del);
        }
        private void start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string taskid = Guid.NewGuid().ToString();
                int row = monitorproduct.SelectedIndex;
                Main.Monitor tk;
                tk = Mainwindow.Advancemonitortask.ElementAt(row);
                string task = "[{\"Taskid\":\"" + taskid + "\",\"Tasksite\":\"" + tk.Region + "\",\"Sku\":\"" + tk.Sku + "\"," +
                 "\"Size\":\"" + tk.Size.Replace("\r", "").Replace("\n", "") + "\",\"Profile\":\"" + tk.Profile + "\",\"Proxies\":\"Default\"," +
                "\"Status\":\"IDLE\",\"giftcard\":\"" + tk.giftcard + "\",\"Code\":\"" + tk.code.Replace("\r", "").Replace("\n", "") + "\",\"Quantity\":\"" + tk.Quantity + "\",\"monitortask\":\"False\",\"AdvanceMonitor\":\"False\"}]";
                Mainwindow.tasklist.Add(taskid, task.Replace("[", "").Replace("]", ""));
                Mainwindow.task.Add(new taskset { Taskid = taskid, Tasksite = tk.Region, Sku = tk.Sku, Size = tk.Size.Replace("\r", "").Replace("\n", ""), Profile = tk.Profile, Proxies = "Default", Status = "IDLE", Quantity = tk.Quantity, monitortask = "False", Account = tk.Account });
            }
            catch
            {
                MessageBox.Show("Missing Information");
            }
        }

    }
}
