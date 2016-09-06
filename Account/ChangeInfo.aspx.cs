using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Security;

public partial class Account_ChangeInfo : System.Web.UI.Page
{
    SqlConnection myConnection = new SqlConnection(Utility.Constants.ConnectionString);
    //TextBox sourcePassword ;
    //TextBox sourceUserID ;
    protected void Page_Load(object sender, EventArgs e)
    {
        FillEmailAddressAndPIN();
    }

    private void FillEmailAddressAndPIN()
    {
        try
        {
           

                 //sourcePassword = (TextBox)PreviousPage.FindControl("password");
                 //sourceUserID = (TextBox)PreviousPage.FindControl("username");
                 //  Response.Write(sourceUserID.Text + sourcePassword.Text);
            if (!Page.IsPostBack)
            {

                myConnection.Open();
                SqlDataReader userData = new SqlCommand(" SELECT [email],[password] FROM [Administration].[general].[Clients] WHERE [empID]='" + Session["empID"].ToString() + "'", myConnection).ExecuteReader();
                //Response.Write("SELECT [email],[password] FROM [Administration].[general].[Clients] WHERE [empID]='" + Session["empID"].ToString() + "'");
                while (userData.Read())
                {
                    email.Text = userData["email"].ToString();
                    //  password.Text = userData["password"].ToString();
                  //  Response.Write(userData["email"].ToString() + " " + userData["password"].ToString());
                }

                myConnection.Close();
            }
            
        }
        catch (Exception e)
        {
            ErrorMsg.Text = e.Message;
        }
    }

    protected void Save_Click(object sender, EventArgs e)
    {
        Response.Write("hello");
        try
        {
            Session["newUser"] = "False";
            Session.Remove("newUser");
            string SQL; bool duplicate;
            myConnection.Open();
            if (email.Text == "")
                throw new Exception("Email cannot be blank.");
            else
                duplicate=CheckForEmailDuplicates(email.Text);
            Response.Write(duplicate);
            if (duplicate == false)
            {
               
                if (password.Text != "")
                {
                    if (password.Text.Length < 8)
                        throw new Exception("Password must be atleast 8 characters in length.");
                    SQL = "UPDATE [Administration].[general].[Clients] SET [email] = '" + email.Text + "', [password] = '" +
                     password.Text + "' WHERE [empID]='" + Session["empID"].ToString() + "'";
                }
                else
                    SQL = "UPDATE [Administration].[general].[Clients] SET [email] = '" + email.Text +
                        "' WHERE [empID]='" + Session["empID"].ToString() + "'";

                Response.Write(SQL);
                if (new SqlCommand(SQL, myConnection).ExecuteNonQuery() == 1)
                {
                    myConnection.Close();
                    Response.Redirect(Page.ResolveUrl("~") );
                    //FormsAuthentication.RedirectFromLoginPage(sourceUserID.Text, false);
                    // Response.Write("User updated");
                }
                else
                {
                    ErrorMsg.Text = "<div class='alert alert-danger'> " +
                                        "<button class='close' data-dismiss='alert'>×</button><strong>Success!</strong>" +
                                        "Email and PIN couldn't be updated." +
                                    "</div>";
                }
                myConnection.Close();
            }
        }
        catch (Exception ex)
        {

            ErrorMsg.Text = "<div class='alert alert-danger'> " + ex.Message + "</div>"; 
        }
        
    }

    private bool CheckForEmailDuplicates(string p)
    {

        string SQL = "SELECT count([email]) FROM [Administration].[general].[Clients] WHERE [email] = '" + email.Text + "' AND [empID] <> '" + Session["empID"].ToString() + "'"; 

       if (Convert.ToInt32(new SqlCommand(SQL, myConnection).ExecuteScalar()) > 0)
       {
           throw new Exception("This email already exists in the system. Please use a different one.");
       }
       return false;
    }
}