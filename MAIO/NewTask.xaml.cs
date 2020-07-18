using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup.Localizer;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MAIO
{
    /// <summary>
    /// NewTask.xaml 的交互逻辑
    /// </summary>
    public partial class NewTask : Window
    {
        public NewTask()
        {
            InitializeComponent();
            Quantity.ItemsSource = Config.qual;
        }
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void mygrid_Loaded(object sender, RoutedEventArgs e)
        {
            giftcard.ItemsSource = Mainwindow.giftcardlist.Keys;

        }
        private void profiles_Loaded_1(object sender, RoutedEventArgs e)
        {
            if (Midtransfer.edit)
            {
                profiles.ItemsSource = Mainwindow.allprofile.Keys;
                size.Document.Blocks.Clear();
                Run run = new Run(Midtransfer.sizeid);
                Paragraph p = new Paragraph();
                p.Inlines.Add(run);
                size.Document.Blocks.Add(p);

                sku.Document.Blocks.Clear();
                Run run2 = new Run(Midtransfer.pid);
                Paragraph p2 = new Paragraph();
                p2.Inlines.Add(run2);
                sku.Document.Blocks.Add(p2);

                discount.Document.Blocks.Clear();
                Run run3 = new Run(Midtransfer.code);
                Paragraph p3 = new Paragraph();
                p3.Inlines.Add(run3);
                discount.Document.Blocks.Add(p3);

                int profile = 0;
                foreach (var i in Mainwindow.allprofile)
                {

                    if (i.Key == Midtransfer.profilesel.ToString())
                    {
                        profiles.SelectedIndex = profile;
                        profile = 0;
                        break;
                    }
                    profile++;
                }
                int gift = 0;
                foreach (var i in Mainwindow.giftcardlist)
                {
                    if (i.Key == Midtransfer.giftcard.ToString())
                    {
                        giftcard.SelectedIndex = gift;
                        gift = 0;
                        break;
                    }
                    gift++;
                }
            }
            else
            {
                profiles.ItemsSource = Mainwindow.allprofile.Keys;
            }

        }

        public delegate void GetTextHandler(string[] st); //声明委托
        public GetTextHandler getTextHandler;
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string productid = new TextRange(sku.Document.ContentStart, sku.Document.ContentEnd).Text;
            string sizeid = new TextRange(size.Document.ContentStart, size.Document.ContentEnd).Text;
            string code = new TextRange(discount.Document.ContentStart, discount.Document.ContentEnd).Text;
            string taskNumber = new TextRange(tasknumber.Document.ContentStart, tasknumber.Document.ContentEnd).Text;
            string[] setup = new string[7];

            try
            {
                if (taskNumber != null) {
                    for (int i = 0; i < int.Parse(taskNumber); i++)
                    {
                        if ((site.SelectedItem != null) && (profiles.SelectedItem != null))
                        {
                            setup[0] = site.SelectedItem.ToString();
                            setup[2] = profiles.SelectedItem.ToString();
                        }
                        else
                        {
                            MessageBox.Show("Check your input");
                        }
                        if (giftcard.SelectedItem != null)
                        {
                            setup[1] = giftcard.SelectedItem.ToString();
                        }
                        setup[3] = productid;
                        setup[4] = sizeid;
                        setup[5] = code;
                        setup[6] = Quantity.SelectedItem.ToString();
                        if ((sizeid != "") && (productid != ""))
                        {
                            getTextHandler(setup);
                        }
                        else
                        {
                            MessageBox.Show("Check your input");
                        }
                    }
                } else {
                    MessageBox.Show("Check your input");
                }
                
            }
            catch (Exception)
            {
                MessageBox.Show("Check your input");
            }
        }

        private void site_Loaded(object sender, RoutedEventArgs e)
        {
            if (Midtransfer.edit)
            {
                #region
                /*   if (Midtransfer.sitesel.Contains("AU"))
                   {
                       site.SelectedIndex = 0;
                   }
                   else if (Midtransfer.sitesel.Contains("CA"))
                   {
                       site.SelectedIndex = 1;
                   }
                   else if (Midtransfer.sitesel.Contains("US"))
                   {
                       site.SelectedIndex = 2;
                   }
                   else if (Midtransfer.sitesel.Contains("UK"))
                   {
                       site.SelectedIndex = 3;
                   }
                   else if (Midtransfer.sitesel.Contains("Footasylum"))
                   {
                       site.SelectedIndex = 4;
                   }
                   else if (Midtransfer.sitesel.Contains("TheNorthFaceUS"))
                   {
                       site.SelectedIndex = 5;
                   }*/
                #endregion
                site.Text = Midtransfer.sitesel;
            }
        }
        private void Quantity_Loaded(object sender, RoutedEventArgs e)
        {
            if (Midtransfer.edit)
            {
                #region
                /* if (int.Parse(Midtransfer.Quantity) == 10)
                 {
                     Quantity.SelectedIndex = 9;
                 }
                 else
                 {
                     for (int i = 0; i < Config.qual.Count; i++)
                     {
                         if (i.ToString() == Midtransfer.Quantity)
                         {
                             Quantity.SelectedIndex = i - 1;
                         }
                     }
                 }*/
                #endregion
                Quantity.Text = Midtransfer.Quantity;
               
            }
            else
            {
                

            }

        }
    }
}
