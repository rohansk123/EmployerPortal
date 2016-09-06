using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.IO;
using System.Text;

public partial class UploadEmployeeFile : System.Web.UI.Page
{
    SqlConnection myConnection = new SqlConnection(Utility.Constants.ConnectionString);
    SqlDataReader myReader = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!Page.IsPostBack)
            downloadBtn.Visible = false;

        Session["title"] = Session["group"] + " - Upload Census Data to Situs";

         if (Session["currentAgency"] == null || Session["group"] == null)
              Response.Redirect(Page.ResolveUrl("~"));
       // groupName.Text = (string)Session["title"];
        myConnection.Open();
       // groupMenu.SQLConnectionString = SiteMaster.MyGlobals.connectionString;
        myConnection.Close();
       // ExampleLink.Text = "<a href='" + Page.ResolveUrl("~") + "Documents/templates/upload_example.csv'>Example 2 Link</a>";
    }

    protected void uploadTerminations(object sender, EventArgs e)
    {
        int rowCount = 0;
        int eeCount = 0;
        string line = null;
        string uploadStatus = "";
        bool columnHeaders = true;
        string SQL = "";
        List<string> errorCol= new List<string>();
        int lnameCol = -1;
        int fnameCol = -1;
        int locationCol = -1;
        int RNCol = -1;
        int AddressCol = -1;
        int CityCol = -1;
        int StateCol = -1;
        int ZipCol = -1;
        int birthdateCol = -1;
        int hiredateCol = -1;
        int ssnCol = -1;
        int relationshipCol = -1;
        int paygroupCol = -1;
        int emailCol = -1;
        int salaryCol = -1;
        int sexCol = -1;
        int homepCol = -1;
        int termDateCol = -1;
        int covEndDateCol = -1;
        int reasonCol = -1;
        int commentsCol = -1;
        int effectiveDateCol = -1;
        int jobClassCol = -1;
        int empID = -1;
        TerminatedEmployee temp;
        List<TerminatedEmployee> terminatedEmployees = new List<TerminatedEmployee>();

        try
        {
            myConnection.Open();
            // *** CHECK IF EMPLOYEE CENSUS HAS BEEN UPLOADED ***
            if ((terminationsFile.PostedFile != null) && (terminationsFile.PostedFile.ContentLength > 0))
            {
                string fn = Path.GetFileName(terminationsFile.PostedFile.FileName);
                string SaveLocation = Utility.MiscUtils.getFolderPath("Documents") + @"\" + Session["group"] + @"\EmployerPortal\" + HttpContext.Current.User.Identity.Name +@"\Uploads\"+ fn;
                Directory.CreateDirectory(Utility.MiscUtils.getFolderPath("Documents") + @"\" + Session["group"] + @"\EmployerPortal\" + HttpContext.Current.User.Identity.Name + @"\Uploads\");
                terminationsFile.PostedFile.SaveAs(SaveLocation);
                uploadStatus += "The file '" + fn + "' has been uploaded.";

                using (StreamReader readFile = new StreamReader(SaveLocation))
                {
                    string[] row;


                 
                    while ((line = readFile.ReadLine()) != null)
                    {
                        temp = new TerminatedEmployee();
                        errorCol = new List<string>();
                        foreach (char a in line)
                            if (a == '\'')
                                line = line.Insert(line.IndexOf(a), "'");

                        row = line.Split(',');
                       
                        //Response.Write("TEST :" + String.Join("|", row) + "<br/>");
                       // if (line.ToLower().Trim().Contains("lname") || line.ToLower().Trim().Contains("last name"))
                        if(columnHeaders)
                        {
                            //Response.Write("HEADERS: <br/>");
                            for (int i = 0; i < row.Length; i++)
                            {
                                if (row[i].ToLower().Trim() == "lname" || row[i].ToLower().Trim() == "last name" || row[i].ToLower().Trim() == "last_name") lnameCol = i;
                                if (row[i].ToLower().Trim() == "fname" || row[i].ToLower().Trim() == "first name" || row[i].ToLower().Trim() == "first_name") fnameCol = i;
                                if (row[i].ToLower().Trim() == "date of birth" || row[i].ToLower().Trim() == "dob") birthdateCol = i;
                                if (row[i].ToLower().Trim() == "ssn" || row[i].ToLower().Trim() == "soc_sec") ssnCol = i;
                                if (row[i].ToLower().Trim() == "termination date") termDateCol = i;
                                if (row[i].ToLower().Trim() == "coverage end date" ) covEndDateCol = i;
                                if (row[i].ToLower().Trim() == "reason" || row[i].ToLower().Trim() == "occupation") reasonCol = i;
                                if (row[i].ToLower().Trim() == "comments") commentsCol = i;
                             }

                            if(ssnCol == -1)
                                  throw new Exception("SSN column is not present");
                            else if(birthdateCol==-1)
                                    throw new Exception("Birthdate column is not present");                            
                            else if(termDateCol==-1)
                                   throw new Exception("Termination date column is not present");                            
                            else if(covEndDateCol==-1)
                                throw new Exception("Coverage end date column is not present");                            
                            else if(reasonCol==-1)
                                throw new Exception("Reason column is not present");
                            else if (commentsCol==-1)
                                throw new Exception("Comments column is not present");

                            columnHeaders = false;
                            continue;
                        }
                        

                        string ssn = "";
                        string dob = "";
                        //Response.Write(row[ssnCol]+"RowSSNlength:" + row[ssnCol].Length);
                        if (row[ssnCol].Length > 0)
                        {
                            rowCount++;

                            string columnsList = "";
                            string valuesList = "";

                            if (lnameCol != -1 && row[lnameCol].Trim() != "")
                            {
                         
                                temp.LName = Utility.MiscUtils.UppercaseFirst(row[lnameCol]).Trim();
                            }
                            else
                                errorCol.Add("lname");

                            if (fnameCol != -1 && row[fnameCol].Trim() != "")
                            {
                                temp.FName = Utility.MiscUtils.UppercaseFirst(row[fnameCol]).Trim();
                            }
                            else
                            {
                                errorCol.Add("fname");
                               
                               
                            }
                                                        
                                temp.SSN = ssn = Utility.MiscUtils.formatSSN(row[ssnCol]);
                            
                             
                                    temp.DOB = dob = Utility.MiscUtils.formatDate("MM/dd/yyyy", row[birthdateCol]);

                                    temp.TerminationDate = Utility.MiscUtils.formatDate("MM/dd/yyyy", row[termDateCol]);


                                    temp.CoverageEndDate = Utility.MiscUtils.formatDate("MM/dd/yyyy", row[covEndDateCol]);
                           
                           
                            
                                temp.Reason = row[reasonCol];
                            
                           
                               
                                temp.Comments = row[commentsCol];
                           
                            
                   
                            terminatedEmployees.Add(temp);
                        }

                       

                        //foreach (string test in row)
                        //    uploadStatus += "| " + test + " |";
                        //uploadStatus += "<br/>";
                       
                        
                    }
                }
                //Response.Write("TEmployees count:"+terminatedEmployees.Count);

               terminatedEmployees=TerminateEmployees(ref terminatedEmployees);
               CreateLogCSV(terminatedEmployees);
                //errorMsg.Text += uploadStatus;
            }
            //if ((eeCount != rowCount) && (errorCol.Count == 0))
            //{
            //    Response.Write("eecount: " + eeCount + " rowcount: " + rowCount);
            //    throw new Exception("Employee data was not uploaded correctly.");
            //}
            myConnection.Close();
        }
        catch (Exception error)
        {
            Response.Write(error.ToString());
            errorMsg.Text += "<div class='alert alert-danger'> \n" +
                                       "    <button class='close' data-dismiss='close'>×</button> \n" +
                                       "    <strong>Error!</strong> " + error.Message + /*error.StackTrace +*/ " <br/> \n" +
                                        line + " <br/> \n";
            if (!String.IsNullOrWhiteSpace(SQL))
                errorMsg.Text += "SQL: " + SQL;
            errorMsg.Text += "</div> \n";
        }

    }

    private void CreateLogCSV(List<TerminatedEmployee> terminatedEmployees)
    {

       string csvPath = (string)Utility.MiscUtils.getFolderPath("Documents") + @"/"+Session["group"]+@"/EmployerPortal/" + HttpContext.Current.User.Identity.Name;
       string csvFile=   "TerminateEmployeesLog"+"_" +DateTime.Now.ToString(@"yyyy-MM-dd_HH-mm") + @".csv";
       Directory.CreateDirectory(csvPath);
       var csv = new StringBuilder();
       string newLine,colHeaders = "First name, Last name, Date of birth, SSN, Termination Date, Coverage End Date, Reason, Comments, Status";
       csv.AppendLine(colHeaders);

       foreach (TerminatedEmployee temp in terminatedEmployees)
       {
           newLine = temp.FName + "," + temp.LName + "," + temp.DOB + "," + temp.SSN + "," + temp.TerminationDate + "," + temp.CoverageEndDate + "," + temp.Reason + "," + temp.Comments + "," + temp.Status;
           csv.AppendLine(newLine);
       }

       //errorMsg.Text += csv;
       //Response.Write(csvPath + @"/" + csvFile);
       File.WriteAllText(csvPath + @"/" + csvFile, csv.ToString());
       downloadBtn.Visible = true;
      // downloadBtn.Click += new EventHandler(btnDownload_Click);
        downloadBtn.CommandArgument= csvFile;
       terminateFileDiv.Visible = false;
       SendLogToAccountManager(csvPath, csvFile);

      // downloadBtn.PostBackUrl =  Page.ResolveUrl("~") + "Documents/"+ Session["group"]+@"/EmployerPortal/" + HttpContext.Current.User.Identity.Name+ @"/" + csvFile;
    }

    private void SendLogToAccountManager(string csvPath, string csvFile)
    {
        string emailText = "Please find attached file which contains the details of the employees that were terminated.<br/><br/><strong>*** THIS IS AN AUTOMATICALLY GENERATED MESSAGE BASED ON A USER ACTION. DO NOT REPLY TO THIS MESSAGE SINCE THIS INBOX IS NOT MONITORED ***";
        if (Utility.MiscUtils.SendEmailToAccountManagerWithAttachments("Termination file uploaded by user:" + HttpContext.Current.User.Identity.Name, csvPath + @"/" + csvFile, emailText, myConnection, Session["group"].ToString(), Session["currentAgency"].ToString()))
            errorMsg.Text = "<div class='alert alert-success'> \n" +
                                       "    <button class='close' data-dismiss='close'>×</button> \n" +
                                       "    <strong>Success!</strong> Your file was uploaded successfully. You can download the log file to check which employees were successfuly terminated by clicking on the 'Download Log' button. A copy of the log file was sent to the account manager. <br/> \n</div>";
                                        
    }

   protected void btnDownload_Click(object sender, EventArgs e)
   {
       LinkButton btn = (LinkButton)sender;
       string fileName = btn.CommandArgument;
     //  Response.Write(Page.ResolveUrl("~") + "Documents/" + Session["group"] + @"/EmployerPortal/" + HttpContext.Current.User.Identity.Name + @"/" + fileName);
       Response.Redirect(Page.ResolveUrl("~") + "Documents/" + Session["group"] + @"/EmployerPortal/" + HttpContext.Current.User.Identity.Name + @"/" + fileName);
    }

    private List<TerminatedEmployee> TerminateEmployees(ref List<TerminatedEmployee> terminatedEmployees)
    {
       

        List<TerminatedEmployee> resultTerminatedEmployees = new List<TerminatedEmployee>();

        foreach (TerminatedEmployee temp in terminatedEmployees)
        {
            if (temp.SSN == "")
            {
                temp.Status = "Failed because SSN is absent.";
                resultTerminatedEmployees.Add(temp);
                continue;
            }
            else if (temp.DOB == "")
            {
                temp.Status = "Failed because DOB is absent.";
                resultTerminatedEmployees.Add(temp);
                continue;
            }
            else if (temp.TerminationDate == "")
            {
                temp.Status = "Failed because termination date is absent.";
                resultTerminatedEmployees.Add(temp);
                continue;
            }
            else if (temp.CoverageEndDate == "")
            {
                temp.Status = "Failed because DOB is absent.";
                resultTerminatedEmployees.Add(temp);
            }
            else if (temp.Reason == "" || (temp.Reason != "Employee Death" && temp.Reason != "Active Military Duty" && temp.Reason != "Entitlement to Medicare"
                && temp.Reason != "Resignation" && temp.Reason != "Reduction in Hours" && temp.Reason != "Retirement"))
            {
                temp.Status = "Failed because termination reason is absent or not one of the specified values.";
                resultTerminatedEmployees.Add(temp);
                continue;
            }
            


            temp.EmpID = FindEmployeeID(temp.SSN, temp.DOB).ToString();
            if (temp.EmpID == "0")
            {
                temp.Status = "Failed because employee could not be found in database.";
                resultTerminatedEmployees.Add(temp);
                continue;
            }
          //  if(!String.IsNullOrEmpty(temp.EmpID))
          if(  Utility.SQLUtils.changeAppStatus("terminated", temp.EmpID))
          {
              bool result;
             // string SQL 
                    if (IfEntryForEmployeeExists(temp.EmpID))
            result=UpdateTerminatedEmployee(temp);
            else
            result=InsertTerminatedEmployee(temp);
             

          
             if (result)
             {
                 Utility.SQLUtils.StopRecurringPayments(temp.EmpID,"Stop recurring payments due to terminate employee file action.");
                 temp.Status = "Termination Successful.";
                resultTerminatedEmployees.Add(temp);
             }
         
           }
        }

        return resultTerminatedEmployees;


    }

    private bool UpdateTerminatedEmployee(TerminatedEmployee temp)
    {

        string SQL = "Update [" + Session["currentAgency"] + "].[" + Session["group"] + "].[TerminatedEmployees]" +
           " SET  [terminationdate]='" + temp.TerminationDate + "', [coverageenddate]='" + temp.CoverageEndDate +
           "',[reason]='" + temp.Reason + "',[comments]='" + temp.Comments + "' " +
               "where[EmpID] = " + temp.EmpID;
        // Response.Write(SQL);
        int update = new SqlCommand(SQL, myConnection).ExecuteNonQuery();
        if (update == 1)
        {
            return true;
        }
        return false;
    }

    private bool InsertTerminatedEmployee(TerminatedEmployee temp)
    {
        //myConnection.Open();
        string SQL = "INSERT INTO [" + Session["currentAgency"] + "].[" + Session["group"] + "].[TerminatedEmployees](EmpID,terminationdate,coverageenddate,reason,comments) " +
                     "VALUES (" + temp.EmpID + ",'" + temp.TerminationDate + "','" + temp.CoverageEndDate + "','" + temp.Reason + "','" + temp.Comments + "') ";
        // Response.Write(SQL);
        int insert = new SqlCommand(SQL, myConnection).ExecuteNonQuery();

        

        
        if (insert == 1)
        {
            return true;
        }
        return false;

    }

    private bool IfEntryForEmployeeExists(string empID)
    {


        string SQL = "SELECT COUNT(*) FROM [" + Session["currentAgency"] + "].[" + Session["group"] + "].[TerminatedEmployees]" +
            " WHERE [EmpID] = " + empID;

        //  string SQL = "SELECT * FROM  [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Employees] WHERE [EMPID] = " + Session["selectedEmpID"];
        //  Response.Write(SQL);
        int empExists = Convert.ToInt32(Utility.SQLUtils.getSingleSQLData(SQL));
        //  Response.Write("empexists:" + empExists);
        if (empExists > 0)
        {
            return true;
        }
        else
            return false;

    }



    private int FindEmployeeID(string ssn, string dob)
    {
       // throw new NotImplementedException();
       // Response.Write("SELECT EMPID FROM [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Employees] WHERE [birthdate]='" + dob + "' and ssn='" + ssn + "'");
        object empID=(Utility.SQLUtils.getSingleSQLData("SELECT EMPID FROM ["+Session["currentAgency"] + "].[" + Session["group"] + "].[Employees] WHERE [birthdate]='"+dob+"' and ssn='"+ssn+"'"));
        if (!string.IsNullOrEmpty(empID.ToString()))
            return Convert.ToInt32(empID);
        else return 0;
    }

    

    
}


class TerminatedEmployee
{
    public string EmpID { get; set; }
    public string SSN { get; set; }
    public string DOB { get; set; }
    public string TerminationDate { get; set; }
    public string FName { get; set; }
    //public float Amount { get; set; }
    public string LName { get; set; }
    public string CoverageEndDate { get; set; }
    public string Reason { get; set; }
    public string Comments { get; set; }
    public string Status { get; set; }
}