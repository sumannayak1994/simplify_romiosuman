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

namespace SimpliFi
{
    public partial class p_block_validation : System.Web.UI.Page
    {
        StringBuilder sbContent = new StringBuilder();

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


        private void ValidatePBLocks()
        {
            string currPath = Common.GetCurrentPath("PBLOCK");
            string currRelativePath = Server.MapPath(currPath);

            var filePaths = Directory.GetFiles(currRelativePath).Where(s => s.EndsWith(".htm") || s.EndsWith(".html"));
            string ext;

            if (filePaths.Count() > 0)
            {
                StringBuilder sbPBlocks = new StringBuilder();

                // Create new stopwatch.
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                foreach (var file in filePaths)
                {
                    ext = Path.GetExtension(file);

                    WebClient client = new WebClient();
                    byte[] buffer = client.DownloadData(file.ToString());

                    string html = System.Text.Encoding.UTF8.GetString(buffer);

                    List<string> list = Extract(html, ext);

                    sbContent.Append("<br><strong class='p-block'> P-Block: <span class='c-red'><a target='_blank' href='http://simplifi.corp.adobe.com" + currPath.Replace("~", "") + Path.GetFileName(file.ToString()) + "'>" + Path.GetFileNameWithoutExtension(file.ToString()) + "</a></span></strong> ");
                    sbContent.Append("<strong class='p-block-count'>Total No of Links Verified: <span class='c-red badge'>" + list.Count() + "</span></strong>");
                    sbContent.Append("<br><br>");

                    CheckURLInParallel(list);
                }

                sbContent.Append("<br><br>");

                stopwatch.Stop();

                lblProgram.Text = "Validation Results";
                lblEmailCounts.Text = filePaths.Count().ToString();
                lblTime.Text = Math.Ceiling(TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds).TotalSeconds).ToString();
            }
            else
            {
                sbContent.Append("<br/><br/><p class='text-center'><strong class='c-red'>No HTML files in given directory</strong> <br/>" + currRelativePath + "</p>");
            }

            divResults.Visible = true;
            divContent.InnerHtml = sbContent.ToString();
        }

