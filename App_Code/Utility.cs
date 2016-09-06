using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Net.Mail;
using System.Net;
using System.Data;
/// <summary>
/// Summary description for Utility
/// </summary>
public static class Utility
{


    /// <summary>
    /// Summary description for Constants
    /// </summary>
    public static class Constants
    {
        public static string defaultAgency = "BenefitsDirect";

        public static string ConnectionString
        {
            get
            {
                return "user id=situsmain;" +
                  "password=WhiteStar708;server=localhost;" +
                  "Trusted_Connection=no;" +
                  "database=" + defaultAgency + "; " +
                  "connection timeout=30;" +
                  "MultipleActiveResultSets=True"; ;
            }
        }

        public static string AdminConnectionString
        {
            get
            {
                return "user id=situsadmin;" +
                                               "password=BlueTrophy205;server=localhost;" +
                                               "Trusted_Connection=no;" +
                                               "database=BenefitsDirect; " +
                                               "connection timeout=30;" +
                                               "MultipleActiveResultSets=True";
            }
        }
       
    }

    public static class SQLUtils
    {
        public static void StopRecurringPayments(string empID, string reason)
        {
            SqlConnection myConnection;
            Dictionary<string, string> logData = new Dictionary<string, string>();

            logData["currentAgency"] = (string)HttpContext.Current.Session["currentAgency"];
            logData["groupName"] = (string)HttpContext.Current.Session["group"];
            logData["username"] = HttpContext.Current.User.Identity.Name;
            //  HttpContext.Current.Response.Write(logData["username"].ToString());
            logData["date"] = DateTime.Now.ToString();
            logData["EmpID"] = empID;
            logData["type"] = Utility.LogUtils.LogType.Payments.ToString();
            logData["details"] = "";
            logData["reason"] = reason;
            //throw new NotImplementedException();
            string existSQL = "SELECT COUNT(EMPID) FROM [" + (string)HttpContext.Current.Session["currentAgency"] + "].[general].[PaymentInfo] WHERE EMPID="
                + empID + " AND [SHORT]='" + (string)HttpContext.Current.Session["group"] + "'";
            // Response.Write(existSQL);
            int rows = Convert.ToInt32(Utility.SQLUtils.getSingleSQLData(existSQL));
            if (rows > 0)
            {
                using (myConnection = new SqlConnection(Utility.Constants.ConnectionString))
                {
                    myConnection.Open();
                    string updateSQL = "UPDATE [" + (string)HttpContext.Current.Session["currentAgency"] + "].[general].[PaymentInfo] SET [ENABLE_RECURRING]=0 WHERE EMPID="
                + empID + " AND [SHORT]='" + (string)HttpContext.Current.Session["group"] + "'";
                    // Response.Write(updateSQL);
                    int rowsEffected = new SqlCommand(updateSQL, myConnection).ExecuteNonQuery();

                    if (rowsEffected > 0)
                    //  HttpContext.Current.Response.Write("<div class='alert alert-success'> <button class='close' data-dismiss='alert'>×</button><strong>Success!</strong> All future payments for this employee have been stopped.</b></div>");
                    {
                        SqlDataReader sql_Details = new SqlCommand("SELECT * FROM [" + HttpContext.Current.Session["currentAgency"] + "].[general].[PaymentInfo] " +
                   " WHERE [empID]=" + empID + " AND [SHORT]='" + (string)HttpContext.Current.Session["group"] + "'", myConnection).ExecuteReader();

                        while (sql_Details.Read())
                        {
                            for (int j = 0; j < sql_Details.FieldCount; j++)
                                logData["details"] += "|" + sql_Details.GetName(j) + "=" + sql_Details.GetValue(j);
                        }
                        sql_Details.Close();

                        Utility.LogUtils.logEntry(logData);
                    }

                }
            }

        }
        public static bool changeAppStatus(string newStatus, string employeeID)
        {
            //*** CREATE THE SQL CONNECTION ***
            SqlConnection myConnection = new SqlConnection(Utility.Constants.ConnectionString);

            string SQL = "";
            bool status = false;

            myConnection.Open();
            Dictionary<string, string> logData = new Dictionary<string, string>();

            logData["currentAgency"] = (string)HttpContext.Current.Session["currentAgency"];
            logData["groupName"] = (string)HttpContext.Current.Session["group"];
            logData["username"] = HttpContext.Current.User.Identity.Name;
          //  HttpContext.Current.Response.Write(logData["username"].ToString());
            logData["date"] = DateTime.Now.ToString();
            logData["EmpID"] = employeeID;
            logData["type"] = Utility.LogUtils.LogType.Census.ToString();
            logData["details"] = "";

            //throw new Exception (archiverString);
            if (!String.IsNullOrWhiteSpace((string)HttpContext.Current.Session["reason"]))
            {
                logData["reason"] = (string)HttpContext.Current.Session["reason"];
                HttpContext.Current.Session.Remove("reason");
            }
            else if (newStatus == "pending")
                logData["reason"] = "Change to Pending";
            else if (newStatus == "completed")
                logData["reason"] = "Change to Completed";
            else if (newStatus == "terminated")
                logData["reason"] = "Employee Terminated";
            else
                throw new Exception("Invalid app status provided.");


            SQL = "UPDATE [" + HttpContext.Current.Session["currentAgency"] + "].[" + HttpContext.Current.Session["group"] + "].[Employees] SET [app_status]='" + newStatus + "' \n"
            + " WHERE [empID]=" + employeeID;
            //throw new Exception(SQL);

            if (new SqlCommand(SQL, myConnection).ExecuteNonQuery() == 1)
                status = true;
            else
                throw new Exception("Update failed.");

            //SQL = "UPDATE [" + Session["currentAgency"] + "].[" + Session["name"] + "].[Transactions] SET [old_transaction]='yes' \n"
            //+ "WHERE [employee_ID]=" + Session["empID"];

            //if (new SqlCommand(SQL, myConnection).ExecuteNonQuery() == 1)
            //    Response.Write(SQL);
            SqlDataReader sql_Details = new SqlCommand("SELECT * FROM [" + HttpContext.Current.Session["currentAgency"] + "].[" + HttpContext.Current.Session["group"] + "].[Employees] " +
                " WHERE [empID]=" + employeeID, myConnection).ExecuteReader();

            while (sql_Details.Read())
            {
                for (int j = 0; j < sql_Details.FieldCount; j++)
                    logData["details"] += "|" + sql_Details.GetName(j) + "=" + sql_Details.GetValue(j);
            }
            sql_Details.Close();

            Utility.LogUtils.logEntry(logData);

            myConnection.Close();
            return status;
        }

