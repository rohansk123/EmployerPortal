using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.IO;

public partial class Grid : SitusPage
{
    SqlConnection myConnection = new SqlConnection(Utility.Constants.ConnectionString);
       
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!String.IsNullOrWhiteSpace(Request["error"]) && Request["error"] == "1")
            ClientScript.RegisterClientScriptBlock(this.GetType(), "Error", "alert('An error has ocurred during enrollment.<br><br> Please try again.');", true);

        //*** CREATE THE SQL CONNECTION ***
        String employeeID="";
        if (!String.IsNullOrWhiteSpace(Request["EID"]) && Request["EID"].ToLower() != "n/a" && doesEmployeeExist(Request["EID"]))
        {
            Session["selectedEmpID"]=        Request["EID"];
        }
        if (Session["selectedEmpID"] != null)
            employeeID = (string)Session["selectedEmpID"];
        myConnection.Open();
        String username = (string)Session["clientname"];
       // String employeeID = (string)Session["empID"];
        String agency = (string)Session["currentAgency"];
        String group = (string)Session["group"];
        SqlDataReader myReader = null;
        SqlCommand myCommand = null;
        Session["agentLicense"] = null;
        //Response.Write(username);
        string statusCheck=""; string bNameCleared="";
        double currentTotal = 0;
        double futureTotal = 0;
        double er_CurrentTotal = 0;
        double er_FutureTotal = 0;
        bool isArchived = true;
        string location = "", jobClass = "";

        try
        {
            if(employeeID!="")
            {
          //  SiteMaster.MyGlobals.myConnection.Close();

            //throw new Exception("SELECT * FROM [" + agency + "].[" + group + "].Employees  WHERE [empID]=" + employeeID);
            myReader = new SqlCommand("SELECT * FROM [" + agency + "].[" + group + "].Employees  WHERE [empID]=" + employeeID, myConnection).ExecuteReader();

            while (myReader.Read())
            {
                if (String.IsNullOrWhiteSpace(myReader["ssn"].ToString()) || myReader["ssn"].ToString().Length != 11)
                    throw new Exception("This employee's <strong>SSN</strong> is either invalid or has not been set.");

                //if (String.IsNullOrWhiteSpace(myReader["salary"].ToString()) || Convert.ToDouble(myReader["salary"].ToString()) <= 0)
                //    throw new Exception("This employee's <strong>salary</strong> has not been set.");

                //if (String.IsNullOrWhiteSpace(myReader["homep"].ToString()))
                //    throw new CensusException("This employee's <strong>phone number</strong> has not been set.");
                Session["selectedEmployeeName"] = myReader["fname"] + " " + myReader["lname"];
                bNameCleared = Utility.MiscUtils.createFileString(myReader["lname"].ToString().Trim() + " " + myReader["fname"].ToString().Trim());
                if (!String.IsNullOrWhiteSpace(myReader["birthDate"].ToString()))
                    Session["selectedBirthDate"] = myReader["birthdate"];
                else
                    Session["selectedBirthDate"] = "Unknown";
                if (myReader["archive"].ToString() == "no")
                    isArchived = false;
                statusCheck = myReader["app_status"].ToString();
                location = myReader["location"].ToString();
                jobClass = myReader["jobclass"].ToString();
            }

            // *** GET LIST OF CURRENT DEDUCTIONS ***
            if (!isArchived)
            {
                List<List<string>> currentDeductions = new List<List<string>>();
                string SQL = "SELECT * FROM [" + agency + "].[" + group + "].[CurrentDeductions] WHERE employee_ID=" + employeeID + "  ORDER BY tier DESC";
                //Response.Write(SQL);
                myCommand = new SqlCommand(SQL, myConnection);
                myReader = myCommand.ExecuteReader();
                while (myReader.Read())
                {
                    List<string> deduction = new List<string>();
                    deduction.Add(myReader["product_name"].ToString()); // index : 0
                    deduction.Add(myReader["code"].ToString()); // index : 1
                    deduction.Add(myReader["coverage_level"].ToString()); // index : 2
                    deduction.Add(myReader["face_value"].ToString()); // index : 3
                    deduction.Add(myReader["tier"].ToString()); // index : 4
                    deduction.Add(myReader["amount"].ToString()); // index : 5
                    deduction.Add(myReader["pre_tax"].ToString()); // index : 6
                    deduction.Add("0"); // index : 7

                    currentDeductions.Add(deduction);
                }
                myReader.Close();

                SQL = "SELECT enrollment_type, product_name, code, product_code, date, company_name, face_value, t.[effective_date], coverage_level, tier, pre_tax, deduction_amt, year,PDF, t.[ID] AS t_ID, p.[ID] AS p_ID,p.[order], " +
                    //" (SELECT CASE \n WHEN EXISTS (SELECT 1 FROM [" + agency + "].INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = p.code + '_RT' AND TABLE_SCHEMA = '" + group + "' AND (COLUMN_NAME = 'age_low' OR COLUMN_NAME = 'age_high' OR COLUMN_NAME = 'age')) \n THEN 1 \n ELSE 0 \n END) AS hasAgeBand, \n " +
                    //" (SELECT CASE \n WHEN EXISTS (SELECT 1 FROM [" + agency + "].INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = p.code + '_RT' AND TABLE_SCHEMA = '" + group + "' AND COLUMN_NAME = 'face_value')\n THEN 1\n ELSE 0\n END) AS hasFaceValue, \n " +
                    //" (SELECT CASE \n WHEN EXISTS (SELECT 1 FROM [" + agency + "].INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = p.code + '_RT' AND TABLE_SCHEMA = '" + group + "' AND COLUMN_NAME = 'salary')\n THEN 1\n ELSE 0\n END) AS hasSalary \n " +
" (SELECT CASE \n WHEN EXISTS (SELECT 1 FROM [" + agency + "].INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = p.code + '_RT' AND TABLE_SCHEMA = '" + group + "' AND COLUMN_NAME = 'ee_or_spouse')\n THEN 1\n ELSE 0\n END) AS hasSpouseRates \n " +
                  "FROM [" + agency + "].[" + group + "].[Products] AS p " +
                  "LEFT JOIN [" + agency + "].[" + group + "].[Transactions] AS t ON t.[product_code]= p.[code] " +
                  "WHERE employee_ID='" + employeeID + "' AND t.product_code = p.code AND t.old_transaction = 'no'" +
                  "UNION ALL " +
                  "SELECT enrollment_type, product_name, code, null product_code, null date, null company_name, null face_value, null effective_date, null coverage_level, null tier, null pre_tax, null deduction_amt, null year, null PDF, null t_ID, [ID] AS p_ID,p.[order], " +
                    //" (SELECT CASE \n WHEN EXISTS (SELECT 1 FROM [" + agency + "].INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = p.code + '_RT' AND TABLE_SCHEMA = '" + group + "' AND (COLUMN_NAME = 'age_low' OR COLUMN_NAME = 'age_high' OR COLUMN_NAME = 'age')) \n THEN 1 \n ELSE 0 \n END) AS hasAgeBand, \n " +
                    //" (SELECT CASE \n WHEN EXISTS (SELECT 1 FROM [" + agency + "].INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = p.code + '_RT' AND TABLE_SCHEMA = '" + group + "' AND COLUMN_NAME = 'face_value')\n THEN 1\n ELSE 0\n END) AS hasFaceValue, \n " +
                    //" (SELECT CASE \n WHEN EXISTS (SELECT 1 FROM [" + agency + "].INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = p.code + '_RT' AND TABLE_SCHEMA = '" + group + "' AND COLUMN_NAME = 'salary')\n THEN 1\n ELSE 0\n END) AS hasSalary \n " +
" (SELECT CASE \n WHEN EXISTS (SELECT 1 FROM [" + agency + "].INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = p.code + '_RT' AND TABLE_SCHEMA = '" + group + "' AND COLUMN_NAME = 'ee_or_spouse')\n THEN 1\n ELSE 0\n END) AS hasSpouseRates \n " +
                  "FROM [" + agency + "].[" + group + "].[Products] AS p " +
                  "WHERE [code] not in " +
                  "(select [product_code] FROM [" + agency + "].[" + group + "].[Transactions] WHERE employee_ID='" + employeeID + "' AND old_transaction = 'no' AND tier!='Spouse' ) ORDER BY  [order], tier DESC";
                //Response.Write(SQL);
                //throw new Exception(SQL);
                myReader = new SqlCommand(SQL, myConnection).ExecuteReader();
                int height = 0;
                int er_Height = 0;
                //List<Dictionary<string, string>> DHTMXStatement = new List<Dictionary<string, string>>();
                List<string[]> DHTMXStatement = new List<string[]>();
                List<string[]> er_DHTMXStatement = new List<string[]>();
                Session["spouseRows"] = " ";
                // *** SET-UP PRODUCTS TABLE ***

                string header = "<div class='widget-content nopadding'> \n" +
                                "   <table class='table table-condensed table-bordered'> \n" +
                                "	    <thead> \n";

                if (statusCheck == "terminated")
                {
                   string date=  FindTerminationDetails(employeeID);


                    header += "<tr><th colspan='5' style='text-align:center;font-size:14px;color:red'>This employee has already been terminated on termination date:"+date+"</th></tr>\n";
                }


                header += "<tr><th colspan='5' style='text-align:center;'><span style='font-size:14px'>" + Session["selectedEmployeeName"] + "&nbsp;   DOB: " + Session["selectedBirthDate"] +"</span>"+
                    "<br/> Location: "+location+" Job Class: "+jobClass+    
                    "</th></tr>\n"+           "		    <tr> <th colspan='5'>Benefits Summary</th> </tr> \n" +
                       /*         "               <a class='logout' href='" + Page.ResolveUrl("~") + "Account/Logout.aspx'><i class='icon icon-share-alt'></i> <span class='text'> Logout</span></a> </th> </tr> \n" +*/
                                "           <tr> <th></th> <th name='cl'>Benefit</th> <th name='fv'>Face Value</th> <th name='tier'>Tier</th> <th>"+Session["billing_mode"]+" Premium</th> </tr> \n";
                //"                <th name='cl'>Benefit</th> <th name='fv'>Face Value</th> <th name='tier'>Tier</th> <th>Pre-Tax?</th> <th>Coverage Amount</th> </tr> ";
                MyGrid.Text = header;
                //"document.getElementById('mygrid_container').style.width = (document.getElementById('mygrid_container').style.width) + 'px';";

                while (myReader.Read())
                {
                    // *** PRINT TO BROWSER RAW DATA ***
                    //Response.Write("<table>");
                    //for (int i = 0; i < myReader.FieldCount; i++)
                    //    Response.Write("<tr><td>" + myReader.GetName(i) + "</td><td>" + myReader.GetValue(i) + "</td></tr>");
                    //Response.Write("</table>COLUMNS:" + myReader.FieldCount + "*** END OF ROW ***<p>");

                    //*** PULL PRODUCTS DATA ***

                    string enrollType = myReader["enrollment_type"].ToString();
                    string currentPC = myReader["product_code"].ToString();
                    string futurePC = myReader["code"].ToString();

                    string productName = "";
                    string PDFhref = "";
                    string f_faceValue = "";
                    string f_deductionAmt = "";
                    string f_tier = "";
                    string f_preTax = "";
                    string f_benefitAmt = "";
                    string c_faceValue = "";
                    string c_deductionAmt = "";
                    string c_tier = "";
                    string c_preTax = "";
                    string c_benefitAmt = "";
                    bool inSpouse = false;

                    if ((!String.IsNullOrWhiteSpace(enrollType) && enrollType != "Drop" && enrollType != "Nothing") || (enrollType == "Nothing" && !String.IsNullOrWhiteSpace(myReader["deduction_amt"].ToString())))
                    {
                        if (currentPC == futurePC)
                        {
                            //productName = "<a class=\"yes\" href=\"EnrollApp/confirm.aspx?ID=" + myReader["t_ID"].ToString() + "&useSelerix=" + myReader["UseSelerix"].ToString() + "&codeX=" + myReader["product_name"].ToString() + "\">" + myReader["product_name"].ToString() + "</a>";
                            string href = "EnrollApp/Default.aspx?code=" + myReader["code"].ToString() + "&employeeID=" + employeeID + "&current=yes";
                            href = "#";
                            //if (enrollType == "Nothing")
                            //    href = "";
                            productName = "<a class=\"yes\" href=\"" + href + "\">" + myReader["product_name"].ToString() + "</a>";
                            if (myReader["coverage_level"].ToString() == "--WAIVED--")
                                productName = "<a class=\"no\" href=\"EnrollApp/Default.aspx?code=" + myReader["code"].ToString() + "&employeeID=" + employeeID + "&current=waived\">" + myReader["product_name"].ToString() + "</a>";
                            f_faceValue = "$" + Utility.MiscUtils.toCurrency(myReader["face_value"].ToString(), true).Replace(",", "&#44;");
                            f_deductionAmt = "$" + Utility.MiscUtils.toCurrency(myReader["deduction_amt"].ToString());
                            f_tier = myReader["tier"].ToString().Replace(",", "&#44;");
                            f_preTax = myReader["pre_tax"].ToString();
                            f_benefitAmt = myReader["coverage_level"].ToString().Replace(",", "&#44;");

                            if (!String.IsNullOrWhiteSpace(myReader["deduction_amt"].ToString()))
                            {
                                if (enrollType == "ER Paid")
                                    er_FutureTotal += Convert.ToDouble(myReader["deduction_amt"].ToString());
                                else
                                    futureTotal += Convert.ToDouble(myReader["deduction_amt"].ToString());
                            }

                            if (myReader["tier"].ToString() == "Spouse")
                            {
                                productName = "<a class=\"spouse\" href=\"EnrollApp/Default.aspx?code=" + myReader["code"].ToString() + "&employeeID=" + employeeID + "&current=yes\">" + myReader["product_name"].ToString() + " - Spouse</a>";

                                productName = "<a class=\"yes\" href=\"#\">" + myReader["product_name"].ToString() + " - Spouse</a>";
                                

                                if (myReader["coverage_level"].ToString() == "--WAIVED--")
                                {
                                    productName = "<a class=\"no\" href=\"EnrollApp/Default.aspx?code=" + myReader["code"].ToString() + "&employeeID=" + employeeID + "&current=waived\">" + myReader["product_name"].ToString() + " - Spouse</a>";
                                    productName = "<a class=\"no\" href=\"#\">" + myReader["product_name"].ToString() + " - Spouse</a>";
                                   
                                    f_faceValue = "--WAIVED--";
                                }
                                else if (f_deductionAmt == "$0.00")
                                    f_deductionAmt = "added to EE.";

                                inSpouse = true;
                            }
                            else inSpouse = false;

                            if (myReader["product_name"].ToString().Contains("Spouse Only") && !(bool)Session["hasSpouse"])
                                productName = null;
                        }
                        else
                        {
                            //Response.Write(myReader["product_code"].ToString());
                            //productName = "<a class=\"no\" href=\"EnrollApp/Default.aspx?code=" + myReader["code"].ToString() + "&employeeID=" + employeeID + "&current=no\">" + myReader["product_name"].ToString() + "</a>";\
                            productName = "";
                            productName = "<a class=\"no\" href=\"#\">" + myReader["product_name"].ToString() + "</a>";
                           
                            f_faceValue = " - ";
                            f_deductionAmt = " - ";
                            f_tier = " - ";
                            f_preTax = " - ";
                            f_benefitAmt = " - ";

                            if (myReader["product_name"].ToString().Contains("Spouse Only") && !(bool)Session["hasSpouse"])
                                productName = null;
                            //Response.Write(myReader["product_name"].ToString().Contains("Spouse Only") + ":" + (bool)Session["hasSpouse"] + productName + "<br/>");
                        }

                        if (enrollType == "ER Paid" && er_DHTMXStatement.Count < 1)
                        {
                            er_DHTMXStatement.Add(new string[] { "0", "", "<tr> <th colspan='11'> Employer Paid Benefits </th> </tr> \n" });
                            er_Height++;
                        }
                    }
                    else if (enrollType == "Drop")
                    {
                        productName = "<a class=\"drop\" style=\"color:#666\" href=\"#\">" + myReader["product_name"].ToString() + " - Drop Only</a>";
                        f_faceValue = " - ";
                        f_deductionAmt = " - ";
                        f_tier = " - ";
                        f_preTax = " - ";
                        f_benefitAmt = " - ";
                    }
                    else
                    {
                        productName = "<a class=\"grey\" style=\"color:#666\" href=\"#\">" + myReader["product_name"].ToString() + "</a>";
                        f_faceValue = " - ";
                        f_deductionAmt = " - ";
                        f_tier = " - ";
                        f_preTax = " - ";
                        f_benefitAmt = " - ";
                    }

                    c_faceValue = " - ";
                    c_deductionAmt = " - ";
                    c_tier = " - ";
                    c_preTax = " - ";
                    c_benefitAmt = " - ";


                    if (User.IsInRole("ViewOnly"))
                    {
                        productName = productName.Replace(productName.Substring(productName.IndexOf("href"), productName.IndexOf(">") - productName.IndexOf("href")), "href=\"#\"");
                    }

                    // *** CHECK IF PDF EXISTS ***
                    //Response.Write(myReader["PDF"].ToString());
                    if (!String.IsNullOrWhiteSpace(myReader["PDF"].ToString()) && myReader["PDF"].ToString() != "N/A")
                        PDFhref = "<a class=\"app\" target=\"_blank\" href=\"" + Page.ResolveUrl("~/Documents") + myReader["PDF"].ToString() + "\">&nbsp;</a>";

                    bool spouseSet = false;
                    bool spouseAdded = false;
                    string spouseRow = "";
                    //Response.Write(futurePC + "<p/>");

                    if (f_faceValue == "$0.00" || f_faceValue == "$")
                        f_faceValue = " - ";
                    if (c_faceValue == "$0.00" || c_faceValue == "$")
                        c_faceValue = " - ";

                    // *** CHECK IF COVERAGE LEVEL IS CHILD RIDER ***
                    if (!String.IsNullOrWhiteSpace(myReader["coverage_level"].ToString()) && myReader["coverage_level"].ToString().Contains("CHILD-RIDER"))
                        f_benefitAmt = " Child Rider";

                    // *** CHECK IF COVERAGE LEVEL IS CHILD RIDER ***
                    if (!String.IsNullOrWhiteSpace(c_benefitAmt) && c_benefitAmt.Contains("CHILD-RIDER"))
                        c_benefitAmt = " Child Rider";

                    if (futurePC.Substring(futurePC.Length - 3, 3) == "_CH")
                        f_faceValue = "";

                    //Response.Write("<br /> " + productName + ":" + enrollType);
                    if (!String.IsNullOrWhiteSpace(productName))
                    {
                        string spouseRows = (string)Session["spouseRows"];
                        if (enrollType == "ER Paid")
                        {
                            er_DHTMXStatement.Add(new string[] { "2", futurePC, "<tr id='" + myReader["t_ID"].ToString() + "'> <td>" + productName + "</td> <td>" + f_benefitAmt + "</td> <td>" + f_faceValue + "</td> <td>" + f_tier + "</td> <td>" + f_deductionAmt + "</td> </tr> \n", f_deductionAmt, f_tier });
                            er_Height++;
                        }
                        else
                        {
                            DHTMXStatement.Add(new string[] { "1", futurePC, "<tr id='" + myReader["t_ID"].ToString() + "'> <td>" + productName + "</td> <td>" + f_benefitAmt + "</td> <td>" + f_faceValue + "</td> <td>" + f_tier + "</td> <td>" + f_deductionAmt + "</td> </tr> \n", f_deductionAmt, f_tier });
                            height++;
                            if (f_tier == "Spouse")
                                spouseRows += futurePC;
                            //Response.Write("<br/>" + DHTMXStatement.Count.ToString() + ":" + productName + futurePC);
                        }

                        if (myReader["hasSpouseRates"].ToString() == "1" && f_tier != "Spouse" && !spouseRows.Contains(futurePC) && !inSpouse)
                        {
                            DHTMXStatement.Add(new string[] { "1", futurePC, "<tr id='" + myReader["t_ID"].ToString() + "'> <td>" + 
                                productName.Insert(productName.LastIndexOf("</a>"), " - Spouse").Replace("class=\"yes\"", "class=\"no\"") +
                                "</td> <td> - </td> <td> - </td> <td> - </td> <td> - </td>  </tr> \n", f_deductionAmt, "Spouse" });
                            height++;
                            spouseRows += futurePC;
                            //Response.Write(spouseRows + "<br/>");
                        }
                        Session["spouseRows"] = spouseRows;
                    }
                    //Response.Write(DHTMXStatement[DHTMXStatement.Count - 1][2] + "<br/>");
                }
                myReader.Close();

                //foreach (string[] statement in DHTMXStatement)
                //    if (statement[0] == "1")
                //        Response.Write(statement[2] + "<br/>");

                // *** ADD TOTALS TO PRODUCTS TABLE ***
                string footer = "<tr> <td colspan='1'>Totals</td> <td colspan='3'><b>" + Session["billing_mode"] + " Premium Total:&nbsp;</b> </td> <td><b>$ " + Utility.MiscUtils.toCurrency(futureTotal.ToString()) + "</b></td> </tr> \n";

                string er_footer = "";

                if (er_DHTMXStatement.Count > 0)
                    er_footer = "<tr> <td colspan='1'>Totals</td> <td colspan='3'><b>Employer Paid Total:&nbsp;</b></td> <td> <b>$ " + Utility.MiscUtils.toCurrency(er_FutureTotal.ToString()) + "</b></td> </tr> \n";

                //Response.Write(height);
                if (!String.IsNullOrWhiteSpace(Request["error"]))
                    MyGrid.Text = "<div class='alert alert-error' style='text-align: left'> \n" +
                        //"     <a href='#' onclick='self.parent.location=\"Home.aspx\";' data-dismiss='alert' href='#'>Go Back</a> \n" +
                              "     <strong>Error!</strong> " + Request["error"] + ". \n </div>" +
                              "<div id='eeGrid_container' style='width:98%;height:" + (height * 20 + 88) + "px; overflow:hidden' scrolling='no'></div>";
                else
                {
                    MyGrid.Text = "<div id='eeGrid_container' style='width:98%;height:" + (height * 20 + 88) + "px; overflow:hidden' scrolling='no'></div>";
                    if (er_DHTMXStatement.Count >= 1)
                        MyGrid.Text += "<div id='erGrid_container' style='width:98%;height:" + (er_Height * 20 + 65) + "px; overflow:hidden' scrolling='no'></div>";
                }

                MyGrid.Text = header;
                foreach (string[] statement in DHTMXStatement)
                {
                    //Response.Write(statement[2]);
                    MyGrid.Text += String.Join("\r\n", statement[2].Replace("%%CL%%", " - ").Replace("%%FV%%", " - ").Replace("%%T%%", " - ").Replace("%%PT%%", " - ").Replace("%%AMT%%", " - "));
                }
                MyGrid.Text += footer;

                if (er_DHTMXStatement.Count > 0)
                    foreach (string[] statement in er_DHTMXStatement)
                        MyGrid.Text += String.Join("\r\n", statement[2].Replace("%%CL%%", " - ").Replace("%%FV%%", " - ").Replace("%%T%%", " - ").Replace("%%PT%%", " - ").Replace("%%AMT%%", " - "));

                MyGrid.Text += er_footer;


                if (!String.IsNullOrWhiteSpace((string)Session["errorMsg"]))
                {
                    string[] errorMsgs = ((string)Session["errorMsg"]).Split(new string[] { "||" }, StringSplitOptions.None);

                    javascript.Text += "<script type='text/javascript'> \n";
                    for (int i = 0; i < errorMsgs.Length; i++)
                    {
                        string msg = errorMsgs[i];
                        if (msg.Split('|')[0] == employeeID)
                        {
                            if (!String.IsNullOrWhiteSpace(msg))
                                javascript.Text += "alert('" + msg + "'); \n";
                            errorMsgs[i] = "";
                        }
                    }
                    javascript.Text += "</script> \n";
                    Session["errorMsg"] = String.Join("||", errorMsgs);
                }


                MyGrid.Text += "<div style='display:table-cell; position:absolute; '><br/>" +
                        "<table style='width:100%;margin-top:2%'> <tr> <td style='text-align:center; vertical-align: middle;'><a class='buttonlink' href='" + Page.ResolveUrl("~") +
                        "/TerminateEmployee.aspx'><div id='TerminateClick' class='btn btn-danger'> <i class='fa fa-minus-circle fa-white'></i> Terminate Employee</div></a></td>";
                   //    " <td><a class='buttonlink' href='#'><div id='' class='btn btn-warning'> <i class='fa fa-edit fa-white'></i> Change Products</div></a></td>"+


                //Response.Write(Utility.MiscUtils.getFolderPath("Documents") + @"\" + Session["group"] + @"\ConfirmationStatements\" + bNameCleared + "_" + employeeID +
                // "_CS.pdf ");
                //Response.Write(File.Exists(Utility.MiscUtils.getFolderPath("Documents") + @"\" + Session["group"] + @"\ConfirmationStatements\" + bNameCleared + "_" + employeeID +
                //       "_CS.pdf"));

                if (File.Exists(Utility.MiscUtils.getFolderPath("Documents") + @"\" + Session["group"] + @"\ConfirmationStatements\signed\" + bNameCleared + "_" + employeeID +
                     "_CS_Signed.pdf"))
                {
                    MyGrid.Text += "<td style='text-align:center; vertical-align: middle;'><a class='buttonlink' target='_blank' href='" + ResolveUrl("~/Documents") + "/" + Session["group"] +
                      "/ConfirmationStatements/signed/" + bNameCleared + "_" + employeeID + "_CS_Signed.pdf'><div id='TerminateClick' class='btn btn-primary'> <i class='fa fa-download fa-white'></i> Download Signed Confirmation Statement</div></a></td>";
                }
                else if (File.Exists(Utility.MiscUtils.getFolderPath("Documents") + @"\" + Session["group"] + @"\ConfirmationStatements\" + bNameCleared + "_" + employeeID +
                     "_CS.pdf"))
                {

                    MyGrid.Text += "<td style='text-align:center; vertical-align: middle;'><a class='buttonlink' target='_blank' href='" + ResolveUrl("~/Documents") + "/" + Session["group"] +
                     "/ConfirmationStatements/" + bNameCleared + "_" + employeeID + "_CS.pdf'><div id='TerminateClick' class='btn btn-info'> <i class='fa fa-download fa-white'></i> Download Unsigned Confirmation Statement</div></a></td>";
         
                }

                MyGrid.Text += "</tr></table></div>";
            }
            else
            {
                MyGrid.Text = "<div class='alert alert-block' style='text-align: left'> \n" +
                              "     <a href='#' onclick='self.parent.location=\"Home.aspx\";' data-dismiss='alert' href='#'>Go Back</a> \n" +
                              "     <h4 class='alert-heading'>Archived</h4> \n" +
                              "     This employee is archived and unavailable for enrollment. \n" +
                              "</div>";
            }
        }
            else
                MyGrid.Text = "<div class='alert alert-block' style='text-align: left'> \n" +
              "     <a href='#' onclick='self.parent.location=\"Default.aspx\";' data-dismiss='alert' href='#'>Go Back</a> \n" +
              "     <h4 class='alert-heading'>Unavailable</h4> \n" +
              "     This employee could not be found in the database. \n" +
              "</div>";

        }
        catch (Exception error)
        {
            MyGrid.Text=("<div class='alert alert-danger alert-block'>" +
            "						<a class='close' data-dismiss='alert' href='#'>×</a>" +
            "						<h4 class='alert-heading'>Error!</h4>" +
            "						" + error.Message +
          //  "                       <br/>Line: " + error.StackTrace +
            "					</div>");
        }
        myConnection.Close();
    }

    private string FindTerminationDetails(string employeeID)
    {
       // throw new NotImplementedException();
        string SQL = "SELECT * FROM [" + Session["currentAgency"] + "].[" + Session["group"] + "].[TerminatedEmployees] where EmpID="+employeeID ;

        string date="";
        Response.Write(SQL);
        SqlDataReader sdr = new SqlCommand(SQL, myConnection).ExecuteReader(); 

        if(sdr.HasRows)
            while (sdr.Read())
            {
                date = sdr["terminationdate"].ToString();
            }

        return date;

    }


}