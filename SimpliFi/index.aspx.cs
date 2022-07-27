using Excel;
using HtmlAgilityPack;
using Microsoft.Office.Interop.Outlook;
using MsgReader.Outlook;
using NHunspell;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services;

namespace SimpliFi
{
    public partial class index : System.Web.UI.Page
    {

        StringBuilder sbContent = new StringBuilder();
        DataTable dtFilteredCGEN = new DataTable();
        string programName = string.Empty;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Common.CheckSession())
            {
                if (!IsPostBack)
                {
                    GetCGENDataFromExcel();

                    SetFlags();

                    divMsg.Visible = false;
                }
            }
            else
            {
                Response.Redirect("login");
            }
        }

        private void SetFlags()
        {
            string emailsPath = Server.MapPath(Common.GetCurrentPath("Emails"));
            string cgenPath = Server.MapPath(Common.GetCurrentPath("cgen"));
            string[] emailFilePaths = null;
            string[] cGENFilePaths = null;

            if (Directory.Exists(emailsPath))
                emailFilePaths = Directory.GetFiles(emailsPath);

            if (Directory.Exists(cgenPath))
                cGENFilePaths = Directory.GetFiles(cgenPath);

            if (cGENFilePaths != null && cGENFilePaths.Count() == 1)
                lblCurrCGENName.Text = Path.GetFileName(cGENFilePaths[0]);
            else
                lblCurrCGENName.Text = "- NA -";

            if (emailFilePaths != null && emailFilePaths.Count() > 0)
                lblCurrEmailCount.Text = emailFilePaths.Count().ToString();
            else
                lblCurrEmailCount.Text = "- NA -";

            switch (Common.GetProgramName())
            {
                case Programs.B2CTriggered:
                    lblHeaderProgramText.Text = "Proof Validation [B2C Triggered]";
                    divTriggered.Visible = true;
                    aTriggerCGEN.Visible = true;
                    break;
                case Programs.B2CBatch:
                    lblHeaderProgramText.Text = "Proof Validation [B2C Batch]";
                    divBatch.Visible = true;
                    aBatchCGEN.Visible = true;
                    liSenderName.Visible = true;
                    li250OK.Visible = true;
                    break;
                case Programs.SophiaTriggered:
                    lblHeaderProgramText.Text = "Proof Validation [Sophia Triggered]";
                    divSophia.Visible = true;
                    aTriggerCGEN.Visible = true;
                    break;
                case Programs.APACBNB:
                    lblHeaderProgramText.Text = "Proof Validation [APAC Batch]";
                    divAPAC.Visible = true;
                    aTriggerCGEN.Visible = true;
                    break;
            }
        }

        protected void btnCGENUpload_Click(object sender, EventArgs e)
        {
            try
            {
                string currPath = Common.GetCurrentPath("cgen");
                string extention = string.Empty;
                bool isUploaded = true;

                if (flCGEN.HasFile)
                {
                    bool isClearingNeeded = IsDirectoryClearingNeeded(currPath, "cgen");

                    if (isClearingNeeded)
                    {
                        string getFileName = string.Empty;

                        foreach (HttpPostedFile htfiles in flCGEN.PostedFiles)
                        {
                            if (Path.GetExtension(flCGEN.FileName) == ".xlsx" || Path.GetExtension(flCGEN.FileName) == ".xls")
                            {
                                getFileName = Path.GetFileName(flCGEN.FileName);

                                htfiles.SaveAs(HttpContext.Current.Server.MapPath(currPath + getFileName));

                                isUploaded = true;
                            }
                            else
                            {
                                SetErrorMsg("Please select a .xlsx or .xls file to upload.", "error");
                                isUploaded = false;
                            }

                        }

                        if (isUploaded)
                        {
                            SetErrorMsg(flProofEmails.PostedFiles.Count.ToString() + " CGEN File Uploaded Successfully", "success");

                            SetFlags();

                            GetCGENDataFromExcel();
                        }
                    }
                }
                else
                {
                    SetErrorMsg("Please select CGEN file to upload.", "error");
                }

                divResults.Visible = false;
            }
            catch (System.Exception ex)
            {
                SetErrorMsg(ex.Message, "error", ex);
            }
        }

        private void CheckCGENFormat()
        {
            GetCGENDataFromExcel();
        }

        private bool IsDirectoryClearingNeeded(string cuurPath, string type)
        {
            bool isClearingNeeded = false;
            string currRelativePath = Server.MapPath(cuurPath);

            try
            {
                if (Directory.Exists(currRelativePath))
                {
                    if (type == "emails")
                    {
                        int countOFEmails = Directory.GetFiles(currRelativePath) != null ? Directory.GetFiles(currRelativePath).Count() : 0;

                        if (countOFEmails > 0)
                        {
                            string[] filePaths;
                            filePaths = Directory.GetFiles(currRelativePath);

                            foreach (string filePath in filePaths)
                                File.Delete(filePath);
                        }

                        isClearingNeeded = true;
                    }
                    else if (type == "cgen")
                    {
                        int countOFCGEN = Directory.GetFiles(currRelativePath) != null ? Directory.GetFiles(currRelativePath).Count() : 0;

                        if (countOFCGEN > 0)
                        {
                            string[] filePaths;
                            filePaths = Directory.GetFiles(currRelativePath);

                            foreach (string filePath in filePaths)
                                File.Delete(filePath);
                        }

                        isClearingNeeded = true;
                    }

                }
                else
                {
                    Directory.CreateDirectory(currRelativePath);

                    isClearingNeeded = true;
                }
            }
            catch (System.Exception ex)
            {
                SetErrorMsg(ex.Message, "error", ex);
            }

            return isClearingNeeded;
        }

        protected void btnProofEmailsUpload_Click(object sender, EventArgs e)
        {
            try
            {
                string currEmailPath = Common.GetCurrentPath("Emails");

                if (flProofEmails.HasFile)
                {
                    bool isClearingNeeded = IsDirectoryClearingNeeded(currEmailPath, "emails");

                    if (isClearingNeeded)
                    {
                        string getFileName = string.Empty;

                        foreach (HttpPostedFile htfiles in flProofEmails.PostedFiles)
                        {
                            getFileName = Path.GetFileName(htfiles.FileName);
                            htfiles.SaveAs(HttpContext.Current.Server.MapPath(currEmailPath + getFileName));
                        }
                        SetErrorMsg(flProofEmails.PostedFiles.Count.ToString() + " Email Files Uploaded Successfully", "success");

                        SetFlags();
                    }
                }
                else
                {
                    SetErrorMsg("Please select Email files to upload.", "error");
                }

                divResults.Visible = false;
            }
            catch (System.Exception ex)
            {
                SetErrorMsg(ex.Message, "error", ex);
            }
        }

        public string Clean(string s)
        {
            StringBuilder sb = new StringBuilder(s);

            sb.Replace("&amp;", "&");
            sb.Replace("%20;", " ");
            sb.Replace("&nbsp;", " ");
            sb.Replace("Acrobat DC", "Acrobat DC");
            sb.Replace("Pro DC", "Pro DC");

            return sb.ToString();
        }

        private void GetMasterTableTemplate()
        {
            if (!string.IsNullOrEmpty(Session[SessionVariables.ProgramName] as string))
            {
                string program = Session[SessionVariables.ProgramName] as string;

                switch (Common.GetProgramName())
                {
                    case Programs.B2CTriggered:
                        sbContent.Append(Programs.B2CTriggeredTableHeaders);
                        break;
                    case Programs.B2CBatch:
                        sbContent.Append(Programs.B2CBatchTableHeaders);
                        break;
                    case Programs.SophiaTriggered:
                        sbContent.Append(Programs.SophiaTriggeredTableHeaders);
                        break;
                    case Programs.APACBNB:
                        sbContent.Append(Programs.B2CTriggeredTableHeaders);
                        break;
                }
            }
        }

        protected void btnValidateProofs_Click(object sender, EventArgs e)
        {
            try
            {
                divMsg.Visible = false;
                bool isValidSubjectLine = false;

                if (CheckIfCGenAndEmailFilesAvailableOrNot())
                {
                    // Create new stopwatch.
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();

                    DataTable dtFiltered;
                    dtFilteredCGEN = GetCGENDataFromExcel();

                    string[] filePaths = Directory.GetFiles((Server.MapPath(Common.GetCurrentPath("Emails"))));

                    if (filePaths.Count() > 0)
                    {
                        GetMasterTableTemplate();

                        Array.Sort(filePaths);

                        foreach (string filePath in filePaths)
                        {
                            using (var msg = new Storage.Message(filePath))
                            {
                                EmailDetails objEmailDetails = new EmailDetails();

                                isValidSubjectLine = CheckSubjectLineFormat(Clean(msg.Subject));

                                if (isValidSubjectLine)
                                {

                                    objEmailDetails.Subject = Clean(msg.Subject);
                                    objEmailDetails.BodyHTML = msg.BodyHtml;
                                    objEmailDetails.From = msg.Sender.Email;
                                    objEmailDetails.EmailFromName = msg.Sender.DisplayName;

                                    objEmailDetails = ProcessBodyHTML(objEmailDetails);

                                    try
                                    {
                                        if (Common.GetProgramName() == Programs.APACBNB)
                                        {
                                            dtFiltered = dtFilteredCGEN.Select("[tag purpose] LIKE '%" + objEmailDetails.APACLocale + "%'").CopyToDataTable();

                                        }
                                        else
                                        {
                                            dtFiltered = dtFilteredCGEN.Select("activityid LIKE '%" + objEmailDetails.ActivityId + "%'").CopyToDataTable();
                                        }
                                    }
                                    catch (System.Exception ex)
                                    {
                                        SetErrorMsg("Some Activity Ids are not Present in CGEN.", "error");

                                        dtFiltered = null;
                                    }

                                    switch (Common.GetProgramName())
                                    {
                                        case Programs.B2CTriggered:
                                            objEmailDetails.CGENFromName = "-NA-";
                                            if (dtFiltered != null)
                                            {
                                                objEmailDetails.CGENOK250 = "-NA-";
                                                CompareProofEmailWithCGEN(dtFiltered, objEmailDetails);
                                            }
                                            break;
                                        case Programs.B2CBatch:
                                            objEmailDetails.CGENFromName = dtFilteredCGEN.Rows[0]["Sender Name"].ToString().Trim();
                                            if (dtFiltered != null)
                                            {
                                                objEmailDetails.CGENOK250 = dtFiltered.Rows[0]["250 OK"].ToString().Trim();
                                                CompareProofEmailWithCGEN(dtFiltered, objEmailDetails);
                                            }
                                            break;
                                        case Programs.SophiaTriggered:
                                            objEmailDetails.CGENFromName = "-NA-";
                                            if (dtFiltered != null)
                                            {
                                                objEmailDetails.CGENOK250 = "-NA-";
                                                CompareProofEmailWithCGEN(dtFiltered, objEmailDetails);
                                            }
                                            break;
                                        case Programs.APACBNB:
                                            objEmailDetails.CGENFromName = "-NA-";
                                            if (dtFiltered != null)
                                            {
                                                objEmailDetails.CGENOK250 = "-NA-";
                                                CompareProofEmailWithCGEN(dtFiltered, objEmailDetails);
                                            }
                                            break;
                                    }
                                }
                            }
                        }

                        stopwatch.Stop();

                        if (isValidSubjectLine)
                        {
                            sbContent.Append("</tr></table>");
                            divContent.InnerHtml = sbContent.ToString();

                            lblProgram.Text = dtFilteredCGEN.Rows[0]["program name"].ToString().Trim();
                            lblEmailCounts.Text = filePaths.Count().ToString();
                            lblTime.Text = Math.Ceiling(TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds).TotalSeconds).ToString();

                            divResults.Visible = true;

                            if (!Common.IsLocalHost())
                            {
                                UsageData objUsageData = new UsageData();
                                Usage objUsage = new Usage();

                                objUsage.LDAP = Session[SessionVariables.UserName].ToString();
                                objUsage.ProgramType = Common.GetProgramName();
                                objUsage.ProgramName = lblProgramName.Text.Trim().Replace("Program: ", "");
                                objUsage.ValidatedOn = DateTime.Now;
                                objUsage.ProofsCount = int.Parse(lblCurrEmailCount.Text);
                                //objUsage.HTMLString = sbContent.ToString();
                                objUsage.TimeTaken = int.Parse(lblTime.Text);

                                objUsageData.LogUsage(objUsage);
                            }
                        }
                    }
                    else
                        SetErrorMsg("No Email Files Present.", "error");
                }
            }
            catch (System.Exception ex)
            {
                if (ex.Message.Contains("Cannot find column") || ex.Message.Contains("The source contains no DataRows."))
                    SetErrorMsg("Incorect CGEN Format, To check the correct format click on the info icon.", "error");
                else
                    SetErrorMsg(ex.Message, "error", ex);

                divContent.InnerHtml = string.Empty;
                divResults.Visible = false;
            }
        }

        private bool CheckSubjectLineFormat(string subjectLine)
        {
            bool isValidSubjectLine = false;

            switch (Common.GetProgramName())
            {
                case Programs.B2CBatch:
                    if (subjectLine.Contains("[") && subjectLine.Contains("]"))
                        isValidSubjectLine = true;
                    else
                    {
                        SetErrorMsg("Uploaded Email Subject Line is not in proper format. Please refer info tab.", "error");
                    }
                    break;
                case Programs.B2CTriggered:
                    if (subjectLine.Contains("Wave -") && subjectLine.Contains("| Locale -") && subjectLine.Contains("| SourceCode -"))
                        isValidSubjectLine = true;
                    else
                    {
                        SetErrorMsg("Uploaded Email Subject Line is not in proper format. Please refer info tab.", "error");
                    }
                    break;
                case Programs.SophiaTriggered:
                    if (subjectLine.Contains("Wave -") && subjectLine.Contains("| Locale -") && subjectLine.Contains("| SourceCode -"))
                        isValidSubjectLine = true;
                    else
                    {
                        SetErrorMsg("Uploaded Email Subject Line is not in proper format. Please refer info tab.", "error");
                    }
                    break;
                case Programs.APACBNB:
                    if (subjectLine.Contains("[PROOF_"))
                        isValidSubjectLine = true;
                    else
                    {
                        SetErrorMsg("Uploaded Email Subject Line is not in proper format. Please refer info tab.", "error");
                    }
                    break;
                default:
                    break;
            }

            return isValidSubjectLine;
        }

        private bool CheckIfCGenAndEmailFilesAvailableOrNot()
        {
            bool retVal = true;

            if (lblCurrCGENName.Text.Trim() == "- NA -" && lblCurrEmailCount.Text.Trim() == "- NA -")
            {
                SetErrorMsg("There is no CGEN or Email files Present. Please upload both to proceed.", "error");
                retVal = false;
            }
            else if (lblCurrCGENName.Text.Trim() == "- NA -")
            {
                SetErrorMsg("CGEN File not Present. Please upload one to proceed.", "error");
                retVal = false;
            }
            else if (lblCurrEmailCount.Text.Trim() == "- NA -")
            {
                SetErrorMsg("No Email Files Present. Please upload one to proceed.", "error");
                retVal = false;
            }

            return retVal;
        }

        private void ValidateSubjectLine(StringBuilder sbProof, EmailDetails objEmailDetails, DataTable dtFilteredCGEN)
        {
            try
            {
                string cGENSub = Clean(dtFilteredCGEN.Rows[0]["Subject Line"].ToString().Trim());
                string emailSub = objEmailDetails.Subject.Trim();

                string spellCheckedSubjectLine = SpellCheck(emailSub, objEmailDetails.IsEnglish);
                bool isSubSpellCorrect = spellCheckedSubjectLine.Contains("wrong-spell") ? false : true;


                //If quote is present
                if (CheckSubjectLineForQuote(cGENSub, emailSub))
                {
                    sbProof.Append("<tr class='c-red'><td>Subject</td><td>" + cGENSub +
                            "</td><td>'" + spellCheckedSubjectLine +
                            "'</td><td><strong class='c-red'>Quote Present</strong></td></tr>");

                    if (isSubSpellCorrect)
                        sbContent.Append("<td data-field='subject'><strong class='c-red'><span class='glyphicon glyphicon-remove-circle'></span></strong></td>");
                    else
                        sbContent.Append("<td data-field='subject'><strong class='c-orange'><span class='glyphicon glyphicon-info-sign'></span></strong></td>");
                }
                else
                {
                    //Check for subject Line
                    if (cGENSub == emailSub)
                    {
                        sbProof.Append("<tr><td>Subject</td><td>" + cGENSub +
                            "</td><td>" + spellCheckedSubjectLine +
                            "</td><td><strong class='c-green'>Matched</strong></td></tr>");

                        if (isSubSpellCorrect)
                            sbContent.Append("<td data-field='subject'><strong class='c-green'><span class='glyphicon glyphicon-ok-circle'></span></strong></td>");
                        else
                            sbContent.Append("<td data-field='subject'><strong class='c-orange'><span class='glyphicon glyphicon-info-sign'></span></strong></td>");
                    }
                    else
                    {
                        sbProof.Append("<tr class='c-red'><td>Subject</td><td>" + cGENSub +
                            "</td><td>" + spellCheckedSubjectLine +
                            "</td><td><strong >Not Matched</strong></td></tr>");

                        sbContent.Append("<td data-field='subject'><strong class='c-red'><span class='glyphicon glyphicon-remove-circle'></span></strong></td>");
                    }
                }
            }
            catch (System.Exception ex)
            {
                SetErrorMsg(ex.Message, "error", ex);
            }
        }

        private void ValidatePreheader(StringBuilder sbProof, EmailDetails objEmailDetails, DataTable dtFilteredCGEN)
        {
            try
            {
                objEmailDetails.PreHeader = string.IsNullOrEmpty(objEmailDetails.PreHeader) ? null : objEmailDetails.PreHeader.Trim();
                string cGENPreHeader = dtFilteredCGEN.Rows[0]["Preheader"] != null ? Clean(dtFilteredCGEN.Rows[0]["Preheader"].ToString()).Trim() : string.Empty;

                string spellCheckedPreHeader = SpellCheck(objEmailDetails.PreHeader, objEmailDetails.IsEnglish);
                bool isPreHeaderSpellCorrect = spellCheckedPreHeader.Contains("wrong-spell") ? false : true;

                //Check for preheader
                if (!string.IsNullOrEmpty(cGENPreHeader) && (cGENPreHeader != objEmailDetails.PreHeader))
                {
                    if (objEmailDetails.DecodedHrefTags.Where(x => x.Type == Constants.MirrorPage).Count() > 0)
                    {
                        string mirrorLink = objEmailDetails.DecodedHrefTags.Where(x => x.Type == Constants.MirrorPage).FirstOrDefault().URL;

                        objEmailDetails.PreHeader = GetPreheaderFromMirrorPage(mirrorLink);

                        objEmailDetails.PreHeader = string.IsNullOrEmpty(objEmailDetails.PreHeader) ? null : objEmailDetails.PreHeader.Trim();

                        spellCheckedPreHeader = SpellCheck(objEmailDetails.PreHeader, objEmailDetails.IsEnglish);
                        isPreHeaderSpellCorrect = spellCheckedPreHeader.Contains("wrong-spell") ? false : true;

                        if (cGENPreHeader == objEmailDetails.PreHeader)
                        {
                            sbProof.Append("<tr><td>Preheader</td><td>" + cGENPreHeader +
                               "</td><td>" + spellCheckedPreHeader +
                               "</td><td><strong class='c-green'>Matched</strong></td></tr>");

                            if (isPreHeaderSpellCorrect)
                                sbContent.Append("<td data-field='preheader'><strong class='c-green'><span class='glyphicon glyphicon-ok-circle'></span></strong></td>");
                            else
                                sbContent.Append("<td data-field='preheader'><strong class='c-orange'><span class='glyphicon glyphicon-info-sign'></span></strong></td>");
                        }
                        else
                        {
                            sbProof.Append("<tr class='c-red'><td>Preheader</td><td>" + cGENPreHeader +
                                "</td><td>" + spellCheckedPreHeader +
                                "</td><td><strong >Not Matched</strong></td></tr>");

                            sbContent.Append("<td data-field='preheader'><strong class='c-red'><span class='glyphicon glyphicon-remove-circle'></span></strong></td>");
                        }
                    }
                }
                else if (string.IsNullOrEmpty(cGENPreHeader) && !string.IsNullOrEmpty(objEmailDetails.PreHeader))
                {
                    sbProof.Append("<tr><td>Preheader</td><td> - NA -" +
                              "</td><td>" + spellCheckedPreHeader +
                              "</td><td><strong class='c-green'>Not Applicable</strong></td></tr>");
                    if (isPreHeaderSpellCorrect)
                        sbContent.Append("<td data-field='preheader'><strong class='c-green'><span class='glyphicon glyphicon-ok-circle'></span></strong></td>");
                    else
                        sbContent.Append("<td data-field='preheader'><strong class='c-orange'><span class='glyphicon glyphicon-info-sign'></span></strong></td>");
                }
                else
                {
                    sbProof.Append("<tr><td>Preheader</td><td>" + cGENPreHeader +
                               "</td><td>" + spellCheckedPreHeader +
                               "</td><td><strong class='c-green'>Matched</strong></td></tr>");

                    if (isPreHeaderSpellCorrect)
                        sbContent.Append("<td data-field='preheader'><strong class='c-green'><span class='glyphicon glyphicon-ok-circle'></span></strong></td>");
                    else
                        sbContent.Append("<td data-field='preheader'><strong class='c-orange'><span class='glyphicon glyphicon-info-sign'></span></strong></td>");
                }
            }
            catch (System.Exception ex)
            {
                SetErrorMsg(ex.Message, "error", ex);
            }
        }

        private void ValidateTagIdAndTagUrls(StringBuilder sbProof, EmailDetails objEmailDetails, DataTable dtFilteredCGEN)
        {
            try
            {
                StringBuilder sbtagURLs = new StringBuilder();

                objEmailDetails.DecodedHrefTags.RemoveAll(x => x == null);

                //Get all the tag urls from Email Object and loop on it.  
                //Tag urls from HTML
                List<UrlAndStatusCode> tagUrls = objEmailDetails.DecodedHrefTags.Where(x => x.Type == Constants.TagfulUrl || x.LocalizedAndTrackingUrl == true).ToList();

                tagUrls.All(x =>
                {
                    Uri tagUrl = new Uri(x.URL);
                    x.TagId = HttpUtility.ParseQueryString(tagUrl.Query).Get(ddlTrackingParameter.SelectedValue);
                    return true;
                });

                var lstCGEN = dtFilteredCGEN.AsEnumerable();

                bool isTagIdAndTagUrlMatched = true;

                foreach (var item in lstCGEN)
                {
                    //Get the Tag ID from CGEN Tag URL
                    Uri cgenTagUrl = new Uri(item.Field<string>("tag full url").ToString());
                    string cgenTagId = HttpUtility.ParseQueryString(cgenTagUrl.Query).Get(ddlTrackingParameter.SelectedValue);

                    if (string.IsNullOrEmpty(cgenTagId))
                    {
                        var regex = new Regex(Regex.Escape("&"));
                        var newURL = regex.Replace(cgenTagUrl.ToString(), "?", 1);

                        cgenTagUrl = new Uri(newURL);

                        cgenTagId = HttpUtility.ParseQueryString(cgenTagUrl.Query).Get(ddlTrackingParameter.SelectedValue);
                    }

                    string htmlTagURL = string.Empty;
                    string htmlTagId = string.Empty;

                    //Get the count of current tagId
                    int tagIdCount = (from r in lstCGEN
                                      where r.Field<string>("tagid/ Code") == cgenTagId
                                      select r.Field<string>("tagid/ Code")).Count();

                    //If tagId count is more than 1, then get the item from HTML according to tag URL
                    if (tagIdCount > 1)
                    {
                        UrlAndStatusCode objURL = tagUrls.Where(x => x.URL.Trim() == cgenTagUrl.ToString()).FirstOrDefault();

                        htmlTagURL = objURL != null ? objURL.URL.ToString() : string.Empty;
                        htmlTagId = objURL != null ? objURL.TagId.ToString() : string.Empty;
                    }
                    else
                    {
                        UrlAndStatusCode objURL = tagUrls.Where(x => x.TagId.Trim() == cgenTagId).FirstOrDefault();

                        htmlTagId = objURL != null ? objURL.TagId.ToString() : string.Empty;
                        htmlTagURL = objURL != null ? objURL.URL.ToString() : string.Empty;
                    }

                    string cgenFormatedUrl = HttpUtility.UrlDecode(cgenTagUrl.ToString());

                    if (cgenTagId == htmlTagId)
                    {
                        sbProof.Append("<tr><td>Tag Id</td><td>" + cgenTagId +
                            "</td><td>" + htmlTagId +
                            "</td><td><strong class='c-green'>Matched</strong></td></tr>");

                        if (cgenFormatedUrl == htmlTagURL)
                        {
                            sbtagURLs.Append("<tr><td>Tag URL</td><td><a href='" + cgenFormatedUrl + "' target='_blank'>" + cgenFormatedUrl +
                                "</a></td><td><a href='" + htmlTagURL + "' target='_blank'>" + htmlTagURL +
                                "</a></td><td><strong class='c-green'>Matched</strong></td></tr>");
                        }
                        else if ((GetRedirectedURLAndStatusCode(cgenFormatedUrl) != null && GetRedirectedURLAndStatusCode(cgenFormatedUrl)[0] == htmlTagURL))
                        {
                            sbtagURLs.Append("<tr><td>Tag URL <i class='fa fa-info-circle hand c-orange' title='CGEN URL is redirecting to the proof URL'></i></td><td><a href='" + cgenFormatedUrl + "' target='_blank'>" + cgenFormatedUrl +
                               "</a></td><td><a href='" + htmlTagURL + "' target='_blank'>" + htmlTagURL +
                               "</a></td><td><strong class='c-green'>Matched</strong></td></tr>");
                        }
                        else
                        {
                            htmlTagURL = string.IsNullOrWhiteSpace(htmlTagURL) ? "- NA -" : htmlTagURL;

                            sbtagURLs.Append("<tr class='c-red'><td>Tag URL</td><td><a href='" + cgenFormatedUrl + "' target='_blank'>" + cgenFormatedUrl +
                                "</a></td><td><a href='" + htmlTagURL + "' target='_blank'>" + htmlTagURL +
                                "</a></td><td><strong>Not Matched</strong></td></tr>");

                            isTagIdAndTagUrlMatched = false;
                        }
                    }
                    else
                    {
                        htmlTagId = string.IsNullOrWhiteSpace(htmlTagId) ? "- NA -" : htmlTagId;

                        sbProof.Append("<tr class='c-red'><td>Tag Id</td><td>" + cgenTagId +
                            "</td><td>" + htmlTagId +
                            "</td><td><strong>Not Matched</strong></td></tr>");

                        sbtagURLs.Append("<tr class='c-red'><td>Tag URL</td><td><a href='" + cgenFormatedUrl + "' target='_blank'>" + cgenFormatedUrl +
                            "</a></td><td> - NA -" +
                            "</td><td><strong>Not Matched</strong></td></tr>");

                        isTagIdAndTagUrlMatched = false;
                    }
                }

                // For Tag id matching
                if (isTagIdAndTagUrlMatched)
                {
                    sbContent.Append("<td  data-field='tagid'><strong class='c-green'><span class='glyphicon glyphicon-ok-circle'></span></strong></td>");
                }
                else
                {
                    sbContent.Append("<td data-field='tagid'><strong class='c-red'><span class='glyphicon glyphicon-remove-circle'></span></strong></td>");
                }

                sbProof.Append(sbtagURLs.ToString());
            }
            catch (System.Exception ex)
            {
                SetErrorMsg(ex.Message, "error", ex);
            }
        }

        private void ValidateMirrorPage(StringBuilder sbProof, EmailDetails objEmailDetails)
        {
            try
            {
                string mirrorLink = string.Empty;
                string status = string.Empty;

                if (objEmailDetails.DecodedHrefTags.Where(x => x.Type == Constants.MirrorPage).Count() > 0)
                {
                    mirrorLink = objEmailDetails.DecodedHrefTags.Where(x => x.Type == Constants.MirrorPage).FirstOrDefault().URL;
                    status = objEmailDetails.DecodedHrefTags.Where(x => x.Type == Constants.MirrorPage).FirstOrDefault().Status;
                }

                //For Mirror Page present or not
                if (!string.IsNullOrWhiteSpace(mirrorLink) &&
                    status != Constants.PageErrorMsg)
                {
                    if (!objEmailDetails.DuplicateMirrorPage)
                    {
                        sbProof.Append("<tr><td>Mirror Page</td><td> - NA -" +
                                "</td><td><a  href='" + mirrorLink + "' target='_blank'> " + mirrorLink +
                                "</a></td><td><strong class='c-green'>Page Available.<br/>No Duplicate Page</strong></td></tr>");

                        sbContent.Append("<td data-field='mirror-presence'><strong class='c-green'><span class='glyphicon glyphicon-ok-circle'></span></strong></td>");
                    }
                    else
                    {
                        sbProof.Append("<tr class='c-red'><td>Mirror Page</td><td> - NA -" +
                                 "</td><td><a  href='" + mirrorLink + "' target='_blank'> " + mirrorLink +
                                 "</a></td><td><strong class='c-green'>Page Available.</strong><strong> <br/>Duplicate Page</strong></td></tr>");

                        sbContent.Append("<td data-field='mirror-presence'><strong class='c-red'><span class='glyphicon glyphicon-remove-circle'></span></strong></td>");
                    }
                }
                else if (string.IsNullOrWhiteSpace(mirrorLink))
                {
                    sbProof.Append("<tr class='c-red'><td>Mirror Page</td><td> - NA -" +
                            "</td><td>- NA -" +
                            "</td><td><strong>Not Available</strong></td></tr>");

                    sbContent.Append("<td data-field='mirror-presence'><strong class='c-red'><span class='glyphicon glyphicon-remove-circle'></span></strong></td>");
                }
                else if (status == Constants.PageErrorMsg)
                {
                    sbProof.Append("<tr class='c-red'><td>Mirror Page</td><td> - NA -" +
                            "</td><td><a  href='" + mirrorLink + "' target='_blank'> " + mirrorLink +
                            "</a></td><td><strong>Page Error</strong></td></tr>");

                    sbContent.Append("<td data-field='mirror-presence'><strong class='c-red'><span class='glyphicon glyphicon-remove-circle'></span></strong></td>");
                }
            }
            catch (System.Exception ex)
            {
                SetErrorMsg(ex.Message, "error", ex);
            }
        }

        private void ValidateLinks(StringBuilder sbProof, EmailDetails objEmailDetails)
        {
            try
            {
                //For Link Check
                List<UrlAndStatusCode> erroredURLs = objEmailDetails.DecodedHrefTags.Where(x => x.Status == Constants.PageErrorMsg).ToList();

                if (erroredURLs.Count() == 0)
                {
                    sbProof.Append("<tr><td>Link Check (" + objEmailDetails.DecodedHrefTags.Count() + ")</td><td> - NA - " +
                       "</td><td>Total Links Present: " + objEmailDetails.DecodedHrefTags.Count() +
                       "</td><td><strong class='c-green'>All Valid</strong></td></tr>");

                    sbContent.Append("<td data-field='link-check'><strong class='c-green'><span class='glyphicon glyphicon-ok-circle'></span></strong></td>");
                }
                else
                {
                    sbProof.Append("<tr class='c-red'><td>Link Check (" + objEmailDetails.DecodedHrefTags.Count() + ")</td><td>-</td><td> Total Error Links: " + erroredURLs.Count() + "<br/><br/>");

                    erroredURLs.All(x =>
                    {
                        sbProof.Append("<a href='" + x.URL + "' target='_blank'> " + x.URL + "</a><br/>***************************<br/>");

                        return true;
                    });

                    sbProof.Append("<td><strong >" + erroredURLs.Count() + " Invalid Urls</strong></td></tr>");

                    sbContent.Append("<td data-field='link-check'><strong class='c-red'><span class='glyphicon glyphicon-remove-circle'></span></strong></td>");
                }
            }
            catch (System.Exception ex)
            {
                SetErrorMsg(ex.Message, "error", ex);
            }
        }

        private void ValidateJunkCharacters(StringBuilder sbProof, EmailDetails objEmailDetails)
        {
            try
            {
                //For Junk Char
                if (objEmailDetails.JunkCharacterPresent == "Present")
                {
                    sbProof.Append("<tr class='c-red'><td>Junk Characters</td><td>- NA -" +
                        "</td><td> - NA - " +
                        "</td><td><strong>" + objEmailDetails.JunkCharacterPresent + "</strong></td></tr>");

                    sbContent.Append("<td data-field='junk'><strong class='c-red'><span class='glyphicon glyphicon-remove-circle'></span></strong></td>");
                }
                else
                {
                    sbProof.Append("<tr><td>Junk Characters</td><td> - NA - " +
                        "</td><td> - NA - " +
                        "</td><td><strong class='c-green'>" + objEmailDetails.JunkCharacterPresent + "</strong></td></tr>");

                    sbContent.Append("<td data-field='junk'><strong class='c-green'><span class='glyphicon glyphicon-ok-circle'></span></strong></td>");

                }
            }
            catch (System.Exception ex)
            {
                SetErrorMsg(ex.Message, "error", ex);
            }
        }

        private void ValidateUnsubLink(StringBuilder sbProof, EmailDetails objEmailDetails)
        {
            try
            {
                //For Unsub link present or not
                if (objEmailDetails.DecodedHrefTags.Where(x => x.Type == Constants.UnsubscribePage).Count() > 0)
                {
                    string unsubLink = objEmailDetails.DecodedHrefTags.Where(x => x.Type == Constants.UnsubscribePage).FirstOrDefault().URL;

                    sbProof.Append("<tr><td>Unsub Link</td><td><a  href='" + unsubLink + "' target='_blank'> " + unsubLink +
                            "</a></td><td><strong class='c-green'>Link Present</strong></td></tr>");

                    sbContent.Append("<td data-field='unsublink-presence'><strong class='c-green'> <span class='glyphicon glyphicon-ok-circle'></span></strong></td>");
                }
                else
                {
                    sbProof.Append("<tr class='c-red'><td>Unsub Link</td><td> - NA -" +
                            "</td><td><strong>Link Not Present</strong></td></tr>");

                    sbContent.Append("<td data-field='unsublink-presence'><strong class='c-red'><span class='glyphicon glyphicon-remove-circle'></span></strong></td>");
                }
            }
            catch (System.Exception ex)
            {
                SetErrorMsg(ex.Message, "error", ex);
            }
        }

        private void ValidateLocale(StringBuilder sbProof, EmailDetails objEmailDetails)
        {
            try
            {
                List<UrlAndStatusCode> localizedURLs = objEmailDetails.DecodedHrefTags.Where(x => x.LocalizedUrl == true).ToList();
                string currUrl;
                bool localeCheckForPrivacy = false;
                bool localeCheckForContact = false;
                bool localeCheckForUnsub = false;
                bool localeCheckForLegal = false;
                bool localeCheckForImpressum = false;

                localizedURLs.All(x =>
                {
                    currUrl = x.URL.ToLower();

                    if (x.Type == Constants.PrivacyPage && (currUrl.Contains("/" + objEmailDetails.EmailLocale.ToLower() + "/")
                        || currUrl.Contains("/" + objEmailDetails.WebLocale1.ToLower() + "/")
                        || currUrl.Contains("/" + objEmailDetails.WebLocale2.ToLower() + "/")
                        || ((objEmailDetails.EmailLocale == "NAM")
                        && (currUrl == Constants.NAMPrivacyPage || currUrl == Constants.NAMPrivacyPageHTTP))))
                    {
                        localeCheckForPrivacy = true;

                        sbProof.Append("<tr><td>Privacy Page<br/> (" + objEmailDetails.EmailLocale + ")</td><td><a  href='" + currUrl + "' target='_blank'> " + currUrl +
                           "</a></td><td><strong class='c-green'>Page Available,<br/>Locale Matched</strong></td></tr>");
                    }
                    else if (x.Type == Constants.ContactPage && (currUrl.Contains("/" + objEmailDetails.EmailLocale.ToLower() + "/")
                            || currUrl.Contains("/" + objEmailDetails.WebLocale1.ToLower() + "/")
                            || currUrl.Contains("/" + objEmailDetails.WebLocale2.ToLower() + "/")
                             || ((objEmailDetails.EmailLocale == "NAM")
                        && (currUrl == Constants.NAMContactPage || currUrl == Constants.NAMContactPageHTTP))))
                    {
                        localeCheckForContact = true;

                        sbProof.Append("<tr><td>Contact Page<br/> (" + objEmailDetails.EmailLocale + ")</td><td><a  href='" + currUrl + "' target='_blank'> " + currUrl +
                           "</a></td><td><strong class='c-green'>Page Available,<br/>Locale Matched</strong></td></tr>");
                    }
                    else if (x.Type == Constants.UnsubscribePage &&
                            ((currUrl.Contains("/" + objEmailDetails.EmailLocale.ToLower() + "/")
                            || currUrl.Contains("/" + objEmailDetails.WebLocale1.ToLower() + "/")
                            || currUrl.Contains("/" + objEmailDetails.WebLocale2.ToLower() + "/")) ||
                            ((string.IsNullOrEmpty(objEmailDetails.WebLocale2) || string.IsNullOrEmpty(objEmailDetails.WebLocale1) || string.IsNullOrEmpty(objEmailDetails.EmailLocale)))))
                    {
                        localeCheckForUnsub = true;

                        sbProof.Append("<tr><td>Unsub Page<br/> (" + objEmailDetails.EmailLocale + ")</td><td><a  href='" + currUrl + "' target='_blank'> " + currUrl +
                           "</a></td><td><strong class='c-green'>Page Available,<br/>Locale Matched</strong></td></tr>");
                    }
                    else if (x.Type == Constants.LegalPage && (currUrl.Contains("/" + objEmailDetails.EmailLocale.ToLower() + "/")
                            || currUrl.Contains("/" + objEmailDetails.WebLocale1.ToLower() + "/")
                            || currUrl.Contains("/" + objEmailDetails.WebLocale2.ToLower() + "/")
                             || ((objEmailDetails.EmailLocale == "NAM")
                        && (currUrl == Constants.NAMLegalPage || currUrl == Constants.NAMLegalPageHTTP))))
                    {
                        localeCheckForLegal = true;

                        sbProof.Append("<tr><td>Legal Page<br/> (" + objEmailDetails.EmailLocale + ") - </td><td><a  href='" + currUrl + "' target='_blank'> " + currUrl +
                           "</a></td><td><strong class='c-green'>Page Available,<br/>Locale Matched</strong></td></tr>");
                    }
                    else if (x.Type == Constants.ImpressumPage && (currUrl.Contains("/" + objEmailDetails.EmailLocale.ToLower() + "/")
                            || currUrl.Contains("/" + objEmailDetails.WebLocale1.ToLower() + "/")
                            || currUrl.Contains("/" + objEmailDetails.WebLocale2.ToLower() + "/")
                             || ((objEmailDetails.EmailLocale == "NAM"))))
                    {
                        localeCheckForImpressum = true;

                        sbProof.Append("<tr><td>Impressum Page</br> (" + objEmailDetails.EmailLocale + ")</td><td><a  href='" + currUrl + "' target='_blank'> " + currUrl +
                           "</a></td><td><strong class='c-green'>Page Available,<br/>Locale Matched</strong></td></tr>");
                    }
                    return true;
                });

                if (localizedURLs.Where(y => y.Type == Constants.PrivacyPage).ToList().Count() == 0)
                    localeCheckForPrivacy = true;
                if (localizedURLs.Where(y => y.Type == Constants.ContactPage).ToList().Count() == 0)
                    localeCheckForContact = true;
                if (localizedURLs.Where(y => y.Type == Constants.LegalPage).ToList().Count() == 0)
                    localeCheckForLegal = true;
                if (localizedURLs.Where(y => y.Type == Constants.ImpressumPage).ToList().Count() == 0)
                    localeCheckForImpressum = true;


                if (localeCheckForUnsub && localeCheckForPrivacy && localeCheckForContact && localeCheckForLegal && localeCheckForImpressum)
                {
                    sbContent.Append("<td data-field='local-check'><strong class='c-green'><span class='glyphicon glyphicon-ok-circle'></span></strong></td>");
                }
                else
                {
                    sbContent.Append("<td data-field='local-check'><strong class='c-red'><span class='glyphicon glyphicon-remove-circle'></span></strong></td>");
                }

                if (!localeCheckForPrivacy && localizedURLs.Where(x => x.Type == Constants.PrivacyPage).Count() > 0)
                {
                    string currPage = localizedURLs.Where(x => x.Type == Constants.PrivacyPage).FirstOrDefault().URL;

                    sbProof.Append("<tr class='c-red'><td>Locale Check (" + objEmailDetails.EmailLocale + ") - Privacy Page</td><td><a href='" + currPage + "' target='_blank'> " + currPage +
                           "</a></td><td><strong class='c-green'>Page Available,</strong><br/><strong>Locale Not Matched</strong></td></tr>");
                }

                if (localizedURLs.Where(x => x.Type == Constants.UnsubscribePage).Count() > 0 && !localeCheckForUnsub)
                {
                    string currPage = localizedURLs.Where(x => x.Type == Constants.UnsubscribePage).FirstOrDefault().URL;

                    sbProof.Append("<tr class='c-red'><td>Locale Check (" + objEmailDetails.EmailLocale + ") - Unsub Page</td><td><a  href='" + currPage + "' target='_blank'> " + currPage +
                           "</a></td><td><strong class='c-green'>Page Available,</strong><br/><strong>Locale Not Matched</strong></td></tr>");
                }

                if (localizedURLs.Where(x => x.Type == Constants.ContactPage).Count() > 0 && !localeCheckForContact)
                {
                    string currPage = localizedURLs.Where(x => x.Type == Constants.ContactPage).FirstOrDefault().URL;

                    sbProof.Append("<tr class='c-red'><td>Locale Check (" + objEmailDetails.EmailLocale + ") - Contact Page</td><td><a  href='" + currPage + "' target='_blank'> " + currPage +
                           "</a></td><td><strong class='c-green'>Page Available,</strong><br/><strong>Locale Not Matched</strong></td></tr>");
                }

                if (localizedURLs.Where(x => x.Type == Constants.LegalPage).Count() > 0 && !localeCheckForLegal)
                {
                    string currPage = localizedURLs.Where(x => x.Type == Constants.LegalPage).FirstOrDefault().URL;

                    sbProof.Append("<tr class='c-red'><td>Legal Page<br/> (" + objEmailDetails.EmailLocale + ")</td><td><a  href='" + currPage + "' target='_blank'> " + currPage +
                           "</a></td><td><strong class='c-green'>Page Available,</strong><br/><strong>Locale Not Matched</strong></td></tr>");
                }

                if (localizedURLs.Where(x => x.Type == Constants.ImpressumPage).Count() > 0 && !localeCheckForImpressum)
                {
                    string currPage = localizedURLs.Where(x => x.Type == Constants.ContactPage).FirstOrDefault().URL;

                    sbProof.Append("<tr class='c-red'><td>Impressum Page<br/> (" + objEmailDetails.EmailLocale + ")</td><td><a  href='" + currPage + "' target='_blank'> " + currPage +
                           "</a></td><td><strong class='c-green'>Page Available,</strong><br/><strong>Locale Not Matched</strong></td></tr>");
                }
            }
            catch (System.Exception ex)
            {
                SetErrorMsg(ex.Message, "error", ex);
            }
        }

        private void ValidateCopyright(StringBuilder sbProof, EmailDetails objEmailDetails)
        {
            if (objEmailDetails.CopyrightCheck == "Not Present")
            {
                sbProof.Append("<tr><td>Copyright Line</td><td>© " + DateTime.Now.Year + " Adobe Systems Incorporated.</td><td><strong class='c-green'>Not Present</strong></td></tr>");

                sbContent.Append("<td data-field='copyright-presence'><strong class='c-green'> <span class='glyphicon glyphicon-ok-circle'></span></strong></td>");
            }
            else if (objEmailDetails.CopyrightCheck == "Present")
            {
                sbProof.Append("<tr class='c-red'><td>Copyright Line</td><td>© " + DateTime.Now.Year + " Adobe.</td><td><strong>Present</strong></td></tr>");

                sbContent.Append("<td data-field='copyright-presence'><strong class='c-red'> <span class='glyphicon glyphicon-remove-circle'></span></strong></td>");
            }
        }

        private void ValidateSenderBook(StringBuilder sbProof, EmailDetails objEmailDetails)
        {
            if (objEmailDetails.SenderBookAddress == "Not Present")
            {
                sbProof.Append("<tr class='c-red'><td>Sender Address Book</td><td>From Address: " + objEmailDetails.From
                    + " <br/> Address in Email: - NA -</td><td><strong>Not Present</strong></td></tr>");

                sbContent.Append("<td data-field='senderbook-presence'><strong class='c-red'> <span class='glyphicon glyphicon-remove-circle'></span></strong></td>");
            }
            else if (objEmailDetails.SenderBookAddress == "Present")
            {
                sbProof.Append("<tr><td>Sender Address Book</td><td>From Address: " + objEmailDetails.From + "<br/>Address in Email: " + objEmailDetails.From + "</td><td><strong class='c-green'>Present</strong></td></tr>");

                sbContent.Append("<td data-field='senderbook-presence'><strong class='c-green'> <span class='glyphicon glyphicon-ok-circle'></span></strong></td>");
            }
        }

        private void ValidateSenderName(StringBuilder sbProof, EmailDetails objEmailDetails)
        {
            try
            {
                if (Common.GetProgramName() == Programs.B2CBatch)
                {
                    if (!string.IsNullOrEmpty(objEmailDetails.CGENFromName))
                    {
                        //For Sender Name
                        if (objEmailDetails.EmailFromName.Trim() == objEmailDetails.CGENFromName.Trim())
                        {
                            sbProof.Append("<tr><td>Sender Name</td><td>" + objEmailDetails.CGENFromName.Trim() +
                                "</td><td>" + objEmailDetails.EmailFromName.Trim() +
                                "</td><td><strong class='c-green'>Matched</strong></td></tr>");

                            sbContent.Append("<td data-field='sender-name'><strong class='c-green'><span class='glyphicon glyphicon-ok-circle'></span></strong></td>");
                        }
                        else
                        {
                            sbProof.Append("<tr class='c-red'><td>Sender Name</td><td>" + objEmailDetails.CGENFromName.Trim() +
                                "</td><td>" + objEmailDetails.EmailFromName.Trim() +
                                "</td><td><strong class='c-red'>Not Matched</strong></td></tr>");

                            sbContent.Append("<td data-field='sender-name'><strong class='c-red'><span class='glyphicon glyphicon-remove-circle'></span></strong></td>");
                        }
                    }
                    else
                    {
                        sbProof.Append("<tr><td>Sender Name</td><td>- NA -" +
                                 "</td><td>" + objEmailDetails.EmailFromName.Trim() +
                                 "</td><td><strong class='c-green'>Optional</strong></td></tr>");

                        sbContent.Append("<td data-field='sender-name'><strong class='c-green'><span class='glyphicon glyphicon-ok-circle'></span></strong></td>");
                    }
                }
            }
            catch (System.Exception ex)
            {
                SetErrorMsg(ex.Message, "error", ex);
            }
        }

        private void Validate250OK(StringBuilder sbProof, EmailDetails objEmailDetails)
        {
            try
            {
                if (Common.GetProgramName() == Programs.B2CBatch)
                {
                    //For Sender Name
                    if (!string.IsNullOrEmpty(objEmailDetails.CGENOK250.Trim()))
                    {
                        if (objEmailDetails.EmailOK250.Trim() == objEmailDetails.CGENOK250.Trim())
                        {
                            sbProof.Append("<tr><td>250 OK</td><td>" + objEmailDetails.CGENOK250.Trim() +
                                "</td><td>" + objEmailDetails.EmailOK250.Trim() +
                                "</td><td><strong class='c-green'>Matched</strong></td></tr>");

                            sbContent.Append("<td data-field='250-ok'><strong class='c-green'><span class='glyphicon glyphicon-ok-circle'></span></strong></td>");
                        }
                        else if (!string.IsNullOrEmpty(objEmailDetails.EmailOK250))
                        {
                            sbProof.Append("<tr class='c-red'><td>250 OK</td><td>" + objEmailDetails.CGENOK250.Trim() +
                                "</td><td>" + objEmailDetails.EmailOK250.Trim() +
                                "</td><td><strong class='c-red'>Not Matched</strong></td></tr>");

                            sbContent.Append("<td data-field='250-ok'><strong class='c-red'><span class='glyphicon glyphicon-remove-circle'></span></strong></td>");
                        }
                        else if (string.IsNullOrEmpty(objEmailDetails.EmailOK250))
                        {
                            string OK250 = !string.IsNullOrEmpty(objEmailDetails.EmailOK250.Trim()) ? objEmailDetails.EmailOK250.Trim() : "- NA -";

                            sbProof.Append("<tr class='c-red'><td>250 OK</td><td>" + objEmailDetails.CGENOK250.Trim() +
                               "</td><td>" + OK250 +
                               "</td><td><strong class='c-red'>Not Matched</strong></td></tr>");

                            sbContent.Append("<td data-field='250-ok'><strong class='c-red'><span class='glyphicon glyphicon-remove-circle'></span></strong></td>");
                        }
                    }
                    else
                    {
                        string OK250 = !string.IsNullOrEmpty(objEmailDetails.EmailOK250.Trim()) ? objEmailDetails.EmailOK250.Trim() : "- NA -";

                        sbProof.Append("<tr><td>250 OK</td><td>- NA -" +
                           "</td><td>" + OK250 +
                           "</td><td><strong class='c-green'>Optional</strong></td></tr>");

                        sbContent.Append("<td data-field='250-ok'><strong class='c-green'><span class='glyphicon glyphicon-ok-circle'></span></strong></td>");
                    }
                }
            }
            catch (System.Exception ex)
            {
                SetErrorMsg(ex.Message, "error", ex);
            }
        }

        private bool CheckSubjectLineForQuote(string cGENSub, string emailSub)
        {
            if (cGENSub.StartsWith("\"") || emailSub.StartsWith("\""))
            {
                return true;
            }
            else
                return false;
        }

        private string SpellCheck(string textStringToValidate, bool isEnglish)
        {
            StringBuilder sbText = new StringBuilder();

            if (isEnglish && rdSpellCheckTrue.Checked)
            {
                using (Hunspell hunspell = new Hunspell("en_us.aff", "en_us.dic"))
                {
                    string[] arrWords = textStringToValidate.Split(' ');
                    bool correct;
                    string punctuation = string.Empty;
                    string trimedWord = string.Empty;
                    string oldWord = string.Empty;

                    arrWords.All(word =>
                    {
                        if (word.Contains(","))
                        {
                            oldWord = word;
                            trimedWord = word.Replace(",", "");
                            punctuation = ",";
                        }
                        else if (word.Contains("!"))
                        {
                            oldWord = word;
                            trimedWord = word.Replace("!", "");
                            punctuation = "!";
                        }
                        else if (word.Contains("?"))
                        {
                            oldWord = word;
                            trimedWord = word.Replace("?", "");
                            punctuation = "?";
                        }
                        else if (word.Contains("."))
                        {
                            oldWord = word;
                            trimedWord = word.Replace(".", "");
                            punctuation = ".";
                        }
                        else if (word.Contains("-"))
                        {
                            oldWord = word;
                            trimedWord = word.Replace("-", "");
                            punctuation = "-";
                        }
                        else
                        {
                            trimedWord = word;
                            oldWord = word;
                            punctuation = string.Empty;
                        }

                        correct = hunspell.Spell(trimedWord);

                        if (correct)
                            sbText.Append(trimedWord + punctuation + " ");
                        else
                            sbText.Append("<span class='wrong-spell'>" + oldWord + "</span> ");

                        return true;
                    });

                    return sbText.ToString();
                }
            }
            else
                return textStringToValidate;
        }

        private void ListOutLinks(StringBuilder sbProof, EmailDetails objEmailDetails)
        {
            sbProof.Append("<strong class='c-blue'>Link Check</strong><table id='table-two-axis' class='customers m-t-10'><tr><th style='width:10%;'>Link Type</th><th style='width:75%;'>Link Details</th><th style='width:15%;'>Remark</th></tr>");

            objEmailDetails.DecodedHrefTags.OrderBy(x => x.Type).All(currUrl =>
            {
                if (currUrl.Status != Constants.PageErrorMsg)
                    sbProof.Append("<tr><td>" + currUrl.Type + "</td>" +
                                   "<td><a href='" + currUrl.URL + "' target='_blank'>" + currUrl.URL + "</a></td><td><strong class='c-green'>" + currUrl.Status + "</strong></td></tr>");
                else
                    sbProof.Append("<tr class='c-red'><td>" + currUrl.Type + "</td>" +
                                  "<td><a href='" + currUrl.URL + "' target='_blank'>" + currUrl.URL + "</a></td><td><strong>" + currUrl.Status + "</strong></td></tr>");

                return true;
            });
        }

        private string GetMirrorLink(EmailDetails objEmailDetails)
        {
            string mirrorLink = "#";

            if (objEmailDetails.DecodedHrefTags.Where(x => x.Type == Constants.MirrorPage).Count() > 0)
            {
                mirrorLink = objEmailDetails.DecodedHrefTags.Where(x => x.Type == Constants.MirrorPage).FirstOrDefault().URL;
            }

            return mirrorLink;
        }

        private void InitializeHTMLForB2CBatch(StringBuilder sbProof, string locale, EmailDetails objEmailDetails)
        {
            sbContent.Append("<tr>" +
              "<td><strong class='p-block'><a target='_blank' title='Click for Mirror Page' href='" + GetMirrorLink(objEmailDetails) + "'>" + objEmailDetails.ActivityId + "</a></strong></td>");

            sbProof.Append("<div class='div-pop-details p-l-0 b-r-2 '><strong>Source Code: <span>" + objEmailDetails.ActivityId + "</span> </strong> </div>");

            sbProof.Append("<strong class='c-blue'>Common Check</strong><table id='table-two-axis' class='customers m-t-10'><tr><th style='width:10%;'></th><th style='width:40%;'>CGEN Details</th><th style='width:40%;'>Proof Details</th><th style='width:10%;'>Remark</th></tr>");

        }

        private void InitializeHTMLForB2CTriggered(StringBuilder sbProof, string locale, EmailDetails objEmailDetails)
        {
            sbContent.Append("<tr>" +
          "<td><strong class='p-block'><a target='_blank' title='Click for Mirror Page' href='" + GetMirrorLink(objEmailDetails) + "'>" + objEmailDetails.ActivityId + " - Day " + objEmailDetails.WaveNo + " - " + locale + "</a></strong></td>");


            sbProof.Append("<div class='div-pop-details p-l-0 b-r-2 '><strong>Source Code: <span>" + objEmailDetails.ActivityId + "</span> &nbsp; &nbsp; " +
                "Day: <span>" + objEmailDetails.WaveNo + "</span>&nbsp; &nbsp;" +
                "Locale: <span >" + locale + "</span></strong> </div>");

            sbProof.Append("<strong class='c-blue'>Common Check</strong><table id='table-two-axis' class='customers m-t-10'><tr><th style='width:10%;'></th><th style='width:40%;'>CGEN Details</th><th style='width:40%;'>Proof Details</th><th style='width:10%;'>Remark</th></tr>");

        }

        private void InitializeHTMLForSophiaTriggered(StringBuilder sbProof, string locale, EmailDetails objEmailDetails)
        {
            sbContent.Append("<tr>" +
              "<td><strong class='p-block'><a target='_blank' title='Click for Mirror Page' href='" + GetMirrorLink(objEmailDetails) + "'>" + objEmailDetails.ActivityId + " - " + locale + "</a></strong></td>");

            sbProof.Append("<div class='div-pop-details p-l-0 b-r-2 '><strong>Source Code: <span>" + objEmailDetails.ActivityId + "</span> &nbsp; &nbsp; " + "</span>&nbsp; &nbsp;" +
                "Locale: <span >" + locale + "</span></strong> </div>");

            sbProof.Append("<strong class='c-blue'>Common Check</strong><table id='table-two-axis' class='customers m-t-10'><tr><th style='width:10%;'></th><th style='width:40%;'>CGEN Details</th><th style='width:40%;'>Proof Details</th><th style='width:10%;'>Remark</th></tr>");

        }

        private void InitializeHTMLForAPACBNB(StringBuilder sbProof, string locale, EmailDetails objEmailDetails)
        {
            sbContent.Append("<tr>" +
          "<td><strong class='p-block'><a target='_blank' title='Click for Mirror Page' href='" + GetMirrorLink(objEmailDetails) + "'>Locale: " + locale + "</a></strong></td>");

            sbProof.Append("<div class='div-pop-details p-l-0 b-r-2 '><strong>Email Details: " +
                "<span >" + locale + "</span></strong> </div>");

            sbProof.Append("<strong class='c-blue'>Common Check</strong><table id='table-two-axis' class='customers m-t-10'><tr><th style='width:10%;'></th><th style='width:40%;'>CGEN Details</th><th style='width:40%;'>Proof Details</th><th style='width:10%;'>Remark</th></tr>");

        }

        private void CompareProofEmailWithCGEN(DataTable dtFilteredCGEN, EmailDetails objEmailDetails)
        {
            StringBuilder sbProof = new StringBuilder();
            string locale = objEmailDetails.EmailLocale.Contains("_") ? objEmailDetails.EmailLocale : objEmailDetails.EmailLocale.ToUpper();

            switch (Common.GetProgramName())
            {
                case Programs.B2CTriggered:
                    InitializeHTMLForB2CTriggered(sbProof, locale, objEmailDetails);
                    break;
                case Programs.B2CBatch:
                    InitializeHTMLForB2CBatch(sbProof, locale, objEmailDetails);
                    break;
                case Programs.SophiaTriggered:
                    InitializeHTMLForSophiaTriggered(sbProof, locale, objEmailDetails);
                    break;
                case Programs.APACBNB:
                    InitializeHTMLForAPACBNB(sbProof, locale, objEmailDetails);
                    break;
            }

            if (dtFilteredCGEN.Rows.Count > 0)
            {
                ValidateSubjectLine(sbProof, objEmailDetails, dtFilteredCGEN);

                ValidatePreheader(sbProof, objEmailDetails, dtFilteredCGEN);

                ValidateTagIdAndTagUrls(sbProof, objEmailDetails, dtFilteredCGEN);

                ValidateMirrorPage(sbProof, objEmailDetails);

                ValidateLinks(sbProof, objEmailDetails);

                //ValidateJunkCharacters(sbProof, objEmailDetails);

                ValidateSenderName(sbProof, objEmailDetails);

                Validate250OK(sbProof, objEmailDetails);

                sbProof.Append("</table> <br/><br/>");

                //Legal Footer Check
                sbProof.Append("<strong class='c-blue'>Legal Footer Check</strong><table id='table-two-axis' class='customers m-t-10'><tr><th style='width:10%;'></th><th style='width:75%;'>Proof Details</th><th style='width:15%;'>Remark</th></tr>");

                ValidateUnsubLink(sbProof, objEmailDetails);

                ValidateLocale(sbProof, objEmailDetails);

                ValidateCopyright(sbProof, objEmailDetails);

                ValidateSenderBook(sbProof, objEmailDetails);

                sbProof.Append("</table><br/><br/>");

                ListOutLinks(sbProof, objEmailDetails);

                sbProof.Append("</table>");

                string finalDetailsHTML = Regex.Replace(sbProof.ToString(), @"[\""]", "", RegexOptions.None);

                //For Details            
                sbContent.Append("<td class='details-td' data-details=\"" + finalDetailsHTML + "\"><a href='#' class='spDetails' title='View Details'><span class='fa fa-list-alt f-20'></span></a></td>");
            }
            else
            {
                sbProof.Append("No records found in CGEN file for Activity ID:" + objEmailDetails.ActivityId);
                sbContent.Append(sbProof.ToString());
            }
        }

        public bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        private DataTable GetCGENDataFromExcel()
        {
            string currPath = Common.GetCurrentPath("cgen");

            try
            {
                bool isDirectoryPresent = Directory.Exists(Server.MapPath(currPath));
                bool isDirectoryEmpty = isDirectoryPresent ? IsDirectoryEmpty(Server.MapPath(currPath)) : false;

                if (isDirectoryPresent && !isDirectoryEmpty)
                {
                    string[] filePaths = Directory.GetFiles(Server.MapPath(currPath));

                    string extension = Path.GetExtension(filePaths[0]);
                    FileStream streamOpen = File.Open(filePaths[0], FileMode.Open, FileAccess.Read);

                    IExcelDataReader excelReader = null;
                    switch (extension.ToLower())
                    {
                        case ".xls":
                            //Excel 97-03
                            excelReader = ExcelReaderFactory.CreateBinaryReader(streamOpen);
                            break;
                        case ".xlsx":
                            //Excel 07
                            excelReader = ExcelReaderFactory.CreateOpenXmlReader(streamOpen);
                            break;
                    }
                    excelReader.IsFirstRowAsColumnNames = true;
                    DataSet dsResult = excelReader.AsDataSet();

                    if (dsResult.Tables.Count > 0)
                    {
                        dtFilteredCGEN = dsResult.Tables[0];

                        if (CheckIfAllRequiredColumnsPresentInCGEN(dtFilteredCGEN))
                            lblProgramName.Text = "Program: " + dtFilteredCGEN.Rows[0]["program name"].ToString().Trim();
                        else
                            SetErrorMsg("Incorect CGEN Format, To check the correct format click on the info icon.", "error");
                    }
                    else
                        SetErrorMsg("Unable to process CGEN File", "error");
                }
                else
                    SetErrorMsg("No CGEN File Present.", "error");

            }
            catch (System.Exception ex)
            {

                SetErrorMsg(ex.Message, "error", ex);
            }

            return dtFilteredCGEN;
        }

        private bool CheckIfAllRequiredColumnsPresentInCGEN(DataTable dtFilteredCGEN)
        {
            bool isValid = false;


            switch (Common.GetProgramName())
            {
                case Programs.B2CTriggered:
                    if (dtFilteredCGEN.Columns.Contains("program name") && dtFilteredCGEN.Columns.Contains("activityid") &&
                           dtFilteredCGEN.Columns.Contains("tagid/ Code") && dtFilteredCGEN.Columns.Contains("tag full url") &&
                           dtFilteredCGEN.Columns.Contains("Subject Line") && dtFilteredCGEN.Columns.Contains("Preheader"))
                        isValid = true;
                    else
                        isValid = false;
                    break;
                case Programs.B2CBatch:
                    if (dtFilteredCGEN.Columns.Contains("program name") && dtFilteredCGEN.Columns.Contains("activityid") &&
                           dtFilteredCGEN.Columns.Contains("tagid/ Code") && dtFilteredCGEN.Columns.Contains("tag full url") &&
                           dtFilteredCGEN.Columns.Contains("Subject Line") && dtFilteredCGEN.Columns.Contains("Preheader") &&
                           dtFilteredCGEN.Columns.Contains("Sender Name") && dtFilteredCGEN.Columns.Contains("250 OK"))
                        isValid = true;
                    else
                        isValid = false;
                    break;
                case Programs.SophiaTriggered:
                    if (dtFilteredCGEN.Columns.Contains("program name") && dtFilteredCGEN.Columns.Contains("activityid") &&
                           dtFilteredCGEN.Columns.Contains("tagid/ Code") && dtFilteredCGEN.Columns.Contains("tag full url") &&
                           dtFilteredCGEN.Columns.Contains("Subject Line") && dtFilteredCGEN.Columns.Contains("Preheader"))
                        isValid = true;
                    else
                        isValid = false;
                    break;
                case Programs.APACBNB:
                    if (dtFilteredCGEN.Columns.Contains("program name") &&
                           dtFilteredCGEN.Columns.Contains("tagid/ Code") && dtFilteredCGEN.Columns.Contains("tag full url") &&
                           dtFilteredCGEN.Columns.Contains("Subject Line") && dtFilteredCGEN.Columns.Contains("Preheader") && dtFilteredCGEN.Columns.Contains("tag purpose"))
                        isValid = true;
                    else
                        isValid = false;
                    break;
            }

            return isValid;
        }

        private void DecodeSubjectLineForB2CTriggered(EmailDetails objEmailDetails)
        {
            string removedProofPartSubjectLine = string.Empty;

            if (objEmailDetails.Subject.Contains("]"))
                removedProofPartSubjectLine = objEmailDetails.Subject.Split(']')[1];
            else
                removedProofPartSubjectLine = objEmailDetails.Subject;

            string[] subjectLineParameters = removedProofPartSubjectLine.Split('|');

            foreach (string item in subjectLineParameters)
            {
                if (item.Contains("Wave"))
                {
                    objEmailDetails.WaveNo = item.Split('-')[1].Trim();
                }
                else if (item.Contains("Locale"))
                {
                    objEmailDetails.EmailLocale = item.Split('-')[1].Trim();
                    objEmailDetails = ReturnActualLocale(item.Split('-')[1].Trim(), objEmailDetails);
                }
                else if (item.Contains("SourceCode"))
                {
                    objEmailDetails.ActivityId = item.Split('-')[1].Trim();
                }
            }

            objEmailDetails.Subject = subjectLineParameters[subjectLineParameters.Length - 1].Trim();

        }

        private void DecodeSubjectLineForSophiaTriggered(EmailDetails objEmailDetails)
        {
            string removedProofPartSubjectLine = string.Empty;

            if (objEmailDetails.Subject.Contains("]"))
                removedProofPartSubjectLine = objEmailDetails.Subject.Split(']')[1];
            else
                removedProofPartSubjectLine = objEmailDetails.Subject;

            string[] subjectLineParameters = removedProofPartSubjectLine.Split('|');

            foreach (string item in subjectLineParameters)
            {
                if (item.Contains("Locale"))
                {
                    objEmailDetails.EmailLocale = item.Split('-')[1].Trim();
                    objEmailDetails = ReturnActualLocale(item.Split('-')[1].Trim(), objEmailDetails);
                }
                else if (item.Contains("SourceCode"))
                {
                    objEmailDetails.ActivityId = item.Split('-')[1].Trim();
                }

                objEmailDetails.WaveNo = string.Empty;
            }

            objEmailDetails.Subject = subjectLineParameters[subjectLineParameters.Length - 1].Trim();

        }

        private void DecodeSubjectLineForB2CBatch(EmailDetails objEmailDetails)
        {
            string[] subjectLineParameters = objEmailDetails.Subject.Split(']');

            objEmailDetails.Subject = subjectLineParameters[1].Trim();

            string activityIDDetails = subjectLineParameters[0].Replace("[", "");

            subjectLineParameters = activityIDDetails.Split(' ');

            objEmailDetails.ActivityId = subjectLineParameters[0].Trim();

            objEmailDetails.WebLocale1 = string.Empty;
            objEmailDetails.WebLocale2 = string.Empty;
            objEmailDetails.EmailLocale = "NAM";
            objEmailDetails.WaveNo = string.Empty;
        }

        private void DecodeSubjectLineForAPACBNB(EmailDetails objEmailDetails)
        {
            try
            {
                string removedProofPartSubjectLine = string.Empty;

                if (objEmailDetails.Subject.Contains("]"))
                    removedProofPartSubjectLine = objEmailDetails.Subject.Split(']')[0];
                else
                    removedProofPartSubjectLine = objEmailDetails.Subject;

                string[] subjectLineParameters = removedProofPartSubjectLine.Replace("[PROOF_", "").Split('_');

                objEmailDetails.EmailLocale = subjectLineParameters[0].Trim().ToLower();

                objEmailDetails.APACLocale = subjectLineParameters[0].Trim();

                objEmailDetails = ReturnActualLocale(subjectLineParameters[0].Trim().ToLower(), objEmailDetails);

                objEmailDetails.Subject = objEmailDetails.Subject.Split(']')[1].Trim();
            }
            catch (System.Exception)
            {
            }
        }

        private EmailDetails ProcessBodyHTML(EmailDetails objEmailDetails)
        {
            try
            {
                switch (Common.GetProgramName())
                {
                    case Programs.B2CTriggered:
                        DecodeSubjectLineForB2CTriggered(objEmailDetails);
                        break;
                    case Programs.B2CBatch:
                        DecodeSubjectLineForB2CBatch(objEmailDetails);
                        break;
                    case Programs.SophiaTriggered:
                        DecodeSubjectLineForSophiaTriggered(objEmailDetails);
                        break;
                    case Programs.APACBNB:
                        DecodeSubjectLineForAPACBNB(objEmailDetails);
                        break;
                }

                HtmlDocument htmlDoc = new HtmlDocument();

                htmlDoc.LoadHtml(objEmailDetails.BodyHTML);
                //objEmailDetails.PreHeader = Clean(htmlDoc.DocumentNode.Descendants("div").FirstOrDefault().InnerText);

                if (htmlDoc.DocumentNode.SelectSingleNode("//div[@style]") != null)
                    objEmailDetails.PreHeader = Clean(htmlDoc.DocumentNode.SelectSingleNode("//div[@style]").InnerText);
                else
                    objEmailDetails.PreHeader = string.Empty;

                HtmlNode obj250Ok = htmlDoc.DocumentNode.SelectSingleNode("//comment()[contains(., 'X-250ok-CID')]");

                if (obj250Ok != null)
                {
                    objEmailDetails.EmailOK250 = htmlDoc.DocumentNode.SelectSingleNode("//comment()[contains(., 'X-250ok-CID')]").OuterHtml;

                    if (!string.IsNullOrEmpty(objEmailDetails.EmailOK250))
                    {
                        objEmailDetails.EmailOK250 = objEmailDetails.EmailOK250.Replace("<!--", "").Replace("-->", "");

                        objEmailDetails.EmailOK250 = objEmailDetails.EmailOK250.Trim().Split(':')[1].Trim();
                    }
                }
                else
                    objEmailDetails.EmailOK250 = string.Empty;

                ExtractAllAHrefTags(htmlDoc, objEmailDetails);

                string htmlToText = htmlDoc.DocumentNode.SelectSingleNode("//body").InnerText;

                if (CheckJunkCharecters(htmlToText))//if (htmlToText.Contains(""))
                    objEmailDetails.JunkCharacterPresent = "Present";
                else
                    objEmailDetails.JunkCharacterPresent = "Not Present";

                if (htmlToText.Contains("© " + DateTime.Now.Year + " Adobe"))
                    objEmailDetails.CopyrightCheck = "Present";
                else
                    objEmailDetails.CopyrightCheck = "Not Present";

                if (htmlToText.Contains(objEmailDetails.From))
                    objEmailDetails.SenderBookAddress = "Present";
                else
                    objEmailDetails.SenderBookAddress = "Not Present";

                return objEmailDetails;
            }
            catch (System.Exception ex)
            {
                SetErrorMsg(ex.Message, "error", ex);
                return objEmailDetails;
            }
        }

        private bool CheckJunkCharecters(string emailBodyText)
        {
            Regex regex = new Regex(@"[^\x00-\x7F]+");
            Match match = regex.Match(emailBodyText);

            if (match.Success)
            {
                return true;
            }
            else
                return false;
        }

        private bool CheckDuplicateMirrorPage(string mirrorUrl)
        {
            bool duplicateMirror = false;

            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument htmlDoc = web.Load(mirrorUrl);

                HtmlNode link = htmlDoc.DocumentNode.SelectNodes("//div[@_label]") != null ? htmlDoc.DocumentNode.SelectNodes("//div[@_label]").FirstOrDefault() : null;

                link = link == null ? (htmlDoc.DocumentNode.SelectNodes("//div[@alias]") != null ? htmlDoc.DocumentNode.SelectNodes("//div[@alias]").FirstOrDefault() : null) : null;

                if (link != null)
                {
                    HtmlAttribute miror = link.Attributes["_label"] != null ? link.Attributes["_label"] : link.Attributes["alias"];

                    if (miror != null && (miror.Value == "Mirror Page" || miror.Value == "Read Online"))
                    {
                        duplicateMirror = true;
                    }
                }

                return duplicateMirror;
            }
            catch (System.Exception ex)
            {
                SetErrorMsg(ex.Message, "error", ex);

                return duplicateMirror;
            }
        }

        private EmailDetails ReturnActualLocale(string locale, EmailDetails objEmailDetails)
        {
            string finalLocale = locale;
            objEmailDetails.IsEnglish = false;

            switch (locale)
            {
                case "ja":
                    objEmailDetails.WebLocale1 = "jp";
                    objEmailDetails.WebLocale2 = "ja";
                    break;
                case "de_DE":
                    objEmailDetails.WebLocale1 = "de";
                    objEmailDetails.WebLocale2 = "de_DE";
                    break;
                case "en_GB":
                    objEmailDetails.WebLocale1 = "uk";
                    objEmailDetails.WebLocale2 = "en";
                    objEmailDetails.IsEnglish = true;
                    break;
                case "ie":
                    objEmailDetails.WebLocale1 = "en";
                    objEmailDetails.WebLocale2 = "ie";
                    objEmailDetails.IsEnglish = true;
                    break;
                case "br":
                    objEmailDetails.WebLocale1 = "pt_br";
                    objEmailDetails.WebLocale2 = "br";
                    break;
                case "NAM":
                    objEmailDetails.WebLocale1 = "en";
                    objEmailDetails.WebLocale2 = "ie";
                    objEmailDetails.IsEnglish = true;
                    break;
                case "en_AU":
                    objEmailDetails.WebLocale1 = "au";
                    objEmailDetails.WebLocale2 = "ie";
                    objEmailDetails.IsEnglish = true;
                    break;
                case "en_NZ":
                    objEmailDetails.WebLocale1 = "nz";
                    objEmailDetails.WebLocale2 = "ie";
                    objEmailDetails.IsEnglish = true;
                    break;
                case "ie_IN":
                    objEmailDetails.WebLocale1 = "in";
                    objEmailDetails.WebLocale2 = "ie";
                    objEmailDetails.IsEnglish = true;
                    break;
                case "ie_ROW":
                    objEmailDetails.WebLocale1 = "ie";
                    objEmailDetails.WebLocale2 = "en";
                    objEmailDetails.IsEnglish = true;
                    break;
                case "pt_BR":
                    objEmailDetails.WebLocale1 = "pt";
                    objEmailDetails.WebLocale2 = "br";
                    break;
                case "sv":
                    objEmailDetails.WebLocale1 = "sv";
                    objEmailDetails.WebLocale2 = "se";
                    break;
                case "nl_NL":
                    objEmailDetails.WebLocale1 = "nl";
                    objEmailDetails.WebLocale2 = "nl";
                    break;
                case "nl_BE":
                    objEmailDetails.WebLocale1 = "nl";
                    objEmailDetails.WebLocale2 = "nl";
                    break;
                case "ko":
                    objEmailDetails.WebLocale1 = "kr";
                    objEmailDetails.WebLocale2 = "ko";
                    break;
                case "ie_SEA":
                    objEmailDetails.WebLocale1 = "sea";
                    objEmailDetails.WebLocale2 = "ie";
                    objEmailDetails.IsEnglish = true;
                    break;
                case "zh_TW":
                    objEmailDetails.WebLocale1 = "zh_TW";
                    objEmailDetails.WebLocale2 = "tw";
                    break;
                case "hk":
                    objEmailDetails.WebLocale1 = "hk_zh";
                    objEmailDetails.WebLocale2 = "hk";
                    break;
                case "fr_CH":
                    objEmailDetails.WebLocale1 = "ch_fr";
                    objEmailDetails.WebLocale2 = "fr";
                    break;
                case "fr_BE":
                    objEmailDetails.WebLocale1 = "be_fr";
                    objEmailDetails.WebLocale2 = "fr";
                    break;
                case "en_IE":
                    objEmailDetails.WebLocale1 = "ie";
                    objEmailDetails.WebLocale2 = "ie";
                    objEmailDetails.IsEnglish = true;
                    break;
                case "de_CH":
                    objEmailDetails.WebLocale1 = "ch_de";
                    objEmailDetails.WebLocale2 = "de";
                    break;
                case "de_AT":
                    objEmailDetails.WebLocale1 = "at";
                    objEmailDetails.WebLocale2 = "de";
                    break;
                case "da":
                    objEmailDetails.WebLocale1 = "da_dk";
                    objEmailDetails.WebLocale2 = "dk";
                    break;
                case "es_ES":
                    objEmailDetails.WebLocale1 = "es";
                    objEmailDetails.WebLocale2 = "es";
                    break;
                case "es_MX":
                    objEmailDetails.WebLocale1 = "es";
                    objEmailDetails.WebLocale2 = "mx";
                    break;
                case "es_SLAM":
                    objEmailDetails.WebLocale1 = "es";
                    objEmailDetails.WebLocale2 = "la";
                    break;
                case "en_CA":
                    objEmailDetails.WebLocale1 = "en";
                    objEmailDetails.WebLocale2 = "en";
                    break;
                case "fr_CA":
                    objEmailDetails.WebLocale1 = "fr";
                    objEmailDetails.WebLocale2 = "be_fr";
                    break;
                case "fr":
                    objEmailDetails.WebLocale1 = "fr";
                    objEmailDetails.WebLocale2 = "be_fr";
                    break;
                case "sv_SE":
                    objEmailDetails.WebLocale1 = "sv";
                    objEmailDetails.WebLocale2 = "se";
                    break;
                case "da_DK":
                    objEmailDetails.WebLocale1 = "dk";
                    objEmailDetails.WebLocale2 = "da";
                    break;
                case "ru_RU":
                    objEmailDetails.WebLocale1 = "ru";
                    objEmailDetails.WebLocale2 = "ru";
                    break;
                case "po_PO":
                    objEmailDetails.WebLocale1 = "po";
                    objEmailDetails.WebLocale2 = "po";
                    break;
                case "pt_PT":
                    objEmailDetails.WebLocale1 = "pt";
                    objEmailDetails.WebLocale2 = "pt";
                    break;
                case "no_NO":
                    objEmailDetails.WebLocale1 = "no";
                    objEmailDetails.WebLocale2 = "no";
                    break;
                case "it_IT":
                    objEmailDetails.WebLocale1 = "it";
                    objEmailDetails.WebLocale2 = "it";
                    break;
                case "fi_FI":
                    objEmailDetails.WebLocale1 = "fi";
                    objEmailDetails.WebLocale2 = "fi";
                    break;
                case "in":
                    objEmailDetails.WebLocale1 = "in";
                    objEmailDetails.WebLocale2 = "in";
                    objEmailDetails.IsEnglish = true;
                    break;
                default:
                    objEmailDetails.WebLocale1 = locale;
                    objEmailDetails.WebLocale2 = locale;
                    break;
            }

            return objEmailDetails;
        }

        private string GetPreheaderFromMirrorPage(string mirrorPage)
        {
            string preHeader = string.Empty;

            try
            {
                HtmlWeb web = new HtmlWeb();
                ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
                HtmlDocument htmlDoc = web.Load(mirrorPage);

                preHeader = Clean(htmlDoc.DocumentNode.Descendants("div").FirstOrDefault().InnerText).Trim();

            }
            catch (System.Exception ex)
            {
                SetErrorMsg(ex.Message, "error", ex);
            }

            return preHeader;
        }

        private void ExtractAllAHrefTags(HtmlDocument htmlSnippet, EmailDetails objEmailDetails)
        {
            try
            {
                List<HtmlNode> lstHyperlinks = new List<HtmlNode>();

                foreach (HtmlNode link in htmlSnippet.DocumentNode.SelectNodes("//a[@href]"))
                {
                    lstHyperlinks.Add(link);
                }

                var times = new ParallelOptions { MaxDegreeOfParallelism = 10 };

                Parallel.ForEach(lstHyperlinks, times, link =>
                {
                    HtmlAttribute attUrl = link.Attributes["originalsrc"] == null ? link.Attributes["href"] : link.Attributes["originalsrc"];

                    if (attUrl != null)
                    {
                        //For Mirror page
                        HtmlAttribute miror = link.Attributes["_label"] != null ? link.Attributes["_label"] : link.Attributes["alias"];

                        try
                        {
                            string urlToParse = attUrl.Value.Replace("&amp;", "&").Replace("&#43;", "+");

                            string[] RedirectedURLAndStatus = GetRedirectedURLAndStatusCode(urlToParse);

                            UrlAndStatusCode DecodedHrefTags = new UrlAndStatusCode();
                            DecodedHrefTags.URL = RedirectedURLAndStatus[0];
                            DecodedHrefTags.Status = RedirectedURLAndStatus[1];

                            if (miror != null && (miror.Value == "Mirror Page" || miror.Value == "Read Online"))
                            {
                                DecodedHrefTags.Type = Constants.MirrorPage;
                                objEmailDetails.DuplicateMirrorPage = CheckDuplicateMirrorPage(DecodedHrefTags.URL);
                            }
                            else
                            {
                                if (DecodedHrefTags.URL.Contains("privacy.html"))
                                {
                                    DecodedHrefTags.Type = Constants.PrivacyPage;
                                    DecodedHrefTags.LocalizedUrl = true;

                                    if (DecodedHrefTags.URL.Contains(ddlTrackingParameter.SelectedValue))
                                        DecodedHrefTags.LocalizedAndTrackingUrl = true;
                                }
                                else if (DecodedHrefTags.URL.Contains("unsubscribe.html"))
                                {
                                    DecodedHrefTags.Type = Constants.UnsubscribePage;
                                    DecodedHrefTags.LocalizedUrl = true;

                                    if (DecodedHrefTags.URL.Contains(ddlTrackingParameter.SelectedValue))
                                        DecodedHrefTags.LocalizedAndTrackingUrl = true;
                                }
                                else if (DecodedHrefTags.URL.Contains(ddlTrackingParameter.SelectedValue))
                                {
                                    DecodedHrefTags.Type = Constants.TagfulUrl;
                                }
                                else if (DecodedHrefTags.URL.Contains("contact.html"))
                                {
                                    DecodedHrefTags.Type = Constants.ContactPage;
                                    DecodedHrefTags.LocalizedUrl = true;

                                    if (DecodedHrefTags.URL.Contains(ddlTrackingParameter.SelectedValue))
                                        DecodedHrefTags.LocalizedAndTrackingUrl = true;
                                }
                                else if (DecodedHrefTags.URL.Contains("terms.html"))
                                {
                                    DecodedHrefTags.Type = Constants.LegalPage;
                                    DecodedHrefTags.LocalizedUrl = true;

                                    if (DecodedHrefTags.URL.Contains(ddlTrackingParameter.SelectedValue))
                                        DecodedHrefTags.LocalizedAndTrackingUrl = true;
                                }
                                else if (DecodedHrefTags.URL.Contains("impressum.html"))
                                {
                                    DecodedHrefTags.Type = Constants.ImpressumPage;
                                    DecodedHrefTags.LocalizedUrl = true;

                                    if (DecodedHrefTags.URL.Contains(ddlTrackingParameter.SelectedValue))
                                        DecodedHrefTags.LocalizedAndTrackingUrl = true;

                                }
                                else if (DecodedHrefTags.URL.Contains("facebook.com") || DecodedHrefTags.URL.Contains("twitter.com")
                                || DecodedHrefTags.URL.Contains("linkedin.com") || DecodedHrefTags.URL.Contains("theblog.adobe.com")
                                || DecodedHrefTags.URL.Contains("medium.com") || DecodedHrefTags.URL.Contains("youtube.com"))
                                {
                                    DecodedHrefTags.Type = Constants.SocialLinks;
                                }
                                else if (DecodedHrefTags.URL.Contains("mailto:"))
                                {
                                    DecodedHrefTags.Type = Constants.Email;
                                }
                                else
                                {
                                    DecodedHrefTags.Type = Constants.OtherLinks;
                                }

                                DecodedHrefTags.URL = HttpUtility.UrlDecode(DecodedHrefTags.URL);
                            }

                            if (DecodedHrefTags != null)
                                objEmailDetails.DecodedHrefTags.Add(DecodedHrefTags);
                        }
                        catch (AggregateException ex)
                        {
                            SetErrorMsg(ex.Message, "error", ex);
                        }
                    }
                    else
                        lstHyperlinks.Remove(link);
                });
            }
            catch (System.Exception ex)
            {
                SetErrorMsg(ex.Message, "error", ex);
            }
        }

        private string[] GetRedirectedURLAndStatusCode1(string url)
        {
            string[] RedirectedURLAndStatus = new string[2];

            try
            {
                if (url.Contains("mailto"))
                {
                    RedirectedURLAndStatus[0] = url.Replace("mailto:", "");
                    RedirectedURLAndStatus[1] = Constants.PageRedirectingMsg;
                }
                else
                {

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "HEAD";
                    // always compress, if you get back a 404 from a HEAD it can be quite big.
                    request.AutomaticDecompression = DecompressionMethods.GZip;
                    request.AllowAutoRedirect = false;
                    request.Timeout = 30000;
                    request.KeepAlive = false;

                    try
                    {
                        using (var response = request.GetResponse() as HttpWebResponse)
                        {
                            if (response.StatusCode == HttpStatusCode.OK ||
                                response.StatusCode == HttpStatusCode.Redirect ||
                                response.StatusCode == HttpStatusCode.MovedPermanently ||
                                response.StatusCode == HttpStatusCode.NotFound ||
                                response.StatusCode == HttpStatusCode.RedirectKeepVerb)
                            {
                                RedirectedURLAndStatus[0] = response.GetResponseHeader("Location");
                                RedirectedURLAndStatus[1] = Constants.PageRedirectingMsg;

                                if (response.ResponseUri.ToString().Trim() != url.Trim())
                                {
                                    RedirectedURLAndStatus = GetRedirectedURLAndStatusCode(RedirectedURLAndStatus[0]);
                                }
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {
                        RedirectedURLAndStatus[0] = url;
                        RedirectedURLAndStatus[1] = Constants.PageErrorMsg;

                        // SetErrorMsg(ex.Message, "error", ex);

                        return RedirectedURLAndStatus;
                    }
                }

                return RedirectedURLAndStatus;
            }
            catch (System.Exception ex)
            {
                RedirectedURLAndStatus[0] = url;
                RedirectedURLAndStatus[1] = Constants.PageErrorMsg;

                SetErrorMsg(ex.Message, "error", ex);

                return RedirectedURLAndStatus;
            }
        }

        public string GetFinalStatus(HttpWebResponse response)
        {
            if (response.StatusCode == HttpStatusCode.OK ||
                                response.StatusCode == HttpStatusCode.Redirect ||
                                response.StatusCode == HttpStatusCode.MovedPermanently ||
                                response.StatusCode == HttpStatusCode.RedirectKeepVerb)
            {
                return Constants.PageRedirectingMsg;
            }
            else
            {
                return Constants.PageErrorMsg;
            }
        }

        public string[] GetRedirectedURLAndStatusCode(string url)
        {
            string[] RedirectedURLAndStatus = new string[2];

            RedirectedURLAndStatus[0] = url;
            RedirectedURLAndStatus[1] = Constants.PageRedirectingMsg;

            try
            {
                if (url.Contains("mailto"))
                {
                    RedirectedURLAndStatus[0] = url.Replace("mailto:", "");
                    RedirectedURLAndStatus[1] = Constants.PageRedirectingMsg;
                }
                else
                {

                    if (string.IsNullOrWhiteSpace(url))
                    {
                        RedirectedURLAndStatus[0] = url;
                        RedirectedURLAndStatus[1] = Constants.PageRedirectingMsg;

                        return RedirectedURLAndStatus;
                    }


                    int maxRedirCount = 8;  // prevent infinite loops
                    string newUrl = url;
                    do
                    {
                        HttpWebRequest req = null;
                        HttpWebResponse resp = null;
                        try
                        {
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                            req = (HttpWebRequest)HttpWebRequest.Create(url);
                            req.Method = "HEAD";
                            req.AllowAutoRedirect = false;
                            resp = (HttpWebResponse)req.GetResponse();
                            switch (resp.StatusCode)
                            {
                                case HttpStatusCode.OK:
                                    {
                                        RedirectedURLAndStatus[0] = newUrl;
                                        RedirectedURLAndStatus[1] = Constants.PageRedirectingMsg;

                                        return RedirectedURLAndStatus;
                                    }
                                case HttpStatusCode.Redirect:
                                case HttpStatusCode.MovedPermanently:
                                case HttpStatusCode.RedirectKeepVerb:
                                case HttpStatusCode.RedirectMethod:
                                    newUrl = resp.Headers["Location"];
                                    if (newUrl == null)
                                    {
                                        RedirectedURLAndStatus[0] = url;
                                        RedirectedURLAndStatus[1] = GetFinalStatus(resp);

                                        return RedirectedURLAndStatus;
                                    }

                                    if (newUrl.IndexOf("://", System.StringComparison.Ordinal) == -1)
                                    {
                                        // Doesn't have a URL Schema, meaning it's a relative or absolute URL
                                        Uri u = new Uri(new Uri(url), newUrl);
                                        newUrl = u.ToString();
                                    }
                                    break;
                                default:
                                    {
                                        RedirectedURLAndStatus[0] = newUrl;
                                        RedirectedURLAndStatus[1] = GetFinalStatus(resp);

                                        return RedirectedURLAndStatus;
                                    }
                            }
                            url = newUrl;
                        }
                        catch (WebException)
                        {
                            if (newUrl.Contains("www.linkedin.com") || newUrl.Contains("www.instagram.com"))
                            {
                                // Return the last known good URL
                                RedirectedURLAndStatus[0] = newUrl;
                                RedirectedURLAndStatus[1] = Constants.PageRedirectingMsg;
                            }
                            else
                            {
                                // Return the last known good URL
                                RedirectedURLAndStatus[0] = newUrl;
                                RedirectedURLAndStatus[1] = Constants.PageErrorMsg;
                            }

                            return RedirectedURLAndStatus;
                        }
                        catch (System.Exception ex)
                        {
                            RedirectedURLAndStatus[0] = url;
                            RedirectedURLAndStatus[1] = Constants.StatusUnknown;
                        }
                        finally
                        {
                            if (resp != null)
                                resp.Close();
                        }
                    } while (maxRedirCount-- > 0);

                    return RedirectedURLAndStatus;
                }
            }
            catch (System.Exception ex)
            {
                return RedirectedURLAndStatus;
            }

            return RedirectedURLAndStatus; ;
        }

        private void SetErrorMsg(string msg, string cssClass, System.Exception ex = null)
        {
            if (ex != null)
            {
                lblMsg.Text = "Some error occured. Please try after sometime."; //new StackTrace(ex).GetFrame(0).GetMethod().Name;
            }
            else
            {
                lblMsg.Text = msg;
            }

            divMsg.Visible = true;
            divMsg.Attributes["class"] = "msg-block m-t-20 " + cssClass;
        }

        [WebMethod]
        public static bool SendEmailViaOutLook(string reportUrl, string programName, string email, string subjectLine)
        {
            try
            {
                string userName = HttpContext.Current.Session[SessionVariables.UserName] as string;
                string userEmail = userName + "@adobe.com";
                string subject = "[Simplifi]" + subjectLine;
                string headerEmail = "<p>Hello Team, <br/><br/>" +
                    "Find the Proof testing details for Program: <strong>" + programName + "</strong> in the below link(pdf).</p>" +
                    "<a href=" + Uri.EscapeUriString(reportUrl).ToString() + ">Click here to Download Report</a>";
                string footerEmail = "<p>Thanks,<br/>Team Simplifi</p>";
                string emailBody = headerEmail + footerEmail;

                // Create a Outlook Application and connect to outlook 
                Application OutlookApplication = new Application();

                // create the MailItem which we want to send 
                MailItem message = (MailItem)OutlookApplication.CreateItem(OlItemType.olMailItem);

                MailAddress toAddress = new MailAddress(email);
                MailAddress ccAddress = new MailAddress(userEmail);

                message.To = toAddress.ToString();
                message.CC = ccAddress.ToString();
                message.Subject = subject;
                message.BodyFormat = OlBodyFormat.olFormatHTML;
                message.HTMLBody = emailBody;

                //Send email
                message.Send();

                return true;
            }
            catch (System.Exception ex)
            {
                return false;
            }
        }

        [WebMethod]
        public static string SavePDF(string binaryData, string reportName)
        {
            try
            {
                string userName = HttpContext.Current.Session[SessionVariables.UserName] as string;
                string currPath = HttpContext.Current.Server.MapPath("~/UploadedFiles/" + userName + "/Reports/");

                if (!Directory.Exists(currPath))
                    Directory.CreateDirectory(currPath);

                var pdfBinary = Convert.FromBase64String(binaryData);

                Regex illegalInFileName = new Regex(@"[\\/:*?""<>|]");
                reportName = illegalInFileName.Replace(reportName, "").Replace(" ", "_");

                reportName = GetFileName(reportName, currPath);

                var fileName = reportName + "_QAed_" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".pdf";
                var fileNameWithPath = currPath + "\\" + fileName;

                if (!File.Exists(fileNameWithPath))
                {
                    // write content to the pdf
                    using (var fs = new FileStream(fileNameWithPath, FileMode.Create))
                    {
                        using (var writer = new BinaryWriter(fs))
                        {
                            writer.Write(pdfBinary, 0, pdfBinary.Length);
                            writer.Close();
                        }
                    }

                    return "http://" + HttpContext.Current.Request.Url.Authority + "/UploadedFiles/" + userName + "/Reports/" + fileName;
                }
                else
                    return "";

            }
            catch (System.Exception ex)
            {
                return "";
            }
        }

        private static string GetFileName(string fileName, string currPath)
        {
            DirectoryInfo d = new DirectoryInfo(currPath);
            FileInfo[] files = d.GetFiles("*.pdf");

            int count = files.Where(x => x.Name.Contains(fileName)).ToList().Count();

            if (count > 0)
                fileName = fileName + "_V-" + count;

            return fileName;
        }
    }
}