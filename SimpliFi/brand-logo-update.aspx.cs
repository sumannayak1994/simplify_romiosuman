using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace SimpliFi
{
    public partial class brand_logo_update : System.Web.UI.Page
    {
        StringBuilder sbContent = new StringBuilder();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Common.CheckSession())
            {
                SetFlags();
                BindPBlockListView();
            }
            else
            {
                Response.Redirect("login");
            }
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (flPBlocks.HasFile)
                {
                    string currPath = Common.GetCurrentPath("PBLOCK");
                    string currRelativePath = Server.MapPath(currPath);

                    string getFileName = string.Empty;

                    //Delete all previous files from tht location
                    string[] filePaths;

                    if (!Directory.Exists(currRelativePath))
                        Directory.CreateDirectory(currRelativePath);

                    filePaths = Directory.GetFiles(currRelativePath);

                    foreach (string filePath in filePaths)
                        File.Delete(filePath);

                    foreach (HttpPostedFile htfiles in flPBlocks.PostedFiles)
                    {
                        getFileName = Path.GetFileName(htfiles.FileName);
                        htfiles.SaveAs(Path.Combine(currRelativePath, getFileName));
                    }

                    SetErrorMsg("HTML Files Uploaded successfully.", "success");

                    SetFlags();

                    BindPBlockListView();
                }
                else
                    SetErrorMsg("No PBlock files selected.", "error");
            }
            catch (Exception ex)
            {
                SetErrorMsg("Unable to upload HTML Files.", "error");
            }
        }

        private void BindPBlockListView()
        {
            string userName = Session[SessionVariables.UserName] as string;
            string currPath = Common.GetCurrentPath("PBLOCK");
            string currRelativePath = Server.MapPath(currPath);

            if (Directory.Exists(currRelativePath))
            {
                string[] filePaths = Directory.GetFiles(currRelativePath, "*.html");

                List<PBlockList> listFileNames = new List<PBlockList>();

                PBlockList pblock;
                foreach (string file in filePaths)
                {
                    pblock = new PBlockList();
                    pblock.PblockName = Path.GetFileName(file).Replace(".html", string.Empty);
                    pblock.URL = "http://" + Request.Url.Authority + "/UploadedFiles/" + userName + "/PBLOCK/" + pblock.PblockName;

                    listFileNames.Add(pblock);
                }

                lvPBlocks.DataSource = listFileNames;
                lvPBlocks.DataBind();

                divPBlocks.Visible = true;
                btnImport.Text = "Import Pblocks(" + filePaths.Count() + ")";
            }
        }

        private void SetFlags()
        {
            string pBlockPath = Server.MapPath(Common.GetCurrentPath("PBLOCK"));
            string[] emailFilePaths = null;

            if (Directory.Exists(pBlockPath))
                emailFilePaths = Directory.GetFiles(pBlockPath);

            if (emailFilePaths != null && emailFilePaths.Count() > 0)
                lblCurrEmailCount.Text = emailFilePaths.Count().ToString();
            else
                lblCurrEmailCount.Text = "- NA -";


        }

        private void SetErrorMsg(string msg, string cssClass)
        {
            divMsg.Visible = true;
            lblMsg.Text = msg;
            divMsg.Attributes["class"] = "msg-block m-t-20 " + cssClass;
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            try
            {
                string LDAP = Session[SessionVariables.UserName] as string;

                UsageData user = new UsageData();
                string password = user.GetUserPassword(LDAP);

                GetSessionToken(LDAP, password);

                string sessionToken = Session[SessionVariables.SessionToken] as string;

                if (!string.IsNullOrEmpty(sessionToken))
                {
                    string currPath = Common.GetCurrentPath("PBLOCK");
                    string currRelativePath = Server.MapPath(currPath);

                    if (Directory.Exists(currRelativePath))
                    {
                        Directory.CreateDirectory(currRelativePath);
                    }

                    string pBlockName = string.Empty;

                    string[] filePaths = Directory.GetFiles(currRelativePath, "*.html");
                    string htmlString = string.Empty;

                    List<PBlockList> listFileNames = new List<PBlockList>();

                    PBlockList pblock;

                    foreach (string file in filePaths)
                    {
                        using (FileStream fs = File.Open(file, FileMode.Open, FileAccess.ReadWrite))
                        {
                            using (StreamReader sr = new StreamReader(fs))
                            {
                                htmlString = sr.ReadToEnd();

                                CCTC.includeViewMethodsSoapClient cctcClient = new CCTC.includeViewMethodsSoapClient("includeViewMethodsSoap");

                                pBlockName = Path.GetFileName(file).Replace(".html", string.Empty);

                                string test = cctcClient.createPesonalisationBlock(sessionToken, pBlockName, pBlockName, false, htmlString);

                                if (!string.IsNullOrEmpty(test))
                                {
                                    pblock = new PBlockList();
                                    pblock.PblockName = test.Split(',')[2];
                                    pblock.Status = test.Split(',')[0];
                                    pblock.Action = test.Split(',')[1];

                                    listFileNames.Add(pblock);
                                }
                            }
                        }
                    }
                    BindResultListView(listFileNames);
                }
                else
                {
                    SetErrorMsg("Session Expired. Please login again.", "error");
                }
            }
            catch (Exception ex)
            {
                SetErrorMsg("Unable to connect to service", "error");
            }
        }

        private void BindResultListView(List<PBlockList> listFileNames)
        {
            lstResult.DataSource = listFileNames;
            lstResult.DataBind();

            divResult.Visible = true;
        }

        private void GetSessionToken(string LDAP, string password)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                XmlElement pSessionInfo = doc.CreateElement("FirstName");
                string pSecurityToken = string.Empty;

                SessionService.sessionMethodsSoapClient sessionClient = new SessionService.sessionMethodsSoapClient("sessionMethodsSoap");

                string sessionToken = sessionClient.Logon("", LDAP, Common.Base64Decode(password), pSessionInfo, out pSessionInfo, out pSecurityToken);

                Session[SessionVariables.SessionToken] = sessionToken;
            }
            catch (Exception ex)
            {
                SetErrorMsg("Unable to connect to service", "error");
            }
        }
    }
}