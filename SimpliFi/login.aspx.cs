using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SimpliFi.Saml;

namespace SimpliFi
{
    public partial class login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblYear.Text = DateTime.Now.Year.ToString();

            if (!IsPostBack)
            {
                try
                {
                    if (Request.QueryString["logout"] == null)
                    {
                        if (Session[SessionVariables.SAMLResponse] == null || !((Response)Session[SessionVariables.SAMLResponse]).IsValid())
                        {
                            // replace with an instance of the users account.
                            AccountSettings accountSettings = new AccountSettings();

                            Response samlResponse = new Response(accountSettings);
                            samlResponse.LoadXmlFromBase64(Request.Form[SessionVariables.SAMLResponse]);

                            Session[SessionVariables.SAMLResponse] = samlResponse;

                            AutenticateUser();
                        }
                        else
                        {
                            AutenticateUser();
                        }
                    }
                    else
                    {
                        divLogin.Visible = true;
                        divProgram.Visible = false;
                        divEnter.Visible = false;

                        if (Request.QueryString["q"] != null)
                            pageHeader.InnerText = "Welcome Back, " + Common.Base64Decode(Request.QueryString["q"]) + " !";
                        else
                            pageHeader.InnerText = "Welcome Back !";
                    }
                }
                catch (Exception ex)
                {
                    Response.Redirect("sso");
                }
            }
        }

        protected void btnEnter_Click(object sender, EventArgs e)
        {
            if (ddlProgram.SelectedIndex > 0)
            {
                Session[SessionVariables.ProgramName] = ddlProgram.SelectedValue;

                Response.Redirect("index");
            }
        }

        private void AutenticateUser()
        {
            Response response = (Response)Session[SessionVariables.SAMLResponse];

            if (response.IsValid())
            {
                string ldap = response.GetUserLDAP();
                string firstName = response.GetUserFirstName();

                Session[SessionVariables.UserName] = ldap;
                Session[SessionVariables.FirstName] = firstName;

                if (CheckUserAccess(ldap, string.Empty))
                {
                    divProgram.Visible = true;
                    divEnter.Visible = true;
                    pageHeader.InnerText = "Select a Program";
                }
                else
                {
                    divProgram.Visible = false;
                    divEnter.Visible = false;

                    pError.Visible = true;
                    lblError.Text = "You do not have access to SimpliFi. To get access <a style='color:#fff;' href='mailto:gmo_idealabs@adobe.com?subject=Access Request For SimpliFi - LDAP: " + Session[SessionVariables.UserName] + "&body=Hello Team,%0D%0DLDAP: " + Session[SessionVariables.UserName] + " has requested access for SimpliFi.%0D%0DThanks%0DSimpliFi' class='link'>Click here</a>.<br><br> Or <br><br>Drop an email to <a style='color:#fff;' href='mailto:gmo_idealabs@adobe.com'>gmo_idealabs@adobe.com</a> with your LDAP.";
                    lblError.Visible = true;
                    pageHeader.InnerText = "Oops!!!";
                }
            }
            else
            {
                pError.Visible = true;
                pageHeader.InnerText = "Oops!!!";
                lblError.Text = "Currently Unable to validate your rights. Please try after sometime.";
                lblError.Visible = true;
                lblError.Focus();
            }
        }

        private bool CheckUserAccess(string LDAP, string password)
        {
            UsageData objUsageData = new UsageData();
            return objUsageData.CheckUserAccess(LDAP, password);
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            Response.Redirect("sso");
        }
    }
}