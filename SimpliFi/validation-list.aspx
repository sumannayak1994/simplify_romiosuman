<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="validation-list.aspx.cs" Inherits="SimpliFi.validation_list" %>
 <asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <script src="js/jspdf.min.js"></script>
        <script src="js/html2canvas.js"></script>

        <script type="text/javascript">      
            $(document).ready(function () {

                $(".adelete").click(function (e) {
                    e.preventDefault();
                    $("#hLoaderText").text("Deleting Report...");
                    $(".loader").show();

                    var fileName = $(this).attr("data-filepath");
                    DeleteReport(fileName);
                });
            });

            $(window).load(function () {
                $(".loader").fadeOut("slow");
            });

            function DeleteReport(fileName) {
                $.ajax({
                    url: 'validation-list.aspx/DeleteReport',
                    type: "POST",
                    dataType: "json",
                    data: JSON.stringify({ 'fileName': fileName }),
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {

                        $(".loader").fadeOut("slow");

                        if (data.d == "1") {
                            SetJqueryMsg("success", "Report deleted successfully. Page will be refreshed automatically after 5 seconds.");

                            setTimeout(function () { location.reload(); }, 5000);

                        }
                        else {
                            alert("Unable to delete Report, Try after sometime.");
                        }
                    }
                });
            }

            function SetJqueryMsg(type, text) {
                $("#divJqueryMsg").removeClass('hide');
                $("#divJqueryMsg").removeClass('error');
                $("#divJqueryMsg").removeClass('success');
                $("#divJqueryMsg").addClass(type);
                $("#lblJqueryMsg").text(text);
            }
        </script>
    </asp:Content>
    <asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <ol class="breadcrumb">
            <li class="breadcrumb-item"><a href="index.aspx">Proof Validation</a><i class="fa fa-angle-right"></i></li>
            <li class="w-80"><a href="validation-list">My Validations</a>
            </li>
            <%--<li class="breadcrumb-info"><a title="New Proof Validation" href="index.aspx"><i class="fa fa-plus pull-right"></i></a></li>--%>
        </ol>

        <div class="loader">
            <h4 id="hLoaderText" class="c-blue loader-text">Please wait a moment...</h4>
        </div>

        <div class="agile-tables" id="divResults">
            <div class="row w3l-table-info" id="divReport">
                <div class="col-sm-12">
                    <div id="divContent">
                        <div class=" msg-block m-t-20 hide" id="divJqueryMsg">
                            <asp:Label runat="server" ID="lblJqueryMsg" Text="Test" />
                        </div>
                        <br />
                        <asp:ListView ID="lvProgramList" runat="server" GroupPlaceholderID="groupPlaceHolder1"
                            ItemPlaceholderID="itemPlaceHolder1" OnPagePropertiesChanging="OnPagePropertiesChanging">
                            <LayoutTemplate>
                                <table class="customers m-t-10">
                                    <tr>
                                        <th style="width: 5%" class="text-center">#
                                        </th>
                                        <th style="width: 64%">Report Name
                                        </th>
                                        <th style="width: 16%" class="text-center">Validated On
                                        </th>
                                        <th style="width: 10%" class="text-center">Action
                                        </th>
                                    </tr>
                                    <asp:PlaceHolder runat="server" ID="groupPlaceHolder1"></asp:PlaceHolder>
                                    <tr>
                                        <td colspan="4">
                                            <asp:DataPager ID="DataPager1" runat="server" PagedControlID="lvProgramList" PageSize="10">
                                                <Fields>
                                                    <asp:NextPreviousPagerField ButtonType="Link" ShowFirstPageButton="false" ShowPreviousPageButton="true"
                                                        ShowNextPageButton="false" />
                                                    <asp:NumericPagerField ButtonType="Link" />
                                                    <asp:NextPreviousPagerField ButtonType="Link" ShowNextPageButton="true" ShowLastPageButton="false" ShowPreviousPageButton="false" />
                                                </Fields>
                                            </asp:DataPager>
                                        </td>
                                    </tr>
                                </table>
                            </LayoutTemplate>
                            <GroupTemplate>
                                <tr>
                                    <asp:PlaceHolder runat="server" ID="itemPlaceHolder1"></asp:PlaceHolder>
                                </tr>
                            </GroupTemplate>
                            <ItemTemplate>
                                <td class="text-center">
                                    <%# Eval("No") %>
                                </td>
                                <td>
                                    <%# Eval("ProgramName") %>
                                </td>
                                <td class="text-center">
                                    <%# Eval("ValidatedOn") %>
                                </td>
                                <td class="text-center">
                                    <a class="c-blue" title="View/Download" target="_blank" href="<%# Eval("URL") %>"><span class='fa fa fa-download f-20'></span></a>
                                    <a class="c-red adelete" title="Delete" data-filepath="<%# Eval("FilePath") %>" href="#"><span class='fa fa fa-trash c-red f-20'></span></a>
                                </td>
                            </ItemTemplate>
                            <EmptyDataTemplate>
                                <div class="msg-block m-t-20 info text-center">
                                    <span>No Reports found. <br />Please save one report to view it in this space.</span>
                                </div>
                            </EmptyDataTemplate>
                        </asp:ListView>
                    </div>
                </div>
            </div>
        </div>
        <br />
        <br />
    </asp:Content>

