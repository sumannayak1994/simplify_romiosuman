using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace SimpliFi
{
    public partial class usage_report : System.Web.UI.Page
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
                        txtFromDate.Text = DateTime.Now.AddDays(-30).Date.ToString("MM/dd/yyyy");
                        txtToDate.Text = DateTime.Now.AddDays(1).Date.ToString("MM/dd/yyyy");

                        BindListViews();
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

        //private void BindReportList()
        //{
        //    UsageData objUsageData = new UsageData();

        //    DateTime fromDate = DateTime.ParseExact(txtFromDate.Text, "MM/dd/yyyy", CultureInfo.InvariantCulture);
        //    DateTime toDate = DateTime.ParseExact(txtToDate.Text, "MM/dd/yyyy", CultureInfo.InvariantCulture);

        //    DataSet ds = objUsageData.GetUsage(string.Empty, fromDate, toDate, Common.GetProgramName());

        //    lvProgramList.DataSource = ds.Tables[0];
        //    lvProgramList.DataBind();
        //}

        //protected void OnPagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        //{
        //    (lvProgramList.FindControl("DataPager1") as DataPager).SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
        //    BindReportList();
        //}

        private void BindReportListByTypes()
        {
            UsageData objUsageData = new UsageData();

            DateTime fromDate = DateTime.ParseExact(txtFromDate.Text, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            DateTime toDate = DateTime.ParseExact(txtToDate.Text, "MM/dd/yyyy", CultureInfo.InvariantCulture);

            DataSet ds = objUsageData.GetUsageByProgramTypes(fromDate, toDate);

            lvReportByProgramType.DataSource = ds.Tables[0];
            lvReportByProgramType.DataBind();

            if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows.Count <= 10)
            {
                (lvReportByProgramType.FindControl("DataPager1") as DataPager).Visible = false;
            }
        }

        protected void OnPagePropertiesChanging_Types(object sender, PagePropertiesChangingEventArgs e)
        {
            (lvReportByProgramType.FindControl("DataPager1") as DataPager).SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
            BindReportListByTypes();
        }

        private void BindReportListByLDAPS()
        {
            UsageData objUsageData = new UsageData();

            DateTime fromDate = DateTime.ParseExact(txtFromDate.Text, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            DateTime toDate = DateTime.ParseExact(txtToDate.Text, "MM/dd/yyyy", CultureInfo.InvariantCulture);

            DataSet ds = objUsageData.GetUsageByLDAPS(fromDate, toDate);

            lvReportByLDAPS.DataSource = ds.Tables[0];
            lvReportByLDAPS.DataBind();

            if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Rows.Count <= 10)
            {
                (lvReportByLDAPS.FindControl("DataPager1") as DataPager).Visible = false;
            }

        }

        protected void OnPagePropertiesChanging_LDAPS(object sender, PagePropertiesChangingEventArgs e)
        {
            (lvReportByLDAPS.FindControl("DataPager1") as DataPager).SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
            BindReportListByLDAPS();
        }

        protected void lbtnSearch_Click(object sender, EventArgs e)
        {
            BindListViews();
        }

        private void BindListViews()
        {
            BindReportListByLDAPS();
            BindReportListByTypes();
        }
    }
}