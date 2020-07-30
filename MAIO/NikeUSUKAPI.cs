using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MAIO
{
    class NikeUSUKAPI
    {
        Random ran = new Random();
         string akbmsz = "ak_bmsc=";
        string xb3traceid = Guid.NewGuid().ToString();
      // string xnikevisitorid = Guid.NewGuid().ToString();
        public string GetHtmlsource(string url, Main.taskset tk, CancellationToken ct)
        {
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            WebProxy wp = new WebProxy();
            string SourceCode = "";
            int random = ran.Next(0, Mainwindow.proxypool.Count);
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
            catch (ArgumentOutOfRangeException)
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
        public string Postlogin(string url, string logininfo, bool isrefresh, string account, Main.taskset tk, CancellationToken ct)
        {
        retry: int random = ran.Next(0, Mainwindow.proxypool.Count);
            WebProxy wp = new WebProxy();
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
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
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Proxy = wp;
            byte[] contentByte = Encoding.UTF8.GetBytes(logininfo);
            req.Method = "POST";
            req.Headers.Add("Accept", "*/*");
            req.Headers.Add("Accept-Language", "en-US,en;q=0.9");
            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            req.ContentType = "application/json";
            req.Headers.Add("Accept-encoding", "gzip, deflate, br");
            req.Headers.Add("Host", "unite.nike.com");
            req.ContentLength = contentByte.Length;
            if (isrefresh == false)
            {
                if (Mainwindow.iscookielistnull)
                {
                    req.Headers.Add("Cookie", "");
                }
                else
                {
                    if ((Config.cjevent != "") && (Config.cid != ""))
                    {
                    reloadcookie: Random ra = new Random();
                        int sleeptime = ra.Next(0, 500);
                        Thread.Sleep(sleeptime);
                        if (Mainwindow.lines.Count == 0)
                        {
                            Mainwindow.iscookielistnull = true;
                        C: if (ct.IsCancellationRequested)
                            {
                                tk.Status = "IDLE";
                                ct.ThrowIfCancellationRequested();
                            }
                            tk.Status = "No Cookie";
                           // MessageBox.Show("1234");
                            //Main.updatelable("123", true);
                            goto C;
                        }
                        else
                        {
                            int cookie = ra.Next(0, Mainwindow.lines.Count);
                            try
                            {
                                Main.updatelable(Mainwindow.lines[cookie], false);
                                req.Headers.Add("Cookie", Mainwindow.lines[cookie] + "; nike_cid=" + Config.cid + "; cid=" + Config.cid + "%7C" + Config.cjevent + "");
                                Mainwindow.lines.RemoveAt(cookie);
                                if (Mainwindow.lines.Count == 0)
                                {
                                    Mainwindow.iscookielistnull = true;
                                }
                            }
                            catch (Exception)
                            {
                                goto reloadcookie;
                            }
                        }
                    }
                    else
                    {
                    reloadcookie: Random ra = new Random();
                        int sleeptime = ra.Next(0, 500);
                        Thread.Sleep(sleeptime);
                    E: if (Mainwindow.lines.Count == 0)
                        {
                            if (ct.IsCancellationRequested)
                            {
                                tk.Status = "IDLE";
                                ct.ThrowIfCancellationRequested();
                            }
                            tk.Status = "No Cookie";
                            Mainwindow.iscookielistnull = true;
                            goto E;
                        }
                        else
                        {
                            int cookie = ra.Next(0, Mainwindow.lines.Count);
                            try
                            {
                               // req.Headers.Add("Cookie", "geoloc=cc=CN,rc=JL,tp=vhigh,tz=GMT+8,la=43.89,lo=125.32; crl8.fpcuid=111b9374-3bae-45a8-a2de-8318a14d95a2; s_ecid=MCMID%7C13712291943217497877934437626721125471; AMCVS_F0935E09512D2C270A490D4D%40AdobeOrg=1; fs_uid=rs.fullstory.com#BM7A6#6266740190363648:6363763799834624/1622087170; anonymousId=A62273FE0B60DBF46517E04C7C973E37; RES_TRACKINGID=743197587189211; ResonanceSegment=1; _ga=GA1.2.1522812375.1595248648; _gcl_au=1.1.1583197471.1595248648; AnalysisUserId=60.210.20.122.77481595248738790; AMCV_F0935E09512D2C270A490D4D%40AdobeOrg=1994364360%7CMCMID%7C13712291943217497877934437626721125471%7CMCAID%7CNONE%7CMCOPTOUT-1595255830s%7CNONE%7CvVersion%7C3.4.0%7CMCIDTS%7C18464%7CMCAAMLH-1595853539%7C11%7CMCAAMB-1595853539%7Cj8Odv6LonN4r3an7LhD3WZrU1bUpAkFkkiY1ncBR96t2PTI; Hm_lvt_ed406c6497cc3917d06fd572612b4bba=1593086327,1593254343,1593308143,1595249821; _gscu_207448657=90389346fy6gzr20; _gscbrs_207448657=1; _smt_uid=5f15949d.14434e20; guidS=35146d34-b642-45e5-c9fe-b4598d6274f0; guidU=3c900a92-0373-496d-ea92-099c9c254091; Hm_lpvt_ed406c6497cc3917d06fd572612b4bba=1595249882; _scid=9d85343b-7714-49ac-8f77-a0182b1a8f83; _fbp=fb.1.1595251459673.1766581573; _pin_unauth=dWlkPVlqUTVaR1l5WVdFdE5tVXpNUzAwTlRZNExXRTJPRFF0WlRsbE5XWm1NRFpoTTJFMw; _sctr=1|1595174400000; CONSUMERCHOICE_SESSION=t; AKA_A2=A; bm_sz=504A8FE1F1FF189568D527EA0CA28B8D~YAAQehTSPI9oRxRzAQAAqlrhdQj9IBMrlugd4qWjpOIWMi5BtkJpAzKNJWZuPysavxndJ5msirM5aY6ANyNCGR+JhJBKJPlKcQs3MLYM8nT5LZoRclmhV/wSGoP1GV1nZMxym2cu3HuEsw2bbLeQb/z2nVHlJEsppgKRogX4dbNNntkyrv1YcXY1KjZzQQ==; bm_mi=B7EAFED065D5130FA7DCABB3F5AB5778~u9omNtt9/bWFr2dXdUp5wLxMvvgzt3aJFWsKkZ6b0RL1zfDg9ycWDWPWQULZrTDn3sFciEHTdVLeb7XAZvbcz3HU/9o3jnvsIbX8yeH+rmusK506+oeAjnb3tSgvAJ20x2LnMstRVz41Jir2PztY8Gdhl0CRJFDO24hok++AoTOWarOchhhmoSFW59NOhEopVNkGz2mnDiCg8o29kLH2eTvVOcrXqppXalFEV2lGj51+06RyZBf8n0X0dpZRF1HipW5widsrKgnAB1WhcOQ7JQ==; bc_nike_australia_triggermail=%7B%22distinct_id%22%3A%20%221736c65822e165-0b2e64ee8521e1-7f2f4867-1704a0-1736c65822f48d%22%2C%22cart_total%22%3A%20100%7D; ppd=homepage|nikecom>homepage; ak_bmsc=B4A1D00A30A00A7E2EEB70C00ECC339E3CD2147A441E00008908185F31310145~pl9QzKO1+JjtkHs58i8R66ZiFEpE2VHvZwtxNuuuFhtLz4fFKhKDcDAkAtZnHN2Yfrii/lrZYjwxO6Z0F8x06Lw1rzgYd9xlsKJjasW57ntwXqEumdIeNP0TiQRttcuiI2YG9h+kLa9cgARzL6h1M+m/+8XAmlP5CwdgYLUSxX7gX/NWA73RRk13oyXSVYqEVDrxXTaB8VrsvXXAObQN7KFXtDDcYv98jDzW2vhSQa+Zimk0yFSeY0h94+CxbysQSU; bounceClientVisit2422v=N4IgNgDiBcIBYBcEQM4FIDMBBNAmAYnvgO6kB0AdgJYDWApmQMYD2AtkQIYCuRIANCABOMEKWJkA5s2YSwDFqzJwaIAL5A; _gid=GA1.2.1148988271.1595410576; _gat_UA-171421696-1=1; _gat_UA-167630499-4=1; NIKE_COMMERCE_COUNTRY=US; NIKE_COMMERCE_LANG_LOCALE=en_US; nike_locale=us/en_us; CONSUMERCHOICE=us/en_us; cid=undefined%7Cundefined; _gat_UA-167630499-2=1; optimizelyEndUserId=oeu1595410594908r0.33317420699330036; RES_SESSIONID=550846637672316; _uetsid=0ec99cb22fc24cc08b3184091dd5454e; _uetvid=bcb6a6620ee9f73367d0347ba1d3af85; bc_nike_triggermail=%7B%22distinct_id%22%3A%20%22172f89e3f24529-05ee350331064a-7f2f4867-1704a0-172f89e3f25684%22%2C%22g_search_engine%22%3A%20%22google%22%2C%22cart_total%22%3A%200%2C%22ch%22%3A%20%221433069869%22%7D; bm_sv=1E06D2DF5ED7C0656474A60717C9E86D~HQeDVMFVxaXnEVqjT4PklPSwNCdsrUJEUG2Y2Bfu4uNZuwM3jIqflwZty3r10p0f3G+V7SmwUX7EFniGuAqc9Gs3eeBqkzY4P18YZnLhOrpApGUGYhDndgivzJs6W/hyBbqGlL39kgD4i83OnoIEkA==; _abck=87746C6109FEA40FE3EF49CDCF4E32CD~-1~YAAQDhTSPIeuuWpzAQAALwfidQR2LcWAAt4GZPUISngPkobfyMD3ZLbevPlNTb1zGT0sgwolk7vNsnBChkIS2+TT2gIAalnYWOsIrwWgOO0DSfiW9wdVprtbmJZfelmFvcAuXXDB9eKESKgC9xBlDh/1ZDVhC0htsPkjBoB/YP0TYCw8ziQswPsa+bczKff2rs9cPcOLZ6djYaR+lJ3R35Ai6MC1YmOZyfwjnP9eWgapnKo3tzRgyfnUCkRY2CrofuSYzYRNrBSo1lkKbNd9OsQd7COU1Tg1QWNkffVLM3UxKiUXl8fiDfpHX16oscaRO/g6udoLZd+ocy+65J3kqbq4g/QWGdHjNyavHe6qVpA=~0~-1~-1");
                                 req.Headers.Add("Cookie", Mainwindow.lines[cookie]);
                                Main.updatelable(Mainwindow.lines[cookie], false);
                                Mainwindow.lines.RemoveAt(cookie);
                            }
                            catch (Exception)
                            {
                                goto reloadcookie;
                            }
                        }

                    }
                }
            }
            req.Headers.Add("Sec-Fetch-Dest", "empty");
            req.Headers.Add("Sec-Fetch-Mode", "cors");
            req.Headers.Add("Sec-Fetch-Site", "same-site");
            req.Referer = "https://www.nike.com/launch/";
            req.Headers.Add("origin", "https://unite.nike.com");
            Stream webstream = req.GetRequestStream();
            webstream.Write(contentByte, 0, contentByte.Length);
            webstream.Close();
            string token = "";
            try
            {
                HttpWebResponse response = (HttpWebResponse)req.GetResponse();
                var wu = response.Headers["Set-Cookie"];
                tk.Status = "Login Successful!";
                Stream tokenStream = response.GetResponseStream();
                StreamReader readhtmlStream = new StreamReader(tokenStream, Encoding.UTF8);
                token = readhtmlStream.ReadToEnd();
            }
            catch (WebException ex)
            {
                HttpWebResponse respgethtml = (HttpWebResponse)ex.Response;
                Stream tokenStream = respgethtml.GetResponseStream();
                StreamReader readhtmlStream = new StreamReader(tokenStream, Encoding.UTF8);
                var chao = readhtmlStream.ReadToEnd();
                tk.Status = "Login Failed";
                goto retry;
            }
            JObject jo = JObject.Parse(token);
            string Authorization = "Bearer " + jo["access_token"].ToString();
            Task task1 = new Task(() => writerefreshtoken("[{\"Token\":\"" + jo["refresh_token"].ToString() + "\",\"Account\":\"" + account + "\"}]", account));
            task1.Start();
            return Authorization;
        }
        public void writerefreshtoken(string token, string account)
        {
            try
            {
                if (!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "refreshtoken.json"))
                {
                    FileStream fs1 = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "refreshtoken.json", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                    StreamWriter sw = new StreamWriter(fs1);
                    JArray ja = JArray.Parse(token);
                    sw.Write(ja.ToString().Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace(" ", ""));
                    sw.Close();
                    fs1.Close();
                }
                else
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "refreshtoken.json";
                    FileInfo fi = new FileInfo(path);
                    if (fi.Length == 0)
                    {
                        FileStream fs1 = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MAIO\\" + "refreshtoken.json", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                        StreamWriter sw = new StreamWriter(fs1);
                        JArray ja = JArray.Parse(token);
                        sw.Write(ja.ToString().Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace(" ", ""));
                        sw.Close();
                        fs1.Close();
                    }
                    else
                    {
                        FileStream fs1 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        StreamReader sr = new StreamReader(fs1);
                        string read = sr.ReadToEnd();
                        JArray ja = JArray.Parse(read);
                        foreach (var i in ja)
                        {
                            if (i.ToString().Contains(account))
                            {
                                ja.Remove(i);
                            }
                        }
                        ja.Add(JObject.Parse(token.Replace("[", "").Replace("]", "")));
                        fs1.SetLength(0);
                        FileStream fs0 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                        StreamWriter sw = new StreamWriter(fs0);
                        sw.Write(ja.ToString().Replace("\n", "").Replace("\t", "").Replace("\r", "").Replace(" ", ""));
                        sw.Close();
                        fs1.Close();
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        public double Postcardinfo(string url, string cardinfo, string Authorization, string cardguid, Main.taskset tk, CancellationToken ct)
        {
        B: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            WebProxy wp = new WebProxy();
            try
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
            catch
            {
                wp = default;
            }
            double balance = 0;
            byte[] contentcardinfo = Encoding.UTF8.GetBytes(cardinfo);
            HttpWebRequest reqcard = (HttpWebRequest)WebRequest.Create(url);
            reqcard.Method = "POST";
            reqcard.Proxy = wp;
            reqcard.ContentType = "application/json";
            reqcard.Accept = "*/*";
            reqcard.Headers.Add("Accept-Encoding", "gzip, deflate");
            reqcard.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            reqcard.Headers.Add("Authorization", Authorization);
            reqcard.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            reqcard.ContentLength = contentcardinfo.Length;
            reqcard.Headers.Add("sec-fetch-dest", "empty");
            reqcard.Headers.Add("sec-fetch-mode", "cors");
            reqcard.Headers.Add("sec-fetch-site", "same-site");
            reqcard.Headers.Add("appid", "com.nike.commerce.snkrs.web");
            reqcard.Headers.Add("x-b3-spanname", "CiCCart");
            reqcard.Headers.Add("x-b3-traceid", xb3traceid);
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
                tk.Status = "Submit Card Success";
            }
            catch (WebException ex)
            {
                tk.Status = ex.Message.ToString();
                goto B;
            }
            return balance;

        }
        public void CheckoutPreview(string url, string Authorization, string checkoutpayload, string GID, Main.taskset tk, CancellationToken ct)
        {
        B: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            WebProxy wp = new WebProxy();
            try
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
            catch
            {
                wp = default;
            }
            HttpWebRequest reqpayment = (HttpWebRequest)WebRequest.Create(url);
            reqpayment.Method = "PUT";
            reqpayment.Proxy = wp;
            reqpayment.ContentType = "application/json; charset=UTF-8";
            byte[] contentpaymentinfo = Encoding.UTF8.GetBytes(checkoutpayload);
            reqpayment.Accept = "application/json";
            reqpayment.Headers.Add("Accept-Encoding", "gzip, deflate");
            reqpayment.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            reqpayment.Headers.Add("Authorization", Authorization);
            if (Mainwindow.iscookielistnull)
            {
                reqpayment.Headers.Add("Cookie", "");
            }
            else
            {
                if ((Config.cjevent != "") && (Config.cid != ""))
                {
                reloadcookie: Random ra = new Random();
                    int sleeptime = ra.Next(0, 500);
                    Thread.Sleep(sleeptime);
                C: if (Mainwindow.lines.Count == 0)
                    {
                        if (ct.IsCancellationRequested)
                        {
                            tk.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                        Mainwindow.iscookielistnull = true;
                        tk.Status = "No Cookie";
                        goto C;
                    }
                    else
                    {
                        int cookie = ra.Next(0, Mainwindow.lines.Count);
                        try
                        {
                            Main.updatelable(Mainwindow.lines[cookie], false);
                            reqpayment.Headers.Add("Cookie", Mainwindow.lines[cookie] + "; nike_cid=" + Config.cid + "; cid=" + Config.cid + "%7C" + Config.cjevent + "");
                            Mainwindow.lines.RemoveAt(cookie);
                            if (Mainwindow.lines.Count == 0)
                            {
                                Mainwindow.iscookielistnull = true;
                            }

                        }
                        catch (Exception)
                        {
                            goto reloadcookie;
                        }
                    }

                }
                else
                {
                reloadcookie: Random ra = new Random();
                    int sleeptime = ra.Next(0, 500);
                    Thread.Sleep(sleeptime);
                D: if (Mainwindow.lines.Count == 0)
                    {
                        if (ct.IsCancellationRequested)
                        {
                            tk.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                        tk.Status = "No Cookie";
                        Mainwindow.iscookielistnull = true;
                        goto D;
                    }
                    else
                    {
                        int cookie = ra.Next(0, Mainwindow.lines.Count);
                        try
                        {
                            reqpayment.Headers.Add("Cookie", Mainwindow.lines[cookie]);
                            Main.updatelable(Mainwindow.lines[cookie], false);
                            Mainwindow.lines.RemoveAt(cookie);

                        }
                        catch (Exception)
                        {
                            goto reloadcookie;
                        }
                    }

                }
            }

            reqpayment.ContentLength = contentpaymentinfo.Length;
            reqpayment.Referer = "https://www.nike.com/us/en/checkout";
            reqpayment.Headers.Add("Origin", "https://www.nike.com");
            reqpayment.Headers.Add("Sec-Fetch-Dest", "empty");
            reqpayment.Headers.Add("Sec-Fetch-Mode", "cors");
            reqpayment.Headers.Add("Sec-Fetch-Site", "same-site");
            reqpayment.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            reqpayment.Headers.Add("x-b3-spanname", "CiCCheckout");
            reqpayment.Headers.Add("x-b3-traceid", xb3traceid);
            Stream paymentstream = reqpayment.GetRequestStream();
            paymentstream.Write(contentpaymentinfo, 0, contentpaymentinfo.Length);
            paymentstream.Close();
            string paymentsuccesscode = "";
            try
            {
                HttpWebResponse resppayment = (HttpWebResponse)reqpayment.GetResponse();
                var wu = resppayment.Headers["Set-Cookie"];
                tk.Status = "Submit Shipping";
                Stream resppaymentStream = resppayment.GetResponseStream();
                StreamReader readpaymenthtmlStream = new StreamReader(resppaymentStream, Encoding.UTF8);
                paymentsuccesscode = readpaymenthtmlStream.ReadToEnd();
                JObject jo = null;
                jo = JObject.Parse(paymentsuccesscode);
                try
                {
                    if (jo.ToString().Contains("error"))
                    {
                        if (jo["error"]["message"].ToString().Contains("Non buyable product(s)"))
                        {
                            tk.Status = "Non buyable product(s)";
                            goto B;
                        }
                    }
                }
                catch (NullReferenceException ex)
                {
                }

            }
            catch (WebException ex)
            {
                HttpWebResponse resppayment = (HttpWebResponse)ex.Response;
                tk.Status = ex.ToString();
               // tk.Status = "SubmitShipping error";
                Stream resppaymentStream = resppayment.GetResponseStream();
                StreamReader readpaymenthtmlStream = new StreamReader(resppaymentStream, Encoding.UTF8);
                paymentsuccesscode = readpaymenthtmlStream.ReadToEnd();
                goto B;
            }

        }
        public string CheckoutPreviewStatus(string url, string Authorization, bool isdiscount, Main.taskset tk, CancellationToken ct, string profile, string pid, string size, string code, string giftcard, string username, string password, bool randomsize, string productid, string skuid)
        {
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            WebProxy wp = new WebProxy();
            try
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
            catch
            {
                wp = default;
            }
            string total = "";
            HttpWebRequest reqcheckstatus = (HttpWebRequest)WebRequest.Create(url);
            reqcheckstatus.Method = "GET";
            reqcheckstatus.Proxy = wp;
            reqcheckstatus.ContentType = "application/json; charset=UTF-8";
            reqcheckstatus.Accept = "application/json";
            reqcheckstatus.Headers.Add("Accept-Encoding", "gzip, deflate");
            reqcheckstatus.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            reqcheckstatus.Headers.Add("Authorization", Authorization);
            reqcheckstatus.Headers.Add("Origin", "https://www.nike.com");
            reqcheckstatus.Headers.Add("Sec-Fetch-Dest", "empty");
            reqcheckstatus.Headers.Add("Sec-Fetch-Mode", "cors");
            reqcheckstatus.Headers.Add("Sec-Fetch-Site", "same-site");
            reqcheckstatus.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            reqcheckstatus.Headers.Add("x-b3-spanname", "CiCCheckout");
            reqcheckstatus.Headers.Add("x-b3-traceid", xb3traceid);
            try
            {
                HttpWebResponse respcheckstatus = (HttpWebResponse)reqcheckstatus.GetResponse();
                var wu=respcheckstatus.Headers["Set-Cookie"];
                string cookiename = "ak_bmsc";
                Regex rex3 = new Regex(@"(?<=" + cookiename + "=)([^;]+)");
                akbmsz +=rex3.Match(wu).ToString();
                string check = "";
                if (respcheckstatus.ContentEncoding == "gzip")
                {
                    Stream respcheckstatusstream = respcheckstatus.GetResponseStream();
                    StreamReader readcheckstatus = new StreamReader(new GZipStream(respcheckstatusstream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                    check = readcheckstatus.ReadToEnd();
                    JObject jo = JObject.Parse(check);
                    if (check.Contains("IN_PROGRESS"))
                    {
                        Thread.Sleep(1000);
                        goto A;
                    }
                    if (check.Contains("Non buyable product(s)"))
                    {
                        tk.Status = "WaitingRestock";
                        string monitorurl = "https://api.nike.com/deliver/available_skus/v1?filter=productIds(" + productid + ")";
                        Monitoring(monitorurl, tk, ct, skuid, randomsize);
                        NikeUSUK UKUS = new NikeUSUK();
                        xb3traceid = Guid.NewGuid().ToString();
                        UKUS.pid = pid;
                        UKUS.size = size;
                        UKUS.profile = profile;
                        UKUS.code = code;
                        UKUS.giftcard = giftcard;
                        UKUS.username = username;
                        UKUS.password = password;
                        UKUS.randomsize = randomsize;
                        UKUS.StartTask(ct);

                    }
                    total = jo["response"]["totals"]["total"].ToString();
                    if (isdiscount)
                    {
                        var discount = jo["response"]["promotionCodes"].ToString();
                        JArray jo2 = JArray.Parse(discount);
                        var discountstatus = jo2[0]["status"].ToString();
                        if (discountstatus != "PROMOTION_APPLIED")
                        {
                            tk.Status = "code applied failed";
                            goto A;
                        }
                    }
                }
                else
                {
                    Stream respcheckstatusstream = respcheckstatus.GetResponseStream();
                    StreamReader readcheckstatus = new StreamReader(respcheckstatusstream, Encoding.GetEncoding("utf-8"));
                    check = readcheckstatus.ReadToEnd();
                    if (check.Contains("IN_PROGRESS"))
                    {
                        Thread.Sleep(1000);
                        goto A;
                    }
                    if (check.Contains("Non buyable product(s)"))
                    {
                        tk.Status = "WaitingRestock";
                        string monitorurl = "https://api.nike.com/deliver/available_skus/v1?filter=productIds(" + productid + ")";
                        string[] group = Monitoring(monitorurl, tk, ct, skuid, randomsize);
                        NikeUSUK UKUS = new NikeUSUK();
                        UKUS.skuid = group[0];
                        UKUS.productID = group[1];
                        xb3traceid = Guid.NewGuid().ToString();
                        UKUS.pid = pid;
                        UKUS.size = size;
                        UKUS.profile = profile;
                        UKUS.code = code;
                        UKUS.giftcard = giftcard;
                        UKUS.username = username;
                        UKUS.password = password;
                        UKUS.randomsize = randomsize;
                        UKUS.StartTask(ct);
                    }
                    JObject jo = JObject.Parse(check);
                    total = jo["response"]["totals"]["total"].ToString();
                    if (isdiscount)
                    {

                        var discount = jo["response"]["promotionCodes"].ToString();
                        JArray jo2 = JArray.Parse(discount);
                        var discountstatus = jo2[0]["status"].ToString();
                        if (discountstatus != "PROMOTION_APPLIED")
                        {
                            tk.Status = "code applied failed";
                            goto A;
                        }
                    }
                }

                if (check.Contains("COMPLETED") == true)
                {
                }
                else
                {
                    Thread.Sleep(3000);
                    goto A;
                }
            }
            catch
            {
                goto A;
            }
            return total;
        }
        public string payment(string url, string Authorization, string paymentinfo, Main.taskset tk, CancellationToken ct)
        {
        retry: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            WebProxy wp = new WebProxy();
            try
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
            catch
            {
                wp = default;
            }
            HttpWebRequest reqprocess = (HttpWebRequest)WebRequest.Create(url);
            reqprocess.Proxy = wp;
            reqprocess.Accept = "application/json";
            reqprocess.Method = "POST";
            reqprocess.ContentType = "application/json; charset=UTF-8";
            byte[] processbyteinfo = Encoding.UTF8.GetBytes(paymentinfo);
            reqprocess.Headers.Add("authorization", Authorization);
            if (Mainwindow.iscookielistnull)
            {
                reqprocess.Headers.Add("Cookie", "");
            }
            else
            {
                if ((Config.cjevent != "") && (Config.cid != ""))
                {
                reloadcookie: Random ra = new Random();
                    int sleeptime = ra.Next(0, 500);
                    Thread.Sleep(sleeptime);
                C: if (Mainwindow.lines.Count == 0)
                    {
                        if (ct.IsCancellationRequested)
                        {
                            tk.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                        Mainwindow.iscookielistnull = true;
                        tk.Status = "No Cookie";
                      //  Main.updatelable("123", true);
                        goto C;
                    }
                    else
                    {
                        if (ct.IsCancellationRequested)
                        {
                            tk.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                        int cookie = ra.Next(0, Mainwindow.lines.Count);
                        try
                        {
                            Main.updatelable(Mainwindow.lines[cookie], false);
                            reqprocess.Headers.Add("Cookie", Mainwindow.lines[cookie] + "; nike_cid=" + Config.cid + "; cid=" + Config.cid + "%7C" + Config.cjevent + "");
                            Mainwindow.lines.RemoveAt(cookie);
                            if (Mainwindow.lines.Count == 0)
                            {
                                Mainwindow.iscookielistnull = true;
                            }
                        }
                        catch (Exception)
                        {
                            goto reloadcookie;
                        }
                    }

                }
                else
                {
                reloadcookie: Random ra = new Random();
                    int sleeptime = ra.Next(0, 500);
                    Thread.Sleep(sleeptime);
                D: if (Mainwindow.lines.Count == 0)
                    {
                        if (ct.IsCancellationRequested)
                        {
                            tk.Status = "IDLE";
                            ct.ThrowIfCancellationRequested();
                        }
                        tk.Status = "No Cookie";
                        Mainwindow.iscookielistnull = true;
                        goto D;
                    }
                    else
                    {
                        int cookie = ra.Next(0, Mainwindow.lines.Count);
                        try
                        {
                            reqprocess.Headers.Add("Cookie", Mainwindow.lines[cookie]);
                            Main.updatelable(Mainwindow.lines[cookie], false);
                            Mainwindow.lines.RemoveAt(cookie);

                        }
                        catch (Exception)
                        {
                            goto reloadcookie;
                        }
                    }
                }
            }
            reqprocess.Headers.Add("Accept-Encoding", "gzip, deflate");
            reqprocess.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            reqprocess.Headers.Add("Origin", "https://www.nike.com");
            reqprocess.Referer = "https://www.nike.com/us/en/checkout";
            reqprocess.Headers.Add("Sec-Fetch-Dest", "empty");
            reqprocess.Headers.Add("Sec-Fetch-Mode", "cors");
            reqprocess.Headers.Add("Sec-Fetch-Site", "same-site");
            reqprocess.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            reqprocess.Headers.Add("x-b3-spanname", "CiCCheckout");
            reqprocess.Headers.Add("x-b3-traceid", xb3traceid);
            Stream processstream = reqprocess.GetRequestStream();
            processstream.Write(processbyteinfo, 0, processbyteinfo.Length);
            processstream.Close();
            string processcode = "";
            try
            {
                HttpWebResponse processpayment = (HttpWebResponse)reqprocess.GetResponse();
                var wu=processpayment.Headers["Set-Cookie"];
                tk.Status = "Submit Billing";
                Stream processtream = processpayment.GetResponseStream();
                StreamReader readprocessstream = new StreamReader(processtream, Encoding.UTF8);
                processcode = readprocessstream.ReadToEnd();
            }
            catch (WebException ex)
            {
                HttpWebResponse processpayment = (HttpWebResponse)ex.Response;
                Stream processtream = processpayment.GetResponseStream();
                StreamReader readprocessstream = new StreamReader(processtream, Encoding.UTF8);
                tk.Status = "SubmitBilling error";
                goto retry;
            }
            JObject joid = JObject.Parse(processcode);
            string id = joid["id"].ToString();
            return id;
        }
        public void paymentjob(string url, string Authorization, CancellationToken ct, Main.taskset tk)
        {
        C: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            WebProxy wp = new WebProxy();
            try
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
            catch
            {
                wp = default;
            }
            HttpWebRequest reqjob = (HttpWebRequest)WebRequest.Create(url);
            reqjob.Proxy = wp;
            reqjob.Accept = "application/json";
            reqjob.Method = "GET";
            reqjob.ContentType = "application/json; charset=UTF-8";
            reqjob.Headers.Add("authorization", Authorization);
            reqjob.Headers.Add("Accept-Encoding", "gzip, deflate");
            reqjob.Headers.Add("Accept-Language", "en-US, en; q=0.9");
            reqjob.Headers.Add("Origin", "https://www.nike.com");
            reqjob.Referer = "https://www.nike.com/us/en/checkout";
            reqjob.Headers.Add("Sec-Fetch-Dest", "empty");
            reqjob.Headers.Add("Sec-Fetch-Mode", "cors");
            reqjob.Headers.Add("Sec-Fetch-Site", "same-site");
            reqjob.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            reqjob.Headers.Add("x-b3-spanname", "CiCCheckout");
            reqjob.Headers.Add("x-b3-traceid", xb3traceid);
            string jobstatus = "";
            try
            {
                HttpWebResponse respjob = (HttpWebResponse)reqjob.GetResponse();
                var wu=respjob.Headers["Set-Cookie"];
                if (respjob.ContentEncoding == "gzip")
                {
                    Stream jobstream = respjob.GetResponseStream();
                    StreamReader read = new StreamReader(new GZipStream(jobstream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                    jobstatus = read.ReadToEnd();
                }
                else
                {
                    Stream jobstream = respjob.GetResponseStream();
                    StreamReader readjobstream = new StreamReader(jobstream, Encoding.UTF8);
                    jobstatus = readjobstream.ReadToEnd();
                }
                if (jobstatus.Contains("COMPLETED") == true)
                {
                }
                else
                {
                    goto C;
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse respjob = (HttpWebResponse)ex.Response;

                goto C;
            }
        }
        public void final(string Authorization, string url, string payload, string GID, Main.taskset tk, CancellationToken ct)
        {
        D: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            WebProxy wp = new WebProxy();
            try
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
            catch
            {
                wp = default;
            }
            string getstatus = "https://api.nike.com/buy/checkouts/v2/" + GID;
            HttpWebRequest reqgetstatus = (HttpWebRequest)WebRequest.Create(getstatus);
            reqgetstatus.Proxy = wp;
            reqgetstatus.Method = "PUT";
            reqgetstatus.Accept = "application/json";
            reqgetstatus.ContentType = "application/json; charset=UTF-8";
            byte[] paymenttokeninfo = Encoding.UTF8.GetBytes(payload);
            reqgetstatus.Headers.Add("authorization", Authorization);
            /*  if (Mainwindow.iscookielistnull)
              {
                  reqgetstatus.Headers.Add("Cookie", "");
              }
              else
              {
                  if ((Config.cjevent != "") && (Config.cid != ""))
                  {
                  reloadcookie: Random ra = new Random();
                      int sleeptime = ra.Next(0, 500);
                      Thread.Sleep(sleeptime);
                  C: if (Mainwindow.lines.Count == 0)
                      {
                          if (ct.IsCancellationRequested)
                          {
                              tk.Status = "IDLE";
                              ct.ThrowIfCancellationRequested();
                          }
                          tk.Status = "No Cookie";
                          Mainwindow.iscookielistnull = true;
                          goto C;
                      }
                      else
                      {
                          int cookie = ra.Next(0, Mainwindow.lines.Count);
                          try
                          {
                              Main.updatelable(Mainwindow.lines[cookie], false);
                              reqgetstatus.Headers.Add("Cookie", Mainwindow.lines[cookie] + "; nike_cid=" + Config.cid + "; cid=" + Config.cid + "%7C" + Config.cjevent + "");
                              Mainwindow.lines.RemoveAt(cookie);
                              if (Mainwindow.lines.Count == 0)
                              {
                                  Mainwindow.iscookielistnull = true;
                              }
                          }
                          catch (Exception)
                          {
                              goto reloadcookie;
                          }
                      }
                  }
                  else
                  {
                  reloadcookie: Random ra = new Random();
                      int sleeptime = ra.Next(0, 500);
                      Thread.Sleep(sleeptime);
                  C: if (Mainwindow.lines.Count == 0)
                      {
                          if (ct.IsCancellationRequested)
                          {
                              tk.Status = "IDLE";
                              ct.ThrowIfCancellationRequested();
                          }
                          Mainwindow.iscookielistnull = true;
                          tk.Status = "No Cookie";
                          goto C;

                      }
                      else
                      {
                          int cookie = ra.Next(0, Mainwindow.lines.Count);
                          try
                          {
                              string subcookie=Mainwindow.lines[cookie] + ";" + akbmsz;
                              reqgetstatus.Headers.Add("Cookie",subcookie.Replace(";","; ") );

                              Main.updatelable(Mainwindow.lines[cookie], false);
                              Mainwindow.lines.RemoveAt(cookie);

                          }
                          catch (Exception)
                          {
                              goto reloadcookie;
                          }
                      }
                  }
              }*/
            reqgetstatus.Headers.Add("Cookie", "CONSUMERCHOICE_SESSION=t; s_ecid=MCMID%7C08335113933111001613113946980067283425; AMCVS_F0935E09512D2C270A490D4D%40AdobeOrg=1; RES_TRACKINGID=284443253138423; ResonanceSegment=1; guidU=3a11afe2-776d-4582-b254-350bcbfcc42c; _gcl_au=1.1.1349716998.1595434350; _fbp=fb.1.1595434350671.2092163292; _ga=GA1.2.676227799.1595434351; AnalysisUserId=60.210.20.122.77481595434355226; _pin_unauth=dWlkPU16Y3dZVEEwTnpFdFlXRXpNQzAwWVdReExUZzVZekl0WkdObVlqTXpZemszWXpObA; crl8.fpcuid=03846fa9-3d3f-4704-a2d2-33ab89f13f57; _scid=2a661624-1208-4dad-abde-b203927c7850; _sctr=1|1595433600000; bc_nike_singapore_triggermail=%7B%22distinct_id%22%3A%20%22173774c38d9682-0ffdb7be458f47-7f2f4867-1704a0-173774c38da5ef%22%2C%22cart_total%22%3A%20149%7D; sq=3; optimizelyEndUserId=oeu1595553435393r0.37849959429315416; s_sess=%20tp%3D1657%3B%20c51%3Dhorizontal%3B%20s_cc%3Dtrue%3B%20s_ppv%3Dnikecom%25253Echeckout%25253Eshipping%252C55%252C54%252C917.3333339691162%3B; bc_nike_canada_triggermail=%7B%22distinct_id%22%3A%20%221737e64fb0795-06afe3b666aeb7-7f2f4867-1704a0-1737e64fb084fb%22%2C%22cart_total%22%3A%20160%7D; cid=undefined%7Cundefined; geoloc=cc=CN,rc=JL,tp=vhigh,tz=GMT+8,la=43.89,lo=125.32; anonymousId=A62273FE0B60DBF46517E04C7C973E37; _gid=GA1.2.296899278.1595934192; utag_main=_st:1595990134377$ses_id:1595988399771%3Bexp-session; bc_nike_australia_triggermail=%7B%22distinct_id%22%3A%20%22173775a1d78400-07b46735cae0ef-7f2f4867-1704a0-173775a1d793c2%22%2C%22cart_total%22%3A%20170%7D; NIKE_COMMERCE_COUNTRY=US; NIKE_COMMERCE_LANG_LOCALE=en_US; CONSUMERCHOICE=us/en_us; nike_locale=us/en_us; lls=3; slCheck=1Ok7NLYrZb+ARA2NCtpsyKdTWlaTbS+usEv27cy91aPRut9PxNugW2IN4sn8g48F0FmfWlOEsD1wIp9C7xt4gvK8Wx5F7NLx8ApdG6nkpwuqOyWFRYGwc9KqV2keLb7U; llCheck=jNZJe1rnB7d5psp0oAVGEAA2AEY+Rpe8KKpleXIAnHfNtsD0NvNZitS2jmmXAGI8ItYuwmaRruJkFJisXqaZc/R4nWiBhdBHWn6q7LyjWVKM4g9ZmKUmunJD+xUFJD5AIbbWGFvehCac/7OlkIeb70xmuZhTorkJo5KEeXa6mpc=; sls=3; fs_uid=rs.fullstory.com#BM7A6#6266740190363648:6356050518556672/1622087170; AMCV_F0935E09512D2C270A490D4D%40AdobeOrg=1994364360%7CMCMID%7C08335113933111001613113946980067283425%7CMCAID%7CNONE%7CMCOPTOUT-1596034688s%7CNONE%7CvVersion%7C3.4.0%7CMCIDTS%7C18473%7CMCAAMLH-1596632288%7C9%7CMCAAMB-1596632288%7Cj8Odv6LonN4r3an7LhD3WZrU1bUpAkFkkiY1ncBR96t2PTI; siteCatalyst_sample=30; dreamcatcher_sample=65; neo_sample=89; guidS=05cf781a-2766-4479-d1fc-09589764206c; guidSTimestamp=1596029236444|1596029939275; ppd=pdp|snkrs>pdp>lebron%2017%20fp; AKA_A2=A; bm_sz=26981972EE83CFDD00A312075C8292E4~YAAQXZ7C3Z8tX4ZzAQAA+sUJmwiCIqWuWyN4ChN25MT/w4yy4pXZi2MkkoNNYrvNIxI3oLfVeoZHHx32PBydOxBLkQ8XKbzywytMZmZErQqXHrxZX/UXivFL0kNNd5HGagQzgJVyx5/n/OpUJCxjgtquBL2w/QXajYpewf5MdvozF2tee6MoYOjq4E5R7Q==; _gat_UA-171421696-1=1; _gat_UA-167630499-2=1; bm_mi=A2211DF285C1EC8716C837717B924654~XN/yXo0MkZ54lQSsSvuFquVjIdxfsyFqlgS1PoA2pAoMZE1QrJjb+y69Y2002CI7wLegYTx5b/mOt9Yw8k/ml1mxFD29RxlFD773N1UUvWRYKysm7SHZF+/h9aO/ds7t0mi2fIoWsrrHd9xvvjLPDPF55asgbXoZ6aAkwXZJylet21TgzTb7wcs0kGQFMn/eLZiDdHcHr8QRviPQ6ZUlQ0S6F71jKAHtPh48q/G8X5Iu//PYl4AnoONQ8kXFYqPlJ6VAyslJH1LpTlVNAqoQNQ==; ak_bmsc=D336D8146B8797A5DF8C5C1DE2753F84DDC29E5D79370000B68B215F2E3D8A1A~plkfhkg2OYaozokEVfLDw1e8OgItvMkuPdLq5kzMdSetoFxEkRJwsxdzyT2h1Hn/sUOyUaUnTbLERVQwauxDnLQhHPfBMhsyPTlhjVV3qpcdIGbYZZXhCg7jtLdRCe4H/rXg5xID64uvRzPcNy2cSyLTQ7inLrXoZG5MDTuVzQPubuG61M5NMLkmFwZOz8XNdD6xTxOkux3EBaX876HNv5IhgVg4NILJpl7m5f+C4eTmKxx+cIRd/aOoDEaobgJmm+; RES_SESSIONID=466682712331018; _uetsid=feee5da5f3ca3f164782a94b834bfd5f; _uetvid=bcb6a6620ee9f73367d0347ba1d3af85; bc_nike_triggermail=%7B%22distinct_id%22%3A%20%22173807e221813a-020cf4978e2fd3-7f2f4867-1704a0-173807e22193e3%22%2C%22cart_total%22%3A%200%2C%22ch%22%3A%20%221433069869%22%7D; _abck=3F11837CD8282673D22ACD936F59D9DA~-1~YAAQXZ7C3QowX4ZzAQAA0z0KmwTCWFRCFtYYqX4w9mBaTS7siHnuPygVUIXnzyovxG/Ov0TKSmKr/F0Yd6Rrn2XllQbPuF2Loh+kpmTUB9BHK1vgmIkmh5Yp1w6XG4txtIixHPSlFvTFN0pZwLfLcEXS9oko/MQ7xFAGk4hrN7vMK5oeU3TIP0+iMlmS0uCHEWzx5FdRV7H4ol45QpJKa2ewnn/U0yfRzy3GXYo3/aBNlAEc5gnXtkt9EsqpW45IBY57bESxkpB37MTbwL8NMyIx5tQs9xOvJfI2v49SaXuxw4WIlDWjyU9UYJgxfcSn/uNUT7n3dOUf8SxWLwbhiZcNhErAyXqvLJexgteNJcY=~-1~-1~-1; bm_sv=6AAC2D128BA53D4F88E55EC40E9E9C70~ZzEqMWy9TV2DXmT7xWq/HgODTGTpFd8SUlYHSd/yLimJKsY8ZIlIPTPH1yx9GyX+p1HDusG9PhaEoFvxQyDaYG6xeAB1E+Z2zis31/OD1aSeVxYb8/90EcwwGMKvMFARDV6XM1uv3RDNrw4ov0jXRA==");
            reqgetstatus.Headers.Add("accept-encoding", "gzip, deflate,br");
            reqgetstatus.Headers.Add("accept-language", "en-US, en; q=0.9");
            reqgetstatus.Headers.Add("appid", "com.nike.commerce.checkout.web");
            reqgetstatus.Headers.Add("Origin", "https://www.nike.com");
            reqgetstatus.Referer = "https://www.nike.com/us/en/checkout";
            reqgetstatus.ContentLength = paymenttokeninfo.Length;
            reqgetstatus.Headers.Add("sec-fetch-dest", "empty");
            reqgetstatus.Headers.Add("sec-fetch-mode", "cors");
            reqgetstatus.Headers.Add("sec-fetch-site", "same-site");
            reqgetstatus.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
            reqgetstatus.Headers.Add("x-b3-spanname", "CiCCheckout");
            reqgetstatus.Headers.Add("x-b3-traceid", xb3traceid);
            Stream paymenttokenstream = reqgetstatus.GetRequestStream();
            paymenttokenstream.Write(paymenttokeninfo, 0, paymenttokeninfo.Length);
            paymenttokenstream.Close();
            try
            {
                HttpWebResponse respgetstatus = (HttpWebResponse)reqgetstatus.GetResponse();
                Stream respcheckstatusstream = respgetstatus.GetResponseStream();
                StreamReader readcheckstatus = new StreamReader(respcheckstatusstream, Encoding.GetEncoding("utf-8"));
                string check = readcheckstatus.ReadToEnd();

            }
            catch (WebException ex)
            {
                HttpWebResponse respjob = (HttpWebResponse)ex.Response;
                tk.Status = "Processing failed";
                Thread.Sleep(2000);
                goto D;
            }
        }
        public string finalorder(string url, string Authorization, string profile, Main.taskset tk, string pid, string size, string code, string giftcard, string username, string password, bool randomsize, CancellationToken ct, string productid, string skuid)
        {
          A: string status = "";
            for (int i = 0; i < 10; i++)
            {
                if (ct.IsCancellationRequested)
                {
                    tk.Status = "IDLE";
                    ct.ThrowIfCancellationRequested();
                }
                WebProxy wp = new WebProxy();
                try
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
                catch
                {
                    wp = default;
                }
                HttpWebRequest reqfinal = (HttpWebRequest)HttpWebRequest.Create(url);
                reqfinal.Proxy = wp;
                reqfinal.Method = "GET";
                reqfinal.Accept = "application/json";
                reqfinal.ContentType = "application/json; charset=UTF-8";
                reqfinal.Headers.Add("authorization", Authorization);
                reqfinal.Headers.Add("Accept-Encoding", "gzip, deflate");
                reqfinal.Headers.Add("Accept-Language", "en-US, en; q=0.9");
                reqfinal.Headers.Add("Origin", "https://www.nike.com");
                reqfinal.Referer = "https://www.nike.com/us/en/checkout";
                reqfinal.Headers.Add("Sec-Fetch-Dest", "empty");
                reqfinal.Headers.Add("Sec-Fetch-Mode", "cors");
                reqfinal.Headers.Add("Sec-Fetch-Site", "same-site");
                reqfinal.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
                reqfinal.Headers.Add("x-b3-spanname", "CiCCheckout");
                reqfinal.Headers.Add("x-b3-traceid", xb3traceid);
                try
                {
                    HttpWebResponse respfinal = (HttpWebResponse)reqfinal.GetResponse();
                    Thread.Sleep(2000);
                    tk.Status = "Submit Order";
                    if (respfinal.ContentEncoding == "gzip")
                    {
                        Stream receive2Stream = respfinal.GetResponseStream();
                        StreamReader read = new StreamReader(new GZipStream(receive2Stream, CompressionMode.Decompress), Encoding.GetEncoding("utf-8"));
                        status = read.ReadToEnd();
                    }
                    else
                    {
                        Stream finalstream = respfinal.GetResponseStream();
                        StreamReader readfinalstream = new StreamReader(finalstream, Encoding.UTF8);
                        status = readfinalstream.ReadToEnd();
                    }
                }
                catch (WebException ex)
                {
                    HttpWebResponse respjob = (HttpWebResponse)ex.Response;
                    Stream jobstream = respjob.GetResponseStream();
                    StreamReader readjobstream = new StreamReader(jobstream, Encoding.UTF8);
                    status = readjobstream.ReadToEnd();
                    tk.Status = "Error";
                    goto A;

                }
                if ((status.Contains("COMPLETED") == true) && (status.Contains("OUT_OF_STOCK")))
                {
                    string monitorurl = "https://api.nike.com/deliver/available_skus/v1?filter=productIds(" + productid + ")";
                    string[] group = Monitoring(monitorurl, tk, ct, skuid, randomsize);
                    NikeUSUK UKUS = new NikeUSUK();
                    UKUS.productID = group[1];
                    UKUS.skuid = group[0];
                    xb3traceid = Guid.NewGuid().ToString();
                    UKUS.pid = pid;
                    UKUS.size = size;
                    UKUS.profile = profile;
                    UKUS.code = code;
                    UKUS.giftcard = giftcard;
                    UKUS.username = username;
                    UKUS.password = password;
                    UKUS.randomsize = randomsize;
                    UKUS.StartTask(ct);


                }
                if (status.Contains("IN_PROGRESS") == true)
                {
                    tk.Status = "Processing";
                }
                if (status.Contains("COMPLETED") == true)
                {
                    break;
                }
            }
            return status;
        }
        public string[] Monitoring(string url, Main.taskset tk, CancellationToken ct, string skuid, bool randomsize)
        {
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string SourceCode = "";
            string[] group = new string[2];
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
            request.Method = "GET";
            request.Host = "api.nike.com";
            request.Accept = "*/*";
            request.Headers.Add("Sec-Fetch-Dest", "document");
            request.Headers.Add("Sec-Fetch-Mode", "navigate");
            request.Headers.Add("Sec-Fetch-Site", "none");
            request.Headers.Add("Sec-Fetch-user", "?1");
            request.Headers.Add("upgrade-insecure-requests", "1");
            request.UserAgent = "Sogou inst spider";
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
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
                JObject jo = JObject.Parse(SourceCode);
                JArray ja = JArray.Parse(jo["objects"].ToString());
                foreach (var i in ja)
                {
                    group[1] = i["productId"].ToString();
                    if (randomsize)
                    {
                        if (i["available"].ToString() != "False" || i["available"].ToString() != "false")
                        {
                            if (i["level"].ToString() == "OOS")
                            {
                            }
                            else
                            {
                                group[0] = i["skuId"].ToString();
                                break;
                            }
                        }
                        else
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
                    }
                    else
                    {
                        group[0] = skuid;
                        if (i["skuId"].ToString() == skuid)
                        {
                            if (i["available"].ToString() != "False" || i["available"].ToString() != "false")
                            {
                                if (i["level"].ToString() == "OOS")
                                {
                                    
                                    goto A;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
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
                        }
                    }
                }
                response.Close();
                readStream.Close();
            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                goto A;
            }
            if (group[0] == null)
            {
                goto A;
            }
            return group;
        }
    }
}
