<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="sso.aspx.cs" Inherits="SimpliFi.sso" %>

<!DOCTYPE html>

<html>

<head>
    <title>SimpliFi - Adobe Systems | ACS GMO Initiative</title>
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
    <link href="https://fonts.googleapis.com/css?family=Lato" rel="stylesheet">
    <link href="css/font-awesome.css" rel="stylesheet">
    <link href="css/style-login.css" rel='stylesheet' type='text/css' />
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
                    $('#lblJqueryError').text("Login Error: Please fix above issues.");
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
    <img class="adobe-logo" src="images/adobe-logo.png" />
    <h1>Welcome to SimpliFi</h1>

    <!-- //form ends here -->
    <!--copyright-->
    <div class="copy-wthree">
        <p>
            &copy;
            <asp:Label runat="server" ID="lblYear"></asp:Label>
            SimpliFi, Adobe Systems | ACS GMO Initiative
        </p>

    </div>
    <!--//copyright-->
</body>

</html>
