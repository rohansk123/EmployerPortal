<%@ Page Title="Grid" Language="C#" MasterPageFile="~/Default.master" AutoEventWireup="true" CodeFile="Grid.aspx.cs" Inherits="Grid" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
<div class="row">
        <div class="col-xs-12 nopadding">
				<div class="widget-box" style="margin:0px">
				
					 <asp:Literal ID="MyGrid" runat="server"/>
                     <br />
                    <%--    <asp:LinkButton ID="goToTerminate" PostBackUrl="~/TerminateEmployee.aspx" runat="server">Terminate?</asp:LinkButton>
                  --%>   </div>
                     </div>
                     </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="script" Runat="Server">
  <asp:Literal ID="javascript" runat="server" />
 
 

</asp:Content>

