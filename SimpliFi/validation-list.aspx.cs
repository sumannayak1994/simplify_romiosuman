using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI.WebControls;

namespace SimpliFi
{
    public partial class validation_list : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Session["userName"] as string))
            {
                if (!IsPostBack)
                {
                    BindReportList();
                }
            }
            else
            {
                Response.Redirect("login.aspx");
            }
        }

        private void BindReportList()
        {
            List<ValidationList> lstValidationList = GetValidationList();

            lvProgramList.DataSource = lstValidationList;
            lvProgramList.DataBind();

            if (lstValidationList.Count > 0 && lstValidationList.Count <= 10)
                (lvProgramList.FindControl("DataPager1") as DataPager).Visible = false;
        }

        protected void OnPagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            (lvProgramList.FindControl("DataPager1") as DataPager).SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
            BindReportList();
        }

        private List<ValidationList> GetValidationList()
        {
            string userName = Session["userName"] as string;
            string currPath = string.Empty;
            string currMappedPath = string.Empty;

            currPath = "~/UploadedFiles/" + userName + "/Reports/";
            currMappedPath = Server.MapPath(currPath);

            try
            {
                if (Directory.Exists(currMappedPath))
                {
                    DirectoryInfo d = new DirectoryInfo(currMappedPath);
                    FileInfo[] reportFiles = d.GetFiles("*.pdf");

                    List<ValidationList> lstValidationList = new List<ValidationList>();
                    ValidationList objValidationList;
                    int count = 1;
                    string fileName = string.Empty;

                    reportFiles.All(report =>
                    {
                        objValidationList = new ValidationList();
                        objValidationList.URL = "https://" + Request.Url.Authority + "/UploadedFiles/" + userName + "/Reports/" + report.Name;
                        objValidationList.FilePath = "/UploadedFiles/" + userName + "/Reports/" + report.Name;

                        fileName = report.Name.Replace(".pdf", "");

                        string[] fileParts = fileName.Split(new string[] { "QAed" }, StringSplitOptions.None);

                        if (fileParts.Count() >= 2)
                        {
                            objValidationList.ProgramName = fileParts[0].Replace("_", " ");
                            DateTime dt;

                            try
                            {
                                dt = DateTime.ParseExact(fileParts[1].Replace("_", " ").Trim(), "yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);
                                objValidationList.ValidatedOn = dt.ToString("dddd, dd MMMM yyyy HH:mm:ss");
                                objValidationList.CreatedOn = dt;
                            }
                            catch (Exception)
                            {
                                objValidationList.ValidatedOn = "- NA -";
                            }

                            lstValidationList.Add(objValidationList);
                        }

                        return true;
                    });


                    lstValidationList = lstValidationList.OrderByDescending(x => x.CreatedOn).ToList();

                    lstValidationList.All(x =>
                    {
                        x.No = count++;
                        return true;
                    });

                    return lstValidationList;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [WebMethod]
        public static int DeleteReport(string fileName)
        {
            try
            {
                string currPath = HttpContext.Current.Server.MapPath(fileName);

                if (File.Exists(currPath))
                {
                    File.Delete(currPath);
                }

                return 1;
            }
            catch (System.Exception)
            {
                return 0;
            }
        }
    }
}