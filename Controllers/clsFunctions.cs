using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Principal;
using Models;
using System.Net;
using System.Xml.Linq;
using System.Reflection.Emit;
namespace Controllers
{
    public class clsFunctions
    {
        public static string MD5(string chuoi)
        {
            string str_md5 = "";
            byte[] mang = System.Text.Encoding.UTF8.GetBytes(chuoi);
            MD5CryptoServiceProvider my_md5 = new MD5CryptoServiceProvider();
            mang = my_md5.ComputeHash(mang);
            foreach (byte b in mang)
            {
                str_md5 += b.ToString("x2");
            }
            return str_md5;
        }
        #region Lấy thông tin IP
        public static string GetIP4Address()
        {
            try
            {
                string IP4Address = String.Empty;
                foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    if (IPA.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        IP4Address = IPA.ToString();
                        //break;
                    }
                }
                return IP4Address;
            }
            catch
            {
                return "Lấy IP bị lỗi";
            }
        }
        public static List<string> GetUseIPv4()
        {
            try
            {
                List<string> lstIP = new List<string>();
                NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface adapter in adapters)
                {
                    if (adapter.OperationalStatus == OperationalStatus.Up && adapter.GetIPProperties().GatewayAddresses.Count > 0)
                    {
                        IPInterfaceProperties properties = adapter.GetIPProperties();
                        var ip = properties.UnicastAddresses.Where(ua => ua.Address.AddressFamily == AddressFamily.InterNetwork).Select(ua => ua.Address.ToString()).FirstOrDefault();
                        lstIP.Add(ip);
                    }
                }
                return lstIP;
            }
            catch { return null; }
        }
        #endregion
        public static void Logs(string action, string detail)
        {
            try
            {
                using (dbDaDDataContext db = dbDaDDataContext.New())
                {
                    db.sp_insert_Logs(clsSecurity._UserLogin.UserID, GetIP4Address(), Environment.MachineName, "", action);
                }
            }
            catch (Exception)
            {
            }
        }
        public static bool Login(string Username, string Pass, bool remember)
        {
            dbDaDDataContext db = dbDaDDataContext.New();
            clsSecurity._UserLogin = db.Users.SingleOrDefault(s => s.Username == Username && s.PasswordHash == Pass);
            if (clsSecurity._UserLogin == null)
            {
                File.Delete(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.xml"));
                return false;

            }
            else
            {
                if (remember)
                {
                    createXMLSettings(Username, Pass);
                }
                else
                {
                    createXMLSettings("", "");
                }
                clsFunctions.Logs("Đăng nhập", "");
                return true;

            }
        }
        static void createXMLSettings(string TK, string MK)
        {
            try
            {
                if (!File.Exists("Settings.xml"))
                {
                    XDocument doc = new XDocument(
                        new XElement("Information",
                        new XElement("Login",
                        new XElement("Username", "" + TK),
                        new XElement("Password", "" + MK)))
                        );
                    doc.Save("Settings.xml");
                }
                else
                {
                    string xmlFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Settings.xml");
                    XDocument doc = XDocument.Load(xmlFile);
                    foreach (XElement element in doc.Descendants("Login"))
                    {
                        element.Element("Username").Value = TK;
                        element.Element("Password").Value = MK;
                    }
                    doc.Save("Settings.xml");

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
