using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;

public partial class _Default : System.Web.UI.Page
{
    SqlConnection myConnection = new SqlConnection(Utility.Constants.ConnectionString);

    SqlDataReader myReader = null;
    SqlCommand myCommand = null;


    List<string> names = new List<string>();
    string SQL = null;
    bool hasJC = false;
    bool hasED = false;
    bool hasCED = false;

    protected void Page_PreInit(object sender, EventArgs e)
    {
        if (Session["newUser"] != null)
        {
            if (Session["newUser"].ToString() == "True")
            {
                Session["newUser"] = "False";
                Session.Remove("newUser");
                Response.Redirect(Page.ResolveUrl("~") + "/Account/ChangeInfo.aspx");
            }

        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
       
        //Session.Clear();

       // Response.Write("employee id:"+ Session["empID"].ToString());
        if (Session["currentAgency"] == null && Session["group"] == null)
        {
            Session["currentAgency"] = Request["agency"];
            Session["group"] = Request["group"];

          
        }

        

        Session["title"] = Session["group"] + " - Add Employee to Situs";
      //  groupName.Text = (string)Session["title"];

        //Response.Write(

        myConnection.Open();
       // groupMenu.SQLConnectionString = SiteMaster.MyGlobals.connectionString;
        try
        {
            //Response.Write("agency: " + Session["currentAgency"].ToString() + " group:" + Session["group"].ToString());
            if (Session["currentAgency"] == null || Session["group"] == null)
            {
                CatastrophicError.Attributes["class"] = "alert alert-danger";
                CatastrophicError.InnerHtml = "<strong> Error! </strong> Your agency or group couldn't be identified. You will not be able to use this site - please contact your account manager.";
                BtnAdd.Enabled = false;
                BtnAdd.Visible = false;
                //BtnAdd.OnClientClick = null;
                return;
            }
            else
            {
                BtnAdd.Enabled = true;
                BtnAdd.Visible = true;
            }
            //Response.Write("select case \n   when exists (SELECT 1 FROM " + Session["currentAgency"] + ".INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'JobClass' AND TABLE_SCHEMA = '" + Session["group"] + "') \n   then 1 \n   else 0 \n end");
            hasJC = Convert.ToBoolean(Utility.SQLUtils.getSingleSQLData(" select case \n   when exists (SELECT 1 FROM " + Session["currentAgency"] + ".INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'JobClass' AND TABLE_SCHEMA = '" + Session["group"] + "') \n   then 1 \n   else 0 \n end"));
        
            hasED = Convert.ToBoolean(Utility.SQLUtils.getSingleSQLData(" select case \n   when exists (SELECT 1 FROM " + Session["currentAgency"] + ".INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Employees' AND TABLE_SCHEMA = '" + Session["group"] + "' AND COLUMN_NAME = 'effective_date') \n   then 1 \n   else 0 \n end"));
            hasCED = Convert.ToBoolean(Utility.SQLUtils.getSingleSQLData(" select case \n   when exists (SELECT 1 FROM " + Session["currentAgency"] + ".INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Employees' AND TABLE_SCHEMA = '" + Session["group"] + "' AND COLUMN_NAME = 'COMBI_effective_date') \n   then 1 \n   else 0 \n end"));

            if (!IsPostBack)
            {
                effective_date.Text = (string)Utility.SQLUtils.getSingleSQLData("SELECT DISTINCT [effective_date] FROM [" + Session["currentAgency"] + "].[general].[Groups] WHERE [short]='" + Session["group"] + "'");
                state.Text = (string)Utility.SQLUtils.getSingleSQLData("SELECT DISTINCT [state] FROM [" + Session["currentAgency"] + "].[general].[Groups] WHERE [short]='" + Session["group"] + "'");
                payGroup.Text = (string)Utility.SQLUtils.getSingleSQLData("SELECT DISTINCT [payroll_modal] FROM [" + Session["currentAgency"] + "].[general].[Groups] WHERE [short]='" + Session["group"] + "'");

                location.Items.Clear();

                //throw new Exception("SELECT DISTINCT [location_name] FROM [" + Session["currentAgency"] + "].[" + Session["name"] + "].[Locations]");
                myReader = new SqlCommand("SELECT DISTINCT [location_name] FROM [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Locations]", myConnection).ExecuteReader();
                while (myReader.Read())
                {
                    location.Items.Add(new ListItem(myReader["location_name"].ToString(), myReader["location_name"].ToString()));
                }
                myReader.Close();

                if (hasJC)
                {
                    myReader = new SqlCommand("SELECT DISTINCT [jobclass_name] FROM [" + Session["currentAgency"] + "].[" + Session["group"] + "].[JobClass]", myConnection).ExecuteReader();
                    while (myReader.Read())
                    {
                        jobclass.Items.Add(new ListItem(myReader["jobclass_name"].ToString(), myReader["jobclass_name"].ToString()));
                    }
                    myReader.Close();
                }
                else
                    jcDiv.Visible = false;
            }

            myReader = new SqlCommand("SELECT [COLUMN_NAME] FROM " + Session["currentAgency"] + ".INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Employees' AND TABLE_SCHEMA = '" + Session["group"] + "'",
                myConnection).ExecuteReader();
            while (myReader.Read())
            {
                if (myReader["COLUMN_NAME"].ToString() == "ext_EmpID")
                    addtnlCtrls.Controls.Add(new Literal()
                    {
                        Text = "<div class='form-group' style='vertical-align:bottom'> \n" +
                                    "<label class='col-sm-3 col-md-3 col-lg-2 control-label' for='ext_EmpID'> External EmpID:</label> \n" +
                                    "<div class='col-sm-9 col-md-9 col-lg-10'> \n" +
                                    "<input type='text' class='addField' id='ext_EmpID' name='ext_EmpID' maxlength='25' validate='false' /> </div> \n" +
                                "</div>"
                    });
            }
            myReader.Close();
        
            zip.Attributes.Add("onkeypress", "FormatNumber(this, '#####-####');");
            homep.Attributes.Add("onkeypress", "FormatNumber(this);");
            workp.Attributes.Add("onkeypress", "FormatNumber(this);");
            ssn.Attributes.Add("onkeypress", "FormatNumber(this, '###-##-####');");
            dob.Attributes.Add("onkeypress", "FormatNumber(this, '##/##/####');");
            doh.Attributes.Add("onkeypress", "FormatNumber(this, '##/##/####');");
            effective_date.Attributes.Add("onkeypress", "FormatNumber(this, '##/##/####');");
            fidelity_effective_date.Attributes.Add("onkeypress", "FormatNumber(this, '##/##/####');");
        }
        catch (Exception error)
        {
            var st = new StackTrace(error, true);
            // Get the top stack frame
            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            Response.Write(error.ToString()+ line);
            errorMsg.Attributes["class"] = "alert alert-danger";
            errorMsg.InnerHtml = error.ToString();
        }
        myConnection.Close();


    }

    protected void AddEmployee(object sender, EventArgs e)
    {
        try
        {
            myConnection.Open();
            errorMsg.Attributes.Clear();
            errorMsg.InnerHtml = "";

            Decimal newEmpID = 0;
            bool datesAreValid = true;

            DateTime bday = DateTime.Parse(dob.Text);
            DateTime hday;
            if (!String.IsNullOrWhiteSpace(doh.Text))
                hday = DateTime.Parse(doh.Text);

            if (DateTime.Compare(bday, new DateTime(DateTime.Today.Year - 18, DateTime.Today.Month, DateTime.Today.Day)) >= 0)
                datesAreValid = false;

            if (DateTime.Compare(bday, DateTime.Parse("01/01/1900")) < 0)
                datesAreValid = false;

            if ((int)Utility.SQLUtils.getSingleSQLData("select case when exists (SELECT 1 FROM [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Employees] WHERE ssn = '" + ssn.Text + "')    then 1  else 0  end")
                == 1)
                throw new Exception("This SSN has already been added.");

            fname.Text = fname.Text.Replace("'", "");
            lname.Text = lname.Text.Replace("'", "");

            if (String.IsNullOrWhiteSpace(salary.Text))
            {
                salary.Text = "0.00";
            }

            if (datesAreValid)
            {
                //Response.Write(SiteMaster.getSingleSQLData("SELECT MAX([empID]) FROM [" + Session["currentAgency"] + "].[" + Session["name"] + "].[Employees] "));
                //(int)SiteMaster.getSingleSQLData("SELECT MAX([empID]) FROM [" + Session["currentAgency"] + "].[" + Session["name"] + "].[Employees] ");

                SQL = "INSERT INTO [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Employees]([fname],[lname],[Address],[City],[State],[Zip],[sex],[ssn],[birthdate],[hiredate],[relationship],[location]," +
                "[dept],[paygroup],[salary],[title],[hpw],[homep],[cellp],[workp],[email],[effective_date],[app_status],[archive])";

                if (hasCED)
                    SQL = SQL.Insert(SQL.LastIndexOf(")"), ",[COMBI_effective_date]");
                if (!String.IsNullOrWhiteSpace(Request["ext_EmpID"]))
                    SQL = SQL.Insert(SQL.LastIndexOf(")"), ",[ext_EmpID]");
                if (hasJC)
                    SQL = SQL.Insert(SQL.LastIndexOf(")"), ",[jobclass]");

                SQL += " VALUES('" + fname.Text.Replace(',', '.').Trim() + "','" + lname.Text.Replace(',', '.').Trim() + "','" + address.Text.Replace(',', '.') + "','" + city.Text.Trim() + "','" + state.Text + "','" + zip.Text.Trim() + "','" + sex.SelectedItem.Value + "','" + ssn.Text +
                "','" + bday.ToString("MM/dd/yyyy") + "','" + doh.Text + "','" + "Employee" + "','" + location.SelectedValue + "','" +
                "','" + payGroup.SelectedValue + "'," + salary.Text.Replace(",", "").Replace("$", "") + ",'" + occupation.Text + "'," + "40" + ",'" + homep.Text + "','" + homep.Text + "','" + workp.Text + "','" + email.Text +
                "','" + effective_date.Text + "','pending','no')\n";
                //Response.Write(bday.ToShortDateString() + " " + bday.ToLongDateString()+" "+bday.ToString()+" " + bday.ToString("MM/dd/yyyy"));
                if (hasCED)
                    SQL = SQL.Insert(SQL.LastIndexOf(")"), ",'" + Utility.SQLUtils.getSingleSQLData("SELECT [effective_date] FROM [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Products] WHERE [code]='COMBI_LIF'") + "'");
                if (!String.IsNullOrWhiteSpace(Request["ext_EmpID"]))
                    SQL = SQL.Insert(SQL.LastIndexOf(")"), ",'" + Request["ext_EmpID"] + "'");
                if (hasJC)
                    SQL = SQL.Insert(SQL.LastIndexOf(")"), ",'" + jobclass.Text + "'");

                SQL += " SELECT SCOPE_IDENTITY();";
                //Response.Write(SQL);

                newEmpID = (Decimal)new SqlCommand(SQL, myConnection).ExecuteScalar();

                if (newEmpID != 0)
                {
                    Dictionary<string, string> logData = new Dictionary<string, string>();

                    logData["currentAgency"] = (string)Session["currentAgency"];
                    logData["groupName"] = (string)Session["group"];
                    logData["username"] = User.Identity.Name;
                    logData["date"] = DateTime.Now.ToLongDateString();
                    logData["EmpID"] = (string)Session["empID"];
                    logData["type"] = "Census";
                    logData["details"] = "";

                    logData["details"] += "\n\n::INSERT ACTION::";
                    SqlDataReader sql_Details = new SqlCommand("SELECT * FROM [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Employees] " +
                        " WHERE [empID]=" + newEmpID, myConnection).ExecuteReader();

                    while (sql_Details.Read())
                    {
                        for (int j = 0; j < sql_Details.FieldCount; j++)
                            logData["details"] += "|" + sql_Details.GetName(j) + "=" + sql_Details.GetValue(j);
                    }
                    sql_Details.Close();

                    logData["reason"] = "Create EE";
                    Utility.LogUtils.logEntry(logData);
                    myConnection.Close();

                    errorMsg.Attributes["class"] = "alert alert-success";
                    errorMsg.InnerHtml = "<strong> Success! </strong> " + fname.Text + " " + lname.Text + " has been added.";
                   // Response.Redirect(Page.ResolveUrl("~") + "Products.aspx?eid=" + newEmpID);
                    SendEmail();
                }
            }
            else throw new FormatException();
        }
        catch (SqlException error)
        {
           errorMsg.Attributes["class"] = "alert alert-danger";
             errorMsg.InnerHtml = /*"<strong> Error! </strong> There was an error caused by the data you entered."*/ SQL + error.Message + error.StackTrace;
        }
        catch (FormatException error)
        {
            errorMsg.Attributes["class"] = "alert alert-danger";
            errorMsg.InnerHtml = "<strong> Error! </strong> The date(s) you have entered are invalid.";
        }
        catch (Exception error)
        {
            errorMsg.Attributes["class"] = "alert alert-danger";
            errorMsg.InnerHtml = "<strong> Error! </strong> " + error.Message;// +error.StackTrace;
        }
    }

    protected void AddLocation(object sender, EventArgs e)
    {
        try
        {
            myConnection.Open();
            int count = 1;
            errorDiv.Attributes.CssStyle.Clear();

            string companyName = (string)new SqlCommand("SELECT DISTINCT [location_name] FROM [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Locations] WHERE [location_name]='" + newLocationName.Text.Trim() + "'", myConnection).ExecuteScalar();
            if (String.IsNullOrWhiteSpace(companyName))
            {
                string newLocation = newLocationName.Text.Replace(" ", "");
                if (newLocation.Length < 5)
                    for (int i = 0; i < (5 - newLocationName.Text.Replace(" ", "").Length); i++)
                        newLocation += "_";

                string code = newLocation.Substring(0, 5).ToUpper();
                myReader = new SqlCommand("SELECT DISTINCT [location_code] FROM [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Locations] WHERE [location_code]='" + code + "'", myConnection).ExecuteReader();
                while (myReader.HasRows)
                {
                    code = code.Remove(code.Length - Utility.MiscUtils.getDigitCount(count), 1) + count++;
                    myReader.Dispose();
                    myReader = new SqlCommand("SELECT DISTINCT [location_code] FROM [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Locations] WHERE [location_code]='" + code + "'", myConnection).ExecuteReader();
                }

                if (!myReader.HasRows)
                {
                    SQL = "INSERT INTO [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Locations]([location_name],[location_code])" +
                            " VALUES('" + newLocationName.Text + "','" + code + "')";
                    //Response.Write(SQL); 
                    myReader.Close();
                    myCommand = new SqlCommand(SQL, myConnection);
                    if (myCommand.ExecuteNonQuery() == 1)
                    {
                        locationErrorMsg.Text += "Created.";

                        location.Items.Add(new ListItem(newLocationName.Text, newLocationName.Text));

                        //Response.Redirect("Add.aspx");
                    }
                    else throw new Exception("There was an error while adding this Company.");
                }
            }
            else throw new Exception("This location already exists.");
            myConnection.Close();
        }
        catch (Exception error)
        {
            locationErrorMsg.Text = error.Message;
        }
    }

    private void SendEmail()
    {
        try
        {
            myConnection.Open();
            var rng = new Random(); var agentName = "";
            var email="";
          //  Response.Write("\n SELECT  [acct_manager],[acct_mgr_email] FROM [" + Session["currentAgency"] + "].[general].[Groups] where [short]='" + Session["group"].ToString() + "'");

            string subject = Session["group"] + ": New Employee Added";
            string message = "A new employee has been added to the group: " + Session["group"] +
                ". First Name=" + fname.Text.Replace(',', '.').Trim() + ", Last Name=" + lname.Text.Replace(',', '.').Trim() + ", Home Phone:" +
                homep.Text + ", Work Phone:" + workp.Text + ", Date of hire:" + doh.Text + ", Effective date:"+effective_date.Text+".<br/><br/>" +
                 "---THIS IS AN AUTOMATICALLY GENERATED MESSAGE. PLEASE DO NOT REPLY TO THIS MESSAGE AS WE ARE UNABLE TO RESPOND FROM THIS ADDRESS---";

            bool success = Utility.MiscUtils.SendEmailToAccountManager(subject, message, myConnection, Session["group"].ToString(), Session["currentAgency"].ToString());

            //myReader = new SqlCommand("SELECT  [acct_manager],[acct_mgr_email] FROM [" + Session["currentAgency"] + "].[general].[Groups] where [short]='" + Session["group"].ToString()+"'", myConnection).ExecuteReader();
            //while (myReader.Read())
            //{
            //    agentName = myReader["acct_manager"].ToString();
            //    email = myReader["acct_mgr_email"].ToString();
            //   // Response.Write("Email: "+email + "AgentName: " + agentName);
            //}
            //if (email != "")
            //{
            //    //var fromAddress = new MailAddress("info@ventureuservers.com", "SITUS Administration");
            //    ////var fromAddress = new MailAddress("customerservice@tandsbenefits.com", agentName);
            //    //var toAddress = new MailAddress(email, agentName);
            //    //const string fromPassword = "showmeins$2011";

            //    //var smtp = new SmtpClient
            //    //{
            //    //    Host = "smtp.gmail.com",
            //    //    Port = 587,
            //    //    EnableSsl = true,
            //    //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    //    UseDefaultCredentials = false,
            //    //    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            //    //};

            //    //var message = new MailMessage(fromAddress, toAddress);
            //    //message.Subject = Session["group"]+": New Employee Added";
            //    //message.Body = "A new employee has been added to the group: " + Session["group"] +
            //    //    ". First Name=" + fname.Text.Replace(',', '.').Trim() + ", Last Name=" + lname.Text.Replace(',', '.').Trim()+", Home Phone:"+
            //    //    homep.Text+", Work Phone:"+workp.Text+", Date of hire:"+doh.Text+".<br/><br/>" +
            //    //     "---THIS IS AN AUTOMATICALLY GENERATED MESSAGE. PLEASE DO NOT REPLY TO THIS MESSAGE AS WE ARE UNABLE TO RESPOND FROM THIS ADDRESS.---";
            //    //message.IsBodyHtml = true;
            //    //smtp.Send(message);
            //}
            
            //Response.Write("<div style='width:100%; height:100%; background:#0a0;'>Document has been sent!</div>");
            //Response.Write("<script language='javascript'>alert('An email has been sent to the address on file.'); window.location('Login.aspx');</script>");
            myConnection.Close();
        }
        catch (Exception ex)
        {
            Response.Write("<script language='javascript'>alert('Error: " + ex.Message + "')</script>");
        }
        finally
        {
            myConnection.Close();
        }
    }
}