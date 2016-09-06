using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.IO;

public partial class UploadEmployeeFile : System.Web.UI.Page
{
    SqlConnection myConnection = new SqlConnection(Utility.Constants.ConnectionString);
    SqlDataReader myReader = null;
    protected void Page_Load(object sender, EventArgs e)
    {

        Session["title"] = Session["group"] + " - Upload Census Data to Situs";

         if (Session["currentAgency"] == null || Session["group"] == null)
              Response.Redirect(Page.ResolveUrl("~"));
       // groupName.Text = (string)Session["title"];
        myConnection.Open();
       // groupMenu.SQLConnectionString = SiteMaster.MyGlobals.connectionString;
        myConnection.Close();
       // ExampleLink.Text = "<a href='" + Page.ResolveUrl("~") + "Documents/templates/upload_example.csv'>Example 2 Link</a>";
    }

    protected void uploadCensus(object sender, EventArgs e)
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
        int workpCol = -1;
        int ext_EmpIDCol = -1;
        int titleCol = -1;
        int checksCol = -1;
        int effectiveDateCol = -1;
        int jobClassCol = -1;

        try
        {
            myConnection.Open();
            // *** CHECK IF EMPLOYEE CENSUS HAS BEEN UPLOADED ***
            if ((censusFile.PostedFile != null) && (censusFile.PostedFile.ContentLength > 0))
            {
                string fn = Path.GetFileName(censusFile.PostedFile.FileName);
                string SaveLocation = Utility.MiscUtils.getFolderPath("Documents") + @"\" + Session["name"] + @"\" + fn;
                censusFile.PostedFile.SaveAs(SaveLocation);
                uploadStatus += "The file '" + fn + "' has been uploaded.";

                using (StreamReader readFile = new StreamReader(SaveLocation))
                {
                    string[] row;

                    while ((line = readFile.ReadLine()) != null)
                    {
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
                                if (row[i].ToLower().Trim() == "location" || row[i].ToLower().Trim() == "loc_name") locationCol = i;
                                if (row[i].ToLower().Trim() == "rn" || row[i].ToLower().Trim() == "record_no") RNCol = i;
                                if (row[i].ToLower().Trim() == "address" || row[i].ToLower().Trim() == "addr") AddressCol = i;
                                if (row[i].ToLower().Trim() == "city") CityCol = i;
                                if (row[i].ToLower().Trim() == "state") StateCol = i;
                                if (row[i].ToLower().Trim() == "zip") ZipCol = i;
                                if (row[i].ToLower().Trim() == "birthdate" || row[i].ToLower().Trim() == "dob") birthdateCol = i;
                                if (row[i].ToLower().Trim() == "hiredate" || row[i].ToLower().Trim() == "doh") hiredateCol = i;
                                if (row[i].ToLower().Trim() == "ssn" || row[i].ToLower().Trim() == "soc_sec") ssnCol = i;
                                if (row[i].ToLower().Trim() == "relationship") relationshipCol = i;
                                if (row[i].ToLower().Trim() == "paygroup" || row[i].ToLower().Trim() == "billing_mode") paygroupCol = i;
                                if (row[i].ToLower().Trim() == "email") emailCol = i;
                                if (row[i].ToLower().Trim() == "salary") salaryCol = i;
                                if (row[i].ToLower().Trim() == "sex" || row[i].ToLower().Trim() == "gender") sexCol = i;
                                if (row[i].ToLower().Trim() == "homep" || row[i].ToLower().Trim() == "phone") homepCol = i;
                                if (row[i].ToLower().Trim() == "workp" || row[i].ToLower().Trim() == "work") workpCol = i;
                                if (row[i].ToLower().Trim() == "ext_empid" || row[i].ToLower().Trim() == "empee_id") ext_EmpIDCol = i;
                                if (row[i].ToLower().Trim() == "title" || row[i].ToLower().Trim() == "occupation") titleCol = i;
                                if (row[i].ToLower().Trim() == "checks") checksCol = i;
                                if (row[i].ToLower().Trim() == "effective_date") effectiveDateCol = i;
                                if (row[i].ToLower().Trim() == "class" || row[i].ToLower().Trim() == "jobclass") jobClassCol = i;
                            }
                            columnHeaders = false;
                            continue;
                        }
                        

                        string gender = "";
                        string ssn = "";
                        string dob = "";
                        string doh = "";

                        if (row.Length > 0)
                        {
                            rowCount++;

                            string columnsList = "";
                            string valuesList = "";

                            if (lnameCol != -1 && row[lnameCol].Trim() != "")
                            {
                                columnsList += "[lname],";
                                valuesList += "'" + Utility.MiscUtils.UppercaseFirst(row[lnameCol]).Trim() + "',";
                            }
                            else
                                errorCol.Add("lname");

                            if (fnameCol != -1 && row[fnameCol].Trim() != "")
                            {
                                columnsList += "[fname],";
                                valuesList += "'" + Utility.MiscUtils.UppercaseFirst(row[fnameCol]).Trim() + "',";
                                
                            }
                            else
                            {
                                errorCol.Add("fname");
                               
                               
                            }
                            if (locationCol != -1)
                            {
                                columnsList += "[location],";
                                valuesList += "'" + row[locationCol] + "',";
                            }
                            if (RNCol != -1)
                            {
                                columnsList += "[RN],";
                                valuesList += "'" + row[RNCol] + "',";
                            }
                            if (AddressCol != -1)
                            {
                                columnsList += "[Address],";
                                valuesList += "'" + row[AddressCol] + "',";
                            }
                            if (CityCol != -1)
                            {
                                columnsList += "[City],";
                                valuesList += "'" + Utility.MiscUtils.UppercaseFirst(row[CityCol]) + "',";
                            }
                            if (StateCol != -1)
                            {
                                columnsList += "[State],";
                                valuesList += "'" + row[StateCol] + "',";
                            }
                            if (ZipCol != -1)
                            {
                                columnsList += "[Zip],";
                                valuesList += "'" + row[ZipCol] + "',";
                            }
                            if (birthdateCol != -1)
                            {
                                if (!String.IsNullOrWhiteSpace(row[birthdateCol]) && row[birthdateCol].Length > 1)
                                {
                                    columnsList += "[birthdate],";
                                    dob = Utility.MiscUtils.formatDate("MM/dd/yyyy", row[birthdateCol]);
                                    valuesList += "'" + dob + "',";
                                }
                                else
                                {
                                    errorCol.Add("birthdate");
                                }
                               
                            }
                            else
                            {
                                errorCol.Add("birthdate");
                            }

                            
                           
                            if (hiredateCol != -1)
                            {
                                if (!String.IsNullOrWhiteSpace(row[hiredateCol]) && row[hiredateCol].Length > 1)
                                {
                                    columnsList += "[hiredate],";
                                    doh = Utility.MiscUtils.formatDate("MM/dd/yyyy", row[hiredateCol]);
                                    valuesList += "'" + doh + "',";
                                }
                                else
                                {
                                    errorCol.Add("hiredate");
                                }

                            }
                            else
                            {
                                errorCol.Add("hiredate");
                            }

                            if (ssnCol != -1 && row[ssnCol].Trim() != "")
                            {
                                columnsList += "[ssn],";
                                ssn = Utility.MiscUtils.formatSSN(row[ssnCol]);
                                valuesList += "'" + ssn + "',";
                            }
                            else
                            {
                                errorCol.Add("ssn");                              

                            }
                            if (relationshipCol != -1)
                            {
                                columnsList += "[relationship],";
                                valuesList += "'" + row[relationshipCol] + "',";
                            }
                            else
                            {
                                columnsList += "[relationship],";
                                valuesList += "'Employee',";
                            }
                            if (paygroupCol != -1)
                            {
                                columnsList += "[paygroup],";
                                valuesList += "'" + row[paygroupCol] + "',";
                            }
                            else
                            {
                                columnsList += "[paygroup],";
                                valuesList += "'" + Session["billing_mode"] + "',";
                            }
                            if (emailCol != -1)
                            {
                                columnsList += "[email],";
                                valuesList += "'" + row[emailCol] + "',";
                            }
                            if (salaryCol != -1)
                            {
                                columnsList += "[salary],";
                                valuesList += "'" + row[salaryCol] + "',";
                            }
                            if (sexCol != -1)
                            {
                                columnsList += "[sex],";
                                gender = Utility.MiscUtils.UppercaseFirst(row[sexCol]);
                                if (Utility.MiscUtils.UppercaseFirst(row[sexCol]) == "M")
                                    gender = "Male";
                                if (Utility.MiscUtils.UppercaseFirst(row[sexCol]) == "F")
                                    gender = "Female";
                                valuesList += "'" + gender + "',";
                            }
                            if (homepCol != -1)
                            {
                                columnsList += "[homep],";
                                valuesList += "'" + row[homepCol] + "',";
                            }
                            if (workpCol != -1)
                            {
                                columnsList += "[workp],";
                                valuesList += "'" + row[workpCol] + "',";
                            }
                            if (ext_EmpIDCol != -1)
                            {
                                columnsList += "[ext_EmpID],";
                                valuesList += "'" + row[ext_EmpIDCol] + "',";
                            }
                            if (titleCol != -1)
                            {
                                columnsList += "[title],";
                                valuesList += "'" + row[titleCol] + "',";
                            }
                            if (checksCol != -1)
                            {
                                columnsList += "[checks],";
                                valuesList += "'" + row[checksCol] + "',";
                            }
                            if (jobClassCol != -1)
                            {
                                columnsList += "[jobclass],";
                                valuesList += "'" + row[jobClassCol] + "',";
                            }
                            if (effectiveDateCol != -1 && !String.IsNullOrWhiteSpace(row[effectiveDateCol]))
                            {
                                columnsList += "[effective_date],";
                                valuesList += "'" + row[effectiveDateCol] + "',";
                            }
                            else if (!String.IsNullOrWhiteSpace((string)Session["effective_date"]))
                            {
                                columnsList += "[effective_date],";
                                valuesList += "'" + Session["effective_date"] + "',";
                            }
                            //Response.Write(columnsList + " : " + valuesList+ "<br/>");
                            //Response.Write(columnsList.LastIndexOf(",") + " : " + columnsList.Length + "<br/>");
                            //columnsList = columnsList.Remove(columnsList.LastIndexOf(","), 1);
                            //valuesList = valuesList.Remove(valuesList.LastIndexOf(","), 1);
                            columnsList += "[hpw],[app_status],[archive]";
                            valuesList += "40,'pending','no'";

                            SQL = "INSERT INTO [" + Session["currentAgency"] + "].[" + Session["group"] + "].[Employees] (" + columnsList + ") " +
                                "VALUES (" + valuesList + ")";
                        }

                        //Response.Write("<br/>" + Session["currentAgency"]);
                       // uploadStatus += "<br/>|" + SQL + "||";
                        //Response.Write("<br/>|" + SQL + "||");
                        if (errorCol.Count == 0)
                        {
                            if (new SqlCommand(SQL, myConnection).ExecuteNonQuery() == 1)
                            {
                                eeCount++;
                                uploadStatus += "<br/> The following row was successfully uploaded: ";
                            }
                        }
                        else
                        {
                            eeCount++;
                            uploadStatus += "<br/> The following row failed to upload because the column(s) <strong>' ";
                            for (int i = 0; i < errorCol.Count; i++)
                            {
                                uploadStatus += errorCol[i];
                                if (i < (errorCol.Count - 1))
                                    uploadStatus += ", ";
                            }
                            uploadStatus += "'</strong> are absent or empty: ";
                        }  

                                //+ errorCol + " is absent or empty.";

                        foreach (string test in row)
                            uploadStatus += "| " + test + " |";
                        uploadStatus += "<br/>";

                    }
                }
                errorMsg.Text += uploadStatus;
            }
            if ((eeCount != rowCount) && (errorCol.Count == 0))
            {
                Response.Write("eecount: " + eeCount + " rowcount: " + rowCount);
                throw new Exception("Employee data was not uploaded correctly.");
            }
            myConnection.Close();
        }
        catch (Exception error)
        {
            Response.Write(error.ToString());
            errorMsg.Text += "<div class='alert alert-error'> \n" +
                                       "    <button class='close' data-dismiss='close'>×</button> \n" +
                                       "    <strong>Error!</strong> " + error.Message + error.StackTrace + " <br/> \n" +
                                        line + " <br/> \n";
            if (!String.IsNullOrWhiteSpace(SQL))
                errorMsg.Text += "SQL: " + SQL;
            errorMsg.Text += "</div> \n";
        }

    }
}