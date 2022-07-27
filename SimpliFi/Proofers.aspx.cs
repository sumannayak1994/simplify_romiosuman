using Excel;
using HtmlAgilityPack;
using Microsoft.Office.Interop.Outlook;
using MsgReader.Outlook;
using NHunspell;
using OfficeOpenXml;
using SimpliFi.Class;
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
using System.Web.UI;
using System.Drawing;
using System.Web.UI.WebControls;
using System.Windows;

namespace SimpliFi
{
    public partial class DCMCheck : System.Web.UI.Page
    {
        public static string dcmPath = string.Empty;
        public List<DCMEntry> dCMEntries = new List<DCMEntry>();
        public List<MSGEntry> mSGEntries = new List<MSGEntry>();
        public List<CGENEntry> cGENEntries = new List<CGENEntry>();
        List<GridViewData> gridViewData = new List<GridViewData>();
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        public void uploadDCMFile(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(Server.MapPath("~/UploadedFiles/DCMFile")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/UploadedFiles/DCMFile"));
                }
                string currDCMPath = Common.GetCurrentPath("DCMFile");

                bool isClearingNeeded = IsDirectoryClearingNeeded(currDCMPath, "dcm");

                if (FileUpload1.HasFile)
                {
                    string fileName = Path.GetFileName(FileUpload1.PostedFile.FileName);
                    dcmPath = Server.MapPath("~/UploadedFiles/DCMFile/" + fileName);
                    FileUpload1.SaveAs(dcmPath);
                    Label1.Text = FileUpload1.PostedFile.FileName;
                }
                else
                {
                    Label1.Text = "No Files";
                }
            }
            catch (System.Exception ex)
            {
                Label1.Text = ex.Message + "error" + ex;
            }
        }

        public void uploadMsgFile(object sender, EventArgs e)
        {
            try
            {
                string currEmailPath = Common.GetCurrentPath("Emails");

                if (FileUpload2.HasFile)
                {
                    bool isClearingNeeded = IsDirectoryClearingNeeded(currEmailPath, "emails");

                    string getFileName = string.Empty;

                    foreach (HttpPostedFile htfiles in FileUpload2.PostedFiles)
                    {
                        getFileName = Path.GetFileName(htfiles.FileName);
                        htfiles.SaveAs(HttpContext.Current.Server.MapPath(currEmailPath + getFileName));
                    }
                    Label2.Text = FileUpload2.PostedFiles.Count.ToString() + "Files uploaded";
                }
                else
                {
                    Label2.Text = "No files";
                }
            }
            catch (System.Exception ex)
            {
                Label2.Text = "Error while uploading the file";
            }
        }

