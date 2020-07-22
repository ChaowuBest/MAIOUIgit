using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Windows;

namespace MAIO
{
    public class CheckCard
    {
        Random ran = new Random();
        public double Postcardinfo(string url, string cardinfo)
        {
        B: int random = ran.Next(0, Mainwindow.proxypool.Count);
           
            WebProxy wp = new WebProxy();
            try
            {
                string proxyg = Mainwindow.proxypool[random].ToString();
                string[] proxy = proxyg.Split(",");
                if (proxy.Length == 2)
                {
                    wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");

                }
                else if (proxy.Length == 4)
                {
                    wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");
                    wp.Credentials = new NetworkCredential(proxy[2], proxy[3]);
                }
            }
            catch
            {
                wp = default;
            }
            double balance = 0;
            byte[] contentcardinfo = Encoding.UTF8.GetBytes(cardinfo);
            HttpWebRequest reqcard = (HttpWebRequest)WebRequest.Create(url);
            reqcard.Proxy = wp;
            reqcard.Method = "POST";
            reqcard.ContentType = "application/json";
            reqcard.Accept = "*/*";
            reqcard.Headers.Add("Accept-Encoding", "gzip, deflate");
            reqcard.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            reqcard.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            reqcard.ContentLength = contentcardinfo.Length;
            reqcard.Headers.Add("sec-fetch-dest", "empty");
            reqcard.Headers.Add("sec-fetch-mode", "cors");
            reqcard.Headers.Add("sec-fetch-site", "same-site");
            reqcard.Headers.Add("appid", "com.nike.commerce.snkrs.web");
            reqcard.Headers.Add("x-b3-spanname", "CiCCart");
            reqcard.Headers.Add("x-b3-traceid", Guid.NewGuid().ToString());
            reqcard.Headers.Add("x-nike-visitid", "1");
            reqcard.Headers.Add("x-nike-visitorid", Guid.NewGuid().ToString());
            Stream cardstream = reqcard.GetRequestStream();
            cardstream.Write(contentcardinfo, 0, contentcardinfo.Length);
            cardstream.Close();
            try
            {
                HttpWebResponse respcard = (HttpWebResponse)reqcard.GetResponse();
                Stream respcardStream = respcard.GetResponseStream();
                StreamReader readhtmlStream = new StreamReader(respcardStream, Encoding.UTF8);
                string SourceCode = readhtmlStream.ReadToEnd();
                if (SourceCode.Contains("balance"))
                {
                    JObject jo = JObject.Parse(SourceCode);
                    string bal = jo["balance"].ToString();
                    balance = Convert.ToDouble(bal);
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse respcard = (HttpWebResponse)ex.Response;
                Stream respcardStream = respcard.GetResponseStream();
                StreamReader readhtmlStream = new StreamReader(respcardStream, Encoding.UTF8);
                string SourceCode = readhtmlStream.ReadToEnd();
                goto B;
            }
            return balance;
        }
    }
}
