
function ManupulateModal() {
    var headerText = $("#myModal").find('.div-pop-details').text().trim();

    $('.md-details-header').text(headerText);
    $('.div-pop-details').remove();
}

$(document).ready(function () {

    $(".loader").hide();

    $('.spDetails').click(function (e) {
        e.preventDefault();

        $('#divDetails').html('');
        $('#divDetails').html($(this).parent().attr('data-details'));

        ManupulateModal();

        $("#myModal").modal();

        return false;

    });

    $('.breadcrumb-info a').click(function () {
        $("#infoModal").modal();
    });

    $("#btnValidateProofs").click(function () {
        $("#hLoaderText").text("Validation in progress...");
        $(".loader").show();
    });

    $('a.spDetails').click(function (e) {
        e.preventDefault();
    });

    $('#btnSaveReport').click(function () {

        var reportName = $("#lblProgramName").text().replace('Program: ', '').trim();
        SaveReport(reportName, "save");
    });

    $("#reportName, #emailId, #subjectLine").keydown(function () {
        $(this).parent().removeClass('has-error');
    });

    $('#btnDownloadReport').click(function () {
        var reportName = $("#lblProgramName").text().replace('Program: ', '').trim();
        GenerateReport(reportName);
    });

    $('#btnSaveAndShare').click(function () {

        if (ValidateSaveAndShareSettings()) {
            $('#reportsModal').modal('hide');
            var fileName = $('#reportName').val().trim() != "" ? $('#reportName').val().trim() : $("#lblProgramName").text();
            //fileName = fileName.replace(/\ /g, '_');
            SaveReport(fileName, "share");
        }
    });

    $('.c-orange .glyphicon-info-sign').attr('title', 'We found some spelling mistakes. Please check details screen.')
});

function ValidateSaveAndShareSettings() {
    var isValid = true;

    //Report name validation
    if ($('#reportName').val().trim() != "" && $('#reportName').val().trim().length > 5) {
        isValid = isValid ? true : false;
    }
    else {
        isValid = false;
        $('#reportName').parent().addClass('has-error');
    }

    //Email id validation
    //if ($('#emailId').val().trim() != "" && ValidateEmail($('#emailId').val().trim())) {
    //    isValid = isValid ? true : false;
    //}
    //else {
    //    isValid = false;
    //    $('#emailId').parent().addClass('has-error');
    //}

    //Subject Line validation
    if ($('#subjectLine').val().trim() != "" && $('#subjectLine').val().trim().length > 5) {
        isValid = isValid ? true : false;
    }
    else {
        isValid = false;
        $('#subjectLine').parent().addClass('has-error');
    }

    return isValid;
}

