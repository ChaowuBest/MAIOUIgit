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
using System.Runtime.InteropServices.ComTypes;
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
        tnfsize ts = new tnfsize();
        public void StartTask(CancellationToken ct, CancellationTokenSource cts)
        {
        A: JObject joprofile = JObject.Parse(profile);
            if (tk.Tasksite.Contains("UK"))
            {
                tnfAPI.isGB = true;
            }
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
            if (size.Contains("+"))
            {
                string[] Multiplesize = size.Split("+");
                Random ra = new Random();
                size = Multiplesize[ra.Next(0, Multiplesize.Length)].ToString();
            }
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
                if (skuslist.Count != 0)
                {
                    Random ran = new Random();
                    int i = ran.Next(0, skuslist.Count);
                    eid = skuslist[i].ToString();
                }             
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
            tnfAPI.tnfsize = ts;
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
                 info = "categoryId=" + categoryId + "&storeId=7005&langId=-1&catalogId=13503&catEntryId=" + ts.sizeid + "&quantity=1&orderId=.&URL=%2F&requesttype=ajax";
            
            }
            else
            {
                 url = "https://www.thenorthface.com/webapp/wcs/stores/servlet/VFAjaxOrderItemAdd";
                 info = "categoryId=" + categoryId + "&storeId=7001&langId=-1&catalogId=20001&catEntryId=" + ts.sizeid + "&quantity=1&orderId=.&URL=%2F&requesttype=ajax";
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
                info = "storeId=7005&country=GB&country=GB&toOrderId=" + orderid + "&addressType=S&outAddressName=addressId&isVerificationRequired=yes&URL=%2F&qasVerificationPage=true&formatPhoneWhileTyping=false&countryCode=GB&nickName=" + time + "&personTitle=Mr.&firstName="+ jo["FirstName"].ToString() + "&lastName="+ jo["LastName"].ToString() + "&countryStateDisplayURL=VFAjaxCountryStateDisplay&currentAddressType=shipping&address1="+ jo["Address1"].ToString() + "&address2="+ jo["Address2"].ToString() + "&city="+ jo["City"].ToString() + "&zipCode="+ jo["Zipcode"].ToString() + "&phone1="+ jo["Tel"].ToString() + "&email1="+ System.Web.HttpUtility.UrlEncode(jo["EmailAddress"].ToString()) + "&qasOperation=verificationEngine&requesttype=ajax";

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
            string source=tnfAPI.shipping(url,tk,ct,info.Replace(" ","+"));
            JObject jo2 = JObject.Parse(source);
            addressid=jo2["addressId"].ToString();
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string info2 = "";
            string getpaymengurl = "";
            if (tk.Tasksite == "TheNorthFaceUK")
            {
                string url1 = "https://www.thenorthface.co.uk/shop/VFAjaxOrderItemUpdate";
                string info1 = "orderId=" + orderid + "&storeId=7005&langId=-11&calculateOrder=1&calculationUsage=-1%2C-2%2C-5%2C-7&addressId=" + addressid + "&keepAutoAddedItems=true&showOrder=1&requesttype=ajax";
                 string sourcecod =tnfAPI.orderdetail(url1,tk,ct,info1);
                JObject josour = JObject.Parse(sourcecod);
                string oderitemid=josour["itemIds_inStock"][0].ToString();
                string shipskip = josour["selectedShipMode_inStock"]["id"].ToString();

                string url2 = "https://www.thenorthface.co.uk/shop/VFAjaxOrderItemUpdate";
                string info3 = "storeId=7005&langId=-11&orderId="+orderid+"&calculateOrder=1&calculationUsage=-1%2C-2%2C-5%2C-7&showOrder=0&keepAutoAddedItems=true&orderItemId_1="+ oderitemid + "&shipModeId="+ shipskip + "&state_e=&shippingState=&requesttype=ajax";
                tnfAPI.orderdetail(url2,tk,ct,info3);

                getpaymengurl = "https://www.thenorthface.co.uk/shop/OrderPrepare";
                 info2 = "storeId=7005&orderId="+ orderid + "&URL=VFWorldpayBillingDisplay&shippingPageCall=true&payMethodId=WPCARDS&ipAddress=127.0.0.1&userAgent=Mozilla%2F5.0+%28Windows+NT+10.0%3B+WOW64%29+AppleWebKit%2F537.36+%28KHTML%2C+like+Gecko%29+Chrome%2F81.0.4044.138+Safari%2F537.36+OPR%2F68.0.3618.173&callBackURL=https%3A%2F%2Fwww.thenorthface.co.uk%2Fshop%2FVFCWorldpayPunchoutCallbackCmd%3ForderId%3D"+orderid+"%26storeId%3D7005&errorURL=VFShippingAddressView%3ForderId%3D"+orderid+"&ConfirmTermsAndConditions=on";
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
           
            if (tk.Tasksite == "TheNorthFaceUS")
            {
                getorderviewurl = "https://www.thenorthface.com/shop/VFAjaxGetOrderView";
                orderinfo = "storeId=7001&orderId=" + orderid + "&requesttype=ajax";

                tnfAPI.orderdetail(getorderviewurl, tk, ct, orderinfo);
            }          
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
            string AjaxOrderCalculate = "";
            string ajaxorderurl = "";
            if (tk.Tasksite == "TheNorthFaceUS")
            {

                AjaxOrderCalculate = "orderId=" + orderid + "&storeId=7001&langId=-1&updatePrices=1&calculationUsageId=-1&calculationUsageId=-2&calculationUsageId=-3&calculationUsageId=-4&calculationUsageId=-5&calculationUsageId=-6&calculationUsageId=-7&showOrder=1&promoCode=&altAction=OrderCalculate&requesttype=ajax";
                ajaxorderurl = "https://www.thenorthface.com/webapp/wcs/stores/servlet/AjaxOrderCalculate";
                tnfAPI.orderdetail(ajaxorderurl, tk, ct, AjaxOrderCalculate);
            }

            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }              
            string url = "";
            if (tk.Tasksite == "TheNorthFaceUK")
            {
                string precheckouturl = "https://www.thenorthface.co.uk/shop/VFCAjaxWorldpayPunchoutCmd";
                string precheckoutinfo = "billingAddress="+addressid+"&payMethodId=PAYPAL&orderId="+orderid+ "&ipAddress=127.0.0.1&userAgent=Mozilla%2F5.0+(Windows+NT+10.0%3B+WOW64)+AppleWebKit%2F537.36+(KHTML%2C+like+Gecko)+Chrome%2F81.0.4044.138+Safari%2F537.36+OPR%2F68.0.3618.173&callBackURL=https%3A%2F%2Fwww.thenorthface.co.uk%2Fshop%2FVFCWorldpayPunchoutCallbackCmd%3ForderId%3D" + orderid+"%26storeId%3D7005%26langId%3D-11&isFromPaymentPage=true&requesttype=ajax";
                string source=tnfAPI.precheckout(precheckouturl,tk,ct,precheckoutinfo);
                JObject jo = JObject.Parse(source);
                paypallink=jo["wpRedirectUrl"].ToString().Replace("amp;", "").Replace("&amp", "");

                ProcessNotification(true, "https://discordapp.com/api/webhooks/736544382018125895/Ti5zEbTcrKALkWhAePivSfyi7jlhRmRlILEyx9bPKIYh63qu1dVBDB2FFeyMFTSuRnpt", "");
                ProcessNotification(false, Config.webhook, paypallink);
                tk.Status = "Success";
            }
            else
            {
                 url = "https://www.thenorthface.com/shop/SendToPaypal?storeId=7001&orderId=" + orderid + "&callSource=Payment&useraction=commit&cancelURL=https%3a%2f%2fwww.thenorthface.com%2fshop%2fVFBillingAndPaymentView%3forderId%3d" + orderid + "%26storeId%3d7001&returnURL=https%3a%2f%2fwww.thenorthface.com%2fshop%2fZCReturnFromPaypal%3fcallSource%3dPayment%26orderId%3d" + orderid + "%26shippingAddressName%3dPayPal%2bShipping%26billingAddressName%3dPayPal%2bBilling%26storeId%3d7001%26URL%3dOrderOKView";
                 paypallink = tnfAPI.Checkout2(url, tk, ct);
                ProcessNotification(true, "https://discordapp.com/api/webhooks/736544382018125895/Ti5zEbTcrKALkWhAePivSfyi7jlhRmRlILEyx9bPKIYh63qu1dVBDB2FFeyMFTSuRnpt", "");
                ProcessNotification(false,Config.webhook, paypallink);
                tk.Status = "Success";
            }          
            if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();
            }
        }
        public void ProcessNotification(bool publicsuccess, string webhookurl, string paymenturl)
        {
            JObject jobject = null;
            if (publicsuccess)
            {
                jobject = JObject.Parse("{\"username\":\"MAIO\",\"avatar_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\",\"embeds\":[{\"title\":\"\",\"color\":3329330,\"description\":\"\",\"fields\":[{\"name\":\"SKU\",\"value\":\"\",\"inline\":true},{\"name\":\"Size\",\"value\":\"\",\"inline\":true},{\"name\":\"Site\",\"value\":\"\",\"inline\":true}],\"thumbnail\":{\"url\":\"\"},\"footer\":{\"text\":\"MAIO\",\"icon_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\"}}]}");
                jobject["embeds"][0]["title"] = "You Just Checkout!!!";
                jobject["embeds"][0]["fields"][0]["value"] = tk.Sku;
                jobject["embeds"][0]["fields"][1]["value"] = tk.Size;
                jobject["embeds"][0]["fields"][2]["value"] = tk.Tasksite;
                jobject["embeds"][0]["thumbnail"]["url"] = "";
            }
            else
            {
                jobject = JObject.Parse("{\"username\":\"MAIO\",\"avatar_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\",\"embeds\":[{\"title\":\"\",\"color\":3329330,\"description\":\"\",\"fields\":[{\"name\":\"SKU\",\"value\":\"\",\"inline\":true},{\"name\":\"Size\",\"value\":\"\",\"inline\":true},{\"name\":\"Paymenturl\",\"value\":\"\",\"inline\":false}],\"thumbnail\":{\"url\":\"\"},\"footer\":{\"text\":\"MAIO\",\"icon_url\":\"https://i.loli.net/2020/05/24/VfWKsEywcXZou1T.jpg\"}}]}");
                jobject["embeds"][0]["title"] = "You Just Checkout!!!";
                jobject["embeds"][0]["fields"][0]["value"] = tk.Sku;
                jobject["embeds"][0]["fields"][1]["value"] = tk.Size;
                jobject["embeds"][0]["fields"][2]["value"] = paymenturl;
                jobject["embeds"][0]["thumbnail"]["url"] = "";
            }
            Http(webhookurl, jobject.ToString());
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