        public static object getSingleSQLData(string SQLString)
        {
            SqlConnection currentConnection = new SqlConnection(Constants.ConnectionString);
            //HttpContext.Current.Response.Write(MyGlobals.myConnection.State.ToString());
            currentConnection.Open();
            try
            {
                var data = new SqlCommand(SQLString, currentConnection).ExecuteScalar();
                //if (data.GetType().ToString() != "System.String")
                //    throw new Exception("DATAIS|" + data.GetType().ToString() + "|");

                if (data != null)
                {
                    if (data.GetType() == typeof(System.Data.SqlClient.SqlException))
                    {
                        currentConnection.Close();
                        throw new Exception("Command failed to retreive data.");
                    }

                    if (data.GetType() == typeof(System.DBNull))
                    {
                        currentConnection.Close();
                        return "";
                    }

                    if (data.GetType().ToString() != "System.Exception" && String.IsNullOrWhiteSpace(data.ToString()))
                    {
                        currentConnection.Close();
                        return "";
                    }
                }
                else
                {
                    currentConnection.Close();
                    return "";
                }

                currentConnection.Close();
                return data;
            }
            catch (SqlException sqlError)
            {
                currentConnection.Close();
                return new Exception(sqlError.Message + "\n Command:" + SQLString + "\n Stack:" + sqlError.StackTrace);
            }
        }
    }

