using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace MAIO
{
    static class Writecoookie
    {
        public static void write()
        {
            try
            {
                FileStream fs1 = new FileStream(Environment.CurrentDirectory + "\\" + "cookie.txt", FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter sw2 = new StreamWriter(fs1);
                fs1.SetLength(0);
                for (int i = 0; i < Mainwindow.lines.Count; i++)
                {
                    sw2.WriteLine(Mainwindow.lines[i]);
                }
                sw2.Close();
                fs1.Close();
            }
            catch
            {
                MessageBox.Show("fail to write cookie");
            }
        }
    }
}
