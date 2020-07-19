using NakedBot;
using Newtonsoft.Json.Linq;
using PuppeteerSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MAIO
{
    class TheNorthFaceUSUK
    {
        public bool randomsize = false;
        public string link = "";
        public string profile = "";
        public string size = "";
        public string sizeid = "";
        public string categoryId = "";
        public string productid = "";
        public string orderid = "";
        public string addressid = "";
        public string tasksite = "";
        public string paypallink = "";
        public string  eid = "";
        public Main.taskset tk = null;
        TheNorthFaceAPI tnfAPI = new TheNorthFaceAPI();
        public void StartTask(CancellationToken ct, CancellationTokenSource cts)
        {
        A: JObject joprofile = JObject.Parse(profile);
            try
            {
                GetSkuid(ct);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (NullReferenceException)
            {
                goto A;
            }
            try
            {
                ATC(ct);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            try
            {
                SubmitShipping(ct, joprofile,cts);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            try
            {
                payment(ct);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        B:
            try
            {
              autocheckout();
            }
            catch
            {
                goto B;
            }
        }
        public void GetSkuid(CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string url = tk.Sku;
            bool isstocknull = true;
            JArray ja = (JArray)tnfAPI.GetHtmlsource(url, tk, ct);
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            categoryId = ja[0]["PDPProduct"]["categoryId"].ToString();
            productid = ja[0]["PDPProduct"]["parentId"].ToString();
            var sizegroup = ja[0]["PDPProduct"]["eids"].ToString();
            
            JArray ja2 = JArray.Parse(sizegroup);
            ArrayList skuslist = new ArrayList();
            for (int i = 0; i < ja2.Count; i++)
            {
                if (randomsize)
                {
                    skuslist.Add(ja2[i].ToString());
                }
                else
                {
                    if (ja2[i].ToString().Contains(size))
                    {
                        eid = ja2[i].ToString();
                        break;
                    }
                }
            }
            if (randomsize)
            {
                Random ran = new Random();
                int i=ran.Next(0,skuslist.Count);
                eid = skuslist[i].ToString();
            }
         B: string monitorurl = "";
            if (tk.Tasksite == "TheNorthFaceUK")
            {
                monitorurl = "https://www.thenorthface.com/webapp/wcs/stores/servlet/VFAjaxProductAvailabilityView?langId=-1&storeId=7005&productId=" + productid + "&requestype=ajax&requesttype=ajax";
            }
            else
            {
                monitorurl = "https://www.thenorthface.com/webapp/wcs/stores/servlet/VFAjaxProductAvailabilityView?langId=-1&storeId=7001&productId=" + productid + "&requestype=ajax&requesttype=ajax";
            }
            tnfAPI.Monitor(monitorurl, tk, ct,eid,sizeid);
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            if (isstocknull==false)
            {
                tk.Status = "Monitoring";
                goto B;
            }
            #region
            /*    foreach (var i in jo["partNumbers"].ToArray())
                {
                    if (i.ToString().Contains(eid))
                    {
                        Regex rex = new Regex(@"\d{6}");
                        sizeid = rex.Match(i.ToString()).ToString();
                        break;
                    }
                }
                foreach (var i in jo["stock"].ToArray())
                {
                    if (i.ToString().Contains(sizeid))
                    {
                        var st = i.ToString().Replace(sizeid, "").Replace(",", "").Replace(" ", "").Replace("\"", "").Replace(":", "");
                        int stock = int.Parse(st);
                        if (stock > 0)
                        {
                            isstocknull = false;
                        }
                        break;
                    }
                }
             if (isstocknull)
            {
                tk.Status = "Monitoring";
                goto B;
            }
            */
            #endregion

        }
        public void ATC(CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string url = "";
            string info = "";
            if (tk.Tasksite == "TheNorthFaceUK")
            {
                 url = "https://www.thenorthface.co.uk/webapp/wcs/stores/servlet/VFAjaxOrderItemAdd";
                 info = "categoryId=" + categoryId + "&storeId=7005&langId=-1&catalogId=13503&catEntryId=" + sizeid + "&quantity=1&orderId=.&URL=%2F&requesttype=ajax";
            }
            else
            {
                 url = "https://www.thenorthface.com/webapp/wcs/stores/servlet/VFAjaxOrderItemAdd";
                 info = "categoryId=" + categoryId + "&storeId=7001&langId=-1&catalogId=20001&catEntryId=" + sizeid + "&quantity=1&orderId=.&URL=%2F&requesttype=ajax";
            }
           
            orderid = tnfAPI.ATC(url, tk, ct, info);
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
        }
        public void SubmitShipping(CancellationToken ct,JObject jo, CancellationTokenSource cts)
        {
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string url = "";
            string info = "";
            long time = (long)(DateTime.Now.ToUniversalTime() - timeStampStartTime).TotalMilliseconds;
            if (tk.Tasksite == "TheNorthFaceUK")
            {
                 url = "https://www.thenorthface.co.uk/shop/VFAjaxAVSAddressAdd";
                 info = "storeId=7005&country=" + jo["Country"].ToString() + "&country=" + jo["Country"].ToString() + "&toOrderId=" + orderid + "&addressType=S&outAddressName=addressId&isVerificationRequired=yes&URL=%2F&qasVerificationPage=true&formatPhoneWhileTyping=true&countryCode=US&nickName=" + time + "&personTitle=Mr.&firstName=" + jo["FirstName"].ToString() + "&lastName=" + jo["LastName"].ToString() + "&address1=" + jo["Address1"].ToString() + "&address2=" + jo["Address2"].ToString() + "&zipCode=" + jo["Zipcode"].ToString() + "&city=" + jo["City"].ToString() + "&state=" + jo["State"].ToString() + "&phone1=" + jo["Tel"].ToString() + "&email1=" + System.Web.HttpUtility.UrlEncode(jo["EmailAddress"].ToString()) + "&qasOperation=verificationEngine&requesttype=ajax";
            }
            else
            {
                 url = "https://www.thenorthface.com/shop/VFAjaxAVSAddressAdd";
                 info = "storeId=7001&country=" + jo["Country"].ToString() + "&country=" + jo["Country"].ToString() + "&toOrderId=" + orderid + "&addressType=S&outAddressName=addressId&isVerificationRequired=yes&URL=%2F&qasVerificationPage=true&formatPhoneWhileTyping=true&countryCode=US&nickName=" + time + "&personTitle=Mr.&firstName=" + jo["FirstName"].ToString() + "&lastName=" + jo["LastName"].ToString() + "&address1=" + jo["Address1"].ToString() + "&address2=" + jo["Address2"].ToString() + "&zipCode=" + jo["Zipcode"].ToString() + "&city=" + jo["City"].ToString() + "&state=" + jo["State"].ToString() + "&phone1=" + jo["Tel"].ToString() + "&email1=" + System.Web.HttpUtility.UrlEncode(jo["EmailAddress"].ToString()) + "&qasOperation=verificationEngine&requesttype=ajax";
            }
                          
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }       
            tnfAPI.shipping(url,tk,ct,info.Replace(" ","+"));

            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string info2 = "";
            string getpaymengurl = "";
            if (tk.Tasksite == "TheNorthFaceUK")
            {
                getpaymengurl = "https://www.thenorthface.co.uk/shop/OrderPrepare";
                info2 = "storeId=7005&orderId=" + orderid + "&URL=VFBillingAndPaymentView&errorURL=VFShippingAddressView%3ForderId%" + orderid + "&containsPayPal=&isSavedAddress=true&shipToStore=&removeGiftServices=";
            }
            else
            {
                 getpaymengurl = "https://www.thenorthface.com/shop/OrderPrepare";
                 info2 = "storeId=7001&orderId=" + orderid + "&URL=VFBillingAndPaymentView&errorURL=VFShippingAddressView%3ForderId%" + orderid + "&containsPayPal=&isSavedAddress=true&shipToStore=&removeGiftServices=";
            }         
            tnfAPI.preorder(getpaymengurl,tk,ct,info2,eid,sizeid,productid,cts);
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
        }
        public void payment(CancellationToken ct)
        {
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string getorderviewurl = "";
            string orderinfo = "";
            if (tk.Tasksite == "TheNorthFaceUK")
            {
                getorderviewurl = "https://www.thenorthface.co.uk/shop/VFAjaxGetOrderView";
                orderinfo = "storeId=7005&orderId=" + orderid + "&requesttype=ajax";
            }
            else
            {
                getorderviewurl = "https://www.thenorthface.com/shop/VFAjaxGetOrderView";
                orderinfo = "storeId=7001&orderId=" + orderid + "&requesttype=ajax";
            }
            TNFUKBillingid tbl = new TNFUKBillingid();
            tnfAPI.orderdetail(getorderviewurl, tk, ct, orderinfo,tbl);
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string AjaxOrderCalculate = "";
            string ajaxorderurl = "";
            if (tk.Tasksite == "TheNorthFaceUK")
            {
                 AjaxOrderCalculate = "orderId=" + orderid + "&storeId=7005&langId=-1&updatePrices=1&calculationUsageId=-1&calculationUsageId=-2&calculationUsageId=-3&calculationUsageId=-4&calculationUsageId=-5&calculationUsageId=-6&calculationUsageId=-7&showOrder=1&promoCode=&altAction=OrderCalculate&requesttype=ajax";
                 ajaxorderurl = "https://www.thenorthface.co.uk/webapp/wcs/stores/servlet/AjaxOrderCalculate";

            }
            else
            {
                 AjaxOrderCalculate = "orderId=" + orderid + "&storeId=7001&langId=-1&updatePrices=1&calculationUsageId=-1&calculationUsageId=-2&calculationUsageId=-3&calculationUsageId=-4&calculationUsageId=-5&calculationUsageId=-6&calculationUsageId=-7&showOrder=1&promoCode=&altAction=OrderCalculate&requesttype=ajax";
                 ajaxorderurl = "https://www.thenorthface.com/webapp/wcs/stores/servlet/AjaxOrderCalculate";
            }

            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }         
            tnfAPI.orderdetail(ajaxorderurl,tk,ct,AjaxOrderCalculate,tbl);
            string url = "";
            if (tk.Tasksite == "TheNorthFaceUK")
            {
                string precheckouturl = "https://www.thenorthface.co.uk/shop/VFCAjaxWorldpayPunchoutCmd";
                string precheckoutinfo = "billingAddress="+tbl.Billingid+"&payMethodId=PAYPAL&orderId="+orderid+ "&ipAddress=47.75.243.59&userAgent=Mozilla%2F5.0+(Windows+NT+10.0%3B+WOW64)+AppleWebKit%2F537.36+(KHTML%2C+like+Gecko)+Chrome%2F81.0.4044.138+Safari%2F537.36+OPR%2F68.0.3618.173&callBackURL=https%3A%2F%2Fwww.thenorthface.co.uk%2Fshop%2FVFCWorldpayPunchoutCallbackCmd%3ForderId%3D" + orderid+"%26storeId%3D7005%26langId%3D-11&isFromPaymentPage=true&requesttype=ajax";
                string source=tnfAPI.precheckout(precheckouturl,tk,ct,precheckoutinfo);
                JObject jo = JObject.Parse(source);
                string wpredirecturl=jo["wpRedirectUrl"].ToString();

            }
            else
            {
                 url = "https://www.thenorthface.com/shop/SendToPaypal?storeId=7001&orderId=" + orderid + "&callSource=Payment&useraction=commit&cancelURL=https%3a%2f%2fwww.thenorthface.com%2fshop%2fVFBillingAndPaymentView%3forderId%3d" + orderid + "%26storeId%3d7001&returnURL=https%3a%2f%2fwww.thenorthface.com%2fshop%2fZCReturnFromPaypal%3fcallSource%3dPayment%26orderId%3d" + orderid + "%26shippingAddressName%3dPayPal%2bShipping%26billingAddressName%3dPayPal%2bBilling%26storeId%3d7001%26URL%3dOrderOKView";
                  paypallink = tnfAPI.Checkout2(url, tk, ct);
            }
           
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string pd2 = "{\"username\":\"MAIO\",\"avatar_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\",\"embeds\":[{\"title\":\"You Just Chekcout!\",\"color\":3329330,\"footer\":{\"text\":\"" + "MAIO" + DateTime.Now.ToLocalTime().ToString() + "\",\"icon_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\"},\"fields\":[{\"name\":\"Checkout out!!!\",\"value\":\"" + paypallink + "\\t\\t\\t\\tSize:" + tk.Size + "\\t\\t\\t\\t\",\"inline\":false}]}]}";         
            Http("https://discordapp.com/api/webhooks/517871792677847050/qry12HP2IqJQb2sAfSNBmpUmFPOdPsVXUYY2_yhDgckgznpeVtRpNbwvO1Oma6nMGeK9", pd2);
            Http(Config.webhook, pd2);
            tk.Status = "Success";

        }
        public void Http(string url, string postDataStr)
        {
        Retry: Random ra = new Random();
            int sleeptime = ra.Next(0, 3000);
            Thread.Sleep(sleeptime);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json; charset=utf-8";
            request.Method = "POST";
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
            catch (WebException)
            {
                Thread.Sleep(500);
                goto Retry;
            }
        }
        public async void autocheckout()
        {
            LaunchOptions launchOptions = await ChromiumBrowser.ChromiumLaunchOptions(true, true);
            launchOptions.Headless = false;
            using (Browser browser = await Puppeteer.LaunchAsync(launchOptions))
            {
                using (Page page = await ChromiumBrowser.NewPageAndInitAsync(browser))
                {
                    await page.SetUserAgentAsync("Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36 OPR/68.0.3618.173");
                    for (int i = 0; i < TnfCheckoutcookie.cookielist.Count; i++)
                    {
                        await page.SetCookieAsync(TnfCheckoutcookie.cookielist[i]);
                    }
                    await page.GoToAsync(paypallink);
                    await Task.Delay(360000000);
                }
            }
            
        }
        private static DateTime timeStampStartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    }
}
