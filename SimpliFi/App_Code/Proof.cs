using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace SimpliFi
{
    /// <summary>
    /// Summary description for Proof
    /// </summary>
    public class Proof
    {
        public string ProgramID { get; set; }
        public string ProgramName { get; set; }
        public string ActivityId { get; set; }
        public string ActivityName { get; set; }
        public string ActivityMediaType { get; set; }
        public string ActivityMediaTypeDetail { get; set; }
        public string ActivityChannel { get; set; }
        public string TagId { get; set; }
        public string TagName { get; set; }
        public string TagPurpose { get; set; }
        public string TagURL { get; set; }
        public string TagfulURL { get; set; }
        public string CreativeFileName { get; set; }
        public string SubjectLine { get; set; }
        public string Preheader { get; set; }
        public string DeploymentDate { get; set; }
        public string SenderName { get; set; }
        public string Ok250 { get; set; }
    }
}