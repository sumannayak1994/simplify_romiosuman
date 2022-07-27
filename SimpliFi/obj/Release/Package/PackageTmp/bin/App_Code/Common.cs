using System.Configuration;
using System.Web;
using SimpliFi.Saml;

namespace SimpliFi
{
    /// <summary>
    /// Summary description for Common
    /// </summary>
    public class Common
    {
        public static string GetProgramName()
        {
            if (!string.IsNullOrEmpty(HttpContext.Current.Session[SessionVariables.ProgramName] as string))
            {
                string program = HttpContext.Current.Session[SessionVariables.ProgramName] as string;

                return program;
            }
            else
                return string.Empty;
        }

        public static bool CheckSession()
        {
            if (!string.IsNullOrEmpty(HttpContext.Current.Session[SessionVariables.UserName] as string)
                && !string.IsNullOrEmpty(HttpContext.Current.Session[SessionVariables.ProgramName] as string))
            {
                if (ConfigurationManager.AppSettings["Host"] == "Server")
                {
                    if ((HttpContext.Current.Session[SessionVariables.SAMLResponse] as Response) != null)
                        return true;
                    else
                        return false;
                }
                else
                {
                    // For Local
                    return true;
                }
            }
            else
                return false;
        }

        public static string GetCurrentPath(string type)
        {
            string userName = HttpContext.Current.Session[SessionVariables.UserName] as string;
            string currPath = string.Empty;

            switch (type)
            {
                case "cgen":
                    currPath = "~/UploadedFiles/" + userName + "/cgen/";
                    break;
                case "Emails":
                    currPath = "~/UploadedFiles/" + userName + "/Emails/";
                    break;
                case "reports":
                    currPath = "~/UploadedFiles/" + userName + "/Reports/";
                    break;
                case "CRF":
                    currPath = "~/UploadedFiles/" + userName + "/Import Files/BNB/CRF/";
                    break;
                case "BNB/CGEN":
                    currPath = "~/UploadedFiles/Master Import Files/BNB/CGEN/";
                    break;
                case "BNB/Proof":
                    currPath = "~/UploadedFiles/Master Import Files/BNB/Proof/";
                    break;
                case "Download Files":
                    currPath = "~/UploadedFiles/" + userName + "/Import Files/BNB/Download Files/";
                    break;
                case "PBLOCK":
                    currPath = "~/UploadedFiles/" + userName + "/PBLOCK/";
                    break;
                case "JSON":
                    currPath = "~/UploadedFiles/" + userName + "/JSON/";
                    break;
            }

            return currPath;
        }

        public static string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);

            if (place == -1)
                return Source;

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }

        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsLocalHost()
        {
            string host = HttpContext.Current.Request.Url.Host.ToLower();
            return (host == "localhost");
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}