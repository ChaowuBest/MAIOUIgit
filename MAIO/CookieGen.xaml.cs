using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
    /// CookieGen.xaml 的交互逻辑
    /// </summary>
    public partial class CookieGen : Window
    {
        public CookieGen()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings settings = new Settings();
                Task task1 = new Task(() =>settings.ws("NIKE"));
                task1.Start();
                Task task2 = new Task(() => settings.ws("NIKE"));
                task2.Start();
            }
            catch
            {
                MessageBox.Show("You must have chrome");
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings settings = new Settings();
                Task task1 = new Task(() => settings.ws("TNF"));
                task1.Start();
                Task task2 = new Task(() => settings.ws("TNF"));
                task2.Start();
            }
            catch
            {
                MessageBox.Show("You must have chrome");
            }
        }
    }
}