function ValidateEmail(email) {
    var pattern = /^\b[A-Z0-9._%-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b$/i

    if (!pattern.test(email)) {
        return false;
    }
    else {
        return true;
    }
}

$(window).load(function () {
    $(".loader").fadeOut("slow");
});

var pdf, page_section, HTML_Width, HTML_Height, top_left_margin, PDF_Width, PDF_Height, canvas_image_width, canvas_image_height;

function GenerateReport(reportName) {

    $("#hLoaderText").text("Downloading your report...");
    $(".loader").show();

    $('#divDetailsForPdf').removeClass('hide');
    $('#divDetailsForPdf').html('');
    $('#divDetailsForPdf').html($("#divResults").html());

    $('#divDetailsForPdf .tblReport').remove();
    $('#divDetailsForPdf .tblMaster  tr td.details-td').remove();
    $('#divDetailsForPdf .tblMaster  tr th:last').remove();
    //$('#divDetailsForPdf .downloadreport').remove();
    $('#divDetailsForPdf .time-info').remove();

    $('#divDetailsForPdf').css("width", "1110px");


    html2canvas(document.getElementById("divDetailsForPdf"), {
        onrendered: function (canvas) {
            var tempcanvas = document.createElement('canvas');

            calculatePDF_height_width("#divDetailsForPdf", 0);

            tempcanvas.width = HTML_Width;
            tempcanvas.height = HTML_Height;

            var context = tempcanvas.getContext('2d');
            context.drawImage(canvas, top_left_margin, top_left_margin, HTML_Width, HTML_Height);
            
            var imgData = canvas.toDataURL('image/jpg');

            var pdf = new jsPDF('p', 'pt', [PDF_Width, PDF_Height]),
                pdfConf = {
                    pagesplit: true,
                    background: '#fff'
                };

            pdf.addImage(imgData, 'JPG', top_left_margin, top_left_margin, HTML_Width, HTML_Height);

            pdf.save(reportName + ".pdf");

            $('#divDetailsForPdf').addClass('hide');

            $(".loader").fadeOut("slow");

            SetJqueryMsg("success", "Report downloaded succesfully.");
        }
    });

}

function calculatePDF_height_width(selector, index) {
    page_section = $(selector).eq(index);
    HTML_Width = page_section.width();
    HTML_Height = page_section.height();
    top_left_margin = 15;
    PDF_Width = HTML_Width + (top_left_margin * 2);
    PDF_Height = (PDF_Width * 1.2) + (top_left_margin * 2);
    canvas_image_width = HTML_Width;
    canvas_image_height = HTML_Height;
}

function SaveReport(fileName, flag) {
    $("#hLoaderText").text("Saving your report...");
    $(".loader").show();

    $('#divDetailsForPdf').removeClass('hide');
    $('#divDetailsForPdf').html('');
    $('#divDetailsForPdf').html($("#divResults").html());

    $('#divDetailsForPdf .tblReport').remove();
    $('#divDetailsForPdf .tblMaster  tr td.details-td').remove();
    $('#divDetailsForPdf .tblMaster  tr th:last').remove();
    //$('#divDetailsForPdf .downloadreport').remove();
    $('#divDetailsForPdf .time-info').remove();

    $('#divDetailsForPdf').css("width","1110px");

    html2canvas(document.getElementById("divDetailsForPdf"), {
        onrendered: function (canvas) {
            var tempcanvas = document.createElement('canvas');

            calculatePDF_height_width("#divDetailsForPdf", 0);

            tempcanvas.width = HTML_Width;
            tempcanvas.height = HTML_Height;

            var context = tempcanvas.getContext('2d');
            context.drawImage(canvas, top_left_margin, top_left_margin, HTML_Width, HTML_Height);

            var imgData = canvas.toDataURL('image/jpg');

            var pdf = new jsPDF('p', 'pt', [PDF_Width, PDF_Height]),
                pdfConf = {
                    pagesplit: true,
                    background: '#fff'
                };

            pdf.addImage(imgData, 'JPG', top_left_margin, top_left_margin, HTML_Width, HTML_Height);
            
            $('#divDetailsForPdf').addClass('hide');
            var binary = pdf.output();
            binary = binary ? btoa(binary) : "";

            $.ajax({
                url: 'index.aspx/SavePDF',
                type: "POST",
                dataType: "json",
                data: JSON.stringify({ 'binaryData': binary, "reportName": fileName }),
                contentType: "application/json; charset=utf-8",
                success: function (data) {

                    if (data.d != "") {

                        if (flag == 'save') {
                            SetJqueryMsg("success", "Report saved to your profile successfully.");

                            $(".loader").fadeOut("slow");
                            window.open(data.d, '_blank');
                        }
                        else if (flag == 'share') {
                            ShareEmail(data.d);
                        }
                    }
                    else {
                        SetJqueryMsg("error", "Unable to save Report, Try after sometime.");
                    }
                }
            });
        }
    });
}

function ShareEmail(reportUrl) {
    var programName = $('#lblProgramName').text().replace('Program: ', '');
    //var email = $('#emailId').val().trim();
    var subjectLine = $('#subjectLine').val().trim();
    var encodedURL = encodeURI(reportUrl);
    var emailDetails = "mailto:?subject=" + subjectLine + "&body=Hello,%0D%0DPlease click the below%20link to see the " + subjectLine + ".%0D%0D" + encodedURL + "%0D%0DThanks%0DSimpliFi";

    $(".loader").fadeOut("slow");
    window.open(emailDetails, '_top');

    //$.ajax({
    //    url: 'index.aspx/SendEmailViaOutLook',
    //    type: "POST",
    //    dataType: "json",
    //    data: JSON.stringify({ "reportUrl": reportUrl, "programName": programName, "email": email, "subjectLine": subjectLine }),
    //    contentType: "application/json; charset=utf-8",
    //    success: function (data) {

    //        $(".loader").fadeOut("slow");

    //        if (data.d) {
    //            SetJqueryMsg("success", "Report saved and shared with " + email + ", successfully.");
    //        }
    //        else {
    //            SetJqueryMsg("error", "Unable to share Report, Try after sometime.");
    //        }
    //    }
    //});
}

function SetJqueryMsg(type, text) {

    $("#divJqueryMsg").removeClass('hide');
    $("#divJqueryMsg").removeClass('error');
    $("#divJqueryMsg").removeClass('success');
    $("#divJqueryMsg").addClass(type);
    $("#lblJqueryMsg").text(text);
}

function OpenReportModal(type) {
    $('#reportName , #subjectLine, #emailId').parent().removeClass('has-error');

    var programName = $("#lblProgramName").text().replace('Program: ', '').trim();
    var subjectLine = "Test Report on: " + programName;

    $('#reportName').val(programName);
    $('#subjectLine').val(subjectLine);

    $("#reportsModal").modal();
}
