<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="json-extractor.aspx.cs" Inherits="SimpliFi.json_extractor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">      
        $(window).load(function () {
            $(".loader").fadeOut("slow");
        });

        $(document).ready(function () {
            $("#btnValidateProofs").click(function () {
                $("#hLoaderText").text("Validation in progress...");
                $(".loader").show();
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <ol class="breadcrumb">
        <li class="breadcrumb-item"><a href="json-extractor">JSON Extractor</a></li>
    </ol>

    <div class="loader">
        <h4 id="hLoaderText" class="c-blue loader-text">Please wait a moment...</h4>
    </div>
    <div class="agile-tables">
        <div class="row w3l-table-info">           
            <div class="col-sm-12 cgen-header b-r-2 p-0">
                <h5>Current JSON Files Count:
                    <asp:Label runat="server" ID="lblJSONCount" CssClass="file-names"></asp:Label>
                </h5>
                <div class="col-sm-12 p-b-20 p-t-10">
                    <div class="col-sm-8">
                        <asp:FileUpload ID="flJSOFiles" runat="server" AllowMultiple="true"  accept=".json"/>
                    </div>
                    <div class="col-sm-4">
                        <asp:Button runat="server" ID="btnUpload" CssClass="pull-right btn btn-primary bg-system dark  text-white text-center b-r-2 " Text="Upload Files" OnClick="btnUpload_Click" />
                    </div>
                </div>
            </div>
            <div class="col-sm-8 p-l-0">
                <div class=" msg-block m-t-20" runat="server" id="divMsg" visible="false">
                    <asp:Label runat="server" ID="lblMsg" Text="Test" />
                </div>
                <div class=" msg-block m-t-20 hide" id="divJqueryMsg">
                    <asp:Label runat="server" ID="lblJqueryMsg" Text="Test" />
                </div>
            </div>
             <div class="col-sm-4 p-r-0">
                <asp:Button runat="server" ID="btnDownloadFile" CssClass="btn btn-primary pull-right m-t-20 m-l-20 bg-success dark text-white text-center b-r-2 " Text="Download CSV" OnClick="btnDownloadFile_Click" />
            </div>
        </div>
    </div>
    <br />
    <br />
</asp:Content>
