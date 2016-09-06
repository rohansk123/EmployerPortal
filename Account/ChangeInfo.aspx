<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ChangeInfo.aspx.cs" Inherits="Account_ChangeInfo" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Change Information</title>
     <meta name="viewport" content="width=device-width, initial-scale=1">
		<link rel="stylesheet" href="<%=Page.ResolveUrl("~")%>Styles/bootstrap.min.css" />
		<link rel="stylesheet" href="<%=Page.ResolveUrl("~")%>Styles/bootstrap-responsive.min.css" />
             <style type="text/css">
            .header-text 
            {
                width:70%;
                text-align:center;
                margin-bottom:5%;
                
            }
            .instructions
            {
                width: 70%;
            }
            .header-text h5
            {
                font-weight:bold !important; 
            }
            
            
                 .form-control   
                 {
                    /* width:25%; */  
                     display: inline-block;
                                            
                 }
                 #SaveBtn
                 {
                     width:18%;
                     margin-left:22%;
                 }
                 #FTBtn
                 {
                    margin-left:20%;   
                 }
                 .alert 
                 {
                    width:55%;
                 }
                 .container
                 {
                     padding-left:19%;
                 }
                 
                        </style>
    <script src="../Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
                         <script type="text/javascript">
                           $(window).load(function () {

                               $('#SaveBtn').click(function (event) {
                                   // alert("Test");

                                   var preventSubmit = false;
                               $(".emailOnly").each(function () {
                    var prevent = false;

                    if ($(this).val().length < 7)
                        prevent = true;
                    else {
                        var email = $(this).val().toLowerCase();

                        if (email.indexOf("@") <= 0)
                            prevent = true;

                        //alert(email.substring(email.length - 4, email.length));
                        if (email.substring(email.length - 4, email.length).indexOf(".") < 0)
                            prevent = true;
                    }

                    if (prevent) {
                        if (!$(this).closest("div").hasClass("alert alert-danger")) {
                            $(this).closest("div").addClass("alert alert-danger");
                            $(this).closest("div").prepend("<span class='errorMsgs'> <strong>Please enter a valid email:</strong> <br /> </span>");
                        }
                        preventSubmit = true;
                    }
                });


                if (preventSubmit)
                    return false;
            });

                           });
                         </script>
</head>
<body>
    <form id="changeInfo" class="form-horizontal" runat="server">
           <div class="container" runat="server">
            <div class="header-text">
            <hr />
				<h5 class="form-signin-heading">Welcome to the Employee Benefit Site. <br />
                On this page, you can change your email or PIN if you want to. Please note that this will change your Email and PIN on the self enroll site as well.
                </h5>
                <hr />
                </div>
               
                <div class="row">
               
                <label for="username" class="col-xs-2">Email</label>
                <div class="col-xs-4">
                <asp:TextBox CssClass="col-xs-2 form-control emailOnly" ID="email" placeholder="" runat="server"/>
                </div>
               </div>
               <br />
               <div class="row">
                <label for="password"  class="col-xs-2">Update PIN</label>
                 <div class="col-xs-4">
                 <asp:TextBox CssClass="form-control" TextMode="password" placeholder="" ID="password" runat="server"/>
                    </div>   
           </div>
                
                <br />
                <asp:Button ID="SaveBtn" CssClass="btn btn-lg btn-success btn-block" runat="server" Text="Proceed" onclick="Save_Click" />
                <br />
                
                     
                <asp:Literal id="ErrorMsg" runat="server" />
               
             <div class="instructions alert alert-info">
                <p>
               To update your PIN, enter a new PIN. Otherwise, leave blank. 
               <br /><br /><strong>
               Next time you login make sure you use the above email and PIN. 
               </strong>
               </p>
                </div>
             </div>
    </form>
</body>
</html>
