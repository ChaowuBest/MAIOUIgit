
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MAIO.ASOS
{
    class ASOSAPI
    {
        Random ran = new Random();
        public string setAtccookie = "";
        public bool getsession = false;
        public string GetHtmlsource(string url, Main.taskset tk, CancellationToken ct,ASOSsession session)
        {
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();

            }
            string SourceCode = "";
            int random = ran.Next(0, Mainwindow.proxypool.Count);
            WebProxy wp = new WebProxy();
            try
            {
                string proxyg = Mainwindow.proxypool[random].ToString();
                string[] proxy = proxyg.Split(":");
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = wp;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var cc = response.Headers["Set-Cookie"];
                string[] cookiename = new string[4] { "asos-ts121", "anon12", "idsvr.session", "idsrv" };
                for (int i = 0; i < cookiename.Length; i++)
                {
                    Regex rex3 = new Regex(@"(?<=" + cookiename[i] + "=)([^;]+)");
                    cookiename[i] += "=" + rex3.Match(cc).ToString();
                    setAtccookie += cookiename[i] + "; ";
                }
                long time2 = (long)(DateTime.Now.ToUniversalTime() - timeStampStartTime).TotalMilliseconds;
                string str1=session.sessionid =MD5Helper.EncryptString(setAtccookie);
                string str2 = MD5Helper.EncryptString(cc);
                string url2 = "https://asoscomltd.tt.omtrdc.net/m2/asoscomltd/mbox/json?mbox=target-global-mbox&mboxSession=73a876e8b8694004890b5a32436ff105&mboxPC=d8f6ae5d727c4996a396e8574bafff40.38_0&mboxPage=c9222fd75ce94a2c85279f5f2d25e23e&mboxVersion=1.2.3&mboxCount=1&mboxTime=1595257495721&mboxHost=www.asos.com&mboxURL=https://www.asos.com/nike/nike-air-force-1-07-trainers-in-white-with-neon-swoosh/prd/20779383?colourwayid=60087308&SearchQuery=nike&mboxReferrer=https://www.asos.com/search/?page=1&q=nike&scrollTo=product-20779383&mboxXDomain=enabled&browserHeight=892&browserWidth=1489&browserTimeOffset=480&screenHeight=1003&screenWidth=1504&colorDepth=24&mboxMCGVID=18747098554919851110758331684818879126&mboxAAMB=RKhpRz8krg2tLO6pguXWp5olkAcUniQYPHaMWWgdJ3xzPWQmdj0y&mboxMCAVID=&mboxMCGLH=11&mboxMCSDID=0A118FE88FFEF152-6073A79AB0BA4CDC";
                //string url2 = "https://asoscomltd.tt.omtrdc.net/m2/asoscomltd/mbox/json?mbox=target-global-mbox&mboxSession="+ str1 + "&mboxPC=&mboxPage="+str2+"&mboxVersion=1.2.3&mboxCount=1&mboxTime=" + time2 + "&mboxHost=www.asos.com&mboxURL=https://www.asos.com/men/&mboxReferrer=https://www.asos.com/men/&mboxXDomain=enabled&browserHeight=809&browserWidth=811&browserTimeOffset=480&screenHeight=1003&screenWidth=1504&colorDepth=24&mboxMCGVID=03573757540991735524373900392433518765&mboxAAMB=RKhpRz8krg2tLO6pguXWp5olkAcUniQYPHaMWWgdJ3xzPWQmdj0y&mboxMCAVID=&mboxMCGLH=11&mboxMCSDID=3D234569427EC313-158308AAA0F99888";
                Task tk1 = new Task(() => Getsession(url2, tk, ct));
                tk1.Start();
               
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.ContentEncoding == "gzip")
                {
                    readStream = new StreamReader(new GZipStream(receiveStream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.UTF8);
                }
                SourceCode = readStream.ReadToEnd();
                JObject jo = JObject.Parse(SourceCode);
                return "bearer " + jo["access_token"].ToString();
            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                goto A;
            }

        }
        public string Getvariant(string url, Main.taskset tk, CancellationToken ct)
        {
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();

            }
            string SourceCode = "";
            int random = ran.Next(0, Mainwindow.proxypool.Count);
            WebProxy wp = new WebProxy();
            try
            {
                string proxyg = Mainwindow.proxypool[random].ToString();
                string[] proxy = proxyg.Split(":");
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = wp;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.ContentEncoding == "gzip")
                {
                    readStream = new StreamReader(new GZipStream(receiveStream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.UTF8);
                }
                JObject jo = null;
                try
                {
                    SourceCode = readStream.ReadToEnd();
                    Regex rex = new Regex(@"(?<=product = )([^;]+)");
                    jo = JObject.Parse(rex.Match(SourceCode).ToString());
                }
                catch
                {
                    goto A;
                }
                return jo["variants"].ToString();
            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                goto A;
            }



        }
        public void Addtocart(string url, Main.taskset tk, CancellationToken ct, string accesstoken, string skuid, string payinfo, ASOSsession session)
        {
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();

            }
            string SourceCode = "";
            int random = ran.Next(0, Mainwindow.proxypool.Count);
            WebProxy wp = new WebProxy();
            try
            {
                string proxyg = Mainwindow.proxypool[random].ToString();
                string[] proxy = proxyg.Split(":");
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
            if (getsession==false)
            {
                goto A;   
            }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            byte[] contentpaymentinfo = Encoding.UTF8.GetBytes(payinfo);
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.ContentLength = contentpaymentinfo.Length;
            request.Headers.Add("Cookie", setAtccookie+ " mboxsession=" + session.sessionid);
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            request.Headers.Add("asos-bag-origin", "EUN");
            request.Headers.Add("asos-c-ismobile", "false");
            request.Headers.Add("asos-c-istablet", "false");
            request.Headers.Add("asos-c-name", "Asos.Commerce.Bag.Sdk");
            request.Headers.Add("asos-c-plat", "Web");
            request.Headers.Add("asos-c-ver", "5.0.2");
            request.Headers.Add("Authorization", accesstoken);
            request.Headers.Add("asos-cid", Guid.NewGuid().ToString());
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-origin");
            request.Headers.Add("origin", "https://www.asos.com");
            request.Proxy = wp;
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            Stream paymentstream = request.GetRequestStream();
            paymentstream.Write(contentpaymentinfo, 0, contentpaymentinfo.Length);
            paymentstream.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.ContentEncoding == "gzip")
                {
                    readStream = new StreamReader(new GZipStream(receiveStream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.UTF8);
                }
                JObject jo = null;
                try
                {
                    SourceCode = readStream.ReadToEnd();
                    Regex rex = new Regex(@"(?<=product = )([^;]+)");
                    jo = JObject.Parse(rex.Match(SourceCode).ToString());
                }
                catch
                {
                    goto A;
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.ContentEncoding == "gzip")
                {
                    readStream = new StreamReader(new GZipStream(receiveStream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.UTF8);
                }
                SourceCode = readStream.ReadToEnd();
                goto A;
            }

        }
        private static DateTime timeStampStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public void Getsession(string url, Main.taskset tk, CancellationToken ct)
        {
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();

            }
            int random = ran.Next(0, Mainwindow.proxypool.Count);
            WebProxy wp = new WebProxy();
            try
            {
                string proxyg = Mainwindow.proxypool[random].ToString();
                string[] proxy = proxyg.Split(":");
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
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = wp;
            request.Headers.Add("Cookie", setAtccookie);
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var cc = response.Headers["Set-Cookie"];
                getsession = true;
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;
                if (response.ContentEncoding == "gzip")
                {
                    readStream = new StreamReader(new GZipStream(receiveStream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.UTF8);
                }
            }
            catch (WebException)
            {
                goto A;
            }

        }
        public partial class MD5Helper
        {
            public static string EncryptString(string str)
            {
                MD5 md5 = MD5.Create();
                byte[] byteOld = Encoding.UTF8.GetBytes(str);
                byte[] byteNew = md5.ComputeHash(byteOld);
                StringBuilder sb = new StringBuilder();
                foreach (byte b in byteNew)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
