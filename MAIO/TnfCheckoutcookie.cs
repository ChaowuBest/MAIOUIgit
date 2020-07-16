using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace MAIO
{
    public static class TnfCheckoutcookie
    {
        public static CookieParam JSESSIONID = new CookieParam();
        public static CookieParam akavpau_VP_Scheduled_Maintenance = new CookieParam();
        public static CookieParam SHOPPINGCART7001 = new CookieParam();
        public static CookieParam WC_PERSISTENT = new CookieParam();
        public static CookieParam WC_ACTIVEPOINTER = new CookieParam();
        public static CookieParam WC_AUTHENTICATION = new CookieParam();
        public static CookieParam WC_USERACTIVITY_ = new CookieParam();
        public static List<CookieParam> cookielist = new List<CookieParam>() { JSESSIONID, akavpau_VP_Scheduled_Maintenance, WC_AUTHENTICATION, SHOPPINGCART7001, WC_USERACTIVITY_, WC_PERSISTENT , WC_ACTIVEPOINTER };       
     
    }
}