    public static class MiscUtils
    {
        public static string createFileString(string fileName)
        {
            string rawData = fileName.Trim();
            return rawData.Replace(" ", "_").Replace(",", "").Replace("/", "-").Replace(@"\", "-").Replace("'", "").Replace("&", "AND").Replace("#", "");
        }
        public static string toCurrency(string preFormat)
        {
            if (!String.IsNullOrWhiteSpace(preFormat))
            {
                if (preFormat.IndexOf(".") != -1)
                {
                    string[] buffer = preFormat.Split('.');
                    if (buffer[1].Length == 1)
                        return (String.Join(".", buffer) + "0");
                    else if (buffer[1].Length == 2)
                        return String.Join(".", buffer);
                    else if (buffer[1].Length > 2)
                    {
                        try
                        {
                            buffer[1] = Math.Round(Convert.ToDouble("0." + buffer[1]), 2).ToString();
                            //HttpContext.Current.Response.Write(buffer[1] + "<br/>");
                            if (buffer[1].Contains("."))
                                buffer[1] = buffer[1].Split('.')[1];
                            else if (buffer[1] == "0")
                                buffer[1] = "00";
                            else if (buffer[1] == "1")
                            {
                                buffer[0] = (Convert.ToInt32(buffer[0]) + 1).ToString();
                                buffer[1] = "00";
                            }

                            if (buffer[1].Length == 1)
                                buffer[1] = buffer[1] + 0;
                        }
                        catch (FormatException e)
                        {
                            buffer[1] = buffer[1].Substring(0, 2);
                        }
                        return String.Join(".", buffer);
                    }
                }
                else return (preFormat + ".00");
            }
            else return "0.00";
            return preFormat;
        }
        public static string toCurrency(string preFormat, bool addCommas)
        {
            if (!String.IsNullOrWhiteSpace(preFormat))
            {
                if (preFormat.IndexOf(".") != -1)
                {
                    string[] buffer = preFormat.Split('.');

                    if (addCommas)
                    {
                        string reverse = null;
                        char[] charArray = buffer[0].ToCharArray();
                        Array.Reverse(charArray);
                        for (int i = 0; i < charArray.Length; i++)
                        {
                            if (i % 3 == 0 && i != 0)
                                reverse += ',';
                            reverse += charArray[i];
                        }
                        charArray = reverse.ToCharArray();
                        Array.Reverse(charArray);
                        buffer[0] = new string(charArray);
                    }

                    if (buffer[1].Length == 1)
                        return (String.Join(".", buffer) + "0");
                    else if (buffer[1].Length == 2)
                        return String.Join(".", buffer);
                    else if (buffer[1].Length > 2)
                    {
                        string initBuffer = buffer[1];
                        try
                        {
                            buffer[1] = Math.Round(Convert.ToDouble("0." + buffer[1]), 2).ToString();
                            //HttpContext.Current.Response.Write(buffer[1]);
                            if (buffer[1].Contains("."))
                                buffer[1] = buffer[1].Split('.')[1];
                            else if (buffer[1] == "0")
                                buffer[1] = "00";
                            else if (buffer[1] == "1")
                            {
                                //HttpContext.Current.Response.Write(" :" + buffer[0].Replace(",", "") + ": ");
                                buffer[0] = (Convert.ToInt32(buffer[0].Replace(",", "")) + 1).ToString();

                                if (addCommas)
                                {
                                    string reverse = null;
                                    char[] charArray = buffer[0].ToCharArray();
                                    Array.Reverse(charArray);
                                    for (int i = 0; i < charArray.Length; i++)
                                    {
                                        if (i % 3 == 0 && i != 0)
                                            reverse += ',';
                                        reverse += charArray[i];
                                    }
                                    charArray = reverse.ToCharArray();
                                    Array.Reverse(charArray);
                                    buffer[0] = new string(charArray);
                                }

                                buffer[1] = "00";
                            }
                        }
                        catch (FormatException e)
                        {
                            buffer[1] = initBuffer.Substring(0, 2);
                        }
                        return String.Join(".", buffer);
                    }
                }
                else
                {
                    if (addCommas)
                    {
                        string reverse = null;
                        char[] charArray = preFormat.ToCharArray();
                        Array.Reverse(charArray);
                        for (int i = 0; i < charArray.Length; i++)
                        {
                            if (i % 3 == 0 && i != 0)
                                reverse += ',';
                            reverse += charArray[i];
                        }
                        charArray = reverse.ToCharArray();
                        Array.Reverse(charArray);
                        preFormat = new string(charArray);
                    }

                    return (preFormat + ".00");
                }
            }
            else return "0.00";
            return preFormat;
        }


        public static string getBillingMode(string shortForm)
        {
            if (shortForm == "M")
                return "Monthly";
            if (shortForm == "SM")
                return "Semi-Monthly";
            if (shortForm == "BW")
                return "Bi-Weekly";
            if (shortForm == "W")
                return "Weekly";
            if (shortForm == "9")
                return "9-thly";
            if (shortForm == "10")
                return "10-thly";
            if (shortForm == "SM")
                return "Semi-Monthly";
            else
                return shortForm;
        }
        public static int getDigitCount(int toCount)
        {
            return (int)Math.Floor(Math.Log10(toCount) + 1);
        }

        public static string getFolderPath(string name)
        {
            return System.Web.HttpContext.Current.Server.MapPath("~/" + name);
        }

        public static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            string[] words = s.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                string word = words[i];
                if (words.Length > 1)
                {
                    words[i] = UppercaseFirst(word);
                }
                else return char.ToUpper(word[0]) + word.Substring(1).ToLower();
            }
            return String.Join(" ", words);
        }

