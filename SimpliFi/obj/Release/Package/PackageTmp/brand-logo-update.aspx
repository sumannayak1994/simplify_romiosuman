<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="brand-logo-update.aspx.cs" Inherits="SimpliFi.brand_logo_update" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">      
        $(window).load(function () {
            $(".loader").fadeOut("slow");
        });

        $(document).ready(function () {
            $("#btnImport").click(function () {
                $("#hLoaderText").text("Import in progress...");
                $(".loader").show();
            });
        });
    </script>
    <style type="text/css">
        .scroll {
            height: 500px;
            overflow-x: hidden;
            overflow-x: auto;
            text-align: justify;
            border: 2px solid #ddd;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <ol class="breadcrumb">
        <li class="breadcrumb-item"><a href="brand-logo-update">Brand Logo Update Wizard</a></li>
    </ol>

    <div class="loader">
        <h4 id="hLoaderText" class="c-blue loader-text">Please wait a moment...</h4>
    </div>
    <div class="agile-tables">
        <div class="row w3l-table-info">
            <div class="col-sm-12 cgen-header b-r-2 p-0">
                <h5>Current P Blocks Count:
                    <asp:Label runat="server" ID="lblCurrEmailCount" CssClass="file-names"></asp:Label>
                </h5>
                <div class="col-sm-12 p-b-20 p-t-10">
                    <div class="col-sm-8">
                        <asp:FileUpload ID="flPBlocks" runat="server" AllowMultiple="true" />
                    </div>
                    <div class="col-sm-4">
                        <asp:Button runat="server" ID="btnUpload" CssClass="pull-right btn btn-primary bg-system dark  text-white text-center b-r-2 " Text="Upload PBlocks" OnClick="btnUpload_Click" />
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
            </div>
        </div>
    </div>
    <div class="agile-tables" runat="server" id="divPBlocks" visible="false">
        <div class="row w3l-table-info" style="height: 500px;">
            <div class="col-sm-5 p-l-0 scroll p-r-0">
                <asp:ListView ID="lvPBlocks" runat="server" GroupPlaceholderID="groupPlaceHolder1"
                    ItemPlaceholderID="itemPlaceHolder1">
                    <LayoutTemplate>
                        <table class="customers tblPblock" style="overflow-y: auto;">
                            <tr>
                                <th style="width: 100%; position: sticky; top: 0;"
                                    class="text-left">PBlock Names
                                </th>
                            </tr>
                            <asp:PlaceHolder runat="server" ID="groupPlaceHolder1"></asp:PlaceHolder>
                        </table>
                    </LayoutTemplate>
                    <GroupTemplate>
                        <tr>
                            <asp:PlaceHolder runat="server" ID="itemPlaceHolder1"></asp:PlaceHolder>
                        </tr>
                    </GroupTemplate>
                    <ItemTemplate>
                        <td class="text-left">
                            <a href="<%# Eval("URL") %>.html" target="_blank"><%# Eval("PblockName") %></a>
                        </td>
                    </ItemTemplate>
                    <EmptyDataTemplate>
                        <div class="msg-block m-t-20 info text-center">
                            <span>No P Block Found.
                        </div>
                    </EmptyDataTemplate>
                </asp:ListView>
            </div>
            <div class="col-sm-2">
                <asp:Button runat="server" ID="btnImport" Style="margin-top: 250px !important; float: left !important; margin-left: 5px;"
                    CssClass="btn btn-primary pull-right m-t-20 m-l-20 bg-success dark text-white text-center b-r-2 " Text="" OnClick="btnImport_Click" />
            </div>
            <div class="col-sm-5 p-l-0 scroll p-r-0" runat="server" id="divResult" visible="false">
                <asp:ListView ID="lstResult" runat="server" GroupPlaceholderID="groupPlaceHolder1"
                    ItemPlaceholderID="itemPlaceHolder1">
                    <LayoutTemplate>
                        <table class="customers tblPblock" style="overflow-y: auto;">
                            <tr>
                                <th style="width: 60%; position: sticky; top: 0;"
                                    class="text-left">PBlock Names
                                </th>
                                <th style="width: 20%; position: sticky; top: 0;"
                                    class="text-left">Action
                                </th>
                                <th style="width: 20%; position: sticky; top: 0;"
                                    class="text-left">Status
                                </th>
                            </tr>
                            <asp:PlaceHolder runat="server" ID="groupPlaceHolder1"></asp:PlaceHolder>
                        </table>
                    </LayoutTemplate>
                    <GroupTemplate>
                        <tr>
                            <asp:PlaceHolder runat="server" ID="itemPlaceHolder1"></asp:PlaceHolder>
                        </tr>
                    </GroupTemplate>
                    <ItemTemplate>
                        <td class="text-left">
                            <%# Eval("PblockName") %>
                        </td>
                         <td class="text-left">
                            <%# Eval("Action") %>
                        </td>
                         <td class="text-left">
                            <%# Eval("Status") %>
                        </td>
                    </ItemTemplate>
                    <EmptyDataTemplate>
                        <div class="msg-block m-t-20 info text-center">
                            <span>No P Block Found.
                        </div>
                    </EmptyDataTemplate>
                </asp:ListView>
            </div>
        </div>
    </div>

    <br />
    <br />
</asp:Content>

