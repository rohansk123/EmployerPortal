<%@ Page Title="" Language="C#" MasterPageFile="~/Test/Master.master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="Test_Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">

<link rel="STYLESHEET" type="text/css" href="<%=Page.ResolveUrl("~")%>Styles/datepicker.css" />
<link rel="STYLESHEET" type="text/css" href="<%=Page.ResolveUrl("~")%>Styles/select2.css"  />
<link rel="STYLESHEET" type="text/css" href="<%=Page.ResolveUrl("~")%>Styles/jquery-ui.css"/>
<asp:Literal ID="javascript" runat="server" />
<%--<script type='text/javascript' src='https://ajax.googleapis.com/ajax/libs/jquery/1.7.2/jquery.min.js'> </script>--%>
    <script type="text/javascript" src="<%=Page.ResolveUrl("~")%>Scripts/codebase/dhtmlxcommon.js"></script>
    <script type="text/javascript" src="<%=Page.ResolveUrl("~")%>Scripts/codebase/dhtmlxpopup.js"></script>
    <script type="text/javascript" src="<%=Page.ResolveUrl("~")%>Scripts/jquery.validate.js"></script>

    <script type="text/javascript">

        function displayError(element, event, errorMsg) {
            if (!element.closest("td").hasClass("alert alert-danger")) {
                element.closest("td").addClass("alert alert-danger");
                element.closest("td").prepend("<span class='errorMsgs'> <strong>" + errorMsg + "</strong> <br /> </span>");
                element.focus();
            }
            event.preventDefault();
        }

        function removeError(element, event) {
            if (element.closest("td").hasClass("alert alert-danger")) {
                element.closest("td").removeClass("alert alert-danger");
                element.closest("td").find(".errorMsgs").remove();
            }
        }
        function validateEmail(email) {
            var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            return re.test(email);
        }

        //ADD DATEPICKER FUNCTIONALITY
        $(function () {
            $("[id$=effective_date]").datepicker({
                format: 'mm/dd/yyyy',
                startDate: '-1d',
                showAnim: "fold"
            });
            $("[id$=dob]").datepicker({
                format: 'mm/dd/yyyy'
            });
            $("[id$=doh]").datepicker({
                format: 'mm/dd/yyyy'
            });
        });

        $(window).load(function () {

            $('#noEmail').change(function () {
                if ($(this).is(':checked')) {
                    $('#email').val('n/a');
                    $('#email').attr('disabled', 'true');
                    $('#email').removeClass('emailOnly');
                }
                else {
                    $('#email').val('');
                    $('#email').removeAttr('disabled');
                    $('#email').addClass('emailOnly');
                }
            });


            $('#BtnAdd').click(function (event) {
                // alert("Test");

                var preventSubmit = false;

                //                event.preventDefault();
                $('.addField[validate!="false"]').each(function () {
                    if (!$(this).val()) {

                        displayError($(this), event, "Required: ");
                    }
                    else removeError($(this), event);

                });

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

                        if (newDate.length < 10) {
                            prevent = true;
                        }
                    }

                    if (prevent) {
                        if (!$(this).closest("td").hasClass("alert alert-danger")) {
                            $(this).closest("td").addClass("alert alert-danger");
                            $(this).closest("td").prepend("<span class='errorMsgs'> <strong>Please enter a valid date:</strong> <br /> </span>");
                        }
                        preventSubmit = true;
                    }
                });

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
                        if (!$(this).closest("td").hasClass("alert alert-danger")) {
                            $(this).closest("td").addClass("alert alert-danger");
                            $(this).closest("td").prepend("<span class='errorMsgs'> <strong>Please enter a valid email:</strong> <br /> </span>");
                        }
                        preventSubmit = true;
                    }
                });


                if (preventSubmit)
                    return false;
            });

        });
    </script>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
	<div class="row">
            <div class="col-xs-12">
				    <div class="widget-box">
					    <div class="widget-title">
						    <span class="icon">
							    <i class="fa fa-plus"></i>
						    </span>
						    <h5>Add Employee</h5>
                        </div>
                        
					    <div class="widget-content nopadding">
		                    <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="fname"> First Name:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox CssClass="addField" name="fname" ID="fname" MaxLength="35" runat="server" /></div>
                            </div>
                            
		                    <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="lname"> Last Name:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox CssClass="addField" name="lname" ID="lname" MaxLength="35" runat="server" /></div>
                            </div>
                            
		                    <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="sex"> Gender:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10"><asp:DropDownList CssClass="addField"  ID="sex" runat="server">
                                    <asp:ListItem value="">Select..</asp:ListItem>
                                    <asp:ListItem value="Male">Male</asp:ListItem>
                                    <asp:ListItem value="Female">Female</asp:ListItem></asp:DropDownList></div>
                            </div>
                            
		                    <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="address"> Address:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox CssClass="addField" ID="address" MaxLength="50" runat="server" /></div>
                            </div>

                            <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="city"> City:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox CssClass="addField" ID="city" MaxLength="35" runat="server" /></div>
                            </div>

                            <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="state"> State:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10"><asp:DropDownList  CssClass="addField"  ID="state" runat="server">
                                    <asp:ListItem Value="">Select... </asp:ListItem>
                                    <asp:ListItem Value="AK">AK</asp:ListItem>
                                    <asp:ListItem Value="AL">AL</asp:ListItem>
                                    <asp:ListItem Value="AR">AR</asp:ListItem>
                                    <asp:ListItem Value="AZ">AZ</asp:ListItem>
                                    <asp:ListItem Value="CA">CA</asp:ListItem>
                                    <asp:ListItem Value="CO">CO</asp:ListItem>
                                    <asp:ListItem Value="CT">CT</asp:ListItem>
                                    <asp:ListItem Value="DC">DC</asp:ListItem>
                                    <asp:ListItem Value="DE">DE</asp:ListItem>
                                    <asp:ListItem Value="FL">FL</asp:ListItem>
                                    <asp:ListItem Value="GA">GA</asp:ListItem>
                                    <asp:ListItem Value="HI">HI</asp:ListItem>
                                    <asp:ListItem Value="IA">IA</asp:ListItem>
                                    <asp:ListItem Value="ID">ID</asp:ListItem>
                                    <asp:ListItem Value="IL">IL</asp:ListItem>
                                    <asp:ListItem Value="IN">IN</asp:ListItem>
                                    <asp:ListItem Value="KS">KS</asp:ListItem>
                                    <asp:ListItem Value="KY">KY</asp:ListItem>
                                    <asp:ListItem Value="LA">LA</asp:ListItem>
                                    <asp:ListItem Value="MA">MA</asp:ListItem>
                                    <asp:ListItem Value="MD">MD</asp:ListItem>
                                    <asp:ListItem Value="ME">ME</asp:ListItem>
                                    <asp:ListItem Value="MI">MI</asp:ListItem>
                                    <asp:ListItem Value="MN">MN</asp:ListItem>
                                    <asp:ListItem Value="MO">MO</asp:ListItem>
                                    <asp:ListItem Value="MS">MS</asp:ListItem>
                                    <asp:ListItem Value="MT">MT</asp:ListItem>
                                    <asp:ListItem Value="NC">NC</asp:ListItem>
                                    <asp:ListItem Value="ND">ND</asp:ListItem>
                                    <asp:ListItem Value="NE">NE</asp:ListItem>
                                    <asp:ListItem Value="NV">NV</asp:ListItem>
                                    <asp:ListItem Value="NH">NH</asp:ListItem>
                                    <asp:ListItem Value="NJ">NJ</asp:ListItem>
                                    <asp:ListItem Value="NM">NM</asp:ListItem>
                                    <asp:ListItem Value="NY">NY</asp:ListItem>
                                    <asp:ListItem Value="OH">OH</asp:ListItem>
                                    <asp:ListItem Value="OK">OK</asp:ListItem>
                                    <asp:ListItem Value="OR">OR</asp:ListItem>
                                    <asp:ListItem Value="PA">PA</asp:ListItem>
                                    <asp:ListItem Value="RI">RI</asp:ListItem>
                                    <asp:ListItem Value="SC">SC</asp:ListItem>
                                    <asp:ListItem Value="SD">SD</asp:ListItem>
                                    <asp:ListItem Value="TN">TN</asp:ListItem>
                                    <asp:ListItem Value="TX">TX</asp:ListItem>
                                    <asp:ListItem Value="UT">UT</asp:ListItem>
                                    <asp:ListItem Value="VA">VA</asp:ListItem>
                                    <asp:ListItem Value="VT">VT</asp:ListItem>
                                    <asp:ListItem Value="WA">WA</asp:ListItem>
                                    <asp:ListItem Value="WI">WI</asp:ListItem>
                                    <asp:ListItem Value="WV">WV</asp:ListItem>
                                    <asp:ListItem Value="WY">WY</asp:ListItem>
                                </asp:DropDownList></div>
                            </div>

                            <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="zip"> Zip:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox CssClass="addField numbersOnly" id="zip" maxlength="10" runat="server" /></div>
                            </div>

                            <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="homep"> Home Phone:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox CssClass="addField numbersOnly" id="homep" maxlength="14" runat="server" /></div>
                            </div>
                            
                            <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="homep"> Work Phone:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox CssClass="addField numbersOnly" id="workp" maxlength="14" runat="server" validate="false" /></div>
                            </div>
                            
                            <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="email"> Email:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox CssClass="addField emailOnly" id="email" maxlength="50" runat="server" clientIDmode="Static" validate="false" />
                                    <br /> <label><input type="checkbox" id="noEmail" value="No" /> This person has no email. </label></div>
                            </div>

                            <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="ssn"> Social Security:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox CssClass="addField numbersOnly" id="ssn" maxlength="11" runat="server" /></div>
                            </div>

                            <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="dob"> Date of Birth:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox CssClass="addField datesOnly" id="dob" MaxLength="10" type="text" data-date-format="dd/mm/yyyy"  class="datepicker form-control input-sm" runat="server" />
                                    &nbsp;<font size="-2" color="#696969">Format:&nbsp; 'mm/dd/yyyy'</font></div>
                            </div>

                            <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="doh"> Date of Hire:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox CssClass="addField datesOnly" id="doh" MaxLength="10" runat="server" />
                                    &nbsp;<font size="-2" color="#696969"></font></div>
                            </div>

                            <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="occupation"> Occupation:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox CssClass="addField" id="occupation" maxlength="25" runat="server" /></div>
                            </div>

                            <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="salary"> Annual Salary:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox CssClass="addField numbersOnly" id="salary" maxlength="14" runat="server" />
                                    &nbsp;<font size="-2" color="#696969">Format:&nbsp; '0000.00' (without $)</font></div>
                            </div>

                            <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="payGroup"> Pay Group:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:DropDownList  CssClass="addField"  ID="payGroup" runat="server">
                                        <asp:ListItem Value="">Select... </asp:ListItem>
                                        <asp:ListItem Value="M">Monthly</asp:ListItem>
                                        <asp:ListItem Value="SM">Semi-Monthly</asp:ListItem>
                                        <asp:ListItem Value="BW">Bi-weekly</asp:ListItem>
                                        <asp:ListItem value="W">Weekly</asp:ListItem>
                                        <asp:ListItem value="9">9-thly</asp:ListItem>
                                        <asp:ListItem value="10">10-thly</asp:ListItem>
                                    </asp:DropDownList></div>
                            </div>

                            <div class="form-group" style="vertical-align:bottom" ID="jcDiv" runat="server">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="jobclass"> Job Class:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:DropDownList  CssClass="addField"  ID="jobclass" runat="server">
                                        <asp:ListItem Value="">Select... </asp:ListItem>
                                    </asp:DropDownList></div>
                            </div>

                            <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="ssn"> Effective Date:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox CssClass="addField datesOnly" id="effective_date" maxlength="10" runat="server" type="text" data-date="12-02-2012" data-date-format="dd-mm-yyyy" value="12-02-2012" class="datepicker form-control input-sm"/>
                                    &nbsp;<font size="-2" color="#696969">Format:&nbsp; 'mm/dd/yyyy'</font></div>
                            </div>
                        

                            <!--<div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="ssn"> Fidelity Effective Date:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:TextBox CssClass="addField" id="fidelity_effective_date" maxlength="14" runat="server" />
                                    &nbsp;<font size="-2" color="#696969">Format:&nbsp; 'mm/dd/yyyy'</font></div>
                            </div> -->

                            <div class="form-group" style="vertical-align:bottom">
                                <label class="col-sm-3 col-md-3 col-lg-2 control-label" for="ssn"> Location:</label>
                                <div class="col-sm-9 col-md-9 col-lg-10">
                                    <asp:DropDownList CssClass="addField"  id="location" runat="server">
                                        <asp:ListItem Selected="True" Value=""> Please Select </asp:ListItem>
                                    </asp:DropDownList> 
                                    <div style="display:inline">
                                        <a class="add_location" href="#addLocation" data-toggle="modal"> Add New Location</a>
                                    </div>

                                    <div id="addLocation" class="modal fade" aria-hidden="true" style="display: none;">
                                    <div class="modal-dialog">
											<div class="modal-content">
								                <div class="modal-header">
									                <button data-dismiss="modal" class="close" type="button">×</button>
									                <h3>Add New Location</h3>
								                </div>
								                <div class="modal-body">
									                <div style="display:table-row">
                                                        <div style="display:table-cell; width: 200px">Location Name: </div>
                                                        <div style="display:table-cell"><asp:TextBox ID="newLocationName" MaxLength="50" runat="server" /> </div>
					                                </div>

		                                            <div style="display:table-row; display: none">
                                                        <div style="display:table-cell; width: 200px">Location: </div>
                                                        <div style="display:table-cell"><asp:DropDownList ID="DropDownList1" runat="server">
                                                                <asp:ListItem value="">Select..</asp:ListItem></asp:DropDownList>
                                                         </div>
					                                </div>
								                </div>
								                <div class="modal-footer">
                                                    <div id="errorDiv" class="alert alert-danger" style="display: none" runat="server">
					                                    <button class="close" data-dismiss="alert">×</button>
					                                    <strong>Error!</strong> <asp:Label ID="locationErrorMsg" runat="server" />
				                                    </div>
					                                    <div style="display:table-cell">
                                                        <asp:Button ID="addBtn" runat="server" Text="Add Location" onclick="AddLocation" CssClass="btn btn-primary" /></div>
                                                </div>
											</div>
										</div>
							        </div>
                                </div>
                            </div>

                            <asp:PlaceHolder ID="addtnlCtrls" runat="server" />

                            <div ID="errorMsg" runat="server"></div>
                            
                            <div class="form-actions"> 
                                <asp:LinkButton ID="BtnAdd" onclick='AddEmployee' runat="server" CssClass="btn btn-primary" ClientIDMode="Static">
                                    <i class="fa fa-plus fa-white"></i> Add Employee
                                </asp:LinkButton>
                            </div>
                    </div>
				</div>
		</div>            
    </div>
   
</asp:Content>

