<%@ Page Title="Termination" Language="C#" MasterPageFile="~/Default.master" AutoEventWireup="true" CodeFile="TerminateEmployee.aspx.cs" Inherits="TerminateEmployee"  Debug="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link rel="STYLESHEET" type="text/css" href="<%=Page.ResolveUrl("~")%>Styles/dhtmlxcombo.css" />
    <link rel="STYLESHEET" type="text/css" href="/NewHire/Styles/datepicker.css" />
<link rel="STYLESHEET" type="text/css" href="/NewHire/Styles/select2.css"  />
<link rel="STYLESHEET" type="text/css" href="/NewHire/Styles/jquery-ui.css"/>

  <style type="text/css">
#BtnTerminate
{
    margin-left:10%;
}
#ChangeProductsBtn
{
    margin-left:2%;
}
</style>
    <script type="text/javascript" src="<%=Page.ResolveUrl("~")%>Scripts/codebase/dhtmlxcommon.js"></script>
    <script type="text/javascript" src="<%=Page.ResolveUrl("~")%>Scripts/codebase/dhtmlxcombo.js"></script>
    <script type="text/javascript" src="<%=Page.ResolveUrl("~")%>Scripts/codebase/ext/dhtmlxcombo_whp.js"></script>
  
    <script type="text/javascript">
     window.dhx_globalImgPath = "<%=Page.ResolveUrl("~")%>Scripts/codebase/imgs/";
   
     
    
    </script>
    
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
    <div class="row">
  
     	<div class="widget-box">
					<%--<div class="widget-title">
						<span class="icon">
							<i class="fa fa-search"></i>
						</span>
						<h5>Enter first few letters of employee's last name </h5>
					</div>--%>
					<div class="widget-content">
      
                         <div class="form-fields" >
                           <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="fname"> Name:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10" style="margin-top:0.5%">
                                    <asp:Label ID="name" runat="server" Text="Label"></asp:Label></div>
                            </div>
                           <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="date">Termination Date:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox CssClass="addField datesOnly" name="dateOfTermination" ID="dateOfTermination" MaxLength="10" runat="server" /></div>
                            </div>                           
		                    <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="date">Coverage End Date:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox CssClass="addField datesOnly" name="coverageEndDate" ID="coverageEndDate" MaxLength="10" runat="server" /></div>
                            </div>     
                            
		                    <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="reason"> Reason:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10"><asp:DropDownList CssClass="addField"  ID="terminationReason" runat="server">
                                    <asp:ListItem value="">Select..</asp:ListItem>
                                    <asp:ListItem value="ActiveMilitary">Active Military Duty</asp:ListItem>
                                      <asp:ListItem value="Death">Employee Death</asp:ListItem>
                                        <asp:ListItem value="Medicare">Entitlement to Medicare</asp:ListItem>
                                         <asp:ListItem value="Reduction">Reduction in Hours</asp:ListItem>
                                            <asp:ListItem value="Resignation">Resignation</asp:ListItem>
                                               <asp:ListItem value="Termination">Termination</asp:ListItem>
                                    <asp:ListItem value="Retirement">Retirement</asp:ListItem></asp:DropDownList></div>
                            </div>
                                 <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="fname"> Comments:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox CssClass="addField" name="fname" ID="terminationComments" TextMode="multiline" Columns="50" Rows="5" MaxLength="35" runat="server" /></div>
                            </div>      

                                    </div>
		              <div ID="errorMsg" runat="server"></div>
                        <div class="form-actions" >
                            <input type="hidden" name="EID" value="" />
					        <asp:LinkButton ID="BtnTerminate" runat="server"  onclick="Terminate_Click" CssClass="btn btn-danger" ClientIDMode="Static">
                             <i class="fa fa-times fa-white"></i> Terminate</asp:LinkButton>
                           <%--     <asp:LinkButton ID="ChangeProductsBtn" runat="server"   CssClass="btn btn-warning" ClientIDMode="Static">
                             <i class="fa fa-edit fa-white"></i> Change Products</asp:LinkButton>--%>
                        </div>
					</div>
				</div>
  
 </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="script" Runat="Server">
    <script type="text/javascript">
     function displayError(element, event, errorMsg) {
            if (!element.closest("div").hasClass("alert alert-danger")) {
                element.closest("div").addClass("alert alert-danger");
                element.closest("div").prepend("<span class='errorMsgs'> <strong>" + errorMsg + "</strong> <br /> </span>");
                element.focus();
            }
            event.preventDefault();
        }

        function removeError(element, event) {
            if (element.closest("div").hasClass("alert alert-danger")) {
                element.closest("div").removeClass("alert alert-danger");
                element.closest("div").find(".errorMsgs").remove();
            }
        }
        $(window).load(function () {
            $(".dhx_combo_input").css("border", "0px");
            // $(".dhx_combo_input").css("margin-top","50px");
            //$('div#ssn_box').remove('input[name="EID"]');
            $('input[name="nameEID"]').val('');



            $("#BtnTerminate").click(function () {
                var preventSubmit = false;

                if ($('select').val() == "") {
                    displayError($('select'), event, "Please select an option: ");
                      preventSubmit = true;
                }
                else{
                    removeError($('select'), event);
                }
                    



                $(".datesOnly").each(function () {

                    var prevent = false;
                    if ($(this).val().length < 10) {

                        var dateSplit = $(this).val().split("/");
                        //alert(dateSplit.length);

                        if (dateSplit.length < 3)
                            prevent = true;
                        else if (dateSplit[2].length < 4)
                            prevent = true;
                        else {
                            if (dateSplit[0].length == 1)
                                dateSplit[0] = "0" + dateSplit[0];

                            if (dateSplit[1].length == 1)
                                dateSplit[1] = "0" + dateSplit[1];
                        }



                        var newDate = dateSplit.join('/');
                        //alert(newDate);
                        $(this).val(newDate);
                        //alert($(this).val().length);

                        if (newDate.length < 10)
                            prevent = true;


                    }

                    if (!prevent) {
                        var comp = $(this).val().split("/");
                        var m = parseInt(comp[0], 10);
                        var d = parseInt(comp[1], 10);
                        var y = parseInt(comp[2], 10);
                        var date = new Date(y, m - 1, d);
                        if (date.getFullYear() == y && date.getMonth() + 1 == m && date.getDate() == d) {
                            // alert('Valid date');
                        } else {
                            //  alert('Invalid date');
                            prevent = true;
                        }
                    }


                    if (prevent) {
                        if (!$(this).closest("div").hasClass("alert alert-danger")) {
                            $(this).closest("div").addClass("alert alert-danger");
                            $(this).closest("div").prepend("<span class='errorMsgs'> <strong>Please enter a valid date:</strong> <br /> </span>");
                        }
                        preventSubmit = true;
                        //return false;
                    }
                });
                if (preventSubmit)
                    return false;
            });
        });
     </script>
</asp:Content>

