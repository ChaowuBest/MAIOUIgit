using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace MAIO
{
    static class Writecoookie
    {
       public static int n = 0;
        public static void write()
        {
            try
            {
                FileStream fs1 = new FileStream(Environment.CurrentDirectory + "\\" + "cookie.json", FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
                StreamWriter sw2 = new StreamWriter(fs1);
                fs1.SetLength(0);
                FileInfo fi = new FileInfo(Environment.CurrentDirectory + "\\" + "cookie.json");
                for (int i=0;i<Mainwindow.cookiewtime.Count;i++)
                {
                    KeyValuePair<long, string> kv = Mainwindow.cookiewtime.ElementAt(i);
                    string cookiewtime = "[{\"cookie\":\"" + kv.Value + "\",\"time\":\"" + kv.Key.ToString() + "\"}]";
                    if (n == 0)
                    {
                        FileStream fs2 = new FileStream(Environment.CurrentDirectory + "\\" + "cookie.json", FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                        JArray ja2 = JArray.Parse(cookiewtime);
                        StreamWriter sw = new StreamWriter(fs2);
                        sw.Write(ja2.ToString().Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace(" ", ""));
                        sw.Close();
                        fs2.Close();
                        n++;
                    }
                    else
                    {
                        using (FileStream fs3 = new FileStream(Environment.CurrentDirectory + "\\" + "cookie.json", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                        {
                            StreamReader sr = new StreamReader(fs3);
                            string read = sr.ReadToEnd();
                            sr.Close();
                            JArray ja3 = JArray.Parse(read);
                            ja3.Add(JObject.Parse(cookiewtime.Replace("[", "").Replace("]", "")));
                            FileStream fs4 = new FileStream(Environment.CurrentDirectory + "\\" + "cookie.json", FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                            fs4.SetLength(0);
                            StreamWriter sw = new StreamWriter(fs4);
                            var chao=ja3.ToString();
                            sw.Write(ja3.ToString().Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace(" ", ""));
                            sw.Close();
                            fs4.Close();
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("fail to write cookie");
            }
        }
    }
}
