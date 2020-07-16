using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MAIO
{
    /// <summary>
    /// version 0.85
    /// </summary>
    public partial class LoginWindow : Window
    {
        public static string version = "0.85";//everychange

        public LoginWindow()
        {
            InitializeComponent();
            checkkey();
        }     
        public void checkkey()
        {
            var hwid = MD5Helper.EncryptString(FingerPrint.Value());
            string path = Environment.CurrentDirectory + "\\" + "config.json";
            if (File.Exists(path))
            {
                if (this.keycheck(hwid))
                {
                    try
                    {
                        Mainwindow MD = new Mainwindow();
                        string config = File.ReadAllText(path);
                        JObject jo = JObject.Parse(config);
                        Config.Key = MD.Key = jo["key"].ToString();
                        Config.webhook = MD.webhook = jo["webhook"].ToString();
                        Config.cid = MD.cid = jo["cid"].ToString();
                        Config.cjevent = MD.cjevent = jo["cjevent"].ToString();
                        Config.delay = jo["delay"].ToString();
                        Config.Usemonitor = jo["Usemonitor"].ToString();
                        Config.UseAdvancemode = jo["Advancemode"].ToString();
                        this.Close();
                        MD.Show();
                    }
                    catch (Exception)
                    {
                        Application.Current.Shutdown();
                    }
                }
            }
        }
        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string path = Environment.CurrentDirectory + "\\" + "config.json";
            Mainwindow MD = new Mainwindow();
            string key = Keyinput.Text;
            var hwid = MD5Helper.EncryptString(FingerPrint.Value());
            var checkkey = AESHelper.Decrypt(key);
            if (checkkey == "")
            {
                Keyinput.Text = "";
            }
            else
            {
                var md5checkkey = MD5Helper.EncryptString(key);
                bool keyvaild = keyauth(md5checkkey, hwid, version);
                if (keyvaild)
                {
                    if (!File.Exists(path))
                    {
                        FileStream fs1 = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                        fs1.Close();
                        File.WriteAllText(path, "{\"webhook\":\"\",\"key\":\"\",\"cid\":\"\",\"cjevent\":\"\",\"delay\":\"\",\"Usemonitor\":\"\",\"Advancemode\":\"\"}");
                        Config.Key = MD.Key = key;
                        Config.webhook = MD.webhook = "";
                        Config.cid = MD.cid = "";
                        Config.cjevent = MD.cjevent = "";
                        Config.delay = "";
                        Config.Usemonitor = "";
                        Config.UseAdvancemode = "";
                        Close();
                        MD.Show();
                    }
                    else
                    {
                        string config = File.ReadAllText(path);
                        JObject jo = JObject.Parse(config);
                        jo["key"] = key;
                        File.WriteAllText(Environment.CurrentDirectory + "\\" + "config.json", config);
                        Config.Key = jo["key"].ToString();
                        Config.webhook = jo["webhook"].ToString();
                        Config.cid = jo["cid"].ToString();
                        Config.cjevent = jo["cjevent"].ToString();
                        Config.delay = jo["delay"].ToString();
                        Config.Usemonitor = jo["Usemonitor"].ToString();
                        Config.UseAdvancemode = jo["Advancemode"].ToString();
                        Close();
                        MD.Show();
                    }
                }
                else
                {
                    Keyinput.Text = "";
                }
            }
        }
       
        public bool keyauth(string md5key, string cpuid, string version)
        {
            var binding = new BasicHttpBinding();
            var endpoint = new EndpointAddress(@"http://49.51.68.105/WebService1.asmx");
            var factory = new ChannelFactory<ServiceReference2.WebService1Soap>(binding, endpoint);
            var callClient = factory.CreateChannel();
            var check = callClient.KeyAUTHAsync(md5key, cpuid, version);
            return check.Result;
        }
        public bool KeyRest(string md5key)
        {
            var binding = new BasicHttpBinding();
            var endpoint = new EndpointAddress(@"http://49.51.68.105/WebService1.asmx");
            var factory = new ChannelFactory<ServiceReference2.WebService1Soap>(binding, endpoint);
            var callClient = factory.CreateChannel();
            var resetkey = callClient.KeyresetAsync(md5key);
            return resetkey.Result;
        }
        public bool keycheck(string cpuid)
        {
            var binding = new BasicHttpBinding();
            var endpoint = new EndpointAddress(@"http://49.51.68.105/WebService1.asmx");
            var factory = new ChannelFactory<ServiceReference2.WebService1Soap>(binding, endpoint);
            var callClient = factory.CreateChannel();
            var resetkey = callClient.keycheckAsync(cpuid);
            return resetkey.Result;
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
        public class AESHelper
        {
            private const string PublicKey = "devbychaowuchaow";
            private const string Iv = "abcdefghijklmnop";
            public static String Encrypt(string str)
            {
                return Encrypt(str, PublicKey);
            }

            public static String Decrypt(string str)
            {
                return Decrypt(str, PublicKey);
            }
            public static string Encrypt(string str, string key)
            {
                Byte[] resultArray = null;
                try
                {
                    Byte[] keyArray = Encoding.UTF8.GetBytes(key);
                    Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);
                    var rijndael = new RijndaelManaged();
                    rijndael.Key = keyArray;
                    rijndael.Mode = CipherMode.ECB;
                    rijndael.Padding = PaddingMode.PKCS7;
                    rijndael.IV = Encoding.UTF8.GetBytes(Iv);
                    ICryptoTransform cTransform = rijndael.CreateEncryptor();
                    resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                }
                catch (Exception)
                { }
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            public static string Decrypt(string str, string key)
            {
                Byte[] resultArray = null;
                try
                {
                    Byte[] keyArray = Encoding.UTF8.GetBytes(key);
                    Byte[] toEncryptArray = Convert.FromBase64String(str);
                    var rijndael = new RijndaelManaged();
                    rijndael.Key = keyArray;
                    rijndael.Mode = CipherMode.ECB;
                    rijndael.Padding = PaddingMode.PKCS7;
                    rijndael.IV = Encoding.UTF8.GetBytes(Iv);
                    ICryptoTransform cTransform = rijndael.CreateDecryptor();
                    resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                }
                catch (Exception)
                {
                    resultArray = System.Text.Encoding.Default.GetBytes("");
                }
                return Encoding.UTF8.GetString(resultArray);

            }
        }
    }
}