        // Formats given date string by given format string. Returns empty string for bad date, invalid characters, or undetected format.
        // Becca 1-8-2015.
        public static string formatDate(string format, string date)
        {   // Detected formats for date string.
            string[] formats = { "MM/dd/yyyy", "M/d/yyyy", "MM/d/yyyy", "M/dd/yyyy", "M/d/yy", "MM/d/yy", "M/dd/yy", "MMddyyyy", "yyyyMMdd", "yyyy-MM-dd" };
            string formattedDate = "00/00/0000";
            // Remove leading and trailing whitespace
            date = date.Trim();
            DateTime dateObject;
            // Try to parse the date.
            if (DateTime.TryParseExact(date, formats, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.DateTimeStyles.None, out dateObject))
            {
                formattedDate = dateObject.ToString(format);
            }
            return formattedDate;
        }


        public static string formatSSN(string preformat)
        {
            string ssn = preformat;
            if (preformat.Length == 7 && !preformat.Contains("-"))
                preformat = preformat.Insert(0, "00");

            if (preformat.Length == 8 && !preformat.Contains("-"))
                preformat = preformat.Insert(0, "0");

            if (preformat.Length == 10 && preformat.Contains("-"))
                preformat = preformat.Insert(0, "0");

            if (preformat.Contains("-"))
                ssn = preformat;
            else if (preformat.Length == 9)
            {
                ssn = preformat.Substring(0, 3) + "-" + preformat.Substring(3, 2) + "-" + preformat.Substring(5, 4);
            }
            return ssn;
        }


