using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SimpliFi
{
    public partial class Whitelisted_Urls : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Common.CheckSession())
            {
                if (!IsPostBack)
                {
                    BindReportList();
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

            DataSet ds = objUsageData.GetWhitelistedURLS();

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


    }
}