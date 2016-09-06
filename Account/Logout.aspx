<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Logout.aspx.cs" Inherits="Account_Logout" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
      <meta name="viewport" content="width=device-width, initial-scale=1"/>
		
        <link href="../Styles/bootstrap.min.css" rel="stylesheet" type="text/css" />
		<link rel="stylesheet" href="<%=Page.ResolveUrl("~")%>Styles/bootstrap-responsive.min.css" />
        <link rel="stylesheet" href="<%=Page.ResolveUrl("~")%>Styles/unicorn.login.css" />
        <style type="text/css">
              .header-text 
            {
                width:70%;
                text-align:center;
                margin-bottom:5%;
                 
            }
            .header-text h4
            {
                font-weight:bold !important; 
            }
             .container
                 {
                     padding-left:19%;
                 }
        </style>
</head>
<body>
    <form id="form1" runat="server">
   
  <div id="Div1" class="container">
            <div class="header-text">
            <hr />
				<h4 class="bg-info"> 
            Thank you for using our system!
                </h4>
                <asp:HyperLink ID="loginLink" class="btn btn-info" runat="server"> LOGIN AGAIN</asp:HyperLink>
                <hr />
                </div>
                </div>
    </form>
</body>
</html>
