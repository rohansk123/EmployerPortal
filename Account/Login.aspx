<%@ Page Title="Log In" Language="C#" AutoEventWireup="true" CodeFile="Login.aspx.cs" Inherits="Account_Login" %>

<!DOCTYPE html>
<html lang="en">
    <head>
        <title>Welcome</title>
        <meta name="viewport" content="width=device-width, initial-scale=1">
		
        <link href="../Styles/bootstrap.min.css" rel="stylesheet" type="text/css" />
		<link rel="stylesheet" href="<%=Page.ResolveUrl("~")%>Styles/bootstrap-responsive.min.css" />
        <link rel="stylesheet" href="<%=Page.ResolveUrl("~")%>Styles/unicorn.login.css" />
        <%--<script type='text/javascript' src="<%=Page.ResolveUrl("~")%>Scripts/jquery.min.js"></script>--%>
        
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
            .header-text h4
            {
                font-weight:bold !important; 
            }
            
            
                 .form-control   
                 {
                     width:25%;   
                     display: inline-block;
                                            
                 }
                 #LoginBtn
                 {
                     width:18%;
                     margin-left:25%;
                 }
                 #FTBtn
                 {
                    margin-left:20%;   
                 }
                 .alert 
                 {
                    width:70%;
                 }
                 .container
                 {
                     padding-left:19%;
                 }
                 
                        </style>
  <%--  <script type="text/javascript">
        $(window).load(function () {
            $("input[type=text]").on("keypress change", (function (e) {
                var charCode = (e.which) ? e.which : e.keyCode;
                if (charCode == 44 || charCode == 39) {
                    return false;
                }
            }));
            $("input.numbersOnly").on("keypress change", (function (e) {
                var charCode = (e.which) ? e.which : e.keyCode;
                var decimalCount = (this.value.match(/\./g)) ? this.value.match(/\./g).length : 0;
                //alert(decimalCount);
                if (charCode > 31 && charCode != 46
                && (charCode < 48 || charCode > 57) || (charCode == 46 && decimalCount > 0)) {
                    return false;
                }
            }));
            $("input.datesOnly").on("keypress change", (function (e) {
                var charCode = (e.which) ? e.which : e.keyCode;
                var decimalCount = (this.value.match(/\./g)) ? this.value.match(/\./g).length : 0;
                //alert(decimalCount);
                if (charCode > 31 && charCode != 46
                && (charCode < 48 || charCode > 57) || (charCode == 46 && decimalCount > 0)) {
                    return false;
                }

                var dateValue = $(this).val();
                var length = dateValue.length;

                if (length <= 3) {
                    var newValue = dateValue.replace('/', '');
                    if (parseInt(newValue) > 12)
                        return false;
                }

                if ((dateValue.match(/\//g) || []).length == 2) {
                    bigMonths = [1, 3, 5, 7, 8, 10, 12];
                    smMonths = [4, 6, 9, 11];
                    var dateSplit = dateValue.split("/");
                    var mm = parseInt(dateSplit[1]);
                    var dd = parseInt(dateSplit[0]);
                    if ((mm > 31 && bigMonths.has(dd)) || (mm > 30 && smMonths.has(dd)) || (mm > 29 && dd == 2))
                        return false;
                }

            }));
        });

    </script>--%>
    </head>
    <body>
       
      
            <form id="loginform" class="form-signin" runat="server" method="post">
              <div id="Div1" class="container">
          <%--  <div class="header-text">
            <hr />
				<h4 class="form-signin-heading">Welcome to the New Hire Portal. <br />
                To access this site, please enter the requested information. 
                </h4>
                <hr />
                </div>--%>
                <div class="row">
                <br /><br />
                <label for="username" class="col-sm-3">Email</label>
                
                <asp:TextBox CssClass="col-sm-3 form-control" ID="username" placeholder="example@domain.com" runat="server"/>
               </div>
               <br />
               <div class="row">
                <label for="password"  class="col-sm-3">PIN</label>
                 <asp:TextBox CssClass="col-sm-3 form-control" TextMode="password" placeholder="MMDDYYYY" ID="password" runat="server"/>
                       
                <%--<div class="control-group">
                    <div class="controls">
                        <div class="input-prepend">
                            <span class="add-on"><i class="icon-briefcase"></i></span><asp:DropDownList ID="agency" runat="server"/>
                        </div>
                    </div>
                </div>--%>
                </div>
                <br />
                <asp:Button ID="LoginBtn" CssClass="btn btn-lg btn-primary btn-block" runat="server" Text="Login" onclick="Login_Click" />
                <br />
                
                     
                <asp:Literal id="ErrorMsg" runat="server" />
                <%--<div  class="form-actions">
                 <asp:Button ID="FTBtn" CssClass="btn-link" runat="server" Text="First Time User?" onclick="FirstTime_Click" />
         
                                            <span style="position:absolute; bottom:5px; left:5px; text-align:right" id="siteseal"><script type="text/javascript" src="https://seal.godaddy.com/getSeal?sealID=6MJUhlzJWyDhPIrmTH0ECgABoQfLKPX5pfMllejPboGWSiHhHZ0GZ"></script></span>
                </div>--%>
             <%--   <div class="instructions">
                <p>
                If this is your first visit to the either the self enrollment or this site, your <strong> UserID</strong> is the <strong>first initial of your first and last names and the last 4 digits of your SSN.</strong>
               For example, if your name is John Smith and the last 4 digits of your SSN is 1453,enter <strong> JS1453</strong>
               </p>
                <br />
               <p>
               If this is your first visit to this site, please enter your <strong>date of birth as your PIN.</strong>
               For example, if your date of birth is March 16, 1960, enter <strong>03161960</strong>. Your 
               date of birth will be your PIN in the future, unless you change your PIN on the following page

               </p>
               <br />
               <p>
               If you have already visited this site before, please enter your <strong> Email</strong> and 
               <strong> PIN</strong> 
               </p>
                </div>--%>
             </div>
            </form>
       
    </body>
</html>
