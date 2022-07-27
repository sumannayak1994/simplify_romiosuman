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
    public partial class proof_check : System.Web.UI.Page
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
            string[] emailFilePaths = null;

            if (Directory.Exists(emailsPath))
                emailFilePaths = Directory.GetFiles(emailsPath);


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

                                objEmailDetails.Subject = Clean(msg.Subject);
                                objEmailDetails.BodyHTML = msg.BodyHtml;
                                objEmailDetails.From = msg.Sender.Email;
                                objEmailDetails.EmailFromName = msg.Sender.DisplayName;

                                objEmailDetails = ProcessBodyHTML(objEmailDetails);

                                FormHTMLTable(objEmailDetails);
                            }
                        }

                        stopwatch.Stop();

                        sbContent.Append("</tr></table>");
                        divContent.InnerHtml = sbContent.ToString();

                        lblTime.Text = Math.Ceiling(TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds).TotalSeconds).ToString();

                        divResults.Visible = true;

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

            if (lblCurrEmailCount.Text.Trim() == "- NA -")
            {
                SetErrorMsg("No Email Files Present. Please upload one to proceed.", "error");
                retVal = false;
            }

            return retVal;
        }

        private void ValidateSubjectLine(StringBuilder sbProof, EmailDetails objEmailDetails)
        {
            try
            {
                string emailSub = objEmailDetails.Subject.Trim();

                string spellCheckedSubjectLine = SpellCheck(emailSub, objEmailDetails.IsEnglish);
                bool isSubSpellCorrect = spellCheckedSubjectLine.Contains("wrong-spell") ? false : true;


                //If quote is present
                if (CheckSubjectLineForQuote(emailSub))
                {
                    sbProof.Append("<tr class='c-red'><td>Subject</td><td>'" + spellCheckedSubjectLine +
                            "'</td><td><strong class='c-red'>Quote Present</strong></td></tr>");

                    if (isSubSpellCorrect)
                        sbContent.Append("<td data-field='subject'><strong class='c-red'><span class='glyphicon glyphicon-remove-circle'></span></strong></td>");
                    else
                        sbContent.Append("<td data-field='subject'><strong class='c-orange'><span class='glyphicon glyphicon-info-sign'></span></strong></td>");
                }
                else
                {
                    //Check for subject Line
                    if (!string.IsNullOrEmpty(emailSub))
                    {
                        sbProof.Append("<tr><td>Subject</td><td>" + spellCheckedSubjectLine +
                            "</td><td><strong class='c-green'>Present</strong></td></tr>");

                        if (isSubSpellCorrect)
                            sbContent.Append("<td data-field='subject'><strong class='c-green'><span class='glyphicon glyphicon-ok-circle'></span></strong></td>");
                        else
                            sbContent.Append("<td data-field='subject'><strong class='c-orange'><span class='glyphicon glyphicon-info-sign'></span></strong></td>");
                    }
                    else
                    {
                        sbProof.Append("<tr class='c-red'><td>Subject</td><td>" + spellCheckedSubjectLine +
                            "</td><td><strong >Not Present</strong></td></tr>");

                        sbContent.Append("<td data-field='subject'><strong class='c-red'><span class='glyphicon glyphicon-remove-circle'></span></strong></td>");
                    }
                }
            }
            catch (System.Exception ex)
            {
                SetErrorMsg(ex.Message, "error", ex);
            }
        }

        private void ValidatePreheader(StringBuilder sbProof, EmailDetails objEmailDetails)
        {
            try
            {
                objEmailDetails.PreHeader = string.IsNullOrEmpty(objEmailDetails.PreHeader) ? null : objEmailDetails.PreHeader.Trim();

                string spellCheckedPreHeader = SpellCheck(objEmailDetails.PreHeader, objEmailDetails.IsEnglish);
                bool isPreHeaderSpellCorrect = spellCheckedPreHeader.Contains("wrong-spell") ? false : true;

                //Check for preheader
                if (string.IsNullOrEmpty(objEmailDetails.PreHeader))
                {
                    if (objEmailDetails.DecodedHrefTags.Where(x => x.Type == Constants.MirrorPage).Count() > 0)
                    {
                        string mirrorLink = objEmailDetails.DecodedHrefTags.Where(x => x.Type == Constants.MirrorPage).FirstOrDefault().URL;

                        objEmailDetails.PreHeader = GetPreheaderFromMirrorPage(mirrorLink);

                        objEmailDetails.PreHeader = string.IsNullOrEmpty(objEmailDetails.PreHeader) ? null : objEmailDetails.PreHeader.Trim();

                        spellCheckedPreHeader = SpellCheck(objEmailDetails.PreHeader, objEmailDetails.IsEnglish);
                        isPreHeaderSpellCorrect = spellCheckedPreHeader.Contains("wrong-spell") ? false : true;

                        if (!string.IsNullOrEmpty(objEmailDetails.PreHeader))
                        {
                            sbProof.Append("<tr><td>Preheader</td><td>" + spellCheckedPreHeader +
                               "</td><td><strong class='c-green'>Present</strong></td></tr>");

                            if (isPreHeaderSpellCorrect)
                                sbContent.Append("<td data-field='preheader'><strong class='c-green'><span class='glyphicon glyphicon-ok-circle'></span></strong></td>");
                            else
                                sbContent.Append("<td data-field='preheader'><strong class='c-orange'><span class='glyphicon glyphicon-info-sign'></span></strong></td>");
                        }
                        else
                        {
                            sbProof.Append("<tr class='c-red'><td>Preheader</td><td>" + spellCheckedPreHeader +
                                "</td><td><strong >Not Present</strong></td></tr>");

                            sbContent.Append("<td data-field='preheader'><strong class='c-red'><span class='glyphicon glyphicon-remove-circle'></span></strong></td>");
                        }
                    }
                }
                else
                {
                    sbProof.Append("<tr><td>Preheader</td><td>" + spellCheckedPreHeader +
                              "</td><td><strong class='c-green'>Present</strong></td></tr>");
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

        private void ValidateTagIdAndTagUrls(StringBuilder sbProof, EmailDetails objEmailDetails)
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

                bool isTagIdAndTagUrlMatched = true;

                foreach (var item in tagUrls)
                {
                    if (!string.IsNullOrEmpty(item.TagId))
                    {
                        sbProof.Append("<tr><td>Tag Id</td><td>" + item.TagId +
                           "</td><td><strong class='c-green'>Present</strong></td></tr>");
                    }
                    else
                    {
                        sbProof.Append("<tr><td>Tag Id</td><td>" + item.TagId +
                           "</td><td><strong class='c-red'>Not Present</strong></td></tr>");
                    }

                    if (!string.IsNullOrEmpty(item.URL))
                    {
                        sbtagURLs.Append("<tr><td>Tag URL</td><td><a href='" + item.URL + "' target='_blank'>" + item.URL +
                            "</a></td><td><strong class='c-green'>present</strong></td></tr>");

                        isTagIdAndTagUrlMatched = true;
                    }
                    else
                    {
                        sbtagURLs.Append("<tr class='c-red'><td>Tag URL</td><td><a href='" + item.URL + "' target='_blank'>" + item.URL +
                            "</a></td><td><strong>Not Present</strong></td></tr>");

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
                        sbProof.Append("<tr><td>Mirror Page</td><td><a  href='" + mirrorLink + "' target='_blank'> " + mirrorLink +
                                "</a></td><td><strong class='c-green'>Page Available.<br/>No Duplicate Page</strong></td></tr>");

                        sbContent.Append("<td data-field='mirror-presence'><strong class='c-green'><span class='glyphicon glyphicon-ok-circle'></span></strong></td>");
                    }
                    else
                    {
                        sbProof.Append("<tr class='c-red'><td>Mirror Page</td><td><a  href='" + mirrorLink + "' target='_blank'> " + mirrorLink +
                                 "</a></td><td><strong class='c-green'>Page Available.</strong><strong> <br/>Duplicate Page</strong></td></tr>");

                        sbContent.Append("<td data-field='mirror-presence'><strong class='c-red'><span class='glyphicon glyphicon-remove-circle'></span></strong></td>");
                    }
                }
                else if (string.IsNullOrWhiteSpace(mirrorLink))
                {
                    sbProof.Append("<tr class='c-red'><td>Mirror Page</td><td>- NA -" +
                            "</td><td><strong>Not Available</strong></td></tr>");

                    sbContent.Append("<td data-field='mirror-presence'><strong class='c-red'><span class='glyphicon glyphicon-remove-circle'></span></strong></td>");
                }
                else if (status == Constants.PageErrorMsg)
                {
                    sbProof.Append("<tr class='c-red'><td>Mirror Page</td><td><a  href='" + mirrorLink + "' target='_blank'> " + mirrorLink +
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
                    sbProof.Append("<tr><td>Link Check (" + objEmailDetails.DecodedHrefTags.Count() + ")</td><td>Total Links Present: " + objEmailDetails.DecodedHrefTags.Count() +
                       "</td><td><strong class='c-green'>All Valid</strong></td></tr>");

                    sbContent.Append("<td data-field='link-check'><strong class='c-green'><span class='glyphicon glyphicon-ok-circle'></span></strong></td>");
                }
                else
                {
                    sbProof.Append("<tr class='c-red'><td>Link Check (" + objEmailDetails.DecodedHrefTags.Count() + ")</td><td> Total Error Links: " + erroredURLs.Count() + "<br/><br/>");

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
            sbProof.Append("<tr><td>Sender Name</td><td>" + objEmailDetails.EmailFromName.Trim() +
                                "</td><td><strong class='c-green'>Present</strong></td></tr>");

            sbContent.Append("<td data-field='sender-name'><strong class='c-green'><span class='glyphicon glyphicon-ok-circle'></span></strong></td>");
        }

        private bool CheckSubjectLineForQuote(string emailSub)
        {
            if (emailSub.StartsWith("\""))
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

        private void InitializeHTMLForB2CTriggered(StringBuilder sbProof, EmailDetails objEmailDetails)
        {
            sbContent.Append("<tr>" +
          "<td><strong class='p-block'><a target='_blank' title='Click for Mirror Page' href='" + GetMirrorLink(objEmailDetails) + "'>" + objEmailDetails.Subject + "</a></strong></td>");


            sbProof.Append("<div class='div-pop-details p-l-0 b-r-2 '><strong>Subject: <span>" + objEmailDetails.Subject + "</span></strong> </div>");

            sbProof.Append("<strong class='c-blue'>Common Check</strong><table id='table-two-axis' class='customers m-t-10'><tr><th style='width:10%;'></th><th style='width:80%;'>Proof Details</th><th style='width:10%;'>Remark</th></tr>");

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

        private void FormHTMLTable(EmailDetails objEmailDetails)
        {
            StringBuilder sbProof = new StringBuilder();
            InitializeHTMLForB2CTriggered(sbProof, objEmailDetails);

            ValidateSubjectLine(sbProof, objEmailDetails);

            ValidatePreheader(sbProof, objEmailDetails);

            ValidateTagIdAndTagUrls(sbProof, objEmailDetails);

            ValidateMirrorPage(sbProof, objEmailDetails);

            ValidateLinks(sbProof, objEmailDetails);

            //ValidateJunkCharacters(sbProof, objEmailDetails);

            ValidateSenderName(sbProof, objEmailDetails);

            sbProof.Append("</table> <br/><br/>");

            //Legal Footer Check
            sbProof.Append("<strong class='c-blue'>Legal Footer Check</strong><table id='table-two-axis' class='customers m-t-10'><tr><th style='width:10%;'></th><th style='width:75%;'>Proof Details</th><th style='width:15%;'>Remark</th></tr>");

            ValidateUnsubLink(sbProof, objEmailDetails);

            ValidateCopyright(sbProof, objEmailDetails);

            ValidateSenderBook(sbProof, objEmailDetails);

            sbProof.Append("</table><br/><br/>");

            ListOutLinks(sbProof, objEmailDetails);

            sbProof.Append("</table>");

            string finalDetailsHTML = Regex.Replace(sbProof.ToString(), @"[\""]", "", RegexOptions.None);

            //For Details            
            sbContent.Append("<td class='details-td' data-details=\"" + finalDetailsHTML + "\"><a href='#' class='spDetails' title='View Details'><span class='fa fa-list-alt f-20'></span></a></td>");

        }

        public bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
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

        [WebMethod(EnableSession = true)]
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

        [WebMethod(EnableSession = true)]
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