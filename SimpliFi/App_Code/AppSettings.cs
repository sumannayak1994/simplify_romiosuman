using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

/// <summary>
/// Summary description for AppSettings
/// </summary>
public class AppSettings
{
    //public string assertionConsumerServiceUrl = "https://dev-855060.okta.com/app/exkb2hxywkIQWuXpA4x6/sso/saml/metadata";
    //public string issuer = "Test App";

    //Adobe
    public string assertionConsumerServiceUrl = "https://adobe.okta.com/app/exk1inc8oelHOy1hT0h8/sso/saml/metadata";
    public string issuer = "SimpliFi";
}
