using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace MAIO
{
    class TheNorthFaceAPI
    { 
        Random ran = new Random();
        public object GetHtmlsource(string url, Main.taskset tk, CancellationToken ct)
        {
        A: if (ct.IsCancellationRequested)
            {
                tk.Status = "IDLE";
                ct.ThrowIfCancellationRequested();

            }
            string SourceCode = "";
            JArray ja = null;
            JObject jo = null;
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
                var chao = System.Web.HttpUtility.UrlDecode(response.Headers["Set-Cookie"]);       
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
                if (url.Contains("VFAjaxProductAvailabilityView"))
                {
                     jo = JObject.Parse(SourceCode);
                    response.Close();
                    readStream.Close();
                    return jo;
                }
                else
                {
                    Regex rex = new Regex(@"(dataLayer = )(?=\[).+(?=\])]");
                    ja = JArray.Parse(rex.Match(SourceCode).ToString().Replace("dataLayer = ", ""));
                    response.Close();
                    readStream.Close();
                    return ja;
                }

              
            }
            catch (WebException ex)
            {
                HttpWebResponse response = (HttpWebResponse)ex.Response;
                goto A;
            }
            
        }
        public void ATC(string url, Main.taskset tk, CancellationToken ct,string info)
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
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            byte[] contentpaymentinfo = Encoding.UTF8.GetBytes(info);
            request.Accept = "application/json, text/javascript, */*; q=0.01";
            request.Headers.Add("Cookie", @"Edgescape-Country=HK; Edgescape-Lat-Long=22.28,114.15; Edgescape-State=; Edgescape-City=HONGKONG; Edgescape-Zip=; bm_sz=DF7EE198A63EBAC642962EB17B5ACAE4~YAAQt1sauAt8Ui1zAQAAv3pdQAjKBL2ytb+juBVmGXJMZcpfiWDRe60xeqveTSmjVx+rocszRwWgo3GJqQLzzSnsd1GImt3imkATnW2MWP+prjBxuyfhhcXPTaKONk3dQBfBe8naLJ0QEp/BBYgyrW3BNTf93K0nz3FnsXDfMfly6j25Aj2kCXY0kegxmATLHN2vNcS+; _gcl_au=1.1.174239280.1594512756; mt.v=2.919073603.1594512756101; AMCVS_FBE01539554391520A4C98C6%40AdobeOrg=1; rmStore=dmid:7973; _pin_unauth=dWlkPU56VXhaak5oT1RFdE5tTXlNQzAwTmpBeExUZzFNRFV0WmpBMllUWTVNV1ZoTkdGbA; _ga=GA1.2.17369792.1594512759; _gid=GA1.2.1514725545.1594512759; s_cc=true; referrerTracking=undefined; referrerTrackingRetain=false; cn3=0; WC_ACTIVEPOINTER=-1%2C7001; DISPLAYNAME7001=guest;USER_TYPE7001=G; WC_SESSION_ESTABLISHED=true; _scid=b0b10765-bf12-434a-b301-b6f2e6d56098; _cs_c=0; crl8.fpcuid=09f314c8-b84b-495a-9143-34adf787fe44; aam_uuid=19029151186136862990786704142897505370; BCSessionID=94e03513-6453-4dbf-8835-db99ff060e47; _sctr=1|1594483200000; _fbp=fb.1.1594512766579.1792027289; RES_TRACKINGID=49561983622289; ResonanceSegment=1; __pr.dp3=bqadeb15nr; _bcvm_vrid_4388611242818298611=2917429003767627T0D8F8428F71A0E5A45925ED37688A28ED8523DF5D3F9235DD0EA396D7C89D6877BF7A670A5362B3A920DA68921F95670667889339E00E25CAC1D139FF07521A9; WC_PERSISTENT=gxjPgeF7SIR2Flbx36SA%2BF2RcmI3%2Bk0qPepcU6cnt1g%3D%3B2020-07-11+20%3A12%3A57.139_1594512763815-178992_7001_260244889%2C-1%2CUSD%2CDMEgrCqHKCS2S0rGOpG8Mi%2BD1JBVp%2FXbxtUEovelxsnRucS0E78cX527aI7ak9kLRqEzGkXgxnQP%2F7CrfI9Mpw%3D%3D_7001; WC_USERACTIVITY_260244889=260244889%2C7001%2Cnull%2Cnull%2C1594512777155%2Cnull%2Cnull%2Cnull%2Cnull%2Cnull%2C798384096%2CHWp3VOa7ltTnbgTUhNN4fNlJoM369BgXA3K%2FLuPgICUOz%2B3oxyFU7cGdiO23pY0FDG%2BnaNvaKPtO6fb7P0%2Bz9ytqSwb9mvgyzUt7MKEFrU0PBbAedG4g9Iq03OcnaV7uMjqdzEfpTNOCvFk4WyA3J%2BQ3oy62WQzwPC5hfYKjZafPoMIFrcmjnw3sugoksntqCEk1BdBA1AV1ue1qeGhc4nhaCEatBKbp81MQoCJt0N8cJ1Xdtbum1aASQz0b5ah8; WC_AUTHENTICATION_260244889=260244889%2CgHhnsntV6aOAuVYkBZbBUjuZ8Bej82zTwXTCxmrMyWI%3D; ttCheckout=1597161600000; ku1-sid=gOAQPhoTj_HO07JGr79MU; ku1-vid=f43c236f-ed39-692d-ec64-027d15f8277f; _abck=01383483A792E1DE1263A18F34E67CE5~0~YAAQt1sauISGUi1zAQAAuO1jQARBzsW8BGaqrgf93VJS392mh+QmxJpaE0MjNnrL7vTdnr6FXAf/K+iDD3llMbmuhBq4vhlCnwgYkjsRTVeSBzeKwvONBv2Vbv1YubStoryeTPCWLu25wRhyrt96H7FOnkmPtu2cnZfGhKoAAlXcMEigCeItZ2+qPIpJ1SDnuiKwOihWZAHm3pqztjkPnExseJiRKTgwsBRdyV7bwoN/JixHpus3rYsZ1pC/xtzZ+WVg9mx5CebAseX+rE5Yz2mtzi1fICkhRmKgXQ0JByTU66LiT6eqd0YzM4CqQenqadoHhole07Qc0dxWzg==~-1~-1~-1; _k_lia_e=c+85aJrn14GzVsa44Z23IOOHHHXvNo+fGUezaQBTdnXpUtbOgpMiqUK40XdtwImq7DM0W0+xUYeBUyuIcJTUuQ0/mwajmpvUEClfPglsrtbPTfWulEO0hPGuH++UOVb8HnGqsPhlEMU31o+G3BTu2Urr3sXMB3qWFg22PiOOYFOicq7XyBJT3idknk7A43Pw4pF5KnNZFVFfDNgqBc15/Tp3u/KRaxcHlSbxDghHz8rTY5TOfLJQ2W8aBi+75fLAwrssqjKT064kNprTF/IIEW/O2CdHksVMIjFDMKZBJsy34H1Ax+yjec3OIRv/9gODqExT1IHFYNdpCK1UNBEbof502jiQRmLXwBhjQtuYUcL/VVnAzoJ6YNW3U06FnOoLerpLla4J0HwNiMBESf/0SR7w7AULWr/KgcdqzAgnusawFZ/zZwRHsKIBmj2A4fl0kJiBgbmrufQHxZmU1k3FZQAy9mEI4miDOKwpQNVeo1IrB0QKg+wEr7ubSxGGEvYJpxhukdjws5v9KNalgXscCdV2grA8cV7n/abzJA089u6mISZYS4v2Eo9OL4Am520Q8RSkF5wVlMwY3Fr9zCHfGiGIyxkfsCmItoDBeT50ov00MkYD5nn30f2sYN57u8UVaiZgWBBprguBA1U8R/vwYM+EMt9UmlzJz+JkaFsn/zo=; _bcvm_vid_4388611242818298611=2917475811858842TDD470AD0B74C11B564AEBEDF41477E68E0D4004D7C81F76B1B9706C836C5F369E4C16FD47A3BCEB68FD71B21227D700C2E2C5AF88099EE3212DA5EC63D859AE5; s_sq=%5B%5BB%5D%5D; VF_ENDLESS_SCROLL=https%3A//www.thenorthface.com/shop/mens-auxiliary-new-arrivals%3FcategoryId%3D226102%26pages%3D0%26scrollPosition%3D150;  AKA_A2=A; akavpau_VP_Scheduled_Maintenance=1594523554~id=992b87c0d0ec27332fc0347c78a4ef3d; mt.sc=%7B%22i%22%3A1594523255786%2C%22d%22%3A%5B%5D%7D; ak_bmsc=1B147C852BEA259DC090AECD05879FEB6873267D8919000097720A5F7D97C642~pl2PMAX/fTseQpI2ug2gcemrq4dJz8fZETAGcNB/5U7Sqq6gis2mBHqojanx+6HrPhhkmXYWIeXPUgiD3MOAxBzDyNW+HhABql8Nqy/m1kkPMtpGGsCfmiWRcO36YggNlQv6efVyk2/kFKrMEiO1nQyZB567OQliN5E2evdKAu6cKz3s1sGIn68msYTk1nxFAiCKadEpx3yAUNZ+NFaQCs6tVcQ/UGlwSPBoYxVFMlPDvYXT7AIC0pBG5ksq2pdkUm; bm_mi=7233BE6428CC2D4F6A117F99470FCC0F~2QjA3OYUfQukp/96ZUG26e5mXWbsYdajg8DV73Un949HkM/bA0ZiglzUqYPkg58aPFO5c9nIrQ9DjVbEgmcpCBk7f98Sxgjme71QKqRU0/PHqoF3tI6otgQN7BdOXy49tqkAtTaS1AgEELra5pN8kJgQFCQfVGyBkg5NGH6UMLOzOjf6TG24wUwOmXF84o8AzGarw9kAsJs2mWkJOxA+10w2DGL9I5V2beHmEqUOiQ7oTEahgB33tvNX9gYadNlBcGR60hGc/a7RkPrNgT8uKQBZapYr9azszMOJhLfu2x8=; stc115921=env:1594523258%7C20200812030738%7C20200712033738%7C1%7C1054958:20210712030738|uid:1594512758040.1821009463.8020716.115921.726715944.:20210712030738|srchist:1054958%3A1594523258%3A20200812030738:20210712030738|tsa:1594523258012.820234419.7605386.4939127857512886.7:20200712033738; _cs_id=2dfc7d2a-b86c-a4e5-d80c-3926fceefedd.1594512765.4.1594523258.1594522746.1.1628676765780.Lax.0; _cs_s=2.1; JSESSIONID=0000AVL1mqoo46ktgFzy_ny9BRR:1e4925945; mp_tnf_mixpanel=%7B%22distinct_id%22%3A%20%22173405df0d0334-0b8c3e37c2faff-7f2f4867-1704a0-173405df0d1339%22%2C%22bc_persist_updated%22%3A%201594512765139%7D; _br_uid_2=uid%3D8811289225167%3Av%3D12.1%3Ats%3D1594512765122%3Ahc%3D26; _gat_UA-4561169-4=1; RES_SESSIONID=492225792234289; bc_pv_end=; AMCV_FBE01539554391520A4C98C6%40AdobeOrg=1278862251%7CMCIDTS%7C18456%7CMCMID%7C19020471946274293180785598655074688903%7CMCAAMLH-1595128060%7C11%7CMCAAMB-1595128060%7CRKhpRz8krg2tLO6pguXWp5olkAcUniQYPHaMWWgdJ3xzPWQmdj0y%7CMCOPTOUT-1594530460s%7CNONE%7CMCAID%7C2F852ABC0515FACD-600009D5210AB07D%7CvVersion%7C4.0.0; _4c_=jVLJbtswEP2VgoecLIm7SANG4DhOkaJpkC7o0aBIyhIiSwJFW0kD%2F3tJ21mQAEV1oDgz773hLE9grGwLpohJyjDBHDHOJuDePg5g%2BgR0H89dPLauAVNQed8P0ywbxzH1gdk5X5VK21R3m2youj7b2HZI1PahbmrlHpPWjolyrt6pZsg2ybpRurYuCVDnk7aEimpbn%2B%2BUq5Wvu%2FbazJaLr2ACdGdsyIdkmqc82P5PsKiA4dq7zmy1X%2FnHPkJGW3wazH0IGLurtV2NtfFV5DJIX72VrdeVj24ISXT3LhopC%2Fexbk03vhBl%2Fsb5whMSB2%2FhunGwkXrbW6c%2BcRGcXWgW%2BH3AD8F0trTOHUD%2F7FfVbWyv1jat%2FKYJvKH2saD3uFMkzORdMARub35%2BX10s54vbb2%2By7UrduT41KB102m28Mzptrc%2BKbBhi0LfldohtnARj3XSFaqKVIZh9%2BZHgFJEUZQNBGDMiRU4ElOx8fncxQ2eb2syQhBjSHEnKcU6xJEjAXDAmBWcM5pQLISE5UwGKrwSV%2BfwSMsSWVzRPOAyfZGSx5Au4lPRsfrecoTjVXRwweq7pZnF9eZj%2F%2F6QKpM%2Fz1a8Dg3MpqcBcpnGnKQmYHOwn4OG45BxKziTDOKyUDxstOI0vggHhanPadsBsXgZtmkipVULzokwKzXUSmo6k4YU0sARHTQ4xRoTmhBOxP5Zx0KDoQ070MWffnOCvaIZZKA1xfkIj%2BoLe1c8PFEyrkiuTGGnCA0nOE4kwTIxVhAmrMbMSvJWE4SfZs6Q4Ku73fwE%3D; gpv=TNF%3Apdp%3ANF0A4CEI; bm_sv=CA119CCCC3D9AC765CEC1AF98A9C339F~vjX1XUnkvHlszrszBKb6ftU6MGouJW21B1bgdBvUbfWn/DGZcgyAsh5q0SsLvv6Ysuu9kuN6mZ0DIceHGpwNZg1mbqNGOnDjYMbh+opTR4UvobH1IZ9zzpLDPdLMyRN5xsJWgi4/rvlg6jbRsuMXntXCBSbhvjLsCPjrCU1AXNE=; s_getNewRepeat=1594523268809-Repeat");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            request.Headers.Add("Accept-Language", "zh-CN,zh;q=0.9");
            request.ContentLength = contentpaymentinfo.Length;
            request.Host = "www.thenorthface.com";
            request.Referer = "https://www.thenorthface.com/shop/mens-auxiliary-new-arrivals/m-glacier-short-nf0a4cei?variationId=ECL";
            request.Headers.Add("Origin", "https://www.thenorthface.com");
            request.Headers.Add("Sec-Fetch-Dest", "empty");
            request.Headers.Add("Sec-Fetch-Mode", "cors");
            request.Headers.Add("Sec-Fetch-Site", "same-site");
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36 OPR/68.0.3618.173";
            request.Headers.Add("X-Requested-With", "XMLHttpRequest");
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var chao = System.Web.HttpUtility.UrlDecode(response.Headers["Set-Cookie"]).Split(";");
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
    }
}
