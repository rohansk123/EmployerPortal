using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;

public partial class TerminateEmployee : System.Web.UI.Page
{
    SqlConnection myConnection = new SqlConnection(Utility.Constants.ConnectionString);
    protected void Page_Load(object sender, EventArgs e)
    {
        //groupMenu.SQLConnectionString = SiteMaster.MyGlobals.connectionString;
        //*** CREATE THE SQL CONNECTION ***
        
        try
        {
            dateOfTermination.Attributes.Add("onkeypress", "FormatNumber(this, '##/##/####');");

            coverageEndDate.Attributes.Add("onkeypress", "FormatNumber(this, '##/##/####');");
           // GetEmployeeDetails();
            name.Text = (string)Session["selectedEmployeeName"];
        }
        catch (Exception error)
        {
            //throw new Exception(error.ToString());
            Response.Write(error.ToString());

            // *** IF SESSION VARIABLE 'name' IS 'Default', REDIRECT TO GROUPS PAGE *** 
            if ((string)Session["group"] == "Default")
                Response.Redirect(Page.ResolveUrl("~"));
        }
        myConnection.Close();
    }

   
 

    protected void Terminate_Click(object sender, EventArgs e)
    {
        //Response.Write(Request.Form["EID"]);
        try
        {
            errorMsg.InnerHtml = "";
            string message  = "";
            string subject = "";

            AddTerminationTableIfItDoesNotExists();
               // Session["empID"] = Request["EID"];
            TerminateCurrentEmployee();
            StopRecurringPayments();
            SendEmail();

                //bool success=Utility.MiscUtils.SendEmail(subject, message, myConnection, Session["group"].ToString(), Session["currentAgency"].ToString());
                //if (success == true)
                //{
                //    errorMsg.Attributes["class"] = "alert alert-success";
                //    errorMsg.InnerHtml = "<strong> Success! </strong> Your account manager has been notified about this employee."; // + SQL + error.Message + error.StackTrace;

                //}
                //else
                //{
                //    errorMsg.Attributes["class"] = "alert alert-success";
                //    errorMsg.InnerHtml = "<strong> Error! </strong> This employee couldn't be removed, please contact your account manager."; // + SQL + error.Message + error.StackTrace;

                //}
                // Response.Redirect("~/Products.aspx?EID=" + Request["EID"]);
           
        }
        catch (Exception ex)
        {
            errorMsg.InnerHtml +="<div class='alert alert-success'><strong>" +ex.Message.ToString()+"</strong></div>";
            Response.Write(ex.ToString());
        }
        finally
        {
            myConnection.Close();
        }
    }
    private void StopRecurringPayments()
    {
        //throw new NotImplementedException();
        string existSQL = "SELECT COUNT(EMPID) FROM [" + Session["currentAgency"] + "].[general].[PaymentInfo] WHERE EMPID="
            + Session["EmpID"] + " AND [SHORT]='" + Session["group"] + "'";
        // Response.Write(existSQL);
        int rows = Convert.ToInt32(Utility.SQLUtils.getSingleSQLData(existSQL));
        if (rows > 0)
        {
            
            //    myConnection.Open();
            if (myConnection.State == ConnectionState.Closed)
                myConnection.Open();
                string updateSQL = "UPDATE [" + Session["currentAgency"] + "].[general].[PaymentInfo] SET [ENABLE_RECURRING]=0 WHERE EMPID="
            + Session["EmpID"] + " AND [SHORT]='" + Session["group"] + "'";
                // Response.Write(updateSQL);
                int rowsEffected = new SqlCommand(updateSQL, myConnection).ExecuteNonQuery();

                if (rowsEffected > 0)
                {
                    errorMsg.Attributes["class"] = "alert alert-success";
                    errorMsg.InnerHtml += "All future payments for this employee have been stopped."; // + SQL + error.Message + error.StackTrace;

                }
                  //  Response.Write("<div class='alert alert-success'> <button class='close' data-dismiss='alert'>×</button><strong>Success!</strong> All future payments for this employee have been stopped.</b></div>");


            
        }

    }
    private void SendEmail()
    {
        string subject = "Group "+Session["group"]+": Employee Terminated";

        string message ="The following employee has been terminated: <br/><br/>"+
           "<table border='1'><tr><th>Employee ID</th><th>Employee Name</th><th>Date of birth</th><th>Termination Date</th><th>Coverage End Date</th><th>Reason</th><th>Comments</th></tr>"+
           "<tr><td>" + Session["selectedEmpID"].ToString() + "</td><td>" + Session["selectedEmployeeName"] + "</td><td>" + Session["selectedBirthDate"] + "</td><td>" + dateOfTermination.Text +
           "</td><td>"+ coverageEndDate.Text+"</td><td>"+ terminationReason.SelectedItem.Text+"</td><td>"+terminationComments.Text+"</td></tr></table><br/>"+
           "---THIS IS AN AUTOMATICALLY GENERATED MESSAGE. PLEASE DO NOT REPLY TO THIS MESSAGE AS WE ARE UNABLE TO RESPOND FROM THIS ADDRESS.---";

        bool success=Utility.MiscUtils.SendEmailToAccountManager(subject, message, myConnection, Session["group"].ToString(), Session["currentAgency"].ToString());
        if (success == true)
        {
            errorMsg.Attributes["class"] = "alert alert-success";
            errorMsg.InnerHtml += "<strong> </strong> Your account manager has been notified about this employee."; // + SQL + error.Message + error.StackTrace;

        }
        else
        {
            errorMsg.Attributes["class"] = "alert alert-danger";
            errorMsg.InnerHtml += "<strong> Error! </strong> The account manager couldn't be notified about this employee."; // + SQL + error.Message + error.StackTrace;

        }
        // Response.Redirect("~/Products.aspx?EID=" + Request["EID"]);
    }

