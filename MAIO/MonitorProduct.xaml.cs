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
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            Main.Monitor del = (Main.Monitor)((Button)sender).DataContext;
            if (dic.Keys.Contains(del.Taskid))
            {
                dic[del.Taskid].Cancel();
                dic.Remove(del.Taskid);
            }
            if (Mainwindow.tasklist.ContainsKey(del.Taskid))
            {
                Config.mn.updatetask(Mainwindow.tasklist[del.Taskid]);
                Mainwindow.tasklist.Remove(del.Taskid.ToString());   
            }
            Mainwindow.Advancemonitortask.Remove(del);
        }
        private void start_Click(object sender, RoutedEventArgs e)
        {
            Main.Monitor tk = (Main.Monitor)((Button)sender).DataContext;
            if (dic.ContainsKey(tk.Taskid))
            {
                dic[tk.Taskid].Cancel();
                dic.Remove(tk.Taskid);
            }
            else
            {
                if (Mainwindow.Advancemonitortask[i].Region == "NikeAU" || Mainwindow.Advancemonitortask[i].Region == "NikeCA" || Mainwindow.Advancemonitortask[i].Region == "NikeMY" || Mainwindow.Advancemonitortask[i].Region == "NikeNZ" || Mainwindow.Advancemonitortask[i].Region == "NikeSG")
                {
                    var cts = new CancellationTokenSource();
                    var ct = cts.Token;
                    dic.Add(tk.Taskid, cts);
                    NikeMonitorProduct NMP = new NikeMonitorProduct();
                    NMP.mn = tk;
                    if (tk.Size == "RA" || tk.Size == "ra" || tk.Size == "" || tk.Size == null || tk.Size == " ")
                    {
                        tk.Size = "RA";
                        NMP.randomsize = true;
                    }
                    Task task1 = new Task(() => { NMP.start(ct, cts); }, ct, TaskCreationOptions.LongRunning);
                    task1.Start();
                }
                else if (Mainwindow.Advancemonitortask[i].Region == "NikeUS" || Mainwindow.Advancemonitortask[i].Region == "NikeUK")
                {
                    var cts = new CancellationTokenSource();
                    var ct = cts.Token;
                    dic.Add(tk.Taskid, cts);
                    NikeMonitorProduct NMP = new NikeMonitorProduct();
                    NMP.mn = tk;
                    if (tk.Size == "RA" || tk.Size == "ra" || tk.Size == "" || tk.Size == null || tk.Size == " ")
                    {
                        tk.Size = "RA";
                        NMP.randomsize = true;
                    }
                    Task task1 = new Task(() => { NMP.start(ct, cts); }, ct, TaskCreationOptions.LongRunning);
                    task1.Start();
                }
            }
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;  // cancels the window close    
            this.Hide();      // Programmatically hides the window
        }
    }
}
