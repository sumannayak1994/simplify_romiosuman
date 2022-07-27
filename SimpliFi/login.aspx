<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="SimpliFi.login" %>

<!DOCTYPE HTML>
<html>

<head>
    <title>SimpliFi - Adobe Inc. | An ACS GMO Initiative</title>
    <!-- Meta-Tags -->
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <meta charset="utf-8">
    <script>
        addEventListener("load", function () {
            setTimeout(hideURLbar, 0);
        }, false);

        function hideURLbar() {
            window.scrollTo(0, 1);
        }

    </script>
    <!-- //Meta-Tags -->
    <!-- Stylesheets -->
    <script src="js/jquery-2.1.4.min.js"></script>
    <link href="css/font-awesome.css" rel="stylesheet">
    <link href="css/style-login.css?v=<%= ConfigurationManager.AppSettings["Version"] %>" rel='stylesheet' type='text/css' />
    <!--// Stylesheets -->

    <script type="text/javascript">


        $(document).ready(function () {

            $("#btnEnter").click(function () {

                $('#pJqueryError').addClass("hide");
                $('#lblJqueryError').text("");
                $('.agile-field-txt').removeClass('error');

                var isValid = true;

                if ($("#ddlProgram").val() != "Select") {
                    isValid = isValid ? true : false;
                }
                else {
                    isValid = false;

                    $("#ddlProgram").closest('.agile-field-txt').addClass('error');
                }

                if (!isValid) {
                    $('#pJqueryError').removeClass("hide");
                    $('#lblJqueryError').text("Error: Please select a program to continue.");
                }

                return isValid;
            });

            $("#ddlProgram").change(function () {
                $(this).closest('.agile-field-txt').removeClass('error');
                RemoveErrorMsg();
            });

            // Handler for .ready() called.
            $('html, body').animate({
                scrollTop: 100
            }, 'slow');
        });

        function RemoveErrorMsg() {
            if ($('.agile-field-txt.error').length == 0) {
                $('#pJqueryError').addClass("hide");
                $('#lblJqueryError').text("");
            }
        }

    </script>
</head>

<body>
    <div class="main-w3ls">
        <div class="left-content">
            <div class="w3ls-content">
                <!-- logo -->
                <h1>
                    <a href="index.html">Adobe Inc.</a>
                </h1>
                <!-- //logo -->
                <h2>Welcome to SimpliFi</h2>
                <div class="copyright">
                    <p>
                        &copy;
                        <asp:Label runat="server" ID="lblYear"></asp:Label>
                        SimpliFi, Adobe Inc. | ACS GMO Initiative
                    </p>
                </div>
            </div>
            <!-- copyright -->

            <!-- //copyright -->
        </div>
        <div class="right-form-agile">
            <!-- content -->
            <div class="sub-main-w3">
                <div style="padding-top: 85px;">
                    <h5 runat="server" id="pageHeader" style="padding-bottom: 20px; font-family: 'Lato', sans-serif;">Select a Program</h5>
                    <form runat="server" id="form1">
                        <div class="agile-field-txt" id="divProgram" runat="server" visible="false">


                            <asp:DropDownList runat="server" ID="ddlProgram" CssClass="hand ">
                                <asp:ListItem Text="Choose Program" Value="Select" />
                                <asp:ListItem Text="B2C Triggered Program" Value="B2CTriggered" />
                                <asp:ListItem Text="B2C Batch Program" Value="B2CBatch" />
                                <asp:ListItem Text="Sophia Triggered Program" Value="SophiaTriggered" />
                                <asp:ListItem Text="APAC Batch Program" Value="APACBBatch" />
                            </asp:DropDownList>
                        </div>
                        <div class="w3ls-login  w3l-sub" id="divEnter" runat="server" visible="false">
                            <asp:Button runat="server" ID="btnEnter" Text="Enter to SimpliFi" OnClick="btnEnter_Click" />
                        </div>
                        <div class="w3ls-login  w3l-sub" id="divLogin" runat="server" visible="false">
                            <asp:Button runat="server" ID="btnLogin" Text="Login" OnClick="btnLogin_Click" />
                        </div>
                        <p runat="server" id="pError" class="login-error">
                            <asp:Label runat="server" ID="lblError" Visible="false"></asp:Label>
                        </p>
                        <p id="pJqueryError" class="login-error hide">
                            <asp:Label runat="server" ID="lblJqueryError"></asp:Label>
                        </p>
                    </form>
                </div>
            </div>
        </div>
    </div>
    <!-- //content -->

</body>

</html>
