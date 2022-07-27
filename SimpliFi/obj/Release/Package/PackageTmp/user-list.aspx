<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="user-list.aspx.cs" Inherits="SimpliFi.user_list" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="js/jspdf.min.js"></script>
    <script src="js/html2canvas.js"></script>

    <script type="text/javascript">      
        $(document).ready(function () {

            $(".agrantAccess").click(function (e) {
                e.preventDefault();

                var ldap = $(this).attr("data-ldap");

                $("#hLoaderText").text("Modifying Access Rights of '" + ldap + "'...");
                $(".loader").show();

                UpdateAccess(ldap, 1);
            });

            $(".aRemoveAccess").click(function (e) {
                e.preventDefault();

                var ldap = $(this).attr("data-ldap");

                $("#hLoaderText").text("Modifying Access Rights of '" + ldap + "'...");
                $(".loader").show();

                UpdateAccess(ldap, 0);
            });

            $(".td-actions").each(function () {
                if ($(this).attr('data-access') == "Yes") {
                    $(this).find('.agrantAccess').hide();
                }
                else if ($(this).attr('data-access') == "No") {
                    $(this).find('.aRemoveAccess').hide();
                }
            });
        });

        $(window).load(function () {
            $(".loader").fadeOut("slow");
        });

        function UpdateAccess(ldap, access) {
            $.ajax({
                url: 'user-list.aspx/UpdateUserAccess',
                type: "POST",
                dataType: "json",
                data: JSON.stringify({ 'LDAP': ldap, 'access': access }),
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    $(".loader").fadeOut("slow");

                    if (parseInt(data.d) > 0) {
                        SetJqueryMsg("success", "Access Rights modified for " + ldap + ". Page will be refreshed automatically after 5 seconds.");

                        setTimeout(function () { location.reload(); }, 5000);
                    }
                    else {
                        alert("Unable to modify access rights, Try after sometime.");
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
        <li class="breadcrumb-item"><a href="index.aspx">Users</a><i class="fa fa-angle-right"></i></li>
        <li class="w-80"><a href="validation-list">Access Rights</a>
        </li>
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
                    <asp:ListView ID="lvUsers" runat="server" GroupPlaceholderID="groupPlaceHolder1"
                        ItemPlaceholderID="itemPlaceHolder1" OnPagePropertiesChanging="OnPagePropertiesChanging">
                        <LayoutTemplate>
                            <table class="customers m-t-10">
                                <tr>
                                    <th style="width: 20%">LDAP
                                    </th>
                                    <th style="width: 20%" class="text-center">Has Access
                                    </th>
                                    <th style="width: 40%" class="text-center">Modified On
                                    </th>
                                    <th style="width: 20%" class="text-center">Action
                                    </th>
                                </tr>
                                <asp:PlaceHolder runat="server" ID="groupPlaceHolder1"></asp:PlaceHolder>
                                <tr>
                                    <td colspan="4">
                                        <asp:DataPager ID="DataPager1" runat="server" PagedControlID="lvUsers" PageSize="10">
                                            <Fields>
                                                <asp:NextPreviousPagerField ButtonType="Link" ShowFirstPageButton="false" ShowPreviousPageButton="true" ShowNextPageButton="false" />
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
                            <td>
                                <%# Eval("LDAP") %>
                            </td>
                            <td class="text-center">
                                <span class="sp-<%# Eval("HasAccess") %>"><%# Eval("HasAccess") %></span>
                            </td>
                            <td class="text-center">
                                <%# Eval("CreatedDate", "{0:dddd, dd MMMM yyyy HH:mm:ss}") %>
                            </td>
                            <td class="text-center td-actions" data-access="<%# Eval("HasAccess") %>">
                                <a class="c-green agrantAccess hand" title="Grant Access" data-ldap="<%# Eval("LDAP") %>"><span class='fa fa fa-unlock-alt f-20'></span></a>
                                <a class="c-red aRemoveAccess hand" title="Remove Access" data-ldap="<%# Eval("LDAP") %>"><span class='fa fa fa-lock c-red f-20'></span></a>
                            </td>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <div class="msg-block m-t-20 info text-center">
                                <span>No Reports found.
                                    <br />
                                    Please save one report to view it in this space.</span>
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

