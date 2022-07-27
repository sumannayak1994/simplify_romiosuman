using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SimpliFi
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Common.CheckSession())
                {
                    pUserName.InnerHtml = "<span>Hi, </span>" + Session[SessionVariables.FirstName] as string;

                    ShowHideLinks();
                }
                else
                {
                    Response.Redirect("login");
                }
            }

            lblYear.Text = DateTime.Now.Year.ToString();
        }

        private void ShowHideLinks()
        {
            switch (Common.GetProgramName())
            {
                case Programs.B2CTriggered:
                    break;
                case Programs.B2CBatch:
                    //liBatchImportFiles.Visible = true;
                    break;
                case Programs.SophiaTriggered:
                    break;
            }

            if (Session[SessionVariables.UserName].ToString() == "tapkumar"
                || Session[SessionVariables.UserName].ToString() == "sukumara"
                || Session[SessionVariables.UserName].ToString() == "smisra")
                liUsers.Visible = true;

        }

        protected void lnkSignout_Click(object sender, EventArgs e)
        {
            string firstName = Session[SessionVariables.FirstName] as string;

            Session[SessionVariables.UserName] = null;
            Session.Abandon();

            Response.Redirect("login?logout=true&q=" + Common.Base64Encode(firstName));
        }
    }
}