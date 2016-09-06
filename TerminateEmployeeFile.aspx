<%@ Page Title="Upload file" Language="C#" MasterPageFile="~/Default.master" AutoEventWireup="true" CodeFile="TerminateEmployeeFile.aspx.cs" Inherits="UploadEmployeeFile" %>

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
                    <span style="color:Red">***NOTE: All fields are required. Dates should in the format MM/DD/YYYY *** </span><br />
                       Note that you can only upload CSV files. Your file can have the following columns: <br /><br />
                       <strong>| 'lname' or 'last name' ||	'fname' or 'first name'  || 'Date of birth' or 'dob' || 'SSN' || 'Termination date' || 'Coverage End Date' ||'Reason' || 'Comments' |                      </strong><br />  <br />
                       <a href="/NewHire/Documents/templates/terminateEmployeeFile.csv">Example file</a>
                     <br /><br />
                      
                        <div class="form-group" runat="server" id="terminateFileDiv">
							<label class="col-lg-3 control-label">Terminate Employees: </label>
							<div class="col-lg-2"><input type="file" id="terminationsFile" name="terminationsFile" runat="server" /> </div>
							<div class="col-lg-7">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:LinkButton ID="uploadBtn" onclick='uploadTerminations' runat="server" CssClass="btn btn-info btn-xs">
                                    <i class="fa fa-upload fa-white"></i> Upload </asp:LinkButton></div>
					    </div>

                        <div class="form-group">
								<div class="col-lg-7">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:LinkButton  ID="downloadBtn"   runat="server" onclick='btnDownload_Click'
                                 CssClass="btn btn-success btn-xs" Visible="false" >
                                    <i class="fa fa-download fa-white"></i> Download Log </asp:LinkButton></div>

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

