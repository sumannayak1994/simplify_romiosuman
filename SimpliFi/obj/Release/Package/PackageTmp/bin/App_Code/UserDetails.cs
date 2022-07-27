using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace SimpliFi
{
    /// <summary>
    /// Summary description for UserDetails
    /// </summary>
    public class UserDetails
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DomainName { get; set; }
        public string Email { get; set; }
        public bool IsAllowed { get; set; }
    }
}