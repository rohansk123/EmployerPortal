using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

public partial class Account_Logout : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
        Response.Cache.SetCacheability(HttpCacheability.NoCache);
        Response.Cache.SetNoStore();
        loginLink.NavigateUrl = Page.ResolveUrl("~") + "/Account/Login.aspx?group=" + Session["group"] + "&agency=" + Session["currentAgency"];
        Session.Clear();
        Session.Abandon();
        Session["title"] = "Thank you.";

        FormsAuthentication.SignOut();
    }
}