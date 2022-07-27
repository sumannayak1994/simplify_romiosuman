<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="p-block-validation.aspx.cs" Inherits="SimpliFi.p_block_validation" %>
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
        <li class="breadcrumb-item"><a href="p-block-validation">URL WhiteListing Validation</a></li>
         <li class="breadcrumb-info"><a title="View All Whitelisted URLS" href="Whitelisted-Urls" class="a-info"><i class="fa fa-info-circle pull-right"></i></a></li>
    </ol>

    <div class="loader">
        <h4 id="hLoaderText" class="c-blue loader-text">Please wait a moment...</h4>
    </div>
    <div class="agile-tables">
        <div class="row w3l-table-info">
            <div class="col-sm-6 w-48 cgen-header b-r-2 p-0">
                <h5>Add New WhiteListed URL to the List.                   
                </h5><a></a>
                <div class="col-sm-12 p-b-20 p-t-10">
                    <div class="col-sm-8">
                        <asp:TextBox runat="server" ID="txtURL" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="col-sm-4">
                        <asp:Button runat="server" ID="btnAddURL" CssClass="pull-right btn btn-primary bg-system dark  text-white text-center b-r-2 " Text="Add URL" OnClick="btnAddURL_Click" />
                    </div>
                </div>
            </div>
            <div class="col-sm-6 w-48 m-l-2 cgen-header b-r-2 p-0">
                <h5>Current P Blocks Count:
                    <asp:Label runat="server" ID="lblCurrEmailCount" CssClass="file-names"></asp:Label>
                </h5>
                <div class="col-sm-12 p-b-20 p-t-10">
                    <div class="col-sm-8">
                        <asp:FileUpload ID="flProofEmails" runat="server" AllowMultiple="true" />
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
             <div class="col-sm-4">
                <asp:Button runat="server" ID="btnValidateProofs" CssClass="btn btn-primary pull-right m-t-20 m-l-20 bg-success dark text-white text-center b-r-2 " Text="Validate HTML Files" OnClick="btnValidateProofs_Click" />
            </div>
        </div>
    </div>
    <div class="agile-tables" id="divResults" runat="server" visible="false">
        <div class="row w3l-table-info" id="divReport">
            <h2 class="p-l-15 f-24">
                <asp:Label runat="server" ID="lblProgram"></asp:Label>
                <br />
                <span class="time-info b-r-2 ">Validated
                    <asp:Label runat="server" ID="lblEmailCounts"></asp:Label>
                    P Block(HTML) files in 
                    <asp:Label runat="server" ID="lblTime"></asp:Label>
                    seconds</span>
            </h2>
            <div class="col-sm-12">
                <div id="divContent" runat="server">
                </div>
            </div>
        </div>
    </div>
    <br />
    <br />
</asp:Content>
