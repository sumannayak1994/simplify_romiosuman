using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace SimpliFi
{
    public partial class footerUpdate : System.Web.UI.Page
    {
        XmlDocument xmlDoc = new XmlDocument();
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void uploadXmlFiles(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(Server.MapPath("~/uploadedXmls")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/uploadedXmls"));
                }
                else
                {
                    string currentXmlPath = Server.MapPath("~/uploadedXmls");
                    int countOFEmails = Directory.GetFiles(currentXmlPath) != null ? Directory.GetFiles(currentXmlPath).Count() : 0;

                    if (countOFEmails > 0)
                    {
                        string[] filePaths;
                        filePaths = Directory.GetFiles(currentXmlPath);

                        foreach (string filePath in filePaths)
                            File.Delete(filePath);
                    }
                }

                if (FileUpload1.HasFile)
                {
                    string fileName = Path.GetFileName(FileUpload1.PostedFile.FileName);
                    string xmlFilePath = Server.MapPath("~/uploadedXmls/" + fileName);
                    FileUpload1.SaveAs(xmlFilePath);
                    Label1.Text = "All files uploaded";
                }
                else
                {
                    Label1.Text = "No files to upload";
                }
            }
            catch (Exception ex)
            {
                Label1.Text = ex.Message + "error" + ex;
            }
        }

        protected void uploadHtmlFiles(object sender, EventArgs e)
        {
            try
            {
                if (!Directory.Exists(Server.MapPath("~/uploadedHTMLs")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/uploadedHTMLs"));
                }
                else
                {
                    string currentHtmlPath = Server.MapPath("~/uploadedHTMLs");
                    int countOFEmails = Directory.GetFiles(currentHtmlPath) != null ? Directory.GetFiles(currentHtmlPath).Count() : 0;

                    if (countOFEmails > 0)
                    {
                        string[] filePaths;
                        filePaths = Directory.GetFiles(currentHtmlPath);

                        foreach (string filePath in filePaths)
                            File.Delete(filePath);
                    }
                }

                if (FileUpload1.HasFile)
                {
                    foreach (var htmlFile in FileUpload1.PostedFiles)
                    {
                        string htmlPath = Server.MapPath("~/uploadedHTMLs/" + htmlFile.FileName);
                        htmlFile.SaveAs(htmlPath);
                    }
                    Label1.Text = "All files uploaded";
                }
                else
                {
                    Label1.Text = "No files to upload";
                }
            }
            catch (Exception ex)
            {
                Label1.Text = ex.Message + "error" + ex;
            }
        }

        protected void updateFooter(object sender, EventArgs e)
        {
            string currentHtmlPath = Server.MapPath("~/uploadedHTMLs");
            string currentFooterPath = Server.MapPath("~/allLegalFooters");

            string[] filePaths = Directory.GetFiles(currentHtmlPath);

            foreach (var filePath in filePaths)
            {                
                var doc = new HtmlDocument();
                doc.Load(filePath);
                var footerDoc = new HtmlDocument();
                string locale = "ie";
                var actualFooterPath = currentFooterPath + "\\" + locale + ".html";
                footerDoc.Load(actualFooterPath);
                doc.DocumentNode.SelectNodes("//td[@class='legal']").First().InnerHtml = footerDoc.DocumentNode.InnerHtml;
                var htmlResult = doc.DocumentNode.InnerHtml;
                File.WriteAllText(@"c:/Users/sumannayak/OneDrive - Adobe/Desktop/DownloadedFiles/test.html", htmlResult);
            }

        }

        public void updateFooterXML(object sender, EventArgs e)
        {

            string currentXmlPath = Server.MapPath("~/uploadedXMLs");
            string currentFooterPath = Server.MapPath("~/allLegalFooters");

            string filePath = Directory.GetFiles(currentXmlPath).First();

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            XmlElement root = xmlDoc.DocumentElement;
            XmlNodeList entities = root.SelectNodes("entities");


            var footerDoc = new HtmlDocument();
            string locale = "ie";
            var actualFooterPath = currentFooterPath + "\\" + locale + ".html";
            footerDoc.Load(actualFooterPath);
            List<string> vs = new List<string>();

            foreach (XmlNode entity in entities)
            {
                XmlNodeList deliveryNode = entity.SelectNodes("delivery");
                XmlNode contentNode = deliveryNode[0].SelectSingleNode("content");
                XmlNode htmlNode = contentNode.SelectSingleNode("html");
                XmlNode sourceNode = htmlNode.SelectSingleNode("source");
                //sourceNode.SelectSingleNode("//td[@class='legal']").InnerXml = "ABC";
                var rawHtml = sourceNode.InnerText;
                var html = new HtmlDocument();
                html.LoadHtml(rawHtml);
                html.OptionWriteEmptyNodes = true;
                html.OptionAutoCloseOnEnd = true;
                html.DocumentNode.SelectSingleNode("//td[@class='legal']").InnerHtml = footerDoc.DocumentNode.InnerHtml;
                entity.SelectSingleNode("delivery/content/html/source").InnerText = html.DocumentNode.OuterHtml.ToString();
            }


            //Download File
            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            XmlTextWriter writer = new XmlTextWriter(stream, System.Text.Encoding.UTF8);

            xmlDoc.WriteTo(writer);
            writer.Flush();
            Response.Clear();
            byte[] byteArray = stream.ToArray();
            Response.AppendHeader("Content-Disposition", "filename=output.xml");
            Response.AppendHeader("Content-Length", byteArray.Length.ToString());
            Response.ContentType = "application/octet-stream";
            Response.BinaryWrite(byteArray);
            writer.Close();

            //MemoryStream ms = new MemoryStream();
            //using (XmlWriter writer = XmlWriter.Create(ms))
            //{
            //    xmlDoc.WriteTo(writer); // Write to memorystream
            //}

            //byte[] data = ms.ToArray();
            //HttpContext.Current.Response.Clear();
            //HttpContext.Current.Response.ContentType = "text/xml";
            //HttpContext.Current.Response.AddHeader("Content-Disposition:",
            //                    "attachment;filename=" + HttpUtility.UrlEncode("samplefile.xml")); // Replace with name here
            //HttpContext.Current.Response.BinaryWrite(data);
            //HttpContext.Current.Response.End();
            //ms.Flush(); // Probably not needed
            //ms.Close();

            //xmlDoc.Save(@"c:/Users/sumannayak/OneDrive - Adobe/Desktop/DownloadedFiles/output.xml");
            //DownloadFile(xmlDoc.InnerXml);
        }

        public void DownloadFile(string innerXml)
        {
            string strFullPath = Server.MapPath("~/output.xml");
            string strContents = null;
            System.IO.StreamReader objReader = default(System.IO.StreamReader);
            objReader = new System.IO.StreamReader(strFullPath);
            strContents = objReader.ReadToEnd();
            objReader.Close();

            string attachment = "attachment; filename=output.xml";
            Response.ClearContent();
            Response.ContentType = "application/xml";
            Response.AddHeader("content-disposition", attachment);
            Response.Write(strContents);
            Response.End();
        }
    }
}