    private void TerminateCurrentEmployee()
    {
        myConnection.Open();
        if (IfEntryForEmployeeExists())
            UpdateTerminatedEmployee();
        else
            InsertTerminatedEmployee();
        myConnection.Close();
    }

    private void UpdateTerminatedEmployee()
    {
      
        string SQL = "Update [" + Session["currentAgency"] + "].[" + Session["group"] + "].[TerminatedEmployees]" +
           " SET  [terminationdate]='"+dateOfTermination.Text+"', [coverageenddate]='"+coverageEndDate.Text+
           "',[reason]='"+terminationReason.SelectedItem.Text+"',[comments]='"+terminationComments.Text+"' "+
               "where[EmpID] = " + Session["selectedEmpID"].ToString();
       // Response.Write(SQL);
        int update = new SqlCommand(SQL, myConnection).ExecuteNonQuery();
        if (update == 1)
        {
            errorMsg.Attributes["class"] = "alert alert-success";
            errorMsg.InnerHtml = "<strong>Success! Employee termination successfully updated.</strong>";
        }
       
    }

    private void InsertTerminatedEmployee()
    {
        //myConnection.Open();
        string SQL = "INSERT INTO [" + Session["currentAgency"] + "].[" + Session["group"] + "].[TerminatedEmployees]" +
           "([EmpID],[terminationdate],[coverageenddate],[reason],[comments]) VALUES(" +Session["selectedEmpID"].ToString()+",'"+
            dateOfTermination.Text + "','" +coverageEndDate.Text+"','" + terminationReason.SelectedItem.Text + "','" + terminationComments.Text + "')" ;
       // Response.Write(SQL);
        int insert = new SqlCommand(SQL, myConnection).ExecuteNonQuery();

        SQL = "UPDATE [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Employees]  "+
            " SET [APP_STATUS]='terminated' where  [empID] = " + Session["selectedEmpID"].ToString();

        new SqlCommand(SQL, myConnection).ExecuteNonQuery();

        if (insert == 1)
        {
            errorMsg.InnerHtml = "<div class='alert alert-success'><strong>Success! Employee termination has been recorded.</strong></div>";
        }
       
    }

    private bool     IfEntryForEmployeeExists()
    {
       

        string SQL = "SELECT COUNT(*) FROM [" + Session["currentAgency"] + "].[" + Session["group"] + "].[TerminatedEmployees]"+
            " WHERE [EmpID] = " + Session["selectedEmpID"]  ;
            
        //  string SQL = "SELECT * FROM  [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Employees] WHERE [EMPID] = " + Session["selectedEmpID"];
      //  Response.Write(SQL);
        int empExists= Convert.ToInt32(Utility.SQLUtils.getSingleSQLData(SQL));
      //  Response.Write("empexists:" + empExists);
       if (empExists > 0)
       {
           return true;
       }
       else
           return false;

    }

    private void AddTerminationTableIfItDoesNotExists()
    {
       

        string SQL = "SELECT COUNT(*) FROM [" + Session["currentAgency"] + "].INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '" + Session["group"] + "' " +
                " AND  TABLE_NAME = 'TerminatedEmployees'";
     //   Response.Write(SQL);
      //  string SQL = "SELECT * FROM  [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Employees] WHERE [EMPID] = " + Session["selectedEmpID"];
          int tableExists= Convert.ToInt32( Utility.SQLUtils.getSingleSQLData(SQL));
          if (tableExists == 0)
          {
              SQL = "CREATE TABLE [" + Session["group"] + "].[TerminatedEmployees]( \n" +
                    "	[ID] [int] IDENTITY(1,1) NOT NULL, \n" +
                    "	[EmpID] [int] NOT NULL, \n" +
                      "	[terminationdate] [varchar](25) NULL, \n" +
                      " [coverageenddate] [varchar](25) NULL, \n"+
                    "	[reason] [varchar](25) NOT NULL, \n" +
                    "	[comments] [varchar](MAX)  NULL, \n" +                 
                    ") ON [PRIMARY] \n" +
                    "GRANT SELECT, INSERT, UPDATE  ON [" + Session["group"] + "].[TerminatedEmployees] TO situsmain \n\n";

            //  Response.Write(SQL);
              SqlConnection adminConnection = new SqlConnection(Utility.Constants.AdminConnectionString);
              adminConnection.Open();
              new SqlCommand(SQL, adminConnection).ExecuteNonQuery();
              adminConnection.Close();
          }

         
    }
}