        public static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        public static bool SendEmailToAccountManager(string subject, string emailText, SqlConnection myConnection, string group, string agency)
        {
            bool success = false;
            try
            {
               // myConnection.Open();
                if (myConnection.State == ConnectionState.Closed)
                    myConnection.Open();
                var rng = new Random(); var agentName = "Rohan";
                var email = "rsureshkumar@ventureusenterprises.com";
                //  Response.Write("\n SELECT  [acct_manager],[acct_mgr_email] FROM [" + Session["currentAgency"] + "].[general].[Groups] where [short]='" + Session["group"].ToString() + "'");

                SqlDataReader myReader = new SqlCommand("SELECT  [acct_manager],[acct_mgr_email] FROM [" + agency + "].[general].[Groups] where [short]='" + group + "'", myConnection).ExecuteReader();
                while (myReader.Read())
                {
                    agentName = myReader["acct_manager"].ToString();
                    email = myReader["acct_mgr_email"].ToString();
                    // Response.Write("Email: "+email + "AgentName: " + agentName);
                }
                if (email != "")
                {
                    var fromAddress = new MailAddress("info@ventureuservers.com", "SITUS Administration");
                    //var fromAddress = new MailAddress("customerservice@tandsbenefits.com", agentName);
                    var toAddress = new MailAddress(email, agentName);
                    const string fromPassword = "showmeins$2011";

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                    };

                    var message = new MailMessage(fromAddress, toAddress);
                    message.Subject = subject;
                    message.Body = "Hello " + agentName + ",<br/><br/>" + emailText;
                    message.IsBodyHtml = true;
                    smtp.Send(message);
                    success = true;
                }

                else
                    throw new Exception("The account manager doesn't exist.");
                //Response.Write("<div style='width:100%; height:100%; background:#0a0;'>Document has been sent!</div>");
                //Response.Write("<script language='javascript'>alert('An email has been sent to the address on file.'); window.location('Login.aspx');</script>");
               
               // myConnection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if(myConnection.State == ConnectionState.Open)
                myConnection.Close();
            }
            return success;
        }


        public static bool SendEmailToAccountManagerWithAttachments(string subject, string PathToAttachment, string emailText, SqlConnection myConnection, string group, string agency)
        {
            bool success = false;
            try
            {
                // myConnection.Open();
                if (myConnection.State == ConnectionState.Closed)
                    myConnection.Open();
                var rng = new Random(); var agentName = "Rohan";
                var email = "rsureshkumar@ventureusenterprises.com";
                //  Response.Write("\n SELECT  [acct_manager],[acct_mgr_email] FROM [" + Session["currentAgency"] + "].[general].[Groups] where [short]='" + Session["group"].ToString() + "'");

                SqlDataReader myReader = new SqlCommand("SELECT  [acct_manager],[acct_mgr_email] FROM [" + agency + "].[general].[Groups] where [short]='" + group + "'", myConnection).ExecuteReader();
                while (myReader.Read())
                {
                    agentName = myReader["acct_manager"].ToString();
                    email = myReader["acct_mgr_email"].ToString();
                    // Response.Write("Email: "+email + "AgentName: " + agentName);
                }
                if (email != "")
                {
                    var fromAddress = new MailAddress("info@ventureuservers.com", "SITUS Administration");
                    //var fromAddress = new MailAddress("customerservice@tandsbenefits.com", agentName);
                    var toAddress = new MailAddress(email, agentName);
                    const string fromPassword = "showmeins$2011";

                    var smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                    };

                    var message = new MailMessage(fromAddress, toAddress);
                    message.Subject = subject;
                    message.Body = "Hello " + agentName + ",<br/><br/>" + emailText;
                    System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(PathToAttachment);
                    attachment.Name = "TerminateEmployeesChangeLog.txt"; 
                    message.Attachments.Add(attachment);                   
                    message.IsBodyHtml = true;
                    smtp.Send(message);
                    success = true;
                }

                else
                    throw new Exception("The account manager doesn't exist.");
                //Response.Write("<div style='width:100%; height:100%; background:#0a0;'>Document has been sent!</div>");
                //Response.Write("<script language='javascript'>alert('An email has been sent to the address on file.'); window.location('Login.aspx');</script>");

                // myConnection.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (myConnection.State == ConnectionState.Open)
                    myConnection.Close();
            }
            return success;
        }


    }

    public static class LogUtils
    {
        public enum LogType
        {
            Transactions = 1,
            Beneficiaries = 2,
            Census = 3,
            Dependents = 4,
            EditLedger = 5,
            Payments=6
        }

        public static int logEntry(Dictionary<string, string> logData)
        {
            SqlConnection currentConnection = new SqlConnection(Constants.ConnectionString);
            currentConnection.Open();

            string SQL = "INSERT INTO [SITUSLogs].[" + logData["groupName"] + "].[MainLog] " +
                "VALUES ('" + logData["username"] + "','" + logData["date"] + "','" + logData["type"] + "','" + logData["EmpID"] + "','" + logData["reason"] + "','" + logData["details"] + "') ";
           // HttpContext.Current.Response.Write(SQL);

            return new SqlCommand(SQL, currentConnection).ExecuteNonQuery();
        }
    }
  
}
public static class DataReaderCheck
{
    public static bool HasColumn(this IDataRecord dr, string columnName)
    {
        for (int i = 0; i < dr.FieldCount; i++)
        {
            if (dr.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                return true;
        }
        return false;
    }
}