using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Threading;

[Serializable]
public class Ideograph
{
    public string symbol;
    public string components;
}

[Serializable]
public class Radical
{
    public string radical;
}

public partial class _Default : System.Web.UI.Page
{
    protected string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
    protected string[] results = new string[10];

    protected void Page_Load(object sender, EventArgs e)
    {
        Welcome.Text = "Tenarai prototype";
        Result.Text = " ";
        Result0.Text = " ";
        Result1.Text = " ";

        //JsonSerialize();
        CreateButtons();
        //sortableUL.Controls.Add(new LiteralControl("<li><button>H</button></li>"));
        //string Method = "<script language='javascript'>" + "$('.SortList').sortable({handle:'button', cancel: '});" + "</script>";
        //ScriptManager.RegisterClientScriptBlock(this.Page, this.GetType(), "MethodCall", Method, false);
    }
    protected void Search(object sender, EventArgs e)
    {
        switch (TextBox.Text.Length)
        {
            case 0:
                Result.Text = "No string entered.";
                break;
            case 1:
                /* Validation code here */
                if (IsValidCJK(TextBox.Text))
                    GetInformation(TextBox.Text);
                else
                    Result.Text = "Invalid character.";
                break;
            default:
                Result.Text = "Too many characters entered. Please enter only one character.";
                break;
        }
    }

    protected Boolean IsValidCJK(String ideograph)
    {
        Boolean retval = false;
        long unicode = Convert.ToChar(ideograph);

        if ((19968 <= unicode && unicode <= 40908) // U+4E00 to U+9FCC
            || (13312 <= unicode && unicode <= 19893) // U+3400 to U+4DB5
            || (131072 <= unicode && unicode <= 173782) // U+20000 to U+2A6D6
            || (173824 <= unicode && unicode <= 177972) // U+2A700 to U+2B734
            || (177984 <= unicode && unicode <= 178205)) // U+2B740 to U+2B81D
            retval = true;

        return retval;
    }

    protected void GetInformation(string ideograph)
    {
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();

            using (SqlCommand cmd = new SqlCommand("sp_get_ideograph_information", con))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@in_ideograph",SqlDbType.NVarChar,1).Value=ideograph;
                cmd.Parameters.Add("@in_ideograph_source", SqlDbType.NVarChar, 1).Value = "J";

                SqlParameter dbRetVal = new SqlParameter("Return", SqlDbType.Int);
                dbRetVal.Direction = ParameterDirection.ReturnValue;

                using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    if (reader.Read())
                    {
                        Result.Text = "Kanji: " + reader.GetValue(0);
                        Result0.Text = "Stroke count: " + reader.GetValue(1);
                        Result1.Text = "Grade learned: " + reader.GetValue(2);
                    }
                    else
                        Result.Text = "Unsupported character.";
                        
                    reader.Close();
                }
            }
        }
    }

    /*
     * Test method to dynamically create buttons
     * */
    protected void CreateButtons()
    {
        int dynamicCount = 0;

        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();

            /* 2015-05-19: commenting out for final project */
            using (SqlCommand cmd = new SqlCommand("sp_get_ideograph_information", con))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@in_ideograph",SqlDbType.NVarChar);
                cmd.Parameters.Add("@in_ideograph_source", SqlDbType.NVarChar).Value = "J";

                using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {

                    while (reader.Read())
                    {
                        if (dynamicCount % 10 == 0)
                            form1.Controls.Add(new LiteralControl("<br /><br />"));

                        if (dynamicCount % 20 == 0)
                            form1.Controls.Add(new LiteralControl("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"));
                        
                        System.Web.UI.HtmlControls.HtmlGenericControl sortable = new System.Web.UI.HtmlControls.HtmlGenericControl("li");
                        sortableUL.Controls.Add(sortable);
                        
                        Button button = new Button();
                        button.ID = "button_" + dynamicCount.ToString();
                        button.Text = (String) reader.GetValue(0);
                        button.CssClass = "ButtonStyle";
                        button.Click += new EventHandler(ButtonHandler);

                        sortable.Controls.Add(button);
                        
                        dynamicCount++;
                    }
                }
            }
        }
    }

    protected void ButtonHandler(object sender, EventArgs e)
    {
        Button button = sender as Button;
        if (button != null)
            GetInformation(button.Text);
    }

    protected void JsonSerialize()
    {
        Ideograph JSONObject = new Ideograph();
        Radical rad = new Radical();
        JavaScriptSerializer serial = new JavaScriptSerializer();
        string JSONString, saveSymbol = "";
        Boolean firstObj = true;

        Response.Write("<script type='text/javascript'> var JSONObj1 = [");
        using (SqlConnection con = new SqlConnection(connectionString))
        {
            con.Open();

            /* 2015-05-19: final project code */
            using (SqlCommand cmd = new SqlCommand("sp_get_information", con))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Parameters.Add("@in_table_name", SqlDbType.NVarChar).Value = "vw_Symbol";

                using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                {
                    while (reader.Read())
                    {
                        if (String.Compare((String)reader.GetValue(0),saveSymbol)!=0)
                        {
                            if (!String.IsNullOrEmpty(saveSymbol))
                            {
                                // Write out the JSON
                                if (!firstObj)
                                    Response.Write(", ");
                                else
                                    firstObj = false;
                                JSONObject.symbol = saveSymbol;

                                JSONObject.components += "]";

                                JSONString = serial.Serialize(JSONObject);
                                Response.Write(JSONString);
                            }
                            JSONObject.components = "[" + (String)reader.GetValue(1);
                        }else
                            JSONObject.components += ", " + (String)reader.GetValue(1);

                        saveSymbol = (String) reader.GetValue(0);                        
                    }
                }
            }

            Response.Write("] </script> <script type='text/javascript'> var JSONObj2 = [");
            firstObj = true;

            using (SqlConnection con2 = new SqlConnection(connectionString))
            {
                con2.Open();
                using (SqlCommand cmd = new SqlCommand("sp_get_information", con2))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Add("@in_table_name", SqlDbType.NVarChar).Value = "vw_Radical";

                    using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            if (!firstObj)
                                Response.Write(", ");
                            else
                                firstObj = false;
                            rad.radical = (String)reader.GetValue(0);
                            JSONString = serial.Serialize(rad);
                            Response.Write(JSONString);
                        }
                    }
                }
            }
        }

        Response.Write("] </script>");
    }
}

