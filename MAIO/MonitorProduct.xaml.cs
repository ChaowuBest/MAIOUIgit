using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
                    Mainwindow.Advancemonitortask.Add(new Main.Monitor{ Region = jo["Tasksite"].ToString(), Sku = jo["Sku"].ToString(), Size = jo["Size"].ToString(), Taskid = jo["Taskid"].ToString() });
                }
            }
            
            monitorproduct.ItemsSource = Mainwindow.Advancemonitortask;
        }
        private void Window_Closing_1(object sender, CancelEventArgs e)
        {
            e.Cancel = true;  // cancels the window close    
            this.Hide();      // Programmatically hides the window
        }
    }
}
