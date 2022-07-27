using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SimpliFi
{
    public partial class user_list : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Session["userName"] as string))
            {
                string LDAP = Session[SessionVariables.UserName].ToString();

                if (LDAP == "tapkumar" || LDAP == "sukumara" || LDAP == "smisra")
                {
                    if (!IsPostBack)
                    {
                        BindReportList();
                    }
                }
                else
                {
                    Response.Redirect("index");
                }
            }
            else
            {
                Response.Redirect("login");
            }
        }

        private void BindReportList()
        {
            UsageData objUsageData = new UsageData();

            DataSet ds = objUsageData.GetUsers();

            lvUsers.DataSource = ds.Tables[0];
            lvUsers.DataBind();

            if (ds.Tables[0].Rows.Count <= 10)
                (lvUsers.FindControl("DataPager1") as DataPager).Visible = false;
        }

        protected void OnPagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            (lvUsers.FindControl("DataPager1") as DataPager).SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
            BindReportList();
        }


        [WebMethod]
        public static int UpdateUserAccess(string LDAP, int access)
        {
            try
            {
                UsageData objUsageData = new UsageData();

                int id = objUsageData.UpdateUserAccess(LDAP, access);

                return id;
            }
            catch (System.Exception)
            {
                return 0;
            }
        }
    }
}