        private void CheckURLInParallel(List<string> listUrls)
        {
            var times = new ParallelOptions { MaxDegreeOfParallelism = 10 };
            StringBuilder sb = new StringBuilder();
            sb.Append("<table class='customers'><tr><th style='width:60%;'>URL</th><th style='width:20%;'>URL Status</th><th style='width:20%;'>Whitelist Status</th></tr>");
            string statusCode;

            UsageData objUsageData = new UsageData();
            List<string> lstURLS = objUsageData.GetURLS();

            Parallel.ForEach(listUrls, times, x =>
            {
                Uri uriResult;
                bool isValidURL = Uri.TryCreate(x, UriKind.Absolute, out uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                if (isValidURL)
                {
                    if (x.Contains(Constants.TrackingParameter))
                    {
                        statusCode = GetStatusCode(x);

                        if (statusCode == "Not Found")
                        {
                            statusCode = "<strong class='not-found'>Not Found</strong>";

                            sb.Append("<tr><td style='width:80%;'><a class='c-red' target='_blank' href=" + x + ">" + x + "</a></td>");
                            sb.Append("<td style='width:20%;'> " + statusCode + "</td>");

                            bool isWhiteListed = GetWhitelistedStatus(x, lstURLS);

                            if (isWhiteListed)
                                sb.Append("<td style='width:20%;'><strong class='c-green'>Yes</td></tr>");
                            else
                                sb.Append("<td style='width:20%;'><strong class='c-red'>No</td></tr>");
                        }
                        else
                        {
                            statusCode = "<strong class='c-orange'>" + statusCode + "</strong>";

                            sb.Append("<tr><td style='width:80%;'><a class='c-orange' target='_blank' href=" + x + ">" + x + "</a></td>");
                            sb.Append("<td style='width:20%;'> " + statusCode + "</td>");

                            bool isWhiteListed = GetWhitelistedStatus(x, lstURLS);

                            if (isWhiteListed)
                                sb.Append("<td style='width:20%;'><strong class='c-green'>Yes</td></tr>");
                            else
                                sb.Append("<td style='width:20%;'><strong class='c-red'>No</td></tr>");
                        }
                    }
                    else
                    {
                        statusCode = GetStatusCode(x);

                        if (statusCode == "Not Found")
                        {
                            statusCode = "<strong class='not-found'>Not Found</strong>";

                            sb.Append("<tr><td style='width:80%;'><a class='c-red' target='_blank' href=" + x + ">" + x + "</a></td>");
                            sb.Append("<td style='width:20%;'>" + statusCode + "</td>");

                            bool isWhiteListed = GetWhitelistedStatus(x, lstURLS);

                            if (isWhiteListed)
                                sb.Append("<td style='width:20%;'><strong class='c-green'>Yes</td></tr>");
                            else
                                sb.Append("<td style='width:20%;'><strong class='c-red'>No</td></tr>");
                        }
                        else
                        {
                            statusCode = "<strong class='c-green'>" + statusCode + "</strong>";

                            sb.Append("<tr><td style='width:80%;'><a target='_blank' href=" + x + ">" + x + "</a></td>");
                            sb.Append("<td style='width:20%;'>" + statusCode + "</td>");

                            bool isWhiteListed = GetWhitelistedStatus(x, lstURLS);

                            if (isWhiteListed)
                                sb.Append("<td style='width:20%;'><strong class='c-green'>Yes</td></tr>");
                            else
                                sb.Append("<td style='width:20%;'><strong class='c-red'>No</td></tr>");
                        }
                    }
                }
                else
                {
                    statusCode = "<strong class='not-found'>Not Valid </strong> <br/> ";

                    sb.Append("<tr><td style='width:80%;'><a class='c-red' target='_blank' href=" + x + ">" + x + "</a></td>");
                    sb.Append("<td style='width:20%;'> " + statusCode + "</td>");

                    bool isWhiteListed = GetWhitelistedStatus(x, lstURLS);

                    if (isWhiteListed)
                        sb.Append("<td style='width:20%;'><strong class='c-green'>Yes</td></tr>");
                    else
                        sb.Append("<td style='width:20%;'><strong class='c-red'>No</td></tr>");
                }
            });

            sb.Append("</table>");
            sbContent.Append(sb.ToString());
        }

        public bool GetWhitelistedStatus(string url, List<string> lstURLS)
        {
            bool isDomainValid = true;

            string domain = ExtractDomainNameFromURL(url);

            isDomainValid = lstURLS.Contains(domain);

            //Do five level of domain verification
            if (!isDomainValid)
            {
                string[] strDomain = domain.Split('.');

                int counter = strDomain.Count();

                for (int i = counter; i >= 2; i--)
                {
                    switch (counter)
                    {
                        case 3:
                            isDomainValid = lstURLS.Contains(strDomain[1] + '.' + strDomain[2]);
                            break;
                        case 4:
                            isDomainValid = lstURLS.Contains(strDomain[2] + '.' + strDomain[3]);
                            break;
                        case 5:
                            isDomainValid = lstURLS.Contains(strDomain[3] + '.' + strDomain[4]);
                            break;
                        default:
                            break;
                    }
                }
            }

            return isDomainValid;
        }


        public static string ExtractDomainNameFromURL(string Url)
        {
            return System.Text.RegularExpressions.Regex.Replace(
                Url,
                @"^([a-zA-Z]+:\/\/)?([^\/]+)\/.*?$",
                "$2"
            );
        }

        private string GetStatusCode(string url)
        {
            string status = String.Empty;

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
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
                            response.StatusCode == HttpStatusCode.MovedPermanently)
                            return "Found";
                        else
                            return "Not Found";
                    }
                }
                catch (Exception ex)
                {
                    return "Not Found";
                }
            }
            catch (Exception ex)
            {
                return "Not Found";
            }
        }

        public List<string> Extract(string html, string ext)
        {
            List<string> list = new List<string>();

            if (ext == ".txt")
            {
                foreach (Match item in Regex.Matches(html, @"(http|ftp|https):\/\/([\w\-_]+(?:(?:\.[\w\-_]+)+))([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?"))
                {
                    list.Add(item.Value);
                }
            }
            else
            {
                Regex regex = new Regex("(?:href|src)=[\"|']?(.*?)[\"|'|>]+", RegexOptions.Singleline | RegexOptions.CultureInvariant);

                if (regex.IsMatch(html))
                {
                    foreach (Match match in regex.Matches(html))
                    {
                        if (match.Groups[1].Value != "<%@ include view=")
                            list.Add(match.Groups[1].Value);
                    }
                }
            }

            return list;
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                if (flProofEmails.HasFile)
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

                    foreach (HttpPostedFile htfiles in flProofEmails.PostedFiles)
                    {
                        getFileName = Path.GetFileName(htfiles.FileName);
                        htfiles.SaveAs(Path.Combine(currRelativePath, getFileName));
                    }

                    SetErrorMsg("HTML Files Uploaded successfully.", "success");

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

        protected void btnValidateProofs_Click(object sender, EventArgs e)
        {
            ValidatePBLocks();
        }

        protected void btnAddURL_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtURL.Text.Trim()))
            {
                UsageData objUsageData = new UsageData();

                int retVal = objUsageData.InsertURL(txtURL.Text.Trim());

                if (retVal >= 1)
                {
                    SetErrorMsg("URL added successfully.", "success");
                }
                else
                {
                    SetErrorMsg("Unable to add URL.", "error");
                }
            }
            else
            {
                SetErrorMsg("Unable to add URL.", "error");
            }
        }
    }
}