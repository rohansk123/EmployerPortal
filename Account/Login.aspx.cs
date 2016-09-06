using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Web.Security;
using System.Text.RegularExpressions;
using System.Globalization;

public partial class Account_Login : System.Web.UI.Page
{
    String employeeID = "";
    string birthdate = "";
    String agency = "";
    String group = "";
    SqlConnection myConnection = new SqlConnection(Utility.Constants.ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {

       // Response.Redirect(Page.ResolveUrl("~") + "/Account/Default.aspx");
        
                //if (Page.User.Identity.IsAuthenticated)
            //Response.Write("INSIDE");
        Session.Clear();

        Session["empID"] = Request["empID"];
        Session["currentAgency"] = Request["agency"];
        Session["group"] = Request["group"];

      //  Response.Write("agency: " + Session["currentAgency"] + "group: " + Session["group"]);

    }

    protected void Login_Click(object sender, EventArgs e)
    {
        try
        {
                       if (username.Text != "" && password.Text != "")
            {
                if (ValidateUser(username.Text, password.Text))
                {

                    if ((string)Session["agency"] == "Administration")
                        Session["agency"] = "BenefitsDirect";

                    bool isNotArchived = (string)Utility.SQLUtils.getSingleSQLData("SELECT [archive] FROM [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Employees] WHERE [empID]=" + Session["empID"]) == "no";
                    //Response.Write("SELECT [archive] FROM [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Employees] WHERE [empID]=" + Session["empID"]);
                    //Response.Write(isNotArchived);
                    if (isNotArchived)
                    {
                       // Session["redirectFromLogin"] = "True";
                        //Response.Redirect("~/Default.aspx");
                        UpdateLastActivityDate();
                        FormsAuthentication.RedirectFromLoginPage(username.Text, false);
                    }
                    else
                        ErrorMsg.Text = "<div class='alert alert-danger'>This account has been deactivated.</div>";
                }

            }
            else
                ErrorMsg.Text = "<div class='alert alert-danger'>Please enter your username and password.</div>";
        }
        catch (Exception ex)
        {
            ErrorMsg.Text = "<div class='alert alert-danger'>"+ex.Message+"</div>";
        }
    }



    private void UpdateLastActivityDate()
    {
        string SQL = "UPDATE [Administration].[general].[Clients] SET [last_activity_date]='" + DateTime.Now + "' WHERE [empID]='" + Session["empID"].ToString() + "'";
        myConnection.Open();
        if (new SqlCommand(SQL, myConnection).ExecuteNonQuery() == 1)
        {
            myConnection.Close();
            return;
        }
        else
        {
            myConnection.Close();
            throw new Exception("There was an error while authenticating this employee due to a networking error. Please try again.");
        }
    }

    private static string ClearSQLString(string str)
    {
        return Regex.Replace(str, @"[\x00'""\b\n\r\t\cZ\\%_]",
            delegate(Match match)
            {
                string v = match.Value;
                switch (v)
                {
                    case "\x00":            // ASCII NUL (0x00) character
                        return "\\0";
                    case "\b":              // BACKSPACE character
                        return "\\b";
                    case "\n":              // NEWLINE (linefeed) character
                        return "\\n";
                    case "\r":              // CARRIAGE RETURN character
                        return "\\r";
                    case "\t":              // TAB
                        return "\\t";
                    case "\u001A":          // Ctrl-Z
                        return "\\Z";
                    default:
                        return "\\" + v;
                }
            });
    }

   

  
    

    private bool ValidateUser(string username, string password)
    {
        try
        {

            if (username.Contains('@'))
            {

                return ValidateExistingUser(username, password);
            }

            else
            {
                if (Session["group"] == null || Session["currentAgency"] == null)
                    throw new Exception("Your agency or group couldn't be found. Please contact your system administrator at rsureshkumar@ventureusenterprises.com");

                if(username.Length!=6)
                    throw new Exception("Please enter a valid username. See instructions below");
              
                if (password.Length != 8 || (!Utility.MiscUtils.IsDigitsOnly(password)))
                    throw new Exception("Please enter a valid birthdate. See instructions below.");


                string dob = convertPasswordToDateOfBirth(password);
                if(checkIfUserLoggedIn(username, password))
                {
                    //prompt email and password
                    throw new Exception("Please login using your email and PIN.");
                }                
                else if (checkUserExists(username, password))
                {
                    registerUserAsNewHirePortalAdmin(username,true);
                }
                else

                   throw new Exception("<strong>Error!</strong> This user couldn't be found. Please check whether the date of birth is accurate as per the instructions below. If you believe you have received this message by error, please contact your account manager.");

                return false;
              

            }


        }
        catch (Exception e)
        {
            ErrorMsg.Text = "<div class='alert alert-danger'>" + e.Message.ToString() + "</div>";

            return false;
        }
        
    }

    private bool checkIfUserLoggedIn(string username, string password)
    {

        DateTime bd;
        if (DateTime.TryParseExact(password, "MMddyyyy",
           CultureInfo.InvariantCulture, DateTimeStyles.None, out bd))
            birthdate = bd.ToString("MM/dd/yyyy");
        else
            throw new Exception("Please enter a valid date.");

        string SQL = "SELECT COUNT(*)  FROM [Administration].[general].[Clients] WHERE [username]='" + username +
                                       "' AND [agency]='" + Session["currentAgency"] + "' AND [group]='" + Session["group"] + "' AND [birthdate]='" + birthdate +
                                       "' AND ([role_name]='admin') AND ([newhire_login]=1 OR [selfenroll_login]=1) ";
        //Response.Write(SQL);
        myConnection.Open();
        int result= Convert.ToInt32(new SqlCommand(SQL, myConnection).ExecuteScalar());
        if (result > 0)
        {
           
            return true;
        }
       
            return false;
        
    }

    private string convertPasswordToDateOfBirth(string password)
    {
        DateTime bd; string birthDate; Response.Write(password);
        if (DateTime.TryParseExact(password, "MMddyyyy",
           CultureInfo.InvariantCulture, DateTimeStyles.None, out bd))
            birthDate = bd.ToString("MM/dd/yyyy");
        else
            throw new Exception("Please enter a valid date of birth as your PIN.");
        return birthDate;
    }

    public string LoginUserID
    {
        get; set;

    }
  



    private bool checkUserExists(string username, string password)
    {
          
        try
        {

          int numberOfUsers = 0; 
            DateTime bd;
            if (DateTime.TryParseExact(password, "MMddyyyy",
               CultureInfo.InvariantCulture, DateTimeStyles.None, out bd))
                birthdate = bd.ToString("MM/dd/yyyy");
            else
                throw new Exception("Please enter a valid date.");

            string SQL = "SELECT COUNT(*) as count,[empID] FROM [Administration].[general].[Clients] WHERE [username]='" + username +
                                           "' AND [agency]='" + Session["currentAgency"] + "' AND [group]='" + Session["group"] + "' AND [birthdate]='" + birthdate +
                                           "' AND [role_name]='admin' GROUP BY [empID]";
         //Response.Write(SQL);// return ;
           
            SqlDataReader clientData = new SqlCommand(SQL, myConnection).ExecuteReader();
            while (clientData.Read())
            {
                numberOfUsers = Convert.ToInt32(clientData["count"].ToString());


                employeeID = clientData["empID"].ToString();



            }
            if (numberOfUsers > 0)
            {
               
                return true;

            }


            return false;
        }
        catch (Exception e)
        {
            Response.Write(e.ToString());
            return false;
        }
        finally
        {
            myConnection.Close();
        }
        

    }

    private void registerUserAsNewHirePortalAdmin(string username, bool newUser)
    {  
        try
        {
            


                string SQL = "UPDATE [Administration].[general].[Clients] SET [newhire_login]=1 WHERE [empID]='" + employeeID + "'";
                myConnection.Open();
                if (new SqlCommand(SQL, myConnection).ExecuteNonQuery() == 1)
                {
                    //Session["redirectFromLogin"] = "True";'
                    if(newUser)
                        Session["NewUser"] = "True";
                    Session["empID"] = employeeID;
                    FormsAuthentication.RedirectFromLoginPage(username, false);
                }
                else
                    throw new Exception("There was an error while updating this New Employee Portal Admin User. Please try again.");
         
              
        
        }         
        
        catch (Exception e)
        {
            Response.Write(e.ToString());
            throw  e;
          
        }
        finally
        {
            myConnection.Close();
        }
       
    }

    private string createUserID(SqlDataReader userData)
    {
        char firstInitial = Char.ToLower(userData["fname"].ToString()[0]);
        
        char lastInitial = Char.ToLower(userData["lname"].ToString()[0]);
        string ssn = userData["ssn"].ToString();
        string last4SSN = ssn.Substring(ssn.Length - 4);
        System.Text.StringBuilder ID = new System.Text.StringBuilder();
        // ID = firstInitial + lastInitial + last4SSN;
        ID.Append(firstInitial);
        ID.Append(lastInitial);
        ID.Append(last4SSN);
        return  ID.ToString();
     

    }


    private static string ConvertPasswordToDateOfBirth(string password, string formatOfPassword, string formatOfDate)
    {
        try
        {
            DateTime bd; string birthdate; //HttpContext.Current.Response.Write(password);
            if (DateTime.TryParseExact(password, formatOfPassword,
               CultureInfo.InvariantCulture, DateTimeStyles.None, out bd))
                birthdate = bd.ToString(formatOfDate);
            else
                throw new Exception("Please enter a valid date of birth as your PIN.");
            return birthdate;
        }
        catch (Exception e)
        {
            throw e;
        }
    }
    private bool ValidateExistingUser(string username, string password)
    {
       // SqlConnection myConnection = new SqlConnection(SiteMaster.MyGlobals.connectionString);
        //throw new Exception(" select case \n   when exists (SELECT 1 FROM [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Employees] WHERE [email]='" + username +
        //    "') then 1 \n   else 0 \n end");
        bool userExists = Convert.ToBoolean(Utility.SQLUtils.getSingleSQLData(" select case \n   when exists (SELECT 1 FROM [Administration].[general].[Clients] WHERE [email]='" + username +
            "' AND [isActive]=1) then 1 \n   else 0 \n end"));
        if (userExists)
        {
            myConnection.Open();
            string toCompare = ""; string DOB="";
            SqlDataReader userData = new SqlCommand(" SELECT [password],[agency],[group],[empID],[birthdate] FROM [Administration].[general].[Clients] WHERE [email]='" + username + "'", myConnection).ExecuteReader();
            // Response.Write(" SELECT [password],[agency],[group],[empID] FROM [Administration].[general].[Clients] WHERE [email]='" + username + "'");

            while (userData.Read())
            {
                Session["empID"] = employeeID = userData["empID"].ToString();
                Session["currentAgency"] = userData["agency"].ToString();
                Session["group"] = userData["group"].ToString();
                toCompare = ConvertPasswordToDateOfBirth(password,"MMddyyyy","MM/dd/yyyy");
                DOB = userData["birthdate"].ToString();
            }
            userData.Close();
            myConnection.Close();

            if (DOB == toCompare)
            {
                bool adminExists = Convert.ToBoolean(Utility.SQLUtils.getSingleSQLData(" select case \n   when exists (SELECT 1 FROM [Administration].[general].[Clients] WHERE [email]='" + username +
           "' AND [role_name]='admin' AND [newhire_login]=1 ) then 1 \n   else 0 \n end"));

                if (adminExists)
                    return true;
                
                else
                {
                    bool selfEnrollExists = Convert.ToBoolean(Utility.SQLUtils.getSingleSQLData(" select case \n   when exists (SELECT 1 FROM [Administration].[general].[Clients] WHERE [email]='" + username +
          "' AND [role_name]='admin' AND [selfenroll_login]=1 ) then 1 \n   else 0 \n end"));

                    if (selfEnrollExists)
                    {
                        registerUserAsNewHirePortalAdmin(username, false);
                        return true;
                    }

                    ErrorMsg.Text = "<div class='alert alert-danger'>This user doesn't have access to this site. If you believe you have received this message in error please contact your account manager. </div>";
         
                    bool guestExists = Convert.ToBoolean(Utility.SQLUtils.getSingleSQLData(" select case \n   when exists (SELECT 1 FROM [Administration].[general].[Clients] WHERE [email]='" + username +
          "' AND [role_name]='admin' AND ([newhire_login]=0 OR [newhire_login] IS NULL) AND ([selfenroll_login]=0 OR [selfenroll_login] IS NULL)) then 1 \n   else 0 \n end"));
                   
                    if(guestExists)
                        ErrorMsg.Text = "<div class='alert alert-danger'>Since this is your first visit, please see instructions below. </div>";
         

                          }
                    return false;
            }
            else
            {
                ErrorMsg.Text = "<div class='alert alert-danger'>The user name or password is incorrect. </div>";
                bool guestExists = Convert.ToBoolean(Utility.SQLUtils.getSingleSQLData(" select case \n   when exists (SELECT 1 FROM [Administration].[general].[Clients] WHERE [email]='" + username +
         "' AND [role_name]='admin' AND ([newhire_login]=0 OR [newhire_login] IS NULL) AND ([selfenroll_login]=0 OR [selfenroll_login] IS NULL)) then 1 \n   else 0 \n end"));
                   
      

                if (guestExists)
                    ErrorMsg.Text = "<div class='alert alert-danger'>Since this is your first visit, please see instructions below. </div>";
         


                return false;
            }
        }
        else
        {
            ErrorMsg.Text = "<div class='alert alert-danger'>This user doesn't exist in the system. If you believe this is in error, please contact your account manager.</div>";
            return false;
        }
    }

  
}
