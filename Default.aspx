<%@ Page Title="Home" Language="C#" MasterPageFile="~/Default.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
   <link rel="STYLESHEET" type="text/css" href="<%=Page.ResolveUrl("~")%>Styles/dhtmlxcombo.css" />
    <script type="text/javascript" src="<%=Page.ResolveUrl("~")%>Scripts/codebase/dhtmlxcommon.js"></script>
    <script type="text/javascript" src="<%=Page.ResolveUrl("~")%>Scripts/codebase/dhtmlxcombo.js"></script>
    <script type="text/javascript" src="<%=Page.ResolveUrl("~")%>Scripts/codebase/ext/dhtmlxcombo_whp.js"></script>
  
    <script type="text/javascript">
     window.dhx_globalImgPath = "<%=Page.ResolveUrl("~")%>Scripts/codebase/imgs/";
     </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
 <input type="hidden" name="EID" value="" />
<div class="row">
        <div class="col-xs-12 nopadding">
				<div class="widget-box" style="margin:0px">
				
					<div class="widget-content nopadding">
                   <%--  <div class="jumbotron">
    <h1>Welcome to the New Hire Portal</h1>
    <p>Here you can search, add or terminate employees</p> 
  </div>--%>
  <h3> &nbsp;Find employee:</h3>
                        &nbsp; In the list below, select the employee or enter first few letters of employee's last name to search.
		                <div class="form-group" style="vertical-align:bottom">
                            <label class="col-xs-3 col-md-3 col-lg-2 control-label">Employee Name: </label>
                            <div class="col-xs-9 col-md-9 col-lg-10">
                                <div  id="name_box" style=" display:inline-table; margin-top: 8px">
                                </div>

                                <script type="text/javascript">
                                    dhtmlx.skin = 'dhx_terrace';
                                    var z = new dhtmlXCombo("name_box", "nameEID", "30%");
                                    z.loadXML("Scripts/XMLGenerators/GenerateEmployeeXMLByGroup.aspx");
                                    z.attachEvent("onKeyPressed", function () {
                                        z.enableFilteringMode('between', "Scripts/XMLGenerators/GenerateEmployeeXMLByGroup.aspx", true);
                                    });
                                    z.attachEvent("onChange", function () {
                                        var eid = document.getElementsByName("nameEID")[0].value;
                                        if (!eid == '' && !isNaN(eid)) {
                                            $('input[name="EID"]').val(eid);
                                             $('form#NewHireForm').submit();
                                        }
                                        //window.location = "<%=Page.ResolveUrl("~")%>Products.aspx?eid=" + eid;
                                    });
                                    z.setOptionHeight(200);
                                </script>

                            </div>
					    </div>
                         <br />
                       <%--  <div  class="col-xs-8 col-md-8 col-lg-8">
                        <h2 style="text-align:center">OR</h2>
                         <br />
                     
                        </div>--%>
                                          
                          <asp:LinkButton ID="BtnAdd" runat="server"  onclick="Add_Click" style="margin-left:2%" CssClass="btn btn-primary" PostBackUrl="~/AddEmployee.aspx" ClientIDMode="Static">
                             <i class="fa fa-plus fa-white"></i> Add Employee</asp:LinkButton>

                             <asp:LinkButton ID="LinkButton1" runat="server"   style="margin-left:2%" CssClass="btn btn-danger" PostBackUrl="~/TerminateEmployeeFile.aspx" ClientIDMode="Static">
                             <i class="fa fa-upload fa-white"></i> Upload Terminate Employee File</asp:LinkButton>

                         <asp:Literal ID="errorMsg" runat="server" />
                      <br /><br />
                       </div>
                      </div>
                     </div>
                    </div>
                   <%-- </div>--%>
                        


</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="script" Runat="Server">
</asp:Content>

