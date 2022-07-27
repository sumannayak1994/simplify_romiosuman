using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using SimpliFi.Saml;


namespace SimpliFi
{
    public partial class sso : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (ConfigurationManager.AppSettings["Host"] == "Server")
                {
                    if (Session[SessionVariables.SAMLResponse] == null || !((Response)Session[SessionVariables.SAMLResponse]).IsValid())
                    {
                        AccountSettings accountSettings = new AccountSettings();

                        AuthRequest req = new AuthRequest(new AppSettings(), accountSettings);

                        string redirectURL = accountSettings.idp_sso_target_url + "?SAMLRequest=" + Server.UrlEncode(req.GetRequest(AuthRequest.AuthRequestFormat.Base64));

                        Response.Redirect(redirectURL);
                    }
                    else
                    {
                        //Session Exists
                        HttpContext.Current.Response.Redirect("login");
                    }
                }
                else {
                    Session[SessionVariables.UserName] = "tapkumar";
                    Session[SessionVariables.FirstName] = "Tapan";
                    Session[SessionVariables.SAMLResponse] = "Test";
                    Session[SessionVariables.ProgramName] = "B2CTriggered";

                    HttpContext.Current.Response.Redirect("json-extractor");
                }
            }
        }
    }
}