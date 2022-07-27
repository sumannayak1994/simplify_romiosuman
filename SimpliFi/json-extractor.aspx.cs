using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SimpliFi
{
    public partial class json_extractor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Common.CheckSession())
            {
                SetFlags();
            }
            else
            {
                Response.Redirect("login");
            }
        }

        protected void btnDownloadFile_Click(object sender, EventArgs e)
        {
            string currPath = Common.GetCurrentPath("JSON");
            string currRelativePath = Server.MapPath(currPath);

            string[] filePaths = Directory.GetFiles(currRelativePath, "*.json");
            string htmlString = string.Empty;

            DirectoryInfo dInfo = new DirectoryInfo(currRelativePath);
            FileInfo[] files = dInfo.GetFiles("*.json").OrderBy(x => x.Name.PadLeft(200, '0')).ToArray();

            DataTable dtCampaignDetails = new DataTable();
            dtCampaignDetails.Columns.Add("Campaign No", typeof(string));
            dtCampaignDetails.Columns.Add("Campaign Id", typeof(string));
            dtCampaignDetails.Columns.Add("Campaign Name", typeof(string));
            dtCampaignDetails.Columns.Add("Template Name", typeof(string));

            int count = 1;

            foreach (FileInfo file in files)
            {
                JObject obj = JObject.Parse(File.ReadAllText(file.FullName));
                string campaignId = obj["id"].ToString();
                string campaignName = obj["name"].ToString();
                int row = 1;

                IList<JToken> objVariations = obj["variations"].Children().ToList();
                IList<JToken> objActions;
                string templateName = string.Empty;

                foreach (JToken variation in objVariations)
                {
                    objActions = variation["actions"].Children().ToList();

                    foreach (JToken action in objActions)
                    {
                        if ((string)action["type"] == "SendEmail" 
                            || (string)action["type"] == "SendEmail.Instructional" 
                            || (string)action["type"] == "SendEmailNoOptOut"
                            || (string)action["type"] == "SendNPSEmail")
                        {
                            var objTemplate = action["params"]["selectedTemplate"];

                            if (objTemplate != null)
                            {
                                if (row == 1)
                                {
                                    dtCampaignDetails.Rows.Add(count, campaignId, campaignName, (string)objTemplate);
                                    row = 0;
                                }
                                else
                                {
                                    dtCampaignDetails.Rows.Add("","", "", (string)objTemplate);
                                }
                            }
                        }
                    }                   
                }
                //Insert a blank row
                //dtCampaignDetails.Rows.Add("", "", "");
                count++;
            }

            ExportCSV(dtCampaignDetails);
        }

        private void ExportCSV(DataTable dt)
        {
            Response.Clear();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment;filename=CampaignDetails.csv");
            Response.Charset = "";
            Response.ContentType = "application/text";


            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < dt.Columns.Count; k++)
            {
                //add separator
                sb.Append(dt.Columns[k].ColumnName + ',');
            }
            //append new line
            sb.Append("\r\n");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int k = 0; k < dt.Columns.Count; k++)
                {
                    //add separator
                    sb.Append(dt.Rows[i][k].ToString().Replace(",", ";") + ',');
                }
                //append new line
                sb.Append("\r\n");
            }
            Response.Output.Write(sb.ToString());
            Response.Flush();
            Response.End();
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (flJSOFiles.HasFile)
                {
                    string currPath = Common.GetCurrentPath("JSON");
                    string currRelativePath = Server.MapPath(currPath);

                    string getFileName = string.Empty;

                    //Delete all previous files from tht location
                    string[] filePaths;

                    if (!Directory.Exists(currRelativePath))
                        Directory.CreateDirectory(currRelativePath);

                    filePaths = Directory.GetFiles(currRelativePath);

                    foreach (string filePath in filePaths)
                        File.Delete(filePath);

                    foreach (HttpPostedFile htfiles in flJSOFiles.PostedFiles)
                    {
                        getFileName = Path.GetFileName(htfiles.FileName);
                        htfiles.SaveAs(Path.Combine(currRelativePath, getFileName));
                    }

                    SetErrorMsg("JSON Files Uploaded successfully.", "success");

                    SetFlags();
                }
            }
            catch (Exception ex)
            {
                SetErrorMsg("Unable to upload HTML Files.", "error");
            }
        }


        private void SetFlags()
        {
            string pBlockPath = Server.MapPath(Common.GetCurrentPath("JSON"));
            string[] emailFilePaths = null;

            if (Directory.Exists(pBlockPath))
                emailFilePaths = Directory.GetFiles(pBlockPath);

            if (emailFilePaths != null && emailFilePaths.Count() > 0)
                lblJSONCount.Text = emailFilePaths.Count().ToString();
            else
                lblJSONCount.Text = "- NA -";


        }

        private void SetErrorMsg(string msg, string cssClass)
        {
            divMsg.Visible = true;
            lblMsg.Text = msg;
            divMsg.Attributes["class"] = "msg-block m-t-20 " + cssClass;
        }
    }
}