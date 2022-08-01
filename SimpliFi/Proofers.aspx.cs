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
        public int errorCount = 0, successCount = 0;        
        public static string dcmPath = string.Empty;
        public List<DCMEntry> dCMEntries = new List<DCMEntry>();
        public List<MSGEntry> mSGEntries = new List<MSGEntry>();
        public List<CGENEntry> cGENEntries = new List<CGENEntry>();
        List<GridViewData> gridViewData = new List<GridViewData>();
        Tuple<Uri, string> tagUrlTuple;
        CGENEntry cgen;
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
                    Label2.Text = FileUpload2.PostedFiles.Count.ToString() + " Files uploaded";
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
            //dcmPath
            //if(!isDCMUploadSuccessfull && !isMSGUploadSuccessfull && !isCGENUploadSuccessfull)
            //{
            //    Label5.Text = "Please upload all the files correctly..";
            //    return;
            //}
            if (!File.Exists(Common.GetCurrentPath("DCMFile")))
            {
                Label5.Text = "Please upload all the files correctly..";
                return;
            }

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

                    // populateGridViewData(msgEntry, dcmEntry);
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
            Page.ClientScript.RegisterStartupScript(this.GetType(), "CallFunction", "Scroll()", true);
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

                        preHeader = doc.GetElementbyId("preHeader") != null ? doc.GetElementbyId("preHeader").InnerText.Replace("&nbsp;", string.Empty) : "NA",

                        headerCopy = doc.GetElementbyId("headerCopy") != null ? doc.GetElementbyId("headerCopy").InnerText.Replace("&nbsp;", " ").ToString().Trim() : "NA",
                        headerBodyCopy = doc.GetElementbyId("headerBodyCopy") != null ? doc.GetElementbyId("headerBodyCopy").InnerText.Replace("&nbsp;", " ").ToString().Trim() : "NA",
                        headerImage = doc.GetElementbyId("headerImage") != null ? doc.GetElementbyId("headerImage").GetAttributes("src").First().Value : "NA",
                        headerImageLink = doc.GetElementbyId("headerImageLink") != null ? doc.GetElementbyId("headerImageLink").GetAttributes("href").First().Value : "NA",
                        headerCTA = doc.GetElementbyId("headerCTA") != null ? doc.GetElementbyId("headerCTA").InnerText.Replace("&nbsp;", " ").ToString().Trim() : "NA",
                        headerCTALink = doc.GetElementbyId("headerCTALink") != null ? doc.GetElementbyId("headerCTALink").GetAttributes("href").First().Value : "NA",
                        // POD1
                        pod1headerCopy = doc.GetElementbyId("pod1headerCopy") != null ? doc.GetElementbyId("pod1headerCopy").InnerText.Replace("&nbsp;", " ").ToString().Trim() : "NA",
                        pod1BodyCopy = doc.GetElementbyId("pod1BodyCopy") != null ? doc.GetElementbyId("pod1BodyCopy").InnerText.Replace("&#8209;", "").Replace("&nbsp;", " ").ToString().Trim() : "NA",
                        pod1Image = doc.GetElementbyId("pod1Image") != null ? doc.GetElementbyId("pod1Image").GetAttributes("src").First().Value : "NA",
                        pod1ImageLink = doc.GetElementbyId("pod1ImageLink") != null ? doc.GetElementbyId("pod1ImageLink").GetAttributes("href").First().Value : "NA",
                        pod1CTA = doc.GetElementbyId("pod1CTA") != null ? doc.GetElementbyId("pod1CTA").InnerText.Replace("&nbsp;", " ").ToString().Trim() : "NA",
                        pod1CTALink = doc.GetElementbyId("pod1CTALink") != null ? doc.GetElementbyId("pod1CTALink").GetAttributes("href").First().Value : "NA",
                        // POD2
                        pod2headerCopy = doc.GetElementbyId("pod2headerCopy") != null ? doc.GetElementbyId("pod2headerCopy").InnerText.Replace("&nbsp;", " ").ToString().Trim() : "NA",
                        pod2BodyCopy = doc.GetElementbyId("pod2BodyCopy") != null ? doc.GetElementbyId("pod2BodyCopy").InnerText.Replace("&#8209;", "").Replace("&nbsp;", " ").ToString().Trim() : "NA",
                        pod2Image = doc.GetElementbyId("pod2Image") != null ? doc.GetElementbyId("pod2Image").GetAttributes("src").First().Value : "NA",
                        pod2ImageLink = doc.GetElementbyId("pod2ImageLink") != null ? doc.GetElementbyId("pod2ImageLink").GetAttributes("href").First().Value : "NA",
                        pod2CTA = doc.GetElementbyId("pod2CTA") != null ? doc.GetElementbyId("pod2CTA").InnerText.Replace("&nbsp;", " ").ToString().Trim() : "NA",
                        pod2CTALink = doc.GetElementbyId("pod2CTALink") != null ? doc.GetElementbyId("pod2CTALink").GetAttributes("href").First().Value : "NA",
                        //POD3
                        pod3headerCopy = doc.GetElementbyId("pod3headerCopy") != null ? doc.GetElementbyId("pod3headerCopy").InnerText.Replace("&nbsp;", " ").ToString().Trim() : "NA",
                        pod3BodyCopy = doc.GetElementbyId("pod3BodyCopy") != null ? doc.GetElementbyId("pod3BodyCopy").InnerText.Replace("&#8209;", "").Replace("&nbsp;", " ").ToString().Trim() : "NA",
                        pod3Image = doc.GetElementbyId("pod3Image") != null ? doc.GetElementbyId("pod3Image").GetAttributes("src").First().Value : "NA",
                        pod3ImageLink = doc.GetElementbyId("pod3ImageLink") != null ? doc.GetElementbyId("pod3ImageLink").GetAttributes("href").First().Value : "NA",
                        pod3CTA = doc.GetElementbyId("pod3CTA") != null ? doc.GetElementbyId("pod3CTA").InnerText.Replace("&nbsp;", " ").ToString().Trim() : "NA",
                        pod3CTALink = doc.GetElementbyId("pod3CTALink") != null ? doc.GetElementbyId("pod3CTALink").GetAttributes("href").First().Value : "NA",
                        //POD4
                        pod4headerCopy = doc.GetElementbyId("pod4headerCopy") != null ? doc.GetElementbyId("pod4headerCopy").InnerText.Replace("&nbsp;", " ").ToString().Trim() : "NA",
                        pod4BodyCopy = doc.GetElementbyId("pod4BodyCopy") != null ? doc.GetElementbyId("pod4BodyCopy").InnerText.Replace("&#8209;", "").Replace("&nbsp;", " ").ToString().Trim() : "NA",
                        pod4Image = doc.GetElementbyId("pod4Image") != null ? doc.GetElementbyId("pod4Image").GetAttributes("src").First().Value : "NA",
                        pod4ImageLink = doc.GetElementbyId("pod4ImageLink") != null ? doc.GetElementbyId("pod4ImageLink").GetAttributes("href").First().Value : "NA",
                        pod4CTA = doc.GetElementbyId("pod4CTA") != null ? doc.GetElementbyId("pod4CTA").InnerText.Replace("&nbsp;", " ").ToString().Trim() : "NA",
                        pod4CTALink = doc.GetElementbyId("pod4CTALink") != null ? doc.GetElementbyId("pod4CTALink").GetAttributes("href").First().Value : "NA",
                        //POD5
                        pod5headerCopy = doc.GetElementbyId("pod5headerCopy") != null ? doc.GetElementbyId("pod5headerCopy").InnerText.Replace("&nbsp;", " ").ToString().Trim() : "NA",
                        pod5BodyCopy = doc.GetElementbyId("pod5BodyCopy") != null ? doc.GetElementbyId("pod5BodyCopy").InnerText.Replace("&#8209;", "").Replace("&nbsp;", " ").ToString().Trim() : "NA",
                        pod5Image = doc.GetElementbyId("pod5Image") != null ? doc.GetElementbyId("pod5Image").GetAttributes("src").First().Value : "NA",
                        pod5ImageLink = doc.GetElementbyId("pod5ImageLink") != null ? doc.GetElementbyId("pod5ImageLink").GetAttributes("href").First().Value : "NA",
                        pod5CTA = doc.GetElementbyId("pod5CTA") != null ? doc.GetElementbyId("pod5CTA").InnerText.Replace("&nbsp;", " ").ToString().Trim() : "NA",
                        pod5CTALink = doc.GetElementbyId("pod5CTALink") != null ? doc.GetElementbyId("pod5CTALink").GetAttributes("href").First().Value : "NA",
                        //POD6
                        pod6headerCopy = doc.GetElementbyId("pod6headerCopy") != null ? doc.GetElementbyId("pod6headerCopy").InnerText.Replace("&nbsp;", " ").ToString().Trim() : "NA",
                        pod6BodyCopy = doc.GetElementbyId("pod6BodyCopy") != null ? doc.GetElementbyId("pod6BodyCopy").InnerText.Replace("&#8209;", "").Replace("&nbsp;", " ").ToString().Trim() : "NA",
                        pod6Image = doc.GetElementbyId("pod6Image") != null ? doc.GetElementbyId("pod6Image").GetAttributes("src").First().Value : "NA",
                        pod6ImageLink = doc.GetElementbyId("pod6ImageLink") != null ? doc.GetElementbyId("pod6ImageLink").GetAttributes("href").First().Value : "NA",
                        pod6CTA = doc.GetElementbyId("pod6CTA") != null ? doc.GetElementbyId("pod6CTA").InnerText.Replace("&nbsp;", " ").ToString().Trim() : "NA",
                        pod6CTALink = doc.GetElementbyId("pod6CTALink") != null ? doc.GetElementbyId("pod6CTALink").GetAttributes("href").First().Value : "NA"

                    });
                    ;
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
                        tag_url = worksheet.Cells[row, 11].Value.ToString().Trim(),
                        tag_full_url = worksheet.Cells[row, 13].Value.ToString().Trim()
                    });
                }
            }
            return cGENEntries;
        }

        public void populateGridViewData(MSGEntry msgEntry, DCMEntry dcmEntry, List<CGENEntry> cgenEntries)
        {
            // Subject Line
            gridViewData.Add(new GridViewData()
            {
                activityId = dcmEntry.Activity_id.Trim(),
                elementType = "Subject Line",
                DCMValue = dcmEntry.Subject_Line,
                MSGValue = msgEntry.subjectLine,
                result = dcmEntry.Subject_Line == msgEntry.subjectLine ? "Subject Line Matched" : "Subject Line Not matched"
            });
            // Pre-Header
            if (dcmEntry.Pre_header != "NA" || msgEntry.preHeader != "NA")
            {
                gridViewData.Add(new GridViewData()
                {
                    activityId = dcmEntry.Activity_id.Trim(),
                    elementType = "Pre-Header",
                    DCMValue = dcmEntry.Pre_header,
                    MSGValue = msgEntry.preHeader,
                    result = string.Equals(dcmEntry.Pre_header.ToString().Replace(" ", ""), msgEntry.preHeader.ToString().Replace(" ", ""), StringComparison.InvariantCultureIgnoreCase) ? "Pre Header Matched" : "Pre Header Not matched"
                });

            }
            // Header Copy
            if (dcmEntry.Header != "NA" || msgEntry.headerCopy != "NA")
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
            // Header Body Copy
            if (dcmEntry.Header_body_copy != "NA" || msgEntry.headerBodyCopy != "NA")
            {
                gridViewData.Add(new GridViewData()
                {
                    activityId = dcmEntry.Activity_id.Trim(),
                    elementType = "HeaderBodyCopy",
                    DCMValue = dcmEntry.Header_body_copy,
                    MSGValue = msgEntry.headerBodyCopy,
                    result = dcmEntry.Header_body_copy == msgEntry.headerBodyCopy ? "Header Body Copy Matched" : "Header Body Copy Not matched"
                });
            }
            // pod1headerCopy
            if (dcmEntry.Pod1_header != "NA" || msgEntry.pod1headerCopy != "NA")
            {
                gridViewData.Add(new GridViewData()
                {
                    activityId = dcmEntry.Activity_id.Trim(),
                    elementType = "Pod1HeaderCopy",
                    DCMValue = dcmEntry.Pod1_header,
                    MSGValue = msgEntry.pod1headerCopy,
                    result = dcmEntry.Pod1_header == msgEntry.pod1headerCopy ? "pod1headerCopy Matched" : "pod1headerCopy Not matched"
                });
            }
            //Pod1BodyCopy
            if (dcmEntry.Pod1_body_copy != "NA" || msgEntry.pod1BodyCopy != "NA")
            {

                gridViewData.Add(new GridViewData()
                {
                    activityId = dcmEntry.Activity_id.Trim(),
                    elementType = "Pod1BodyCopy",
                    DCMValue = dcmEntry.Pod1_body_copy,
                    MSGValue = msgEntry.pod1BodyCopy,
                    result = dcmEntry.Pod1_body_copy == msgEntry.pod1BodyCopy ? "pod1BodyCopy Matched" : "pod1BodyCopy Not matched"
                });
            }
            // Pod1Image
            if (dcmEntry.Pod1_image != "NA" || msgEntry.pod1Image != "NA")
            {
                gridViewData.Add(new GridViewData()
                {
                    activityId = dcmEntry.Activity_id.Trim(),
                    elementType = "Pod1Image",
                    DCMValue = dcmEntry.Pod1_image,
                    MSGValue = msgEntry.pod1Image,
                    result = dcmEntry.Pod1_image == msgEntry.pod1Image ? "pod1Image Matched" : "pod1Image Not matched"
                });
            }
            // POD1 CTA
            if (dcmEntry.CTA_button1_text != "NA" || msgEntry.pod1CTA != "NA")
            {
                gridViewData.Add(new GridViewData()
                {
                    activityId = dcmEntry.Activity_id.Trim(),
                    elementType = "Pod1CTA",
                    DCMValue = dcmEntry.CTA_button1_text,
                    MSGValue = msgEntry.pod1CTA,
                    result = dcmEntry.CTA_button1_text == msgEntry.pod1CTA ? "pod1Image Matched" : "pod1Image Not matched"
                });
            }

            //Pod1ImageLink            
            if (msgEntry.pod1ImageLink != "NA")
            {
                tagUrlTuple = GetTagIdAndFullUrl(msgEntry.pod1ImageLink);
                cgen = cgenEntries.Find(x => x.tag_id == tagUrlTuple.Item2);

                gridViewData.Add(new GridViewData()
                {
                    activityId = dcmEntry.Activity_id.Trim(),
                    elementType = "Pod1ImageLink",
                    DCMValue = cgen.tag_full_url,
                    MSGValue = tagUrlTuple.Item1.ToString(),
                    result = cgen.tag_url == tagUrlTuple.Item1.GetLeftPart(UriPartial.Path) && cgen.tag_id == tagUrlTuple.Item2 ? "Pod1ImageLink matched" : "Pod1ImageLink Not matched"
                });
            }

            //Pod1CTALink
            if (msgEntry.pod1CTALink != "NA")
            {
                tagUrlTuple = null; //initializing the Tuple
                tagUrlTuple = GetTagIdAndFullUrl(msgEntry.pod1CTALink);            

                gridViewData.Add(new GridViewData()
                {
                    activityId = dcmEntry.Activity_id.Trim(),
                    elementType = "Pod1CTALink",
                    DCMValue = cgen.tag_full_url,
                    MSGValue = tagUrlTuple.Item1.ToString(),
                    result = cgen.tag_url == tagUrlTuple.Item1.GetLeftPart(UriPartial.Path) && cgen.tag_id == tagUrlTuple.Item2 ? "Pod1CTALink matched" : "Pod1CTALink Not matched"
                });
            }

            // Pod2headerCopy
            if (dcmEntry.Pod2_header != "NA" || msgEntry.pod2headerCopy != "NA")
            {
                gridViewData.Add(new GridViewData()
                {
                    activityId = dcmEntry.Activity_id.Trim(),
                    elementType = "Pod2HeaderCopy",
                    DCMValue = dcmEntry.Pod2_header,
                    MSGValue = msgEntry.pod2headerCopy,
                    result = dcmEntry.Pod2_header == msgEntry.pod2headerCopy ? "Pod2headerCopy Matched" : "Pod2headerCopy Not matched"
                });
            }

            //Pod2BodyCopy
            if (dcmEntry.Pod2_body_copy != "NA" || msgEntry.pod2BodyCopy != "NA")
            {

                gridViewData.Add(new GridViewData()
                {
                    activityId = dcmEntry.Activity_id.Trim(),
                    elementType = "Pod2BodyCopy",
                    DCMValue = dcmEntry.Pod2_body_copy,
                    MSGValue = msgEntry.pod2BodyCopy,
                    result = dcmEntry.Pod2_body_copy == msgEntry.pod2BodyCopy ? "Pod2BodyCopy Matched" : "Pod2BodyCopy Not matched"
                });
            }
            // Pod2Image
            if (dcmEntry.Pod2_image != "NA" || msgEntry.pod2Image != "NA")
            {
                gridViewData.Add(new GridViewData()
                {
                    activityId = dcmEntry.Activity_id.Trim(),
                    elementType = "Pod2Image",
                    DCMValue = dcmEntry.Pod2_image,
                    MSGValue = msgEntry.pod2Image,
                    result = dcmEntry.Pod2_image == msgEntry.pod2Image ? "Pod2Image Matched" : "Pod2Image Not matched"
                });
            }
            // Pod2 CTA
            if (dcmEntry.CTA_button1_text != "NA" || msgEntry.pod2CTA != "NA")
            {
                gridViewData.Add(new GridViewData()
                {
                    activityId = dcmEntry.Activity_id.Trim(),
                    elementType = "Pod2CTA",
                    DCMValue = dcmEntry.CTA_button1_text,
                    MSGValue = msgEntry.pod2CTA,
                    result = dcmEntry.CTA_button1_text == msgEntry.pod2CTA ? "Pod2Image Matched" : "Pod2Image Not matched"
                });
            }



            // Pod3headerCopy
            if (dcmEntry.Pod3_header != "NA" || msgEntry.pod3headerCopy != "NA")
            {
                gridViewData.Add(new GridViewData()
                {
                    activityId = dcmEntry.Activity_id.Trim(),
                    elementType = "Pod3HeaderCopy",
                    DCMValue = dcmEntry.Pod3_header,
                    MSGValue = msgEntry.pod3headerCopy,
                    result = dcmEntry.Pod3_header == msgEntry.pod3headerCopy ? "Pod3headerCopy Matched" : "Pod3headerCopy Not matched"
                });
            }
            //Pod3BodyCopy
            if (dcmEntry.Pod3_body_copy != "NA" || msgEntry.pod3BodyCopy != "NA")
            {

                gridViewData.Add(new GridViewData()
                {
                    activityId = dcmEntry.Activity_id.Trim(),
                    elementType = "Pod3BodyCopy",
                    DCMValue = dcmEntry.Pod3_body_copy,
                    MSGValue = msgEntry.pod3BodyCopy,
                    result = dcmEntry.Pod3_body_copy == msgEntry.pod3BodyCopy ? "Pod3BodyCopy Matched" : "Pod3BodyCopy Not matched"
                });
            }
            // Pod3Image
            if (dcmEntry.Pod3_image != "NA" || msgEntry.pod3Image != "NA")
            {
                gridViewData.Add(new GridViewData()
                {
                    activityId = dcmEntry.Activity_id.Trim(),
                    elementType = "Pod3Image",
                    DCMValue = dcmEntry.Pod3_image,
                    MSGValue = msgEntry.pod3Image,
                    result = dcmEntry.Pod3_image == msgEntry.pod3Image ? "Pod3Image Matched" : "Pod3Image Not matched"
                });
            }
            // Pod3 CTA
            if (dcmEntry.CTA_button1_text != "NA" || msgEntry.pod3CTA != "NA")
            {
                gridViewData.Add(new GridViewData()
                {
                    activityId = dcmEntry.Activity_id.Trim(),
                    elementType = "Pod3CTA",
                    DCMValue = dcmEntry.CTA_button1_text,
                    MSGValue = msgEntry.pod3CTA,
                    result = dcmEntry.CTA_button1_text == msgEntry.pod3CTA ? "Pod3Image Matched" : "Pod3Image Not matched"
                });
            }


            // Pod4headerCopy
            if (dcmEntry.Pod4_header != "NA" || msgEntry.pod4headerCopy != "NA")
            {
                gridViewData.Add(new GridViewData()
                {
                    activityId = dcmEntry.Activity_id.Trim(),
                    elementType = "Pod4HeaderCopy",
                    DCMValue = dcmEntry.Pod4_header,
                    MSGValue = msgEntry.pod4headerCopy,
                    result = dcmEntry.Pod4_header == msgEntry.pod4headerCopy ? "Pod4headerCopy Matched" : "Pod4headerCopy Not matched"
                });
            }
            //Pod4BodyCopy
            if (dcmEntry.Pod4_body_copy != "NA" || msgEntry.pod4BodyCopy != "NA")
            {

                gridViewData.Add(new GridViewData()
                {
                    activityId = dcmEntry.Activity_id.Trim(),
                    elementType = "Pod4BodyCopy",
                    DCMValue = dcmEntry.Pod4_body_copy,
                    MSGValue = msgEntry.pod4BodyCopy,
                    result = dcmEntry.Pod4_body_copy == msgEntry.pod4BodyCopy ? "Pod4BodyCopy Matched" : "Pod4BodyCopy Not matched"
                });
            }
            // Pod4Image
            if (dcmEntry.Pod4_image != "NA" || msgEntry.pod4Image != "NA")
            {
                gridViewData.Add(new GridViewData()
                {
                    activityId = dcmEntry.Activity_id.Trim(),
                    elementType = "Pod4Image",
                    DCMValue = dcmEntry.Pod4_image,
                    MSGValue = msgEntry.pod4Image,
                    result = dcmEntry.Pod4_image == msgEntry.pod4Image ? "Pod4Image Matched" : "Pod4Image Not matched"
                });
            }
            // Pod4 CTA
            if (dcmEntry.CTA_button1_text != "NA" || msgEntry.pod4CTA != "NA")
            {
                gridViewData.Add(new GridViewData()
                {
                    activityId = dcmEntry.Activity_id.Trim(),
                    elementType = "Pod4CTA",
                    DCMValue = dcmEntry.CTA_button1_text,
                    MSGValue = msgEntry.pod4CTA,
                    result = dcmEntry.CTA_button1_text == msgEntry.pod4CTA ? "Pod4Image Matched" : "Pod4Image Not matched"
                });
            }

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
                            Activity_name = entry[0] != null ? entry[0].Replace("\"", "").Trim() : "NA",
                            Segment = entry[1] != null ? entry[1].Replace("\"", "").Trim() : "NA",
                            Locale = entry[2] != null ? entry[2].Replace("\"", "").Trim() : "NA",
                            day = entry[3] != null ? entry[3].Replace("\"", "").Trim() : "NA",
                            AB_version = entry[4] != null ? entry[4].Replace("\"", "").Trim() : "NA",
                            Activity_id = entry[5] != null ? entry[5].Replace("\"", "").Trim() : "NA",
                            Template = entry[6] != null ? entry[6].Replace("\"", "").Trim() : "NA",
                            Subject_Line = entry[7] != null ? entry[7].Replace("\"", "").Trim() : "NA",
                            Pre_header = entry[8] != null ? entry[8].Replace("\"", "").Trim() : "NA",
                            Product_logo_image = entry[9] != null ? entry[9].Replace("\"", "").Trim() : "NA",
                            Product_logo_alt_tag_ = entry[10] != null ? entry[10].Replace("\"", "").Trim() : "NA",
                            Header = entry[11] != null ? entry[11].Replace("\"", "").Trim() : "NA",
                            Header_body_copy = entry[12] != null ? entry[12].Replace("\"", "").Trim() : "NA",
                            Pod1_header = entry[13] != null ? entry[13].Replace("\"", "").Trim() : "NA",
                            Pod1_body_copy = entry[14] != null ? entry[14].Replace("\"", "").Trim() : "NA",
                            CTA_button1_text = entry[15] != null ? entry[15].Replace("\"", "").Trim() : "NA",
                            CTA_button1_tag_name = entry[16] != null ? entry[16].Replace("\"", "").Trim() : "NA",
                            CTA_button1_target = entry[17] != null ? entry[17].Replace("\"", "").Trim() : "NA",
                            CTA_link1_text = entry[18] != null ? entry[18].Replace("\"", "").Trim() : "NA",
                            CTA_link1_tag_name = entry[19] != null ? entry[19].Replace("\"", "").Trim() : "NA",
                            CTA_link1_target = entry[20] != null ? entry[20].Replace("\"", "").Trim() : "NA",
                            Pod1_image = entry[21] != null ? entry[21].Replace("\"", "").Trim() : "NA",
                            Pod1_image_width = entry[22] != null ? entry[22].Replace("\"", "").Trim() : "NA",
                            Pod1_image_alt_tag = entry[23] != null ? entry[23].Replace("\"", "").Trim() : "NA",
                            Pod2_header = entry[24] != null ? entry[24].Replace("\"", "").Trim() : "NA",
                            Pod2_body_copy = entry[25] != null ? entry[25].Replace("\"", "").Trim() : "NA",
                            CTA_button2_text = entry[26] != null ? entry[26].Replace("\"", "").Trim() : "NA",
                            CTA_button2_tag_name = entry[27] != null ? entry[27].Replace("\"", "").Trim() : "NA",
                            CTA_button2_target = entry[28] != null ? entry[28].Replace("\"", "").Trim() : "NA",
                            CTA_link2_text = entry[29] != null ? entry[29].Replace("\"", "").Trim() : "NA",
                            CTA_link2_tag_name = entry[30] != null ? entry[30].Replace("\"", "").Trim() : "NA",
                            CTA_link2_target = entry[31] != null ? entry[31].Replace("\"", "").Trim() : "NA",
                            Pod2_image = entry[32] != null ? entry[32].Replace("\"", "").Trim() : "NA",
                            Pod2_image_width = entry[33] != null ? entry[33].Replace("\"", "").Trim() : "NA",
                            Pod2_image_alt_tag = entry[34] != null ? entry[34].Replace("\"", "").Trim() : "NA",
                            Pod3_header = entry[35] != null ? entry[35].Replace("\"", "").Trim() : "NA",
                            Pod3_body_copy = entry[36] != null ? entry[36].Replace("\"", "").Trim() : "NA",
                            CTA_button3_text = entry[37] != null ? entry[37].Replace("\"", "").Trim() : "NA",
                            CTA_button3_tag_name = entry[38] != null ? entry[38].Replace("\"", "").Trim() : "NA",
                            CTA_button3_target = entry[39] != null ? entry[39].Replace("\"", "").Trim() : "NA",
                            CTA_link3_text = entry[40] != null ? entry[40].Replace("\"", "").Trim() : "NA",
                            CTA_link3_tag_name = entry[41] != null ? entry[41].Replace("\"", "").Trim() : "NA",
                            CTA_link3_target = entry[42] != null ? entry[42].Replace("\"", "").Trim() : "NA",
                            Pod3_image = entry[43] != null ? entry[43].Replace("\"", "").Trim() : "NA",
                            Pod3_image_width = entry[44] != null ? entry[44].Replace("\"", "").Trim() : "NA",
                            Pod3_image_alt_tag = entry[45] != null ? entry[45].Replace("\"", "").Trim() : "NA",
                            Pod4_header = entry[46] != null ? entry[46].Replace("\"", "").Trim() : "NA",
                            Pod4_body_copy = entry[47] != null ? entry[47].Replace("\"", "").Trim() : "NA",
                            CTA_button4_text = entry[48] != null ? entry[48].Replace("\"", "").Trim() : "NA",
                            CTA_button4_tag_name = entry[49] != null ? entry[49].Replace("\"", "").Trim() : "NA",
                            CTA_button4_target = entry[50] != null ? entry[50].Replace("\"", "").Trim() : "NA",
                            CTA_link4_text = entry[51] != null ? entry[51].Replace("\"", "").Trim() : "NA",
                            CTA_link4_tag_name = entry[52] != null ? entry[52].Replace("\"", "").Trim() : "NA",
                            CTA_link4_target = entry[53] != null ? entry[53].Replace("\"", "").Trim() : "NA",
                            Pod4_image = entry[54] != null ? entry[54].Replace("\"", "").Trim() : "NA",
                            Pod4_image_width = entry[55] != null ? entry[55].Replace("\"", "").Trim() : "NA",
                            Pod4_image_alt_tag = entry[56] != null ? entry[56].Replace("\"", "").Trim() : "NA",
                            Trademark = entry[57] != null ? entry[57].Replace("\"", "").Trim() : "NA",
                            Legal = entry[58] != null ? entry[58].Replace("\"", "").Trim() : "NA"
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
        public static Tuple<Uri, string> GetTagIdAndFullUrl(string url)
        {
            Uri ourUri = new Uri(url);
            WebRequest myWebRequest = WebRequest.Create(url);
            WebResponse myWebResponse = myWebRequest.GetResponse();
            Uri myUri = new Uri(myWebResponse.ResponseUri.ToString());
            string currentMsgTagId = HttpUtility.ParseQueryString(myUri.Query).Get("trackingid");

            return Tuple.Create(myUri, currentMsgTagId);
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
                        errorCount++;
                        e.Row.Cells[4].ForeColor = Color.IndianRed;
                    }
                    else
                    {
                        successCount++;
                        e.Row.Cells[4].ForeColor = Color.GreenYellow;
                    }


                }
            }
            int finalSuccessCount = successCount / 5;
            int finalErrorCount = errorCount / 5;
            labelSuccessCount.Text = finalSuccessCount.ToString();
            LabelErrorCount.Text = finalErrorCount.ToString();

        }

    }
}