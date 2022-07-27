<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="proof-check.aspx.cs" Inherits="SimpliFi.proof_check" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdnjs.cloudflare.com/ajax/libs/tether/1.4.0/js/tether.min.js" integrity="sha384-DztdAPBWPRXSA/3eYEEUWrWCy7G5KFbe8fFjk5JAIxUYHKkDx6Qin1DkWx51bBrb" crossorigin="anonymous"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/html2canvas/0.4.1/html2canvas.min.js"></script>
    <script src="js/proof-validation.js?v=<%= ConfigurationManager.AppSettings["Version"] %>"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <ol class="breadcrumb">
        <li class="breadcrumb-item"><a href="index">
            <asp:label id="lblHeaderProgramText" text="" runat="server" />
        </a><i class="fa fa-angle-right"></i></li>
        <li class="w-80"><a href="#">
            <asp:label runat="server" id="lblProgramName"></asp:label>
        </a></li>
        <li class="breadcrumb-info"><a title="Read me before using this tool!" href="#" class="a-info"><i class="fa fa-info-circle pull-right"></i></a></li>
    </ol>

    <div class="loader">
        <h4 id="hLoaderText" class="loader-text">Please wait a moment...</h4>
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
            <div class="col-sm-12 cgen-header b-r-2 p-0 m-l-0">
                <h5>Current Proof Emails Count:
                    <asp:label runat="server" id="lblCurrEmailCount" cssclass="file-names"></asp:label>
                </h5>
                <div class="col-sm-12 p-b-20 p-t-10">
                    <%--<p>Choose the Email Files to upload</p>--%>
                    <div class="col-sm-10">
                        <asp:fileupload id="flProofEmails" runat="server" accept=".msg" allowmultiple="true" />
                    </div>
                    <div class="col-sm-2">
                        <asp:button runat="server" id="btnProofEmailsUpload" cssclass="btn btn-primary bg-system dark  text-white text-center b-r-2 " text="Upload Emails" onclick="btnProofEmailsUpload_Click" />
                    </div>
                </div>
            </div>
            <div class="col-sm-6 p-l-0 p-t-25">
                <div class="form-group">
                    <asp:label runat="server" cssclass="col-sm-5 control-label" id="Label1" text="Select Tracking Parameter:" />
                    <div class="col-sm-7">
                        <asp:dropdownlist runat="server" id="ddlTrackingParameter" cssclass="form-control">
                            <asp:ListItem Text="trackingid" Value="trackingid" Selected="True" />
                            <asp:ListItem Text="sdid" Value="sdid" />
                        </asp:dropdownlist>
                    </div>
                </div>

            </div>
            <div class="col-sm-4 p-t-25">
                <asp:label runat="server" id="lblSpellCheck" cssclass="control-label" text="Spell Check for English:" />
                <asp:radiobutton runat="server" id="rdSpellCheckTrue" groupname="spellCheck" text="Enable" checked="true" cssclass="radio-inline" />
                <asp:radiobutton runat="server" id="rdSpellCheckFalse" groupname="spellCheck" text="Disable" cssclass="radio-inline" />
            </div>
            <div class="col-sm-2">
                <asp:button runat="server" id="btnValidateProofs" cssclass="btn btn-primary pull-right m-t-20 m-l-20 bg-success dark text-white text-center b-r-2 " text="Validate Proof" onclick="btnValidateProofs_Click" />
            </div>
            <div class="col-sm-12 p-l-0">
                <div class=" msg-block m-t-20" runat="server" id="divMsg" visible="false">
                    <asp:label runat="server" id="lblMsg" text="Test" />
                </div>
                <div class=" msg-block m-t-20 hide" id="divJqueryMsg">
                    <asp:label runat="server" id="lblJqueryMsg" text="Test" />
                </div>
            </div>
        </div>
    </div>

    <div class="agile-tables" id="divResults" runat="server" visible="false">
        <div class="row w3l-table-info" id="divReport">
            <h2 class="p-l-15 f-24">
                <span class="time-info b-r-2 ">Validated
                    <asp:label runat="server" id="lblEmailCounts"></asp:label>
                    proof emails in 
                    <asp:label runat="server" id="lblTime"></asp:label>
                    seconds</span><br /><br />
                <div class="col-sm-12 p-l-0">
                    <div id="divContent" runat="server">
                    </div>
                </div>
        </div>
    </div>
</asp:Content>
