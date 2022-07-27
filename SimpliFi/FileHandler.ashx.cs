using Excel;
using HtmlAgilityPack;
using Microsoft.Office.Interop.Outlook;
using MsgReader.Outlook;
using NHunspell;
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
namespace SimpliFi
{
    /// <summary>
    /// Summary description for FileHandler
    /// </summary>
    public class FileHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.Files.Count > 0)
            {
                HttpFileCollection files = context.Request.Files;
                foreach (string key in files)
                {
                    HttpPostedFile file = files[key];
                    string fileName = file.FileName;
                    fileName = context.Server.MapPath("~/UploadedFiles/HTML/" + fileName);
                    file.SaveAs(fileName);
                }
            }
            context.Response.ContentType = "text/plain";
            context.Response.Write("File(s) uploaded successfully!");
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}