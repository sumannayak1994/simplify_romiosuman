using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace SimpliFi
{
    /// <summary>
    /// Summary description for EmailDetails
    /// </summary>
    public class EmailDetails
    {
        public string Subject { get; set; }
        public string BodyHTML { get; set; }
        public string From { get; set; }
        public string SenetOn { get; set; }
        public string PreHeader { get; set; }
        public string WaveNo { get; set; }
        public string EmailLocale { get; set; }
        public string WebLocale1 { get; set; }
        public string WebLocale2 { get; set; }
        public string ActivityId { get; set; }
        public string JunkCharacterPresent { get; set; }
        public string CopyrightCheck { get; set; }
        public string SenderBookAddress { get; set; }
        public bool DuplicateMirrorPage { get; set; }
        public string EmailFromName { get; set; }
        public string EmailOK250 { get; set; }
        public string CGENFromName { get; set; }
        public string CGENOK250 { get; set; }
        public bool IsEnglish { get; set; }
        public string APACLocale { get; set; }

        public List<UrlAndStatusCode> DecodedHrefTags { get; set; }

        public EmailDetails()
        {
            DecodedHrefTags = new List<UrlAndStatusCode>();
        }
    }

    public class UrlAndStatusCode
    {
        public string URL { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string TagId { get; set; }
        public bool LocalizedUrl { get; set; }
        public bool LocalizedAndTrackingUrl { get; set; }
    }

    public class CGenActivityDetails
    {
        public string ActivityId { get; set; }
        public string TagId { get; set; }
        public string Tagname { get; set; }
        public string TagfulURL { get; set; }
        public string CreativeFileName { get; set; }
        public string SubjectLine { get; set; }
        public string PreHeader { get; set; }

    }

    public class Constants
    {
        public const string MirrorPage = "Mirror Page";
        public const string PrivacyPage = "Privacy Page";
        public const string UnsubscribePage = "Unsubscribe Page";
        public const string TagfulUrl = "Tagful URL";
        public const string ContactPage = "Contact Page";
        public const string LegalPage = "Legal Page";
        public const string ImpressumPage = "Impressum Page";
        public const string Email = "Email";
        public const string SocialLinks = "Social Links";
        public const string OtherLinks = "Other Links";
        public const string NAMPrivacyPage = "https://www.adobe.com/privacy.html";
        public const string NAMContactPage = "https://www.adobe.com/privacy.html";
        public const string NAMPrivacyPageHTTP = "http://www.adobe.com/privacy.html";
        public const string NAMContactPageHTTP = "http://www.adobe.com/privacy.html";
        public const string NAMLegalPage = "https://www.adobe.com/legal/terms.html";
        public const string NAMLegalPageHTTP = "http://www.adobe.com/legal/terms.html";
        public const string PageErrorMsg = "Page Error";
        public const string PageRedirectingMsg = "Valid";
        public const string TrackingParameter = "trackingid";
        public const string StatusUnknown = "Status Unknown(Please click on the link and check.)";
        //public const string TrackingParameter = "sdid";
    }

    public class SessionVariables
    {
        public const string ProgramName = "program";
        public const string UserName = "userName";
        public const string SessionToken = "sessionToken";
        public const string FirstName = "firstName";
        public const string SAMLResponse = "SAMLResponse";
    }
    public class Programs
    {
        public const string B2CTriggered = "B2CTriggered";
        public const string B2CBatch = "B2CBatch";
        public const string SophiaTriggered = "SophiaTriggered";
        public const string APACBNB = "APACBBatch";

        public const string B2CTriggeredTableHeaders = "<table class='customers tblMaster tblReport'><tr><th class='p-b-t-6' colspan='11'>" +
                          "<button id='btnSaveReport' type='button' class='btn btn-table text-white text-center'><i class='fa fa-save'></i>&nbsp;Save</button>" +
                          "<button id='btnDownloadReport' type='button' class='btn btn-table text-white text-center'><i class='fa fa-download'></i>&nbsp;Download</button>" +
                          "<button type = 'button'onclick='javascript:OpenReportModal(2)' class='btn btn-table text-white text-center '><i class='fa fa-send'></i>&nbsp;Save and Share</button>" +
                          "</th></tr></table><table class='customers tblMaster'><tr>" +
                          "<th style='width:25%;'>Proof Detail</th>" +
                          "<th style='width:7%;'>Subject Line</th>" +
                          "<th style='width:8%;'>Preheader</th>" +
                          "<th style='width:12%;'>TagID & Tag URLs</th>" +
                          "<th style='width:6%;'>Mirror Page</th>" +
                          "<th style='width:6%;'>Link Check</th>" +
                          //"<th style='width:6%;'>Junk Char</th>" +
                          "<th style='width:6%;'>Unsub Link</th>" +
                          "<th style='width:6%;'>Locale Check</th>" +
                          "<th style='width:8%;'>Copyright</th>" +
                          "<th style='width:8%;'>Sender book</th>" +
                          "<th style='width:8%; text-align:center;'>Details</th></tr>";

        public const string B2CBatchTableHeaders = "<table class='customers tblMaster tblReport'><tr><th class='p-b-t-6' colspan='13'>" +
                          "<button id='btnSaveReport' type='button' class='btn btn-table text-white text-center'><i class='fa fa-save'></i>&nbsp;Save</button>" +
                          "<button id='btnDownloadReport' type='button' class='btn btn-table text-white text-center'><i class='fa fa-download'></i>&nbsp;Download</button>" +
                          "<button type = 'button'onclick='javascript:OpenReportModal(2)' class='btn btn-table text-white text-center '><i class='fa fa-send'></i>&nbsp;Save and Share</button>" +
                           "</th></tr></table><table class='customers tblMaster'><tr>" +
                           "<th style='width:15%;'>Proof Detail</th>" +
                           "<th style='width:6%;'>Subject Line</th>" +
                           "<th style='width:8%;'>Preheader</th>" +
                           "<th style='width:10%;'>TagID & Tag URLs</th>" +
                           "<th style='width:6%;'>Mirror Page</th>" +
                           "<th style='width:6%;'>Link Check</th>" +
                           //"<th style='width:6%;'>Junk Char</th>" +
                           "<th style='width:7%;'>Sender Name</th>" +
                           "<th style='width:7%;'>250 OK</th>" +
                           "<th style='width:6%;'>Unsub Link</th>" +
                           "<th style='width:6%;'>Locale Check</th>" +
                           "<th style='width:9%;'>Copyright</th>" +
                           "<th style='width:8%;'>Sender book</th>" +
                           "<th style='width:5%; text-align:center;'>Details</th></tr>";

        public const string SophiaTriggeredTableHeaders = "<table class='customers tblMaster tblReport'><tr><th class='p-b-t-6' colspan='11'>" +
                          "<button id='btnSaveReport' type='button' class='btn btn-table text-white text-center'><i class='fa fa-save'></i>&nbsp;Save</button>" +
                          "<button id='btnDownloadReport' type='button' class='btn btn-table text-white text-center'><i class='fa fa-download'></i>&nbsp;Download</button>" +
                          "<button type = 'button'onclick='javascript:OpenReportModal(2)' class='btn btn-table text-white text-center '><i class='fa fa-send'></i>&nbsp;Save and Share</button>" +
                         "</th></tr></table><table class='customers tblMaster'><tr>" +
                         "<th style='width:25%;'>Proof Detail</th>" +
                         "<th style='width:7%;'>Subject Line</th>" +
                         "<th style='width:8%;'>Preheader</th>" +
                         "<th style='width:12%;'>TagID & Tag URLs</th>" +
                         "<th style='width:6%;'>Mirror Page</th>" +
                         "<th style='width:6%;'>Link Check</th>" +
                         //"<th style='width:6%;'>Junk Char</th>" +
                         "<th style='width:6%;'>Unsub Link</th>" +
                         "<th style='width:6%;'>Locale Check</th>" +
                         "<th style='width:8%;'>Copyright</th>" +
                         "<th style='width:8%;'>Sender book</th>" +
                         "<th style='width:8%; text-align:center;'>Details</th></tr>";
    }

    public class ValidationList
    {
        public int No { get; set; }
        public string ProgramName { get; set; }
        public string ValidatedOn { get; set; }
        public string URL { get; set; }
        public string FilePath { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class PBlockList
    {
        public string PblockName { get; set; }
        public string URL { get; set; }
        public string Status { get; set; }
        public string Action { get; set; }
    }

    /// <summary>
    /// Summary description for Usage
    /// </summary>
    public class Usage
    {
        public int Id { get; set; }

        public string LDAP { get; set; }

        public string ProgramName { get; set; }

        public string ProgramType { get; set; }

        public DateTime? ValidatedOn { get; set; }

        public int? ProofsCount { get; set; }

        public string HTMLString { get; set; }

        public int? TimeTaken { get; set; }

    }

}