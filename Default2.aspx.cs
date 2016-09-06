using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

public partial class Default2 : System.Web.UI.Page
{
    SqlConnection myConnection = new SqlConnection(Utility.Constants.ConnectionString);

    SqlDataReader myReader = null;
    SqlCommand myCommand = null;


    List<string> names = new List<string>();
    string SQL = null;
    bool hasJC = false;
    bool hasED = false;
    bool hasCED = false;

    protected void Page_Load(object sender, EventArgs e)
    {
        Session.Clear();



        Session["currentAgency"] = Request["agency"];
        Session["group"] = Request["group"];
        Session["title"] = Session["group"] + " - Add Employees";


        //Response.Write(

        myConnection.Open();
        // groupMenu.SQLConnectionString = SiteMaster.MyGlobals.connectionString;
        try
        {

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
            
            Response.Write(error.ToString() );
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
                "','" + bday.ToShortDateString() + "','" + doh.Text + "','" + "Employee" + "','" + location.SelectedValue + "','" +
                "','" + payGroup.SelectedValue + "'," + salary.Text.Replace(",", "").Replace("$", "") + ",'" + occupation.Text + "'," + "40" + ",'" + homep.Text + "','" + homep.Text + "','" + workp.Text + "','" + email.Text +
                "','" + effective_date.Text + "','pending','no')\n";

                if (hasCED)
                    SQL = SQL.Insert(SQL.LastIndexOf(")"), ",'" + Utility.SQLUtils.getSingleSQLData("SELECT [effective_date] FROM [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Products] WHERE [code]='COMBI_LIF'") + "'");
                if (!String.IsNullOrWhiteSpace(Request["ext_EmpID"]))
                    SQL = SQL.Insert(SQL.LastIndexOf(")"), ",'" + Request["ext_EmpID"] + "'");
                if (hasJC)
                    SQL = SQL.Insert(SQL.LastIndexOf(")"), ",'" + jobclass.Text + "'");

                SQL += " SELECT SCOPE_IDENTITY();";
                Response.Write(SQL);

                newEmpID = (Decimal)new SqlCommand(SQL, myConnection).ExecuteScalar();

                if (newEmpID != 0)
                {
                    Session["details"] += "\n\n::INSERT ACTION::";
                    SqlDataReader sql_Details = new SqlCommand("SELECT * FROM [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Employees] " +
                        " WHERE [empID]=" + newEmpID, myConnection).ExecuteReader();

                    while (sql_Details.Read())
                    {
                        for (int j = 0; j < sql_Details.FieldCount; j++)
                            Session["details"] += "|" + sql_Details.GetName(j) + "=" + sql_Details.GetValue(j);
                    }
                    sql_Details.Close();

                    SQL = "INSERT INTO [" + Session["currentAgency"] + "].[" + Session["group"] + "].[MainLog] " +
                         "VALUES ('" + User.Identity.Name + "','" + DateTime.Now.ToLongDateString() + "','AddEE','" + newEmpID + "','Add New Employee','" + Session["details"] + "') ";
                    Response.Write(SQL);
                    new SqlCommand(SQL, myConnection).ExecuteNonQuery();
                    myConnection.Close();

                    errorMsg.Attributes["class"] = "alert alert-success";
                    errorMsg.InnerText = "<strong> Success! </strong> " + fname.Text + " " + lname.Text + " has been added.";
                    Response.Redirect(Page.ResolveUrl("~") + "Products.aspx?eid=" + newEmpID);
                }
            }
            else throw new FormatException();
        }
        catch (SqlException error)
        {
            errorMsg.Attributes["class"] = "alert alert-danger";
            errorMsg.InnerHtml = "<strong> Error! </strong> There was an error caused by the data you entered."; // + SQL + error.Message + error.StackTrace;
        }
        catch (FormatException error)
        {
            errorMsg.Attributes["class"] = "alert alert-danger";
            errorMsg.InnerHtml = "<strong> Error! </strong> The date(s) you have entered are invalid.";
        }
        catch (Exception error)
        {
            errorMsg.Attributes["class"] = "alert alert-danger";
            errorMsg.InnerHtml = "<strong> Error! </strong> " + error.Message + error.StackTrace;
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
}