<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="usage-report.aspx.cs" Inherits="SimpliFi.usage_report" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link rel="stylesheet" href="//code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css">
    <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>
    <script type="text/javascript">      
        $(document).ready(function () {
            $("#txtFromDate, #txtToDate").attr('readonly', 'readonly');
        });
        $(function () {
            var dateFormat = "mm/dd/yy",
                from = $("#txtFromDate")
                    .datepicker({
                        defaultDate: "+1w",
                        numberOfMonths: 2,
                        maxDate: 1
                    })
                    .on("change", function () {
                        to.datepicker("option", "minDate", getDate(this));
                    }),
                to = $("#txtToDate").datepicker({
                    defaultDate: "+1w",
                    numberOfMonths: 2,
                    maxDate: 1
                })
                    .on("change", function () {
                        from.datepicker("option", "maxDate", getDate(this));
                    });

            function getDate(element) {
                var date;
                try {
                    date = $.datepicker.parseDate(dateFormat, element.value);
                } catch (error) {
                    date = null;
                }

                return date;
            }
        });
    </script>
    <script type="text/javascript">    

        $(window).load(function () {
            $(".loader").fadeOut("slow");
        });
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
    <div class="four-grids">
        <div class="col-md-3 four-grid">
            <div class="four-agileits">
                <div class="icon">
                    <i class="glyphicon glyphicon-user" aria-hidden="true"></i>
                </div>
                <div class="four-text">
                    <h3>Users</h3>
                    <h4>24,420  </h4>

                </div>

            </div>
        </div>
        <div class="col-md-3 four-grid">
            <div class="four-agileinfo">
                <div class="icon">
                    <i class="glyphicon glyphicon-list-alt" aria-hidden="true"></i>
                </div>
                <div class="four-text">
                    <h3>Programs</h3>
                    <h4>15,520</h4>

                </div>

            </div>
        </div>
        <div class="col-md-3 four-grid">
            <div class="four-w3ls">
                <div class="icon">
                    <i class="glyphicon glyphicon-folder-open" aria-hidden="true"></i>
                </div>
                <div class="four-text">
                    <h3>Proofs</h3>
                    <h4>12,430</h4>

                </div>

            </div>
        </div>
        <div class="col-md-3 four-grid">
            <div class="four-wthree">
                <div class="icon">
                    <i class="glyphicon glyphicon-briefcase" aria-hidden="true"></i>
                </div>
                <div class="four-text">
                    <h3>Time</h3>
                    <h4>14,430</h4>

                </div>

            </div>
        </div>
        <div class="clearfix"></div>
    </div>
    <div class="breadcrumb">
        <div class="row">
            <div class="form-group col-sm-5">
                <label>From Date</label>
                <asp:TextBox runat="server" ID="txtFromDate" CssClass="form-control" />
            </div>

            <div class="form-group col-sm-5">
                <label>To Date</label>

                <asp:TextBox runat="server" ID="txtToDate" CssClass="form-control" />

            </div>
            <div class="form-group col-sm-2">
                <asp:LinkButton ID="lbtnSearch" Style="margin-top: 25px;" runat="server" OnClick="lbtnSearch_Click" CssClass="btn btn-primary bg-system dark  text-white text-center b-r-2 "><span class='fa fa fa-search f-20'></span> Search</asp:LinkButton>
            </div>
        </div>
    </div>

    <div class="loader">
        <h4 id="hLoaderText" class="c-blue loader-text">Please wait a moment...</h4>
    </div>

    <div class="agile-tables" id="divResults">
        <div class="row w3l-table-info" id="divReport">
            <div class="col-sm-6">
                <asp:ListView ID="lvReportByProgramType" runat="server" GroupPlaceholderID="groupPlaceHolder1"
                    ItemPlaceholderID="itemPlaceHolder1" OnPagePropertiesChanging="OnPagePropertiesChanging_Types">
                    <LayoutTemplate>
                        <table class="customers m-t-10">
                            <tr>
                                <th style="width: 10%" class="text-center">#
                                </th>
                                <th style="width: 30%">Program Type
                                </th>
                                <th style="width: 30%" class="text-center">Usage Frequency
                                </th>
                                <th style="width: 30%" class="text-center">Proofs Count
                                </th>
                            </tr>
                            <asp:PlaceHolder runat="server" ID="groupPlaceHolder1"></asp:PlaceHolder>
                            <tr>
                                <td colspan="4">
                                    <asp:DataPager ID="DataPager1" runat="server" PagedControlID="lvReportByProgramType" PageSize="10">
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
                            <%# Eval("ProgramType") %>
                        </td>
                        <td class="text-center">
                            <%# Eval("Usage Count") %>
                        </t>
                        <td class="text-center">
                            <%# Eval("Proofs Count") %>
                        </td>
                    </ItemTemplate>
                    <EmptyDataTemplate>
                        <div class="msg-block m-t-20 info text-center">
                            <span>No Reports found.
                        </div>
                    </EmptyDataTemplate>
                </asp:ListView>
            </div>
            <div class="col-sm-6">
                <asp:ListView ID="lvReportByLDAPS" runat="server" GroupPlaceholderID="groupPlaceHolder1"
                    ItemPlaceholderID="itemPlaceHolder1" OnPagePropertiesChanging="OnPagePropertiesChanging_LDAPS">
                    <LayoutTemplate>
                        <table class="customers m-t-10">
                            <tr>
                                <th style="width: 10%" class="text-center">#
                                </th>
                                <th style="width: 30%">LDAP
                                </th>
                                <th style="width: 30%" class="text-center">Usage Frequency
                                </th>
                                <th style="width: 30%" class="text-center">Proofs Count
                                </th>
                            </tr>
                            <asp:PlaceHolder runat="server" ID="groupPlaceHolder1"></asp:PlaceHolder>
                            <tr>
                                <td colspan="4">
                                    <asp:DataPager ID="DataPager1" runat="server" PagedControlID="lvReportByLDAPS" PageSize="10">
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
                            <%# Eval("LDAP") %>
                        </td>
                        <td class="text-center">
                            <%# Eval("Usage Count") %>
                        </t>
                        <td class="text-center">
                            <%# Eval("Proofs Count") %>
                        </td>
                    </ItemTemplate>
                    <EmptyDataTemplate>
                        <div class="msg-block m-t-20 info text-center">
                            <span>No Reports found.
                        </div>
                    </EmptyDataTemplate>
                </asp:ListView>
            </div>
            <div class="col-sm-12">
                <%--<asp:ListView ID="lvProgramList" runat="server" GroupPlaceholderID="groupPlaceHolder1"
                        ItemPlaceholderID="itemPlaceHolder1" OnPagePropertiesChanging="OnPagePropertiesChanging">
                        <LayoutTemplate>
                            <table class="customers m-t-10">
                                <tr>
                                    <th style="width: 5%" class="text-center">#
                                    </th>
                                    <th style="width: 10%">LDAP
                                    </th>
                                    <th style="width: 15%">Program Type
                                    </th>
                                    <th style="width: 40%">Program Name
                                    </th>
                                    <th style="width: 10%">Proofs Count
                                    </th>
                                    <th style="width: 10%" class="text-center">Validated On
                                    </th>
                                    <th style="width: 10%" class="text-center">View
                                    </th>
                                </tr>
                                <asp:PlaceHolder runat="server" ID="groupPlaceHolder1"></asp:PlaceHolder>
                                <tr>
                                    <td colspan="7">
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
                                <%# Eval("Id") %>
                            </td>
                            <td>
                                <%# Eval("LDAP") %>
                            </td>
                            <td>
                                <%# Eval("ProgramType") %>
                            </td>
                            <td>
                                <%# Eval("ProgramName") %>
                            </td>
                            <td class="text-center">
                                <%# Eval("ProofsCount") %>
                            </td>
                            <td class="text-center">
                                <%# Eval("ValidatedOn") %>
                            </td>
                            <td class="text-center">
                                <a title="View/Download" target="_blank" href="<%# Eval("ValidatedOn") %>"><span class='fa fa fa-download f-20'></span></a>
                            </td>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <div class="msg-block m-t-20 info text-center">
                                <span>No Reports found.
                                    <br />
                                    Please save one report to view it in this space.</span>
                            </div>
                        </EmptyDataTemplate>
                    </asp:ListView>--%>
            </div>
        </div>
    </div>
    <br />
    <br />
</asp:Content>

