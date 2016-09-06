using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Net;

public partial class SitusPage : System.Web.UI.Page
{
    protected void Page_PreInit(object sender, EventArgs e)
    {
        //Response.Write("<br> ca:" + Session["currentAgency"]);

        //Response.Write("<br>test:" + (string)Session["name"] + doesGroupExist((string)Session["name"]));
        //if (String.IsNullOrWhiteSpace((string)Session["name"]) && this.GetType().Name != "default_aspx" && this.GetType().Name != "home_aspx")
        //{
        //    Response.Write("GroupShort: " + Session["name"]);
        //    Response.Redirect(Page.ResolveUrl("~"));
        //}

        //if (this.GetType().Name == "products_aspx" && String.IsNullOrWhiteSpace(Request["EID"]) && String.IsNullOrWhiteSpace((string)Session["empID"]))
        //{
        //    Response.Redirect(Page.ResolveUrl("~/Home.aspx"));
        //}

        //if (this.GetType().Name == "products_aspx" && !String.IsNullOrWhiteSpace(Request["EID"]))
        //{
        //    Session["empID"] = Request["EID"];
        //}

        //if (!String.IsNullOrWhiteSpace((string)Session["name"]) && (string)Session["name"] != "SITUSGroups" && (string)Session["name"] != "SITUS Administration")
        //{
        //    if (Session["ProductsList"] == null || this.GetType().Name == "home_aspx")
        //    {
        //        Session["ProductsList"] = generateProductsList();
        //    }
        //}
        //else
        //    Session.Remove("ProductsList");
    }

    protected bool doesGroupExist(string groupName)
    {
        if (String.IsNullOrWhiteSpace((string)Session["currentAgency"]))
            return false;
        return Convert.ToBoolean(Utility.SQLUtils.getSingleSQLData(" select case \n   when exists (SELECT DISTINCT name FROM " + Session["currentAgency"] + ".sys.schemas WHERE name = '" + groupName + "') \n   then 1 \n   else 0 \n end"));
    }

    protected bool doesEmployeeExist(string EEID)
    {
        //throw new Exception(" select case \n   when exists (SELECT DISTINCT empID FROM " + Session["currentAgency"] + "." + Session["name"] + ".Employees WHERE empID = '" + EEID + "') \n   then 1 \n   else 0 \n end");
        return Convert.ToBoolean(Utility.SQLUtils.getSingleSQLData(" select case \n   when exists (SELECT DISTINCT empID FROM " + Session["currentAgency"] + ".[" + Session["group"] + "].Employees WHERE empID = '" + EEID + "') \n   then 1 \n   else 0 \n end"));
    }

    public class CensusException : Exception
    {
        public CensusException()
        {
        }

        public CensusException(string message)
            : base(message)
        {
        }
    }

    protected static Dictionary<string, Dictionary<string, Object>> generateProductsList()
    {
        //*** GET PRODUCTS DATA ***
        SqlConnection SQLConnection = new SqlConnection(Utility.Constants.ConnectionString);
        SQLConnection.Open();

        string agency = (string)HttpContext.Current.Session["currentAgency"];
        string group = (string)HttpContext.Current.Session["group"];
        Dictionary<string, Dictionary<string, Object>> Products = new Dictionary<string, Dictionary<string, Object>>();

        SqlDataReader myReader = new SqlCommand("SELECT *, " +
" (SELECT CASE \n WHEN EXISTS (SELECT 1 FROM [" + agency + "].INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = code + '_RT' AND TABLE_SCHEMA = '" + group + "' AND COLUMN_NAME = 'ee_or_spouse')\n THEN 1\n ELSE 0\n END) AS hasSpouseRates \n " +
          " FROM [" + agency + "].[" + group + "].[Products] ORDER BY [order] ", SQLConnection).ExecuteReader();
        while (myReader.Read())
        {
            Dictionary<string, Object> newEntry = new Dictionary<string, Object>();
            for (int i = 0; i < myReader.FieldCount; i++)
                newEntry.Add(myReader.GetName(i), myReader.GetValue(i));
            //Response.Write(myReader["code"] + "<br/>");
            Products[myReader["code"].ToString()] = newEntry;

            if (Convert.ToBoolean(myReader["hasSpouseRates"]))
            {
                Dictionary<string, Object> spouseEntry = new Dictionary<string, Object>();
                for (int i = 0; i < myReader.FieldCount; i++)
                    spouseEntry.Add(myReader.GetName(i), myReader.GetValue(i));
                spouseEntry["product_name"] = spouseEntry["product_name"] + " - Spouse";
                Products[myReader["code"].ToString() + "_SPOUSE"] = spouseEntry;
            }
            HttpContext.Current.Session["hasLocation"] = myReader.HasColumn("location");
        }
        myReader.Close();
        SQLConnection.Close();

        return Products;
    }
}