        protected void uploadCGENFile(object sender, EventArgs e)
        {
            string cgenPath = string.Empty;
            try
            {

                if (!Directory.Exists(Server.MapPath("~/UploadedFiles/CGEN")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/UploadedFiles/CGEN"));
                }
                if (FileUpload3.HasFile)
                {

                    string fileName = Path.GetFileName(FileUpload3.PostedFile.FileName);
                    cgenPath = Server.MapPath("~/UploadedFiles/CGEN/" + fileName);
                    FileUpload3.SaveAs(cgenPath);
                    Label3.Text = FileUpload3.PostedFile.FileName;
                }
                else
                {
                    Label3.Text = "No Files";
                }

            }
            catch (System.Exception ex)
            {
                Label3.Text = "Error while File upload";
            }
        }
        public void validate(object sender, EventArgs e)
        {
            var dCMEntries = loadDCMList();
            var msgEntries = loadMsgList();
            var cgenEntries = loadCGENList();

            if (dCMEntries.Count == mSGEntries.Count)
            {
                foreach (var dcmEntry in dCMEntries)
                {
                    var currentActivityID = dcmEntry.Activity_id;
                    MSGEntry msgEntry = mSGEntries.Find(x => x.activityID == currentActivityID);
                    List<CGENEntry> cgenEntry = cgenEntries.FindAll(x => x.activity_id == currentActivityID);

                    //  populateGridViewData(msgEntry, dcmEntry);
                    populateGridViewData(msgEntry, dcmEntry, cgenEntry);
                }

                if (gridViewData.Count != 0)
                {
                    populateMessages.DataSource = gridViewData;
                    populateMessages.DataBind();
                }
                else
                {
                    Label1.Text = "No data found";
                }
            }

            else
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "alertMessage", "alert('Error in file upload')", true);
            }
        }

        public List<MSGEntry> loadMsgList()
        {

            string[] filepaths = Directory.GetFiles((Server.MapPath(Common.GetCurrentPath("Emails"))));

            foreach (string filepath in filepaths)
            {
                HtmlDocument doc = new HtmlDocument();
                using (var msg = new Storage.Message(filepath))
                {
                    doc.LoadHtml(msg.BodyHtml);

                    mSGEntries.Add(new MSGEntry()
                    {
                        activityID = msg.Subject.Substring(0, msg.Subject.IndexOf("-")).Trim(),
                        subjectLine = msg.Subject.Substring(msg.Subject.IndexOf("-") + 1).Trim(),
                        preHeader = doc.GetElementbyId("preHeader") != null ? doc.GetElementbyId("preHeader").InnerText.Replace("&nbsp;", " ").ToString() : "NA",

                        headerCopy = doc.GetElementbyId("abc") != null ? doc.GetElementbyId("headerCopy").InnerText.Replace("&nbsp;", " ").ToString() : "NA",
                        headerBodyCopy = doc.GetElementbyId("headerBodyCopy") != null ? doc.GetElementbyId("headerBodyCopy").InnerText.Replace("&nbsp;", " ").ToString() : "NA",
                        headerImage = doc.GetElementbyId("headerImage")!= null ? doc.GetElementbyId("headerImage").GetAttributes("src").First().Value : "NA",
                        headerImageLink = doc.GetElementbyId("headerImageLink") != null ? doc.GetElementbyId("headerImageLink").GetAttributes("href").First().Value : "NA",
                        headerCTA = doc.GetElementbyId("headerCTA") != null ? doc.GetElementbyId("headerCTA").InnerText.Replace("&nbsp;", " ").ToString() :"NA",
                        headerCTALink = doc.GetElementbyId("headerCTALink") != null ? doc.GetElementbyId("headerCTALink").GetAttributes("href").First().Value : "NA",

                        pod1headerCopy = doc.GetElementbyId("pod1headerCopy") != null ? doc.GetElementbyId("pod1headerCopy").InnerText.Replace("&nbsp;", " ").ToString() : "NA",
                        pod1BodyCopy = doc.GetElementbyId("pod1BodyCopy") != null ? doc.GetElementbyId("pod1BodyCopy").InnerText.Replace("&#8209;", "").Replace("&nbsp;", " ").ToString() : "NA",
                        pod1Image = doc.GetElementbyId("pod1Image") != null ? doc.GetElementbyId("pod1Image").GetAttributes("src").First().Value : "NA",
                        pod1ImageLink = doc.GetElementbyId("pod1ImageLink")!= null ? doc.GetElementbyId("pod1ImageLink").GetAttributes("href").First().Value :"NA",
                        pod1CTA = doc.GetElementbyId("pod1CTA") != null ? doc.GetElementbyId("pod1CTA").InnerText.Replace("&nbsp;", " ").ToString() : "NA",
                        pod1CTALink = doc.GetElementbyId("pod1CTALink") != null ? doc.GetElementbyId("pod1CTALink").GetAttributes("href").First().Value : "NA",

                        pod2headerCopy = doc.GetElementbyId("pod2headerCopy") != null ? doc.GetElementbyId("pod2headerCopy").InnerText.Replace("&nbsp;", " ").ToString() : "NA",
                        pod2BodyCopy = doc.GetElementbyId("pod2BodyCopy") != null ? doc.GetElementbyId("pod2BodyCopy").InnerText.Replace("&#8209;", "").Replace("&nbsp;", " ").ToString() : "NA",
                        pod2Image = doc.GetElementbyId("pod2Image") != null ? doc.GetElementbyId("pod2Image").GetAttributes("src").First().Value : "NA",
                        pod2ImageLink = doc.GetElementbyId("pod2ImageLink") != null ? doc.GetElementbyId("pod2ImageLink").GetAttributes("href").First().Value : "NA",
                        pod2CTA = doc.GetElementbyId("pod2CTA") != null ? doc.GetElementbyId("pod2CTA").InnerText.Replace("&nbsp;", " ").ToString() : "NA",
                        pod2CTALink = doc.GetElementbyId("pod2CTALink") != null ? doc.GetElementbyId("pod2CTALink").GetAttributes("href").First().Value : "NA"
                    });
                }
            }
            return mSGEntries;
        }

        public List<CGENEntry> loadCGENList()
        {
            string[] filepath = Directory.GetFiles((Server.MapPath("~/UploadedFiles/CGEN/")));

            using (ExcelPackage package = new ExcelPackage(filepath[0]))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                int colCount = worksheet.Dimension.End.Column;
                int rowCount = worksheet.Dimension.End.Row;
                for (int row = 2; row <= rowCount; row++) //skip header
                {
                    cGENEntries.Add(new CGENEntry()
                    {
                        activity_id = worksheet.Cells[row, 3].Value.ToString().Trim(),
                        tag_id = worksheet.Cells[row, 8].Value.ToString().Trim(),
                        tag_name = worksheet.Cells[row, 9].Value.ToString().Trim(),
                        tag_full_url = worksheet.Cells[row, 13].Value.ToString().Trim()
                    });
                }
            }
            return cGENEntries;
        }

        public void populateGridViewData(MSGEntry msgEntry, DCMEntry dcmEntry, List<CGENEntry> cgenEntries)
        {
            gridViewData.Add(new GridViewData()
            {
                activityId = dcmEntry.Activity_id.Trim(),
                elementType = "Subject Line",
                DCMValue = dcmEntry.Subject_Line,
                MSGValue = msgEntry.subjectLine,
                result = dcmEntry.Subject_Line == msgEntry.subjectLine ? "Subject Line Matched" : "Subject Line Not matched"
            });
            gridViewData.Add(new GridViewData()
            {
                activityId = dcmEntry.Activity_id.Trim(),
                elementType = "Pre-Header",
                DCMValue = dcmEntry.Pre_header,
                MSGValue = msgEntry.preHeader,
                result = dcmEntry.Pre_header == msgEntry.preHeader ? "Pre Header Matched" : "Pre Header Not matched"
            });

            if(dcmEntry.Header != "NA" || msgEntry.headerCopy != "NA")
            {
                gridViewData.Add(new GridViewData()
                {
                    activityId = dcmEntry.Activity_id.Trim(),
                    elementType = "HeaderCopy",
                    DCMValue = dcmEntry.Header,
                    MSGValue = msgEntry.headerCopy,
                    result = dcmEntry.Header == msgEntry.headerCopy ? "Header Matched" : "Header Not matched"
                });
            }
            
            gridViewData.Add(new GridViewData()
            {
                activityId = dcmEntry.Activity_id.Trim(),
                elementType = "HeaderBodyCopy",
                DCMValue = dcmEntry.Header_body_copy,
                MSGValue = msgEntry.headerBodyCopy,
                result = dcmEntry.Header_body_copy == msgEntry.headerBodyCopy ? "Header Matched" : "Header Not matched"
            });

            //pod1
            gridViewData.Add(new GridViewData()
            {
                activityId = dcmEntry.Activity_id.Trim(),
                elementType = "Pod1HeaderCopy",
                DCMValue = dcmEntry.Pod1_header,
                MSGValue = msgEntry.pod1headerCopy,
                result = dcmEntry.Pod1_header == msgEntry.pod1headerCopy ? "pod1headerCopy Matched" : "pod1headerCopy Not matched"
            });
            gridViewData.Add(new GridViewData()
            {
                activityId = dcmEntry.Activity_id.Trim(),
                elementType = "Pod1BodyCopy",
                DCMValue = dcmEntry.Pod1_body_copy,
                MSGValue = msgEntry.pod1BodyCopy,
                result = dcmEntry.Pod1_body_copy == msgEntry.pod1BodyCopy ? "pod1BodyCopy Matched" : "pod1BodyCopy Not matched"
            });
            gridViewData.Add(new GridViewData()
            {
                activityId = dcmEntry.Activity_id.Trim(),
                elementType = "Pod1Image",
                DCMValue = dcmEntry.Pod1_image,
                MSGValue = msgEntry.pod1Image,
                result = dcmEntry.Pod1_image == msgEntry.pod1Image ? "pod1Image Matched" : "pod1Image Not matched"
            });

            var abc = GetTagIdAndFullUrl(msgEntry.pod1ImageLink);
            var cgen = cgenEntries.Find(x => x.tag_id == abc.Item2);
            gridViewData.Add(new GridViewData()
            {
                activityId = dcmEntry.Activity_id.Trim(),
                elementType = "Pod1ImageLink",
                DCMValue = cgen.tag_full_url,
                MSGValue = abc.Item1,
                result = cgen.tag_full_url == msgEntry.pod1ImageLink ? "Pod1ImageLink matched" : "Pod1ImageLink Not matched"
            });
            gridViewData.Add(new GridViewData()
            {
                activityId = dcmEntry.Activity_id.Trim(),
                elementType = "Pod1CTALink",
                DCMValue = cgen.tag_full_url,
                MSGValue = GetTagIdAndFullUrl(msgEntry.pod1CTALink).Item1,
                result = cgen.tag_full_url == msgEntry.pod1CTALink ? "Pod1CTALink matched" : "Pod1CTALink Not matched"
            });
            //gridViewData.Add(new GridViewData()
            //{
            //    activityId = dcmEntry.Activity_id.Trim(),
            //    elementType = "Pod1CTAText",
            //    DCMValue = dcmEntry.CTA_button1_text,
            //    MSGValue = msgEntry.pod1CTA,
            //    result = dcmEntry.CTA_button1_text == msgEntry.pod1CTA ? "Pod1CTAText matched" : "Pod1CTAText Not matched"
            //});
            Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "Scroll()", true);
        }

        public string GetHrefValueByHtmlId(string v, HtmlDocument document)
        {
            string value;
            var linkDoc = new HtmlDocument();
            linkDoc.LoadHtml(document.GetElementbyId(v).InnerHtml);

            value = linkDoc.DocumentNode.Descendants("a").First().Attributes["href"].Value;
            return value;

        }


        public List<DCMEntry> loadDCMList()
        {
            if (File.Exists(dcmPath))
            {
                List<string> DCMArrayList = File.ReadAllLines(dcmPath).ToList();
                int lineNumber = 0;

                foreach (var DCMObject in DCMArrayList)
                {
                    if (lineNumber != 0)
                    {
                        string[] entry = DCMObject.Split('\t');

                        dCMEntries.Add(new DCMEntry
                        {
                            //Activity_name = entry[0].ToString(),
                            //Segment = entry[1].ToString(),
                            //Locale = entry[2].ToString(),
                            //day = entry[3].ToString(),
                            //AB_version = entry[4].ToString(),
                            Activity_id = entry[5].ToString(),
                            //Template = entry[6].ToString(),
                            Subject_Line = entry[7].ToString(),
                            Pre_header = entry[8].ToString(),
                            //Product_logo_image = entry[9].ToString(),
                            //Product_logo_alt_tag_ = entry[10].ToString(),
                            //Header = entry[11] != null ? entry[11].ToString() : "NA",
                            Header = "NA",
                            Header_body_copy = entry[12].ToString(),
                            Pod1_header = entry[13].ToString(),
                            Pod1_body_copy = entry[14].ToString(),
                            //CTA_button1_text = entry[15].ToString(),
                            //CTA_button1_tag_name = entry[16].ToString(),
                            //CTA_button1_target = entry[17].ToString(),
                            //CTA_link1_text = entry[18].ToString(),
                            //CTA_link1_tag_name = entry[19].ToString(),
                            //CTA_link1_target = entry[20].ToString(),
                            Pod1_image = entry[21].ToString()
                            //Pod1_image_width = entry[22].ToString(),
                            //Pod1_image_alt_tag = entry[23].ToString(),
                            //Pod2_header = entry[24].ToString(),
                            //Pod2_body_copy = entry[25].ToString(),
                            //CTA_button2_text = entry[26].ToString(),
                            //CTA_button2_tag_name = entry[27].ToString(),
                            //CTA_button2_target = entry[28].ToString(),
                            //CTA_link2_text = entry[29].ToString(),
                            //CTA_link2_tag_name = entry[30].ToString(),
                            //CTA_link2_target = entry[31].ToString(),
                            //Pod2_image = entry[32].ToString(),
                            //Pod2_image_width = entry[33].ToString(),
                            //Pod2_image_alt_tag = entry[34].ToString(),
                            //Pod3_header = entry[35].ToString(),
                            //Pod3_body_copy = entry[36].ToString(),
                            //CTA_button3_text = entry[37].ToString(),
                            //CTA_button3_tag_name = entry[38].ToString(),
                            //CTA_button3_target = entry[39].ToString(),
                            //CTA_link3_text = entry[40].ToString(),
                            //CTA_link3_tag_name = entry[41].ToString(),
                            //CTA_link3_target = entry[42].ToString(),
                            //Pod3_image = entry[43].ToString(),
                            //Pod3_image_width = entry[44].ToString(),
                            //Pod3_image_alt_tag = entry[45].ToString(),
                            //Pod4_header = entry[46].ToString(),
                            //Pod4_body_copy = entry[47].ToString(),
                            //CTA_button4_text = entry[48].ToString(),
                            //CTA_button4_tag_name = entry[49].ToString(),
                            //CTA_button4_target = entry[50].ToString(),
                            //CTA_link4_text = entry[51].ToString(),
                            //CTA_link4_tag_name = entry[52].ToString(),
                            //CTA_link4_target = entry[53].ToString(),
                            //Pod4_image = entry[54].ToString(),
                            //Pod4_image_width = entry[55].ToString(),
                            //Pod4_image_alt_tag = entry[56].ToString(),
                            //Trademark = entry[57].ToString(),
                            //Legal = entry[58].ToString()
                        });
                    }
                    lineNumber += 1;

                }
            }
            return dCMEntries;
        }

        private bool IsDirectoryClearingNeeded(string cuurPath, string v)
        {
            bool isClearingNeeded = false;
            string currRelativePath = Server.MapPath(cuurPath);

            try
            {
                if (Directory.Exists(currRelativePath))
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
                else
                {
                    Directory.CreateDirectory(currRelativePath);

                    isClearingNeeded = true;
                }
            }
            catch (System.Exception ex)
            {
                Label1.Text = ex.Message + "error" + ex;
            }

            return isClearingNeeded;
        }
        public static Tuple<string, string> GetTagIdAndFullUrl(string url)
        {
            Uri ourUri = new Uri(url);
            WebRequest myWebRequest = WebRequest.Create(url);
            WebResponse myWebResponse = myWebRequest.GetResponse();
            Uri myUri = new Uri(myWebResponse.ResponseUri.ToString());
            string currentMsgTagId = HttpUtility.ParseQueryString(myUri.Query).Get("trackingid");

            return Tuple.Create(myWebResponse.ResponseUri.ToString(), currentMsgTagId);
        }

        
        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                string message = e.Row.Cells[4].Text;

                foreach (TableCell cell in e.Row.Cells)
                {
                    string msg = message.Substring(message.Length - 11);
                    if (message.Substring(message.Length - 11) == "Not matched")
                    {

                        e.Row.Cells[4].ForeColor = Color.IndianRed;
                    }
                    else
                    {
                        e.Row.Cells[4].ForeColor = Color.GreenYellow;
                    }


                }
            }
        }

    }
}