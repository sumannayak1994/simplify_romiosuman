<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Proofers.aspx.cs" Inherits="SimpliFi.DCMCheck" %>
<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
   <head runat="server">
      <meta charset="utf-8">
      <link rel="stylesheet/less" type="text/css" href="styles.less" />
      <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css">
      <script src="Scripts/jquery-3.3.1.min.js" type="text/javascript"></script>
      <script type="text/javascript">
          function makeItLive() {
              $("#mainContent").css("filter", "blur(1.5px)");
              document.getElementById("makeItLive").style.display = "inline";
          }



          function Scroll() {
              document.getElementById("makeItLive").style.display = "none";
              $('html, body').animate({
                  scrollTop: $("#analysis").offset().top
              }, 'slow');
          }

      </script>
      <style>
         @font-face {
         font-family: playFairDisplayBold;
         src:url("/fonts/PlayfairDisplay-Bold.ttf");
         }
         playFairDisplayBold{
         font-family: playFairDisplayBold;
         }
         body {
         padding:0px;
         margin : 0px;
         min-height:100%;             
         background:linear-gradient(0deg, rgba(0,0,0,0.8), rgba(0,0,0,0.8)),url('Images/blackHole.jpg');
         background-color:black;
         background-size:auto;
         background-position: center;
         }
         /* General button style */
         .btn {
         border: none;
         font-family: 'Lato';
         font-size: 12px;
         color: inherit;
         background: none;
         cursor: pointer;
         padding: 25px 80px;
         display: inline-block;
         margin: 5px 5px;
         text-transform: uppercase;
         letter-spacing: 1px;
         font-weight: 700;
         outline: none;
         position: relative;
         -webkit-transition: all 0.3s;
         -moz-transition: all 0.3s;
         transition: all 0.3s;
         }
         .btn:after {
         content: '';
         position: absolute;
         z-index: -1;
         -webkit-transition: all 0.3s;
         -moz-transition: all 0.3s;
         transition: all 0.3s;
         }
         /* Pseudo elements for icons */
         .btn:before {
         font-family: 'FontAwesome';
         speak: none;
         font-style: normal;
         font-weight: normal;
         font-variant: normal;
         text-transform: none;
         line-height: 1;
         position: relative;
         -webkit-font-smoothing: antialiased;
         }
         /* Icon separator */
         .btn-sep {
         padding:10px 24px 10px 48px;
         }
         .btn-sep:before {
         background: rgba(0,0,0,0.15);
         }
         /* Button 1 */
         .btn-1 {
         background: #3498DB;
         color: #fff;
         }
         .btn-1:hover {
         background: #2980B9;
         }
         .btn-1:active {
         background: #2980B9;
         top: 2px;
         }
         .btn-1:before {
         position: absolute;
         height: 100%;
         left: 0;
         top: 0;
         line-height: 3;
         font-size: 100%;
         width: 40px;
         }
         /* Button 2 */
         .btn-2 {
         background: #2ECC71;
         color: #fff;
         }
         .btn-2:hover {
         background: #27AE60;
         }
         .btn-2:active {
         background: #27AE60;
         top: 2px;
         }
         .btn-2:before {
         position: absolute;
         height: 100%;
         left: 0;
         top: 0;
         line-height: 3;
         font-size: 100%;
         width: 40px;
         }
         /* Button 3 */
         .btn-3 {
         background: #E74C3C;
         color: #fff;
         }
         .btn-3:hover {
         background: #C0392B;
         }
         .btn-3:active {
         background: #C0392B;
         top: 2px;
         }
         .btn-3:before {
         position: absolute;
         height: 100%;
         left: 0;
         top: 0;
         line-height: 3;
         font-size: 100%;
         width: 40px;
         }
         /* Button 3 */
         .btn-4 {
         background: #34495E;
         color: #fff;
         top: 98px;
         left: -479px;
         }
         .btn-4:hover {
         background: #2C3E50;
         }
         .btn-4:active {
         background: #2C3E50;
         top: 2px;
         }
         .btn-4:before {
         position: absolute;
         height: 100%;
         left: 0;
         top: 0;
         line-height: 3;
         font-size: 100%;
         width: 40px;
         }
         {
         box-sizing: border-box;
         }
         /* Create three equal columns that floats next to each other */
         .column {
         float: left;
         width: 32.5%;
         height: 200px; /* Should be removed. Only for demonstration */
         padding-top:20px;
         }
         /* Clear floats after the columns */
         .row:after {
         content: "";
         display: table;
         clear: both;
         }
         /* Header/Logo Title */
         .header {
         padding: 20px;
         color: #b57edc;
         }
         .icon-bar {
         overflow: auto;
         }
         .icon-bar a {
         float:left ;
         text-align: center;
         padding-left : 20px;
         padding-right:20px;
         transition: all 0.3s ease;
         font-size: 36px;
         color:#b57edc;
         }
         .icon-bar a:hover {
         color:#86c232;
         }
         .active {
         color:#86c232;
         }
         .highlight
         {
         background-color:#6B6E70;
         border:1px dashed #b57edc;
         height: 160px;
         }
         /* General button style */
         .btn {
         border: none;
         font-size: 17px;
         color: inherit;
         background: none;
         cursor: pointer;
         padding: 10px 30px;
         display: inline-block;
         margin: 5px 5px;
         letter-spacing: 1px;
         font-weight: 300;
         outline: none;
         position: relative;
         -webkit-transition: all 0.3s;
         -moz-transition: all 0.3s;
         transition: all 0.3s;
         }
         .btn:after {
         content: '';
         position: absolute;
         z-index: -1;
         -webkit-transition: all 0.3s;
         -moz-transition: all 0.3s;
         transition: all 0.3s;
         }
         /* Pseudo elements for icons */
         .btn:before {
         font-family: 'FontAwesome';
         speak: none;
         font-style: normal;
         font-weight: normal;
         font-variant: normal;
         text-transform: none;
         line-height: 1;
         position: relative;
         -webkit-font-smoothing: antialiased;
         }   
         /* Main button style */
         .btnMain {
         border: none;
         font-size: 17px;
         color: inherit;
         background: none;
         cursor: pointer;
         padding: 22px 120px;
         display: inline-block;
         margin: 5px 5px;
         letter-spacing: 1px;
         font-weight: 300;
         outline: none;
         position: relative;
         -webkit-transition: all 0.3s;
         -moz-transition: all 0.3s;
         transition: all 0.3s;
         }
         .btnMain:after {
         content: '';
         position: absolute;
         z-index: -1;
         -webkit-transition: all 0.3s;
         -moz-transition: all 0.3s;
         transition: all 0.3s;
         }
         /* Pseudo elements for icons */
         .btnMain:before {
         font-family: 'FontAwesome';
         speak: none;
         font-style: normal;
         font-weight: normal;
         font-variant: normal;
         text-transform: none;
         line-height: 1;
         position: relative;
         -webkit-font-smoothing: antialiased;
         } 
         /* Button 1 */
         .btn-1 {
         background: transparent;
         color: #6B6E70;
         border : 1px solid #86c232;
         }
         .btn-1:hover {
         border : 1px solid #b57edc;
         box-shadow: 0 0 10px #b57edc;
         background:transparent;
         color: white;
         border-radius: 30px;
         }
         .btn-1:active {
         top: 2px;
         border : 1px solid #b57edc;
         box-shadow: 0 0 10px #b57edc;
         background:transparent;
         color: white;
         border-radius: 30px;
         }
         .btn-1:before {
         position: absolute;
         height: 100%;
         left: 0;
         top: 0;
         line-height: 3;
         font-size: 100%;
         width: 40px;
         }
         /* Button 2 */
         .btn-2 {
         background: transparent;
         color: #6B6E70;
         border : 1px solid #86c232;
         }
         .btn-2:hover {
         color: white;
         border : 1px solid #b57edc;
         box-shadow: 0 0 10px #b57edc;
         background:transparent;
         border-radius: 30px;
         }
         .btn-2:active {
         color: white;
         top: 2px;
         border : 1px solid #b57edc;
         box-shadow: 0 0 10px #b57edc;
         background:transparent;
         border-radius: 30px;
         }
         .btn-2:before {
         position: absolute;
         height: 100%;
         left: 0;
         top: 0;
         line-height: 3;
         font-size: 100%;
         width: 40px;
         }
         /* Button 3 */
         .btn-3 {
         background: transparent;
         color: #6B6E70;
         border : 1px solid #86c232;
         background:transparent;
         }
         .btn-3:hover {
         color: white;
         border : 1px solid #b57edc;
         box-shadow: 0 0 10px #b57edc;
         background:transparent;
         border-radius: 30px;
         }
         .btn-3:active {
         color: white;
         top: 2px;
         border : 1px solid #b57edc;
         box-shadow: 0 0 10px #b57edc;
         border-radius: 30px;
         }
         .btn-3:before {
         position: absolute;
         height: 100%;
         left: 0;
         top: 0;
         line-height: 3;
         font-size: 100%;
         width: 40px;
         }
         /* Button 4 */
         .btn-4 {
         background: transparent;
         color: #fff;
         border : 1px solid #6B6E70;
         }
         .btn-4:hover {
         border : 1px solid #86c232;
         box-shadow: 0 0 10px #86c232;
         background: transparent;
         }
         .btn-4:active {
         top: 2px;
         border : 1px solid #86c232;
         box-shadow: 0 0 10px #86c232;
         background: transparent;
         }
         .btn-4:before {
         position: absolute;
         height: 100%;
         left: 0;
         top: 0;
         line-height: 3;
         font-size: 100%;
         width: 40px;
         }
         /* Button 5 */
         .btn-5 {
         background: transparent;
         color:#6B6E70;
         border : 1px solid #b57edc;
         transition: .3s;
         }
         .btn-5:active {
         animation: pulse 1s infinite;
         transition: .3s;
         border : 1px solid #86c232;
         color : white;
         box-shadow: 0 0 10px #86c232;
         }
         .btn-5:hover {
         animation: pulse 1s infinite;
         transition: .3s;
         color:white;
         border : 1px solid #86c232;
         box-shadow: 0 0 10px #86c232;
         }
         .link {
         overflow: hidden;
         position: relative;
         display: inline-block;
         color: #dcdcdc;
         font-family:'Trebuchet MS';
         font-size:15px;
         letter-spacing:2px;
         }
         .link::before,
         .link::after {
         content: '';
         position: absolute;
         width: 100%;
         left: 0;
         }
         .link::before {
         height: 2px;
         bottom: 0;
         transform-origin: 100% 50%;
         transform: scaleX(0);
         transition: transform .3s cubic-bezier(0.76, 0, 0.24, 1);
         }
         .link::after {
         content: attr(data-replace);
         height: 100%;
         top: 0;
         transform-origin: 100% 50%;
         transform: translate3d(200%, 0, 0);
         transition: transform .3s cubic-bezier(0.76, 0, 0.24, 1);
         color: #86c232;
         }
         .link:hover::before {
         transform-origin: 0% 50%;
         transform: scaleX(1);
         }
         .link:hover::after {
         transform: translate3d(0, 0, 0);
         }
         .link span {
         display: inline-block;
         transition: transform .3s cubic-bezier(0.76, 0, 0.24, 1);
         }
         .link:hover span {
         transform: translate3d(-200%, 0, 0);
         }
         @keyframes pulse {
         0% {
         transform: scale(1);
         }
         70% {
         transform: scale(.9);
         }
         100% {
         transform: scale(1);
         }
         }
         /* Page Content */
         .content {padding:20px;}
         #myBtn {
         display: none;
         position: fixed;
         bottom: 20px;
         right: 30px;
         z-index: 99;
         font-size: 18px;
         border: none;
         outline: none;
         background-color: #b57edc;
         color: white;
         cursor: pointer;
         padding: 15px;
         border-radius: 4px;
         font-family:'Trebuchet MS';
         }
         #myBtn:hover {
         background-color: #86C232;
         }
         .container {
         position: absolute;
         width: 100px;
         height: 100px;
         top: 0;
         bottom: 0;
         left: 0;
         right: 0;
         margin: auto;
         }
         .item {
         width: 50px;
         height: 50px;
         position: absolute;
         }
         .item-1 {
         background-color: #86C232;
         top: 0;
         left: 0;
         z-index: 1;
         animation: item-1_move 1.8s cubic-bezier(.6,.01,.4,1) infinite;
         }
         .item-2 {
         background-color: white;
         top: 0;
         right: 0;
         animation: item-2_move 1.8s cubic-bezier(.6,.01,.4,1) infinite;
         }
         .item-3 {
         background-color: #b57edc;
         bottom: 0;
         right: 0;
         z-index: 1;
         animation: item-3_move 1.8s cubic-bezier(.6,.01,.4,1) infinite;
         }
         .item-4 {
         background-color: #6B6E70;
         bottom: 0;
         left: 0;
         animation: item-4_move 1.8s cubic-bezier(.6,.01,.4,1) infinite;
         }
         @keyframes item-1_move {
         0%, 100% {transform: translate(0, 0)}
         25% {transform: translate(0, 100px)}
         50% {transform: translate(100px, 100px)}
         75% {transform: translate(100px, 0)}
         }
         @keyframes item-2_move {
         0%, 100% {transform: translate(0, 0)}
         25% {transform: translate(-100px, 0)}
         50% {transform: translate(-100px, 100px)}
         75% {transform: translate(0, 100px)}
         }
         @keyframes item-3_move {
         0%, 100% {transform: translate(0, 0)}
         25% {transform: translate(0, -100px)}
         50% {transform: translate(-100px, -100px)}
         75% {transform: translate(-100px, 0)}
         }
         @keyframes item-4_move {
         0%, 100% {transform: translate(0, 0)}
         25% {transform: translate(100px, 0)}
         50% {transform: translate(100px, -100px)}
         75% {transform: translate(0, -100px)}
         }
         .about-border {
         display: block;
         width: 300px;
         height: 1px;
         background: #6B6E70;
         margin: 20px auto;
         }
      </style>
      <link rel = "icon" href = "images/p.png"   type = "image/x-icon">
      <title>Proofers</title>
   </head>
   <body>
      <div id="mainContent">
         <div class="header">
            <table width="100%">
               <tr>
                  <td width="33.33%" style="text-align:left">
                     <div class="icon-bar">
                        <a class="active" href="#"><i  style="font-size: 0.73em" class="fa fa-home"></i></a> 
                     </div>
                  </td>
                  <td style="text-align:center">
                     <div><span style="font-family:'Trebuchet MS';letter-spacing:-2px;font-size:50px;font-weight:100,">pr</span><span style="font-family:'Trebuchet MS';letter-spacing:-10px;font-size:50px;padding-right:6px;color:#86c232">O-O</span><span style="font-family:'Trebuchet MS';letter-spacing:-2px;font-size:50px;">fers</span>
                     </div>
                  </td>
                  <td width="33.33%" style="text-align:right">
                     <div class="icon-bar" style="float:right !important">
                        <a class="active" href="#"><i style="font-size: 0.63em" class="fa fa-bars"></i></a> 
                     </div>
                  </td>
               </tr>
            </table>
         </div>
         <div id="tagline" align="center" style="padding-top:10px;padding-bottom:60px;">
            <p><a href="#" class="link" data-replace="And your Email Proofing is done !"><span>Upload your Proofs/Files/CGENs below & Click Validate</span></a></p>
         </div>
         <form id="form1" runat="server" enctype="multipart/form-data">
            <div class="row" align="center" style="">
               <div class="column"  style="background-color:transparent;">
                  <p style="padding-bottom:10px"> <span style="font-family:Trebuchet MS;font-size:20px;letter-spacing:2px;color:#6B6E70;"> Select</span><span style="font-family:Trebuchet MS;font-size:20px;letter-spacing:2px;color:#b57edc;"> DCM </span><span style="font-family:Trebuchet MS;font-size:20px;letter-spacing:2px;color:#6B6E70;">File:</span></p>
                  <p style="padding-bottom:10px">
                     <asp:FileUpload class="btn btn-4 btn-sep"  ID="FileUpload1" runat="server" style="font-family:Trebuchet MS;font-size:15px;letter-spacing:2px;color:#6B6E70; top: 1px; left: 3px;" ToolTip="Select DCM file" ForeColor="white" accept=".txt"/>
                  </p>
                  <p style="padding-bottom:10px">
                     <asp:Label ID="Label1" runat="server" style="font-family:Trebuchet MS;font-size:15px;letter-spacing:2px;color:#dcdcdc;"></asp:Label>
                  </p>
                  <asp:Button class="btn btn-1" ID="Button1" runat="server" OnClick="uploadDCMFile" Text="Upload DCM File" style="font-family:Trebuchet MS;font-size:20px;letter-spacing:2px;top: 7px; left: 0px;" />
               </div>
               <div class="column" style="background-color:transparent;border-left:1px solid #6B6E70">
                  <p style="padding-bottom:10px">  <span style="font-family:Trebuchet MS;font-size:20px;letter-spacing:2px;color:#6B6E70;"> Select</span><span style="font-family:Trebuchet MS;font-size:20px;letter-spacing:2px;color:#b57edc;"> .MSG </span><span style="font-family:Trebuchet MS;font-size:20px;letter-spacing:2px;color:#6B6E70;">File:</span></p>
                  <p style="padding-bottom:10px">
                     <asp:FileUpload  class="btn btn-4 btn-sep" style="font-family:Trebuchet MS;font-size:15px;letter-spacing:2px;color:#6B6E70; top: 1px; left: 0px;" ID="FileUpload2" runat="server" AllowMultiple="true" ToolTip="Select .MSG file.." accept=".msg"/>
                  </p>
                  <p style="padding-bottom:10px">
                     <asp:Label ID="Label2" runat="server" style="font-family:Trebuchet MS;font-size:15px;letter-spacing:2px;color:#dcdcdc;"></asp:Label>
                  </p>
                  <asp:Button  class="btn btn-2" style="font-family:Trebuchet MS;font-size:20px;letter-spacing:2px; top: 2px; left: 0px;"  ID="Button3" runat="server" OnClick="uploadMsgFile" Text="Upload .Msg File" />
               </div>
               <div class="column" style="background-color:transparent;border-left:1px solid #6B6E70">
                  <p style="padding-bottom:10px"> <span style="font-family:Trebuchet MS;font-size:20px;letter-spacing:2px;color:#6B6E70;"> Select</span><span style="font-family:Trebuchet MS;font-size:20px;letter-spacing:2px;color:#b57edc;"> CGEN </span><span style="font-family:Trebuchet MS;font-size:20px;letter-spacing:2px;color:#6B6E70;">File:</span></p>
                  <p style="padding-bottom:10px">
                     <asp:FileUpload class="btn btn-4" style="font-family:Trebuchet MS;font-size:15px;letter-spacing:2px;color:#6B6E70; top: 1px; left: 0px;" ID="FileUpload3" runat="server" ToolTip="Select excel file.." accept=".xlsx"/>
                  </p>
                  <p style="padding-bottom:10px">
                     <asp:Label ID="Label3" runat="server" style="font-family:Trebuchet MS;font-size:15px;letter-spacing:2px;color:#dcdcdc;"></asp:Label>
                  </p>
                  <asp:Button  class="btn btn-3" style="font-family:Trebuchet MS;font-size:20px;letter-spacing:2px;top: 2px; left: 0px;"  ID="Button4" runat="server" OnClick="uploadCGENFile" Text="Upload CGEN File" />
               </div>
                <asp:Label ID="Label4" runat="server" Text="Label"></asp:Label>
            </div>
            <div id="mainButton" style="padding-top:100px;" align="center">
               <asp:Button class="btnMain btn-5"  style="font-family:Trebuchet MS;font-size:20px;letter-spacing:2px; top: 26px; left: 0px;" ID="Button2" runat="server" OnClick="validate" Text="VALIDATE" OnClientClick="return makeItLive();"  />
               <br/>
               <asp:Label ID="Label5" runat="server" Text="" style="color:white"></asp:Label>
            </div>
            <div align="center" id="analysis" style="display:block;padding-top:100px;padding-bottom:0px;">
               <span style="font-family:Trebuchet MS;font-size:25px;color:#dcdcdc;letter-spacing:3px;">ANALYSIS</span>
               <span class="about-border" ></span>
               <p style="font-family:Trebuchet MS;font-size:15px;color:#dcdcdc;letter-spacing:3px;padding-top:10px;padding-bottom:10px">
                  <span>Error Count : </span>
                  <span style="font-family:Trebuchet MS;font-size:15px;color:red;letter-spacing:3px;padding-top:10px;padding-bottom:10px">
                     <asp:Label ID="LabelErrorCount" runat="server" Text="Label"></asp:Label>
                  </span>
                  <span>; Success Count :  </span>
                  <span style="font-family:Trebuchet MS;font-size:15px;color:#86c232;letter-spacing:3px;padding-top:10px;padding-bottom:10px">
                     <asp:Label ID="labelSuccessCount" runat="server" Text="Label"></asp:Label>
                  </span>
               </p>
            </div>
            <div id="gridView" style="padding-top:25px;width: 1000px; max-width: 1000px">
               <asp:GridView ID="populateMessages" runat="server" AutoGenerateColumns="False" Font-Names="Trebuchet MS"  Font-Size="15px" ForeColor="#dcdcdc" GridLines="both" BorderColor="#6B6E70" Width="100%" align="center" OnRowDataBound="OnRowDataBound" style="max-width:100%;" >
                  <Columns>
                     <asp:BoundField HeaderText="Activity ID" DataField="activityId"/>
                     <asp:BoundField HeaderText="Element Type" DataField="elementType" ItemStyle-Font-Size="15px" />
                     <asp:BoundField HeaderText="DCM Value" DataField="DCMValue"   ItemStyle-Font-Size="10.9px" />
                     <asp:BoundField HeaderText="MSG Value" DataField="MSGValue"  ItemStyle-Font-Size="11px" />
                     <asp:BoundField HeaderText="Result" DataField="result" ItemStyle-Width="100" />
                  </Columns>
                  <FooterStyle ForeColor="White" Font-Bold="false" />
                  <HeaderStyle  BackColor="transparent" ForeColor="#b57edc"  />
               </asp:GridView>
            </div>
            <button onclick="topFunction()" id="myBtn" title="Go to top">&uarr;  Top</button>
            <div id="footer" align="center">
               <p  style="font-family:Trebuchet MS;font-size:15px;color:white;padding-top:50px;">
               </p>
               <p ><span style="font-family:Trebuchet MS;font-size:15px;color:#dcdcdc;padding-top:40px;">&#169; An </span><span style="font-family:Trebuchet MS;font-size:15px;color:#86c232;padding-top:40px;"> ACS GMO </span> <span style="font-family:Trebuchet MS;font-size:15px;color:#dcdcdc;padding-top:40px;">Initiative</span></p>
            </div>
      </div>
      <div id="makeItLive" class="container" style="display:none" >
      <div class="item item-1"></div>
      <div class="item item-2"></div>
      <div class="item item-3"></div>
      <div class="item item-4"></div>
      </div>
      </form>
      <script>
          //Get the button
          var mybutton = document.getElementById("myBtn");



          // When the user scrolls down 20px from the top of the document, show the button

          window.onscroll = function () { scrollFunction() };

          function scrollFunction() {

              if (document.body.scrollTop > 20 || document.documentElement.scrollTop > 20) {

                  mybutton.style.display = "block";

              } else {
                  mybutton.style.display = "none";
              }

          }
          // When the user clicks on the button, scroll to the top of the document

          function topFunction() {

              document.body.scrollTop = 0;

              document.documentElement.scrollTop = 0;

          }
      </script>
   </body>
</html>