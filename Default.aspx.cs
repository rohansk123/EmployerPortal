using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

public partial class _Default : System.Web.UI.Page
{
    SqlConnection myConnection = new SqlConnection(Utility.Constants.ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
     //Response.Write(HttpContext.Current.User.Identity.Name);
        //groupMenu.SQLConnectionString = SiteMaster.MyGlobals.connectionString;
        //*** CREATE THE SQL CONNECTION ***
        if (Session["newUser"] != null)
        {
            if (Session["newUser"].ToString() == "True")
            {

                Response.Redirect(Page.ResolveUrl("~") + "/Account/ChangeInfo.aspx");
            }

        }
        try
        {
            myConnection.Open();
            string name = null;

        
            // *** CHECK IF USER ATTEMPTED ROUTE CHANGE ***
            if (!String.IsNullOrWhiteSpace((string)Page.RouteData.Values["GroupName"]))
                name = (string)Page.RouteData.Values["GroupName"];
            // *** ELSE USE SESSION VARIABLE ***
            else if (String.IsNullOrWhiteSpace(name))
                name = (string)Session["group"];

            // *** REASSIGN GROUP SESSION VARIABLES ***
            if (doesGroupExist(name))
            {
                string SQL = "SELECT * FROM [" + Session["currentAgency"] + "].[general].[Groups] WHERE short='" + name + "'";
                // Response.Write(SQL);
                SqlDataReader myReader = new SqlCommand(SQL, myConnection).ExecuteReader();

                if (myReader.HasRows)
                {
                    while (myReader.Read())
                    {
                        // *** CLEAR ALL SESSION DATA, PRESERVE AGENCY VALUES ***
                        string currentAgency = null;
                        string agency = null;
                        string titleColor = null;

                        if (!String.IsNullOrWhiteSpace((string)Session["agency"]))
                            agency = (string)Session["agency"];

                        if (!String.IsNullOrWhiteSpace((string)Session["currentAgency"]))
                            currentAgency = (string)Session["currentAgency"];

                        if (Convert.ToBoolean(myReader["OE_active"]))
                            Session["titleColor"] = "black";
                        else
                            Session["titleColor"] = "red";
                        //Session.Clear();

                        if (!String.IsNullOrWhiteSpace(agency))
                        {
                            Session["currentAgency"] = currentAgency;
                            Session["agency"] = agency;
                        }
                        //throw new Exception("<p/> " + Convert.ToBoolean(myReader["OE_active"]).ToString());
                        // *** UPDATE SESSION VARIABLES ***
                        Session["groupID"] = myReader["group_number"].ToString();
                        Session["group"] = myReader["short"].ToString();
                        Session["state"] = myReader["state"].ToString();
                        Session["billing_mode"] = Utility.MiscUtils.getBillingMode(myReader["payroll_modal"].ToString());
                        Session["title"] = myReader["group_name"].ToString();
                        //Session["empID"] = "N/A";
                        Session["effective_date"] = myReader["effective_date"].ToString();
                        //Response.Write(myReader["effective_date"].ToString());

                        if (Request["singleGroup"] != null)
                        {
                            Session.Remove("empID");
                            Session.Remove("EEData");
                            Session.Remove("ssn");
                        }
                        Form.Action = "Grid.aspx";
                        // Response.Write(" select case \n   when exists (SELECT 1 FROM [" + Session["currentAgency"] + "].INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='" + name + "' AND TABLE_NAME='JobClass') then 1 \n   else 0 \n end");
                        Session["hasJobClass"] = Convert.ToBoolean(Utility.SQLUtils.getSingleSQLData(" select case \n   when exists (SELECT 1 FROM [" + Session["currentAgency"] + "].INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA='" + name + "' AND TABLE_NAME='JobClass') then 1 \n   else 0 \n end"));
                    }
                    myReader.Close();
                }
                else
                {
                    // *** IF GROUP DOES NOT EXIST, REDIRECT TO GROUPS PAGE *** 
                    myReader.Close();
                    myConnection.Close();
                    Response.Redirect(Page.ResolveUrl("~"));
                }
            }
            //else
            //{
            //    // *** IF GROUP DOES NOT EXIST, REDIRECT TO GROUPS PAGE *** 
            //    myConnection.Close();
            //    Response.Redirect(Page.ResolveUrl("~"));
            //}

            //if (Session["title"].ToString().Contains("[Agency: "))
            //    Response.Redirect(Page.ResolveUrl("~"));

            //Response.Write("TEST: " + (string)Session["name"] + doesGroupExist((string)Session["name"]));

            // *** IF SESSION VARIABLE 'name' HAS NO VALUE, REDIRECT TO GROUPS PAGE *** 
            //if (String.IsNullOrWhiteSpace((string)Session["group"]) || (string)Session["group"] == "Default")
            //    Response.Redirect(Page.ResolveUrl("~"));
        }
        catch (Exception error)
        {
           
            Response.Write(error.ToString());

            // *** IF SESSION VARIABLE 'name' IS 'Default', REDIRECT TO GROUPS PAGE *** 
            //if ((string)Session["group"] == "Default")
            //    Response.Redirect(Page.ResolveUrl("~"));
        }
        myConnection.Close();
    }

    protected bool doesGroupExist(string groupName)
    {
        if (String.IsNullOrWhiteSpace((string)Session["currentAgency"]))
            return false;
        return Convert.ToBoolean(Utility.SQLUtils.getSingleSQLData(" select case \n   when exists (SELECT DISTINCT name FROM " + Session["currentAgency"] + ".sys.schemas WHERE name = '" + groupName + "') \n   then 1 \n   else 0 \n end"));
    }

    protected void Add_Click(object sender, EventArgs e)
    {
        Response.Redirect(Page.ResolveUrl("~/AddEmployee.aspx"));
    }
}