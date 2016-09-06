using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Diagnostics;

public partial class Scripts_XMLGenerators_GenerateEmployeeXMLByGroup : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //*** CREATE THE SQL CONNECTION ***
            SqlConnection myConnection = new SqlConnection(Utility.Constants.ConnectionString);

            myConnection.Open();
            Response.ContentType = "application/xml";
            Response.Write("<?xml version=\"1.0\"?> \n");
            Response.Write("<complete> \n");
            Response.Write("<option value=\"\"/> \n");
            //*** PULL LIST OF EMPLOYEES ***

            SqlDataReader myReader = null;
            //SqlCommand myCommand = new SqlCommand("SELECT * FROM [" +Session["currentAgency"] + "].[" + Session["name"] + "].[Employees]", myConnection);
            SqlCommand myCommand = null;
           // Response.Write(Request["mask"].ToString());
            if (!String.IsNullOrWhiteSpace(Request["mask"]))
            {
                string sql = null;
                myReader = null;

                // *** CHECK IF REQUESTING BY SSN ***
                if (!String.IsNullOrWhiteSpace(Request["ssn"]) && Request["ssn"] == "1")
                {
                    //SqlCommand myCommand = new SqlCommand("SELECT * FROM [" +Session["currentAgency"] + "].[" + Session["name"] + "].[Employees]", myConnection);
                    sql = "SELECT * FROM [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Employees] WHERE RIGHT([ssn],4) LIKE '" + Request["mask"] + "%'" + " AND archive = 'no' ORDER BY [lname], [fname]";
                 //   Response.Write(sql);
                    myReader = new SqlCommand(sql, myConnection).ExecuteReader();

                    while (myReader.Read())
                    {
                        //Response.Items.Add(new ListItem(myReader["fname"].ToString() + " " + myReader["lname"].ToString(), myReader["empID"].ToString()));
                        //String usr = myReader["fname"].ToString() + " " + myReader["lname"].ToString();
                        //Response.Write("<option value='" + usr + "'>" + usr + "</option> \n");
                        Response.Write("<option value=\"" + myReader["empID"].ToString() + "\">" + myReader["ssn"].ToString().Substring(7, 4) + ": " + myReader["lname"].ToString() + ", " + myReader["fname"].ToString() + "</option> \n");
                    }
                }
                // *** ELSE REQUEST BY NAME ***
                else
                {
                    sql = "SELECT * FROM [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Employees] WHERE [lname] LIKE '" + Request["mask"] + "%'" + " AND archive = 'no' ORDER BY [lname], [fname]";
                  //  Response.Write(sql);
                    myCommand = new SqlCommand(sql, myConnection);
                    myReader = myCommand.ExecuteReader();

                    while (myReader.Read())
                    {
                        Response.Write("<option value=\"" + myReader["empID"].ToString() + "\">" + myReader["lname"].ToString() + ", " + myReader["fname"].ToString() + ", " + HttpUtility.HtmlEncode(myReader["location"].ToString()) + "</option> \n");
                    }

                }
                myReader.Close();
                myConnection.Close();
                Response.Write("</complete>");
            }
            else
            {
                myCommand = new SqlCommand("SELECT top 1000  [empID],[fname],[lname],[ssn],[location] FROM [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Employees] WHERE archive = 'no' ORDER BY [lname], [fname]", myConnection);
            //    Response.Write("SELECT  [empID],[fname],[lname],[ssn],[location] FROM [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Employees] WHERE archive = 'no' ORDER BY [lname], [fname]");
                
                myReader = myCommand.ExecuteReader();

                while (myReader.Read())
                {
                    // *** CHECK IF REQUESTING BY SSN ***
                    if (Request["ssn"] == "1")
                    {
                        Response.Write("<option value=\"" + myReader["empID"].ToString() + "\">" + myReader["ssn"].ToString().Substring(7, 4) + ": " + myReader["lname"].ToString() + ", " + myReader["fname"].ToString() + "</option> \n");
                    }
                    // *** ELSE REQUEST BY NAME ***
                    else Response.Write("<option value=\"" + myReader["empID"].ToString() + "\">" + myReader["lname"].ToString() + ", " + myReader["fname"].ToString()  + "</option> \n");
                }
                Response.Write("</complete>");
                myReader.Close();
                myConnection.Close();
            }
        }
        catch (Exception error)
        {
            //Response.Write(error.ToString());
            var st = new StackTrace(error, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            Response.Write(line);
        }
    }
}