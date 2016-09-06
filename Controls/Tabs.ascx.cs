using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Controls_Tabs : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string tabsContent = "<header class=' header-main'> <div class='navbar navbar-default' role='navigation'>" +

       "<button type='button' class='navbar-toggle' data-toggle='collapse' data-target='.navbar-collapse'>" +
       "<span class='sr-only'>Toggle navigation</span> <span class='icon-bar'></span> <span class='icon-bar'></span>" +
        "<span class='icon-bar'></span> </button><div id='navlist' class='navbar-collapse collapse'><ul class='nav navbar-nav '>";

        if (!Request.Url.ToString().Contains("Upload") && !Request.Url.ToString().Contains("Terminate"))
            tabsContent += "<li id='information' class='active dropdown'><a  href='" + Page.ResolveUrl("~");
         else
            tabsContent += "<li id='information' class='dropdown'><a  href='" + Page.ResolveUrl("~");
                
       tabsContent+="'> <span> <i class=\"fa fa-home\"></i> Home</span></a></li>";


       if (Request.Url.ToString().Contains("Upload"))
       {
           tabsContent += "<li id='information' class='active dropdown'><a  href='" + Page.ResolveUrl("~/UploadEmployeeFile.aspx");
           tabsContent += "'> <span><i class='fa fa-plus-square fa-white'></i> Upload Employee File</span></a></li>";
           tabsContent += "<li id='information' class=' dropdown'><a  href='" + Page.ResolveUrl("~/TerminateEmployeeFile.aspx");
           tabsContent += "'> <span><i class='fa fa-minus-square fa-white'></i> Upload Terminations</span></a></li>";
       }
       else if (Request.Url.ToString().Contains("AddEmployee"))
       {
           tabsContent += "<li id='information' class='dropdown'><a  href='" + Page.ResolveUrl("~/UploadEmployeeFile.aspx");
           tabsContent += "'> <span><i class='fa fa-plus-square fa-white'></i> Upload Employee File</span></a></li>";
           tabsContent += "<li id='information' class='dropdown'><a  href='" + Page.ResolveUrl("~/TerminateEmployeeFile.aspx");
           tabsContent += "'> <span><i class='fa fa-minus-square fa-white'></i> Upload Terminations</span></a></li>";

       }
       else if (Request.Url.ToString().Contains("TerminateEmployeeFile"))
       {
           tabsContent += "<li id='information' class='dropdown'><a  href='" + Page.ResolveUrl("~/UploadEmployeeFile.aspx");
           tabsContent += "'> <span><i class='fa fa-plus-square fa-white'></i> Upload Employee File</span></a></li>";
           tabsContent += "<li id='information' class='active dropdown'><a  href='" + Page.ResolveUrl("~/TerminateEmployeeFile.aspx");
           tabsContent += "'> <span><i class='fa fa-minus-square fa-white'></i> Upload Terminations</span></a></li>";

       }
       else if (Request.Url.ToString().Contains("Grid"))
       {
           tabsContent += "<li id='information' class='dropdown'><a  href='" + Page.ResolveUrl("~/TerminateEmployeeFile.aspx");
           tabsContent += "'> <span><i class='fa fa-minus-square fa-white'></i> Upload Terminations</span></a></li>";
       }

       //if (Request.Url.ToString().Contains("Terminate"))
       //    tabsContent += "<li id='information' class='active dropdown'><a  href='" + Page.ResolveUrl("~/TerminateEmployee.aspx");
       //else
       //    tabsContent += "<li id='information' class='dropdown'><a  href='" + Page.ResolveUrl("~/TerminateEmployee.aspx");

       //tabsContent += "'> <span>Terminate Employee</span></a></li>";

       tabsContent += "<li id='information' class='dropdown'><a  href='" + Page.ResolveUrl("~/Account/Logout.aspx") + "'> <span><i class=\"fa fa-sign-out\"></i> Sign-out</span></a></li>";
  
       tabsContent += "   </ul></div></div></header>";

        TabsContent.Text = tabsContent;

    }
}