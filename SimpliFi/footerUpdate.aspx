<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="footerUpdate.aspx.cs" Inherits="SimpliFi.footerUpdate" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:FileUpload ID="FileUpload1" runat="server" />
&nbsp;&nbsp;&nbsp;
            <asp:Button ID="Button1" runat="server" OnClick="uploadXmlFiles" Text="Upload Xml files" />
&nbsp;<asp:Label ID="Label1" runat="server"></asp:Label>
        </div>
        <p>
            <asp:Button ID="Button2" runat="server" OnClick="updateFooterXML" Text="Update Footer" />
        </p>
    </form>
</body>
</html>
