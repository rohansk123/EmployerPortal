<%@ Page Title="Upload file" Language="C#" MasterPageFile="~/Default.master" AutoEventWireup="true" CodeFile="UploadEmployeeFile.aspx.cs" Inherits="UploadEmployeeFile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="body" Runat="Server">
 <%--<form class="form-horizontal" id="CensusForm" runat="server">--%>
 <div class="row">
        <div class="col-xs-12">
				<div class="widget-box">
					<%--<div class="widget-title">
						<span class="icon">
							<i class="fa fa-upload"></i>
						</span>
						<h5><asp:Label id="groupName" Text="" runat="server" /></h5>
					</div>--%>
					<div class="widget-content ">
                       Note that you can only upload CSV files. Your file can have the following column headers and corresponding data (<span style="color:Red">color</span> indicates required): <br /><br />
                       <strong>| <span style="color:Red">'lname' or 'last name'</span> ||	<span style="color:Red">'fname' or 'first name' </span> || 'address' or 'addr' || 'city' || 'state' || 'zip' || <span style="color:Red">'ssn' </span> || <span style="color:Red">'dob' or 'birthdate' </span> || <span style="color:Red">'doh' or 'hiredate' </span>|| 'gender' or 'sex' || 'salary' || 'title' or 'occupation' || 'location' or 'loc_name' 
                       || 'class' or 'jobclass' || 'ext_EmpID' or 'empee_id' |
                       </strong><br />  <br />
                       <a href="/NewHire/Documents/templates/upload_example.csv">Example file</a>
                     <br /><br />
                      
                        <div class="form-group">
							<label class="col-lg-3 control-label">Upload Employee census: </label>
							<div class="col-lg-2"><input type="file" id="censusFile" name="censusFile" runat="server" /> </div>
							<div class="col-lg-7">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:LinkButton ID="censusBtn" onclick='uploadCensus' runat="server" CssClass="btn btn-success btn-xs">
                                    <i class="fa fa-upload fa-white"></i> Upload </asp:LinkButton></div>
					    </div>
                         <br />
                        
                         <asp:Literal ID="errorMsg" runat="server" />
                       
                       </div>
                      </div>
                     </div>
                    </div>
                        
<%-- </form>--%>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="script" Runat="Server">
</asp:Content>

