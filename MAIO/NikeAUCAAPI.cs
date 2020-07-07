using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Windows.Shapes;

namespace MAIO
{
    class NikeAUCAAPI
    {
        Random ran = new Random();
        string xb3traceid = Guid.NewGuid().ToString();
        string xnikevisitorid = Guid.NewGuid().ToString();
        public string GetHtmlsource(string url,Main.taskset tk, CancellationToken ct)
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
                SourceCode = readStream.ReadToEnd();
                response.Close();
                readStream.Close();
                tk.Status = "Get Size";
            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                tk.Status = "Get Size Error";
                tk.Status = "Change Proxy";
                goto A;
            }
            return SourceCode;
        }     
        public void PutMethod(string url, string payinfo,Main.taskset tk, CancellationToken ct)
        {

        B: if (ct.IsCancellationRequested)
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
            request.Method = "PUT";
            request.Proxy = wp;
            request.ContentType = "application/json; charset=UTF-8";
            byte[] contentpaymentinfo = Encoding.UTF8.GetBytes(payinfo);
            request.Accept = "application/json";
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("appid", "com.nike.commerce.nikedotcom.web");
            request.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            if (Config.UseAdvancemode == "True")
            {
                string path = Environment.CurrentDirectory + "\\" + "advancecookie.txt";
                FileStream fs2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                StreamReader sr = new StreamReader(fs2);
                string cookie = sr.ReadToEnd();
                request.Headers.Add("Cookie", cookie);
                sr.Close();
                fs2.Close();
            }
            else
            {
                if (Mainwindow.iscookielistnull)
                {
                    request.Headers.Add("Cookie", "");
                }
                else
                {
                    Random ra = new Random();
                reloadcookie: int sleeptime = ra.Next(0, 500);
                    Thread.Sleep(sleeptime);
                    if (Mainwindow.lines.Count == 0)
                    {
                        Mainwindow.iscookielistnull = true;
                    C: tk.Status = "No Cookie";

                        if (Mainwindow.iscookielistnull)
                        {
                            goto C;
                        }
                        else
                        {
                            goto D;
                        }
                    }
                D: int cookie = ra.Next(0, Mainwindow.lines.Count);
                    try
                    {
                        request.Headers.Add("Cookie", Mainwindow.lines[cookie]);
                        Mainwindow.lines.RemoveAt(cookie);
                    }
                    catch (Exception)
                    {
                        goto reloadcookie;
                    }
                }
            }          
            Main.updatelable();
            request.ContentLength = contentpaymentinfo.Length;
            request.Headers.Add("Origin", "https://www.nike.com");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-site");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            request.Headers.Add("x-b3-spanname", "CiCCart");
            request.Headers.Add("x-b3-traceid", xb3traceid);
            request.Headers.Add("x-nike-visitid", "1");
            request.Headers.Add("x-nike-visitorid", xnikevisitorid);
            Stream paymentstream = request.GetRequestStream();
            paymentstream.Write(contentpaymentinfo, 0, contentpaymentinfo.Length);
            paymentstream.Close();
            try
            {
                HttpWebResponse resppayment = (HttpWebResponse)request.GetResponse();
                tk.Status = "SubmitOrder";
            }
            catch (WebException ex)
            {
                HttpWebResponse resppayment = (HttpWebResponse)ex.Response;
                tk.Status = resppayment.StatusCode.ToString();      
                Thread.Sleep(1500);
                goto B;
            }          
        }
        public string GetMethod(string url,string profile,string size,string pid,bool randomsize, Main.taskset tk, CancellationToken ct, CancellationTokenSource cts)
        {
        C: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            int random = ran.Next(0, Mainwindow.proxypool.Count);
            string proxyg = Mainwindow.proxypool[random].ToString();
            string[] proxy = proxyg.Split(":");
            WebProxy wp = new WebProxy();
            if (proxy.Length == 2)
            {
                wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");

            }
            else if (proxy.Length == 4)
            {
                wp.Address = new Uri("http://" + proxy[0] + ":" + proxy[1] + "/");
                wp.Credentials = new NetworkCredential(proxy[2], proxy[3]);
            }
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = wp;
            request.ContentType = "application/json; charset=UTF-8";
            request.Method = "GET";
            request.Accept = "application/json";
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("appid", "com.nike.commerce.nikedotcom.web");
            request.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            request.Headers.Add("Origin", "https://www.nike.com");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-site");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            request.Headers.Add("x-b3-spanname", "CiCCart");
            request.Headers.Add("x-b3-traceid", xb3traceid);
            request.Headers.Add("x-nike-visitid", "1");
            request.Headers.Add("x-nike-visitorid", xnikevisitorid);
            string sourcecode = "";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                tk.Status = "processing";
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                if (response.ContentEncoding == "gzip")
                {
                    readStream = new StreamReader(new GZipStream(receiveStream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.UTF8);
                }
                sourcecode = readStream.ReadToEnd();
                 
                if (sourcecode.Contains("COMPLETED") == false)
                {
                    Thread.Sleep(1500);
                    goto C;
                }
                if ((sourcecode.Contains("COMPLETED") == true) && (sourcecode.Contains("error")))
                {
                  /*  JObject jo = JObject.Parse(sourcecode);
                    string error = jo["error"].ToString();
                    JObject jo2 = JObject.Parse(error);
                    var reason = jo2["errors"][0].ToString();
                    JObject jo3 = JObject.Parse(reason);
                    string errormessage = jo3["code"].ToString();
                    tk.Status = errormessage;
                    tk.Status = errormessage+"Retrying";
                    int rae = ran.Next(1000, 3000);
                    Thread.Sleep(rae);
                    xb3traceid = Guid.NewGuid().ToString();
                    xnikevisitorid = Guid.NewGuid().ToString();
                    NikeAUCA NAU = new NikeAUCA();
                    NAU.size = size;
                    NAU.pid = pid;
                    NAU.profile = profile;
                    NAU.randomsize = randomsize;
                    NAU.StartTask(ct,cts);*/
                }
            }
            catch (WebException ex)
            {
                tk.Status = ex.Status.ToString()+"Retrying";
                goto C;
            }
            return sourcecode;
        }
    }
}
