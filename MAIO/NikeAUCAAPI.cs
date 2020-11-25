using System;
using System.Net;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Threading;
using Newtonsoft.Json.Linq;
using static MAIO.Main;
using Newtonsoft.Json;
using System.ServiceModel;
using System.Collections;
using System.Windows;

namespace MAIO
{
    class NikeAUCAAPI
    {
        Random ran = new Random();
        string xb3traceid = Guid.NewGuid().ToString();
        string xnikevisitorid = Guid.NewGuid().ToString();
        public int failedretry = 0;
        public string GetHtmlsource(string url, Main.taskset tk, CancellationToken ct)
        {
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            Thread.Sleep(1);
            string SourceCode = "";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 5000;
            request.Proxy = getproxy();
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            request.Headers.Add("Accept-Encoding", "gzip, deflate br");
            request.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            request.Headers.Add("Sec-Fetch-Dest", "document");
            request.Headers.Add("cache-control", "max-age=0");
            request.Headers.Add("Sec-Fetch-Mode", "navigate");
            request.Headers.Add("Sec-Fetch-Site", "none");
            request.Headers.Add("Sec-Fetch-user", "?1");
            request.Headers.Add("upgrade-insecure-requests", "1");
            request.UserAgent = "Sogou inst spider";
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
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                response.Close();
                readStream.Close();
            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                tk.Status = "Change Proxy";
                goto A;
            }
            return SourceCode;
        }
        public void PutMethod(string url, string payinfo, Main.taskset tk, CancellationToken ct, string GID)
        {
            if (Config.UseAdvancemode == "True")
            {
            C:
                string[] sendcookie = null;
                if (Mainwindow.iscookielistnull || Mainwindow.lines.Count == 0)
                {
                    tk.Status = "Get Cookie Error";
                    Thread.Sleep(1);
                    goto C;
                }
                else
                {
                    Thread.Sleep(1);
                    Random ra = new Random();
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    int sleeptime = ra.Next(0, 100);
                    Thread.Sleep(sleeptime);
                    int cookie = ra.Next(0, Mainwindow.lines.Count);
                    try
                    {
                        Main.updatelable(Mainwindow.lines[cookie], false);
                        sendcookie = Mainwindow.lines[cookie].Split(";");
                        Mainwindow.lines.RemoveAt(cookie);
                       // if (Mainwindow.lines.Count == 0)
                      //  {
                     //       Mainwindow.iscookielistnull = true;
                     //   }
                    }
                    catch (Exception)
                    {
                        goto C;
                    }
                }
                string proxy = "";
                try
                {
                    int random = ran.Next(0, Mainwindow.proxypool.Count);
                    proxy = Mainwindow.proxypool[random].ToString();
                }
                catch
                {
                    proxy = "";
                }
                try
                {
                    string sendjson = null;
                    try
                    {
                        var chao = JsonConvert.SerializeObject(payinfo).Replace("\"{", "{").Replace("}\"", "}");
                        Random ran = new Random();
                        sendjson = "{\"data\":{\"headers\":{\"Content-Type\":\"application/json\",\"Origin\":\" https://www.nike.com\",\"Accept\":\"application/json\",\"x-nike-visitid\":\"1\",\"x-nike-visitorid\":\"" + xnikevisitorid + "\",\"appid\":\"com.nike.commerce.nikedotcom.web\",\"X-B3-SpanName\":\"CiCCart\",\"X-B3-TraceId\":\"" + xb3traceid + "\"},\"url\":\"" + url + "\",\"method\":\"PUT\",\"data\":\"" + chao + "\",\"proxy\":\"" + proxy + "\",\"cookies\":[{\"Name\":\"_abck\",\"TimeStamp\":\"" + DateTime.Now.ToLocalTime().ToString() + "\",\"Value\":\"" + sendcookie[1].Replace("_abck=", "") + "\",\"Comment\":\"\",\"CommentUri\":null,\"HttpOnly\":false,\"Discard\":false,\"Expired\":false,\"Secure\":false,\"Domain\":\".nike.com\",\"Expires\":\"0001-01-01T00:00:00\",\"Path\":\"/\",\"Port\":\"\",\"Version\":0},{\"Name\":\"bm_sz\",\"TimeStamp\":\"2020/8/26 23:42:48\",\"Value\":\"" + sendcookie[0].Replace("bm_sz=", "") + "\",\"Comment\":\"\",\"CommentUri\":null,\"HttpOnly\":false,\"Discard\":false,\"Expired\":false,\"Secure\":false,\"Domain\":\".nike.com\",\"Expires\":\"0001-01-01T00:00:00\",\"Path\":\"/\",\"Port\":\"\",\"Version\":0}],\"id\":\"" + tk.Taskid + "\"},\"type\":\"request\"}";
                    }                                                                                                                                                                                                                                                                                                                                                                                                                      //Proxy: 94.131.28.12:16666:hv282517:zg612330
                    catch (Exception)
                    {
                    }
                    allSockets[0].Send(sendjson);
                B: bool fordidden = false;
                    JObject sValue = null;
                    try
                    {
                        if (returnstatus.TryGetValue(tk.Taskid, out sValue))
                        {
                            if (ct.IsCancellationRequested)
                            {
                                tk.Status = "IDLE";
                                ct.ThrowIfCancellationRequested();
                            }
                            else if (sValue.ToString().Contains("Failed to fetch"))
                            {
                                tk.Status = "Forbidden";
                                fordidden = true;
                                returnstatus.Remove(tk.Taskid);
                            }
                            if (sValue["status"].ToString() == "202")
                            {
                                tk.Status = "Submit Order";
                                returnstatus.Remove(tk.Taskid);
                            }
                            else if (sValue["status"].ToString() == "403")
                            {
                                tk.Status = "Submit Order Forbidden";
                                fordidden = true;
                                returnstatus.Remove(tk.Taskid);
                            }
                            else
                            {
                                tk.Status = "Forbidden";
                                fordidden = true;
                                returnstatus.Remove(tk.Taskid);
                            }
                        }
                        else
                        {
                            Thread.Sleep(500);
                            goto B;
                        }
                    }
                    catch (NullReferenceException)
                    {
                        Thread.Sleep(500);
                        goto B;
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                    if (fordidden)
                    {
                        failedretry++;
                        if (failedretry > 20)
                        {
                            Main.autorestock(tk);
                            if (ct.IsCancellationRequested)
                            {
                                tk.Status = "IDLE";
                                ct.ThrowIfCancellationRequested();
                            }
                        }
                        Thread.Sleep(1500);
                        goto C;
                    }
                }
                catch
                {
                    goto C;
                }
            }
            else
            {
            B: if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://127.0.0.1:1234/buy/partner_cart_preorder/v1/" + GID);
                request.Method = "PUT";
                request.ContentType = "application/json; charset=UTF-8";
                byte[] contentpaymentinfo = Encoding.UTF8.GetBytes(payinfo);
                request.Accept = "application/json";
                request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
                request.Headers.Add("cloud_stack", "buy_domain");
                request.Headers.Add("appid", "com.nike.commerce.nikedotcom.web");
                request.Headers.Add("Server-Host", "api.nike.com:443");
                request.Headers.Add("Proxy-Address", getAdvproxy());
            C: if (Mainwindow.iscookielistnull || Mainwindow.lines.Count == 0)
                {
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    try
                    {
                        var binding = new BasicHttpBinding();
                        var endpoint = new EndpointAddress(@"http://49.51.68.105/WebService1.asmx");
                        var factory = new ChannelFactory<ServiceReference2.WebService1Soap>(binding, endpoint);
                        var callClient = factory.CreateChannel();
                        JObject result = JObject.Parse(callClient.getcookieAsync(Config.hwid).Result);
                        request.Headers.Add("Cookie", result["cookie"].ToString());
                        //  request.Headers.Add("Cookie","bm_sz=9B2364F51452FC53D08D50289B431B8A~YAAQRf2Yc4f0GDx1AQAAguO/Rgm6DDPa+MSXhAPi/dUMHQuTb1BlS0OKKWEBTAwjHf2J4CTeCEJttTcBk1BtIARcOgfZKuEmlumoRzUxmKFZD5fImpwwM94BKVa3vPNnCTbQL9YnjI9VFcdYciSIWfJxTZpMasNmne1KOGTZcGxtw4GkNZhYvfyXc92LiQr494ge6D/0ir+BEpw6xLNr1wHA70W6qo8BH+zy1plIw5kjhJC3U8jh7Y2jCHqoQRUexJfjGz2mF7Uw+YnqYIkVPh+Pr1X+z8ubbQ==;_abck=690B9E741739EF88CC8E49574EBAADD9~-1~YAAQRf2Yc5v1GDx1AQAAO/G/RgTmXlBetKqMuNIhLd/zY9YSY4/Mimib3KIKQsEjrQJV8LTrKQB050ykD/eLeY0pci4TUICA/ojIr+bwiSr73bmJSKILYaz94pw7h1VYPHQ5zokqqcpf6nAC3tDRwj+wUizZClbD4sr7Q0iws3hjbW4DxeR3bOKkvyI+JM8oqJyvaoAjghNAbttXosGbFtISFJGSxXpH6xLbZZbnUwMglmw139kf6TLqLrRk0pS3Yb3gpJn0eZuHcWirh5YHjOo4Do2nlxuNZRJ4NYlV0QwBuHyFplB68sr2pWcduMGOaySMB4LKivwND2aRVs5zcPDZHSwF0BJZ9LcNaWoe1nZrjKI8psFNH5taGvpgS1znwiXQUyV1byv3JgUtVAonXIfZGcbcLnwScemgxaHSZ209UboB3uyl~-1~-1~-1");
                    }
                    catch
                    {
                        tk.Status = "Get Cookie Error";
                        Thread.Sleep(1);
                        goto C;
                    }
                }
                else
                {
                    Random ra = new Random();
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    Thread.Sleep(1);
                    int cookie = ra.Next(0, Mainwindow.lines.Count);
                    try
                    {
                        Main.updatelable(Mainwindow.lines[cookie], false);
                        request.Headers.Add("Cookie", Mainwindow.lines[cookie]);
                        Mainwindow.lines.RemoveAt(cookie);
                        if (Mainwindow.lines.Count == 0)
                        {
                            Mainwindow.iscookielistnull = true;
                        }
                    }
                    catch (Exception)
                    {
                        goto C;
                    }
                }
                request.ContentLength = contentpaymentinfo.Length;
                request.Headers.Add("Origin", "https://www.nike.com");
                request.Headers.Add("Sec-Fetch-Dest", "empty");
                request.Headers.Add("Sec-Fetch-Mode", "cors");
                request.Headers.Add("Sec-Fetch-Site", "same-site");
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36";
                request.Headers.Add("X-B3-SpanName", "CiCCart");
                request.Headers.Add("X-B3-TraceId", xb3traceid);
                request.Headers.Add("x-nike-visitid", "1");
                request.Headers.Add("x-nike-visitorid", xnikevisitorid);
                Stream paymentstream = request.GetRequestStream();
                paymentstream.Write(contentpaymentinfo, 0, contentpaymentinfo.Length);
                paymentstream.Close();
                try
                {
                    HttpWebResponse resppayment = (HttpWebResponse)request.GetResponse();
                    tk.Status = "Submit Order";
                    Stream receiveStream = resppayment.GetResponseStream();
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    if (resppayment.ContentEncoding == "gzip")
                    {
                        readStream = new StreamReader(new GZipStream(receiveStream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                    }
                    else
                    {
                        readStream = new StreamReader(receiveStream, Encoding.UTF8);
                    }
                    string sourcecode = readStream.ReadToEnd();
                }
                catch (WebException ex)
                {
                    HttpWebResponse respgetstatus = (HttpWebResponse)ex.Response;
                    Stream respcheckstatusstream = respgetstatus.GetResponseStream();
                    StreamReader readcheckstatus = new StreamReader(respcheckstatusstream, Encoding.GetEncoding("utf-8"));
                    string check = readcheckstatus.ReadToEnd();
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                    tk.Status = "Submit Order Forbidden";
                    failedretry++;
                    if (failedretry > 20)
                    {
                        Main.autorestock(tk);
                        if (ct.IsCancellationRequested)
                        {
                            tk.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                    }
                    Thread.Sleep(1500);
                    goto B;
                }
            }
        }
        public string GetMethod(string url, string iamgeurl, Main.taskset tk, CancellationToken ct)
        {
        C: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            Thread.Sleep(1);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = getproxy();
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
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36";
            request.Headers.Add("x-b3-spanname", "CiCCart");
            request.Headers.Add("x-b3-traceid", xb3traceid);
            request.Headers.Add("x-nike-visitid", "1");
            request.Headers.Add("x-nike-visitorid", xnikevisitorid);
            request.Timeout = 5000;
            string sourcecode = "";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                tk.Status = "Processing";
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
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                if (sourcecode.Contains("COMPLETED") == false)
                {
                    Thread.Sleep(200);
                    goto C;
                }
                if (sourcecode.Contains("Reservation with Inventory"))
                {
                    tk.Status = "WaitingRestock";
                    Main.autorestock(tk);
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                }
                if ((sourcecode.Contains("COMPLETED") == true) && (sourcecode.Contains("error")))
                {
                    JObject jo = JObject.Parse(sourcecode);
                    try
                    {
                        tk.Status = jo["error"]["errors"][0]["code"].ToString();
                    }
                    catch
                    {

                    }
                    Thread.Sleep(3000);
                    Main.autorestock(tk);
                    if (ct.IsCancellationRequested)
                    {
                        tk.Status = "IDLE";
                        ct.ThrowIfCancellationRequested();
                    }
                }
            }
            catch (WebException ex)
            {
                //HttpWebResponse response = (HttpWebResponse)ex.Response;
                tk.Status = "Processing Forbidden";
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                goto C;
            }
            return sourcecode;
        }
        public string[] Monitoring(string url, taskset tk, CancellationToken ct, string info, bool randomsize, string skuid, bool multisize, ArrayList skulist)
        {
            DateTime dtone = Convert.ToDateTime(DateTime.Now.ToLocalTime().ToString());
            if (tk.monitortask == "True")
            {
                try
                {
                    ArrayList ary = null;
                    if (share_dog_skuid.TryGetValue(tk.Tasksite + tk.Sku, out ary) == false)
                    {
                        share_dog_skuid.Add(tk.Tasksite + tk.Sku, new ArrayList());
                    }
                    else
                    {
                        share_dog_skuid[tk.Tasksite + tk.Sku].Clear();
                    }
                }
                catch { }
            }
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            DateTime dttwo = Convert.ToDateTime(DateTime.Now.ToLocalTime().ToString());
            TimeSpan ts = dttwo - dtone;
            if (ts.TotalMinutes >= 50)
            {
                Random ran = new Random();
                int sleeptime = ran.Next(0, 600);
                Thread.Sleep(sleeptime);
                Main.autorestock(tk);
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
            }
            Thread.Sleep(1);
            string traceid = Guid.NewGuid().ToString();
            string nikevistid = Guid.NewGuid().ToString();
            string SourceCode = "";
            string[] group = new string[2];
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Proxy = monitorproxy();
            request.Method = "POST";
            request.Host = "api.nike.com";
            request.Accept = "*/*";
            request.ContentType = "application/json; charset=UTF-8";
            byte[] contentcardinfo = Encoding.UTF8.GetBytes(info);
            request.ContentLength = contentcardinfo.Length;
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-site");
            request.Headers.Add("X-B3-SpanName", "CiCCart");
            request.Headers.Add("X-B3-TraceId", traceid);
            request.Headers.Add("x-nike-visitid", "1");
            request.Headers.Add("x-nike-visitorid", nikevistid);
            request.Headers.Add("upgrade-insecure-requests", "1");
            request.UserAgent = "Sogou inst spider";
            Stream cardstream = request.GetRequestStream();
            cardstream.Write(contentcardinfo, 0, contentcardinfo.Length);
            cardstream.Close();
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                traceid = Guid.NewGuid().ToString();
                nikevistid = Guid.NewGuid().ToString();
                tk.Status = "WaitingRestock";
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
                if (SourceCode.Contains("Product not found") || SourceCode.Contains("errors"))
                {
                    tk.Status = "Check Stock Error";
                    goto A;
                }
                JObject jo = JObject.Parse(SourceCode);
                JArray ja = JArray.Parse(jo["data"]["skus"][0]["product"]["skus"].ToString());
                try
                {
                    if (share_dog_skuid.ContainsKey(tk.Tasksite + tk.Sku))
                    {
                        share_dog_skuid[tk.Tasksite + tk.Sku].Clear();
                    }
                }
                catch { }
                for (int i = 0; i < ja.Count; i++)
                {
                    Thread.Sleep(1);
                    group[1] = jo["data"]["skus"][0]["product"]["id"].ToString();
                    if (randomsize)
                    {
                        if (ja[i]["availability"]["level"].ToString() != "OOS")
                        {
                            if (tk.monitortask == "True")
                            {
                                share_dog_skuid[tk.Tasksite + tk.Sku].Add(ja[i]["id"].ToString());
                            }
                            else
                            {
                                group[0] = ja[i]["id"].ToString();
                                break;
                            }
                        }
                    }
                    else if (multisize)
                    {
                        for (int n = 0; n < skulist.Count; n++)
                        {
                            Thread.Sleep(1);

                            if (skulist[n].ToString() == ja[i]["id"].ToString())
                            {
                                if (ja[i]["availability"]["level"].ToString() != "OOS")
                                {
                                    if (tk.monitortask == "True")
                                    {
                                        share_dog_skuid[tk.Tasksite + tk.Sku].Add(ja[i]["id"].ToString());
                                    }
                                    else
                                    {
                                        group[0] = ja[i]["id"].ToString();
                                        break;
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        group[0] = skuid;
                        if (ja[i]["id"].ToString() == skuid)
                        {
                            if (ja[i]["availability"]["level"].ToString() == "OOS")
                            {
                                if (Config.delay == "")
                                {
                                    Thread.Sleep(1);
                                }
                                else
                                {
                                    Thread.Sleep(int.Parse(Config.delay));
                                }
                                goto A;
                            }
                            else
                            {
                                if (tk.monitortask == "True")
                                {
                                    share_dog_skuid[tk.Tasksite + tk.Sku].Add(ja[i]["id"].ToString());
                                    break;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }

                }
                response.Close();
                readStream.Close();
            }
            catch (WebException ex)
            {
                tk.Status = "Proxy Error";
                goto A;
            }
            if (group[0] == null)
            {
                if (tk.monitortask == "True")
                {
                    if (share_dog_skuid[tk.Tasksite + tk.Sku].Count == 0)
                    {
                        share_dog[tk.Tasksite + tk.Sku] = false;
                        if (ct.IsCancellationRequested)
                        {
                            tk.Status = "IDLE";
                            share_dog[tk.Tasksite + tk.Sku] = false;
                            share_dog_skuid[tk.Tasksite + tk.Sku].Clear();
                            ct.ThrowIfCancellationRequested();
                        }
                        goto A;
                    }
                }
                else
                {
                    goto A;
                }
            }
            if (tk.monitortask == "True")
            {
                tk.Status = "Get Stock";
                share_dog[tk.Tasksite + tk.Sku] = true;
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    share_dog[tk.Tasksite + tk.Sku] = false;
                    share_dog_skuid[tk.Tasksite + tk.Sku].Clear();
                    ct.ThrowIfCancellationRequested();
                }
                goto A;
            }
            return group;
        }
        public void Http(string url, string postDataStr)
        {
            Thread.Sleep(1);
        Retry: Random ra = new Random();
            int sleeptime = ra.Next(0, 3000);
            Thread.Sleep(sleeptime);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "post";
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            byte[] bytes = Encoding.UTF8.GetBytes(postDataStr);
            request.ContentLength = bytes.Length;
            Stream webstream = request.GetRequestStream();
            webstream.Write(bytes, 0, bytes.Length);
            webstream.Close();
            try
            {
                HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                Thread.Sleep(1000);
                goto Retry;
            }

        }
        public WebProxy monitorproxy()
        {
            WebProxy wp = new WebProxy();
            if (Mainwindow.monitorproxypool.Count == 0)
            {
                wp = default;
            }
            else
            {
                int random = ran.Next(0, Mainwindow.monitorproxypool.Count);
                string proxyg = Mainwindow.monitorproxypool[random].ToString();
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
            return wp;
        }
        public WebProxy getproxy()
        {

            WebProxy wp = new WebProxy();
            if (Mainwindow.proxypool.Count == 0)
            {
                wp = default;
            }
            else
            {
                int random = ran.Next(0, Mainwindow.proxypool.Count);
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
            return wp;
        }
        public string getAdvproxy()
        {
            string proxyaddress = null;
            if (Mainwindow.proxypool.Count == 0)
            {
                proxyaddress = "";
            }
            else
            {
                int random = ran.Next(0, Mainwindow.proxypool.Count);
                string proxyg = Mainwindow.proxypool[random].ToString();
                string[] proxy = proxyg.Split(":");
                if (proxy.Length == 2)
                {
                    proxyaddress = "http://" + proxy[0] + ":" + proxy[1] + "";
                }
                else if (proxy.Length == 4)
                {
                    proxyaddress = "http://" + proxy[2] + ":" + proxy[3] + "@" + proxy[0] + ":" + proxy[1] + "";
                }
            }
            return proxyaddress;
        }
    }
}