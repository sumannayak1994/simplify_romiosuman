<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="SimpliFi.index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="js/jspdf.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/tether/1.4.0/js/tether.min.js" integrity="sha384-DztdAPBWPRXSA/3eYEEUWrWCy7G5KFbe8fFjk5JAIxUYHKkDx6Qin1DkWx51bBrb" crossorigin="anonymous"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/html2canvas/0.4.1/html2canvas.min.js"></script>
    <script src="js/proof-validation.js?v=<%= ConfigurationManager.AppSettings["Version"] %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <ol class="breadcrumb">
        <li class="breadcrumb-item"><a href="index">
            <asp:Label ID="lblHeaderProgramText" Text="" runat="server" />
        </a><i class="fa fa-angle-right"></i></li>
        <li class="w-80"><a href="#">
            <asp:Label runat="server" ID="lblProgramName"></asp:Label>
        </a></li>
        <li class="breadcrumb-info"><a title="Read me before using this tool!" href="#" class="a-info"><i class="fa fa-info-circle pull-right"></i></a></li>
    </ol>

    <div class="loader">
        <h4 id="hLoaderText" class="loader-text">Please wait a moment...</h4>
    </div>

    <div class="modal fade" id="reportsModal" role="dialog">
        <div class="modal-dialog modal-md">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title c-blue">Save Report</h4>
                </div>
                <div class="modal-body ">
                    <div class="f-14">
                        <%--<div class="col-md-12">--%>
                        <label class="lbl-filename">File Name: </label>
                        <div class="input-group input-icon right div-filename">
                            <span class="input-group-addon">
                                <i class="fa fa-file-pdf-o"></i>
                            </span>
                            <input id="reportName" class="form-control1" placeholder="Report Name">
                        </div>
                        <label class="lbl-sub">Subject Line: </label>
                        <div class="input-group input-icon right div-sub">
                            <span class="input-group-addon">
                                <i class="fa fa-paragraph"></i>
                            </span>
                            <input id="subjectLine" class="form-control1" placeholder="Subject Line">
                        </div>
                        <%--</div>--%>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" id="btnSaveAndShare" class="btn btn-primary btn-share dark text-white text-center b-r-2 ">Save and Share</button>
                    <button type="button" class="btn btn-primary btn-cancel dark text-white text-center b-r-2 " data-dismiss="modal">Cancel</button>
                </div>
            </div>

        </div>
    </div>

    <div class="modal fade" id="infoModal" role="dialog">
        <div class="modal-dialog modal-lg">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title c-blue">Instruction Guide <small>Step by Step process explaining how to use the tool.</small></h4>
                </div>
                <div class="modal-body ">
                    <div class="f-14">
                        <div class="well p-l-15 m-b-20 b-r-2 ">
                            <p>This tool validates the below points by comparing CGEN file with the uploaded proof emails.</p>
                            <ul>
                                <li>Matches the subject line with CGEN</li>
                                <li>Matches the preheader with CGEN</li>
                                <li>Checks for spelling mistakes in Subject Line and Preheader for English</li>
                                <li>Matches the tag ids and tag URLs with CGEN</li>
                                <li>Checks if unsub page is available or not</li>
                                <li>Checks if mirror page is available or not</li>
                                <li>Checks if duplicate mirror page is available in web version or not</li>
                                <li>Checks if all hyperlinks are working or not</li>
                                <li>Checks for proper localization of privacy policy, contact page and unsub page</li>
                                <li>Checks for junk characters</li>
                                <li>Checks for Copyright</li>
                                <li>Checks for Senderbook Address</li>
                                <li id="liSenderName" runat="server" visible="false">Checks for Sender Name</li>
                                <li id="li250OK" runat="server" visible="false">Checks for 250 OK status</li>
                            </ul>
                        </div>
                        <strong class="c-blue">Step 1: Upload the CGEN File</strong>
                        <br>
                        <br>
                        <ul>
                            <li>Save the exact CGEN file, you upload to ACI as .xls or .xlsx format.</li>
                            <li>Upload the excel file to the system using the upload link for CGEN.</li>
                        </ul>
                        <span class="c-red"><strong>Note:</strong> make sure you do not alter the column headers.</span>
                        <a id="aTriggerCGEN" visible="false" runat="server" href="UploadedFiles/CGEN/CGen_Acrobat%20Trials%20Recipe%202%20NAM_v8.xlsx" download class="hvr-icon-down col-3 btn-download b-r-2 ">Download Sample CGEN</a>
                        <a id="aBatchCGEN" visible="false" runat="server" href="UploadedFiles/CGEN/Simpli_CGEN_B2C_Batch.xlsx" download class="hvr-icon-down col-3 btn-download b-r-2 ">Download Sample CGEN</a>
                        <br>
                        <br>
                        <strong class="c-blue">Step 2: Upload Email Files</strong>
                        <br>
                        <br>
                        <div id="divSophia" visible="false" runat="server">
                            <ul>
                                <li>While sending proofs from CCTC make sure to replace the below line as your subject line.</li>
                            </ul>
                            <p class="well">Locale - &lt;%= targetData.segmentLocale %&gt; | SourceCode - &lt;%= targetData.campaignSourceCode %&gt; | &lt;%= targetData.samCapFirst.subjectLine %&gt;</p>
                        </div>
                        <div id="divTriggered" visible="false" runat="server">
                            <ul>
                                <li>While sending proofs from ACI make sure to replace the below line as your subject line.</li>
                            </ul>
                            <p class="well">Wave - &lt;%= targetData.wave %&gt; | Locale - &lt;%= targetData.segmentLocale %&gt; | SourceCode - &lt;%= targetData.campaignSourceCode %&gt; | &lt;%= targetData.samCapFirst.subjectLine %&gt;</p>
                        </div>
                        <div id="divBatch" visible="false" runat="server">
                            <ul>
                                <li>While sending proofs from ACI make sure your subject line looks like below.</li>
                            </ul>
                            <p class="well">[A217742 Proof 2 multipart/alternative] Ta-da—keynote speakers announced</p>
                        </div>
                        <div id="divAPAC" visible="false" runat="server">
                            <ul>
                                <li>While sending proofs from ACI make sure your subject line looks like below.</li>
                            </ul>
                            <p class="well">[A217742 Proof 2 multipart/alternative] Ta-da—keynote speakers announced</p>
                        </div>
                        <ul>
                            <li>Now open your outlook and select and save the proof emails you want to validate, in a folder.</li>
                            <li>Choose all the emails and upload using the upload link for emails.</li>
                            <li>Once you upload all the emails, click on the Validate Proof button.</li>
                        </ul>
                        <br>
                        <%-- <span class="c-red"><strong>Note:</strong> This tool is using multi threading programming, so sometimes skips one email, in that case you will not find the Details(<span class='glyphicon glyphicon-new-window c-blue f-14'></span>) icon at the end.
                           <br />
                            So, Once you are done with the rest of the proof validation, Upload the Errored Proof again and validate it.
                        </span>
                        <br>
                        <br>--%>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary bg-danger dark text-white text-center b-r-2 " data-dismiss="modal">Close</button>
                </div>
            </div>

        </div>
    </div>
    <div class="modal fade" id="myModal" role="dialog">
        <div class="modal-dialog modal-lg">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title c-blue md-details-header">Proof Details</h4>
                </div>
                <div class="modal-body">
                    <div id="divDetails"></div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-primary bg-danger dark text-white text-center b-r-2 " data-dismiss="modal">Close</button>
                </div>
            </div>

        </div>
    </div>
    <div class="agile-tables">
        <div class="row w3l-table-info">
            <%--<h2>Upload CGEN File & Proof Emails</h2>--%>
            <div class="col-sm-6 w-48 cgen-header b-r-2 p-0">
                <h5>Current CGEN:
                    <asp:Label runat="server" ID="lblCurrCGENName" CssClass="file-names"></asp:Label>
                </h5>
                <div class="col-sm-12 p-b-20 p-t-10">
                    <%--<p>Choose the CGEN File to upload</p>--%>
                    <div class="col-sm-8">
                        <asp:FileUpload ID="flCGEN" runat="server" accept=".xls, .xlsx" />
                    </div>
                    <div class="col-sm-4">
                        <asp:Button runat="server" ID="btnCGENUpload" CssClass="btn btn-primary bg-primary dark text-white text-center b-r-2 " Text="Upload CGEN" OnClick="btnCGENUpload_Click" />
                    </div>
                </div>
            </div>
            <div class="col-sm-6 w-48 cgen-header b-r-2 p-0 m-l-2">
                <h5>Current Proof Emails Count:
                    <asp:Label runat="server" ID="lblCurrEmailCount" CssClass="file-names"></asp:Label>
                </h5>
                <div class="col-sm-12 p-b-20 p-t-10">
                    <%--<p>Choose the Email Files to upload</p>--%>
                    <div class="col-sm-8">
                        <asp:FileUpload ID="flProofEmails" runat="server" accept=".msg" AllowMultiple="true" />
                    </div>
                    <div class="col-sm-4">
                        <asp:Button runat="server" ID="btnProofEmailsUpload" CssClass="btn btn-primary bg-system dark  text-white text-center b-r-2 " Text="Upload Emails" OnClick="btnProofEmailsUpload_Click" />
                    </div>
                </div>
            </div>
            <div class="col-sm-6 p-l-0 p-t-25">
                <div class="form-group">
                    <asp:Label runat="server" CssClass="col-sm-5 control-label" ID="Label1" Text="Select Tracking Parameter:" />
                    <div class="col-sm-7">
                        <asp:DropDownList runat="server" ID="ddlTrackingParameter" CssClass="form-control">
                            <asp:ListItem Text="trackingid" Value="trackingid" Selected="True" />
                            <asp:ListItem Text="sdid" Value="sdid" />
                        </asp:DropDownList>
                    </div>
                </div>

            </div>
            <div class="col-sm-4 p-t-25">
                <asp:Label runat="server" ID="lblSpellCheck" CssClass="control-label" Text="Spell Check for English:" />
                <asp:RadioButton runat="server" ID="rdSpellCheckTrue" GroupName="spellCheck" Text="Enable" Checked="true" CssClass="radio-inline" />
                <asp:RadioButton runat="server" ID="rdSpellCheckFalse" GroupName="spellCheck" Text="Disable" CssClass="radio-inline" />
            </div>
            <div class="col-sm-2">
                <asp:Button runat="server" ID="btnValidateProofs" CssClass="btn btn-primary pull-right m-t-20 m-l-20 bg-success dark text-white text-center b-r-2 " Text="Validate Proof" OnClick="btnValidateProofs_Click" />
            </div>
            <div class="col-sm-12 p-l-0">
                <div class=" msg-block m-t-20" runat="server" id="divMsg" visible="false">
                    <asp:Label runat="server" ID="lblMsg" Text="Test" />
                </div>
                <div class=" msg-block m-t-20 hide" id="divJqueryMsg">
                    <asp:Label runat="server" ID="lblJqueryMsg" Text="Test" />
                </div>
            </div>
        </div>
    </div>

    <div class="agile-tables" id="divResults" runat="server" visible="false">
        <%--<a href="javascript:GenerateReport()" title="Download Report" class="downloadreport btn m-t-20 m-l-20 bg-blue dark text-white text-center b-r-2 pull-right"><i class="fa fa fa-download"></i></a>--%>
        <%--<a href="javascript:OpenReportModal()" title="Save/Download/Rename Report" class="downloadreport btn m-t-20 m-l-20  text-white text-center bg-green b-r-2 pull-right"><i class="fa fa-cog"></i></a>--%>
        <div class="row w3l-table-info" id="divReport">
            <h2 class="p-l-15 f-24">Program:
                <asp:Label runat="server" ID="lblProgram"></asp:Label>
                <br />
                <span class="time-info b-r-2 ">Validated
                    <asp:Label runat="server" ID="lblEmailCounts"></asp:Label>
                    proof emails in 
                    <asp:Label runat="server" ID="lblTime"></asp:Label>
                    seconds</span>

            </h2>
            <br />
            <div class="col-sm-12">
                <div id="divContent" runat="server">
                </div>
            </div>
        </div>
    </div>

    <img id="imgTest" />

    <div id="divDetailsForPdf" class="agile-tables hide">
    </div>
    <br />
    <br />
</asp:Content>
