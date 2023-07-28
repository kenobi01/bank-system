using BankSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BankSystem.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Session["loginSession"] = null;
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection fc)
        {
            // Create a login object and assign values from the form collection
            login l = new login();
            l.username = fc["username"];
            l.password = fc["password"];

            // Check if the login session is null and assign the username to the session
            if (Session["loginSession"] == null)
            {
                Session["loginSession"] = l.username;
            }

            SqlConnection con = null;
            SqlDataReader dr = null;
            try
            {
                // Get the connection string
                string connectionString = GetConnectionString();

                // Create a new SQL connection
                con = new SqlConnection(connectionString);

                con.Open();

                // Construct the SQL query string
                string str = $"select * from Customer where Username = " + $" '{l.username}' and Password = '{l.password}'";

                // Create a SQL command object with the query string and connection
                SqlCommand cmd = new SqlCommand(str, con);

                // Execute the query and get the data reader
                dr = cmd.ExecuteReader();

                if (ModelState.IsValid)
                {
                    // Check if the data reader has rows, indicating successful login
                    if (dr.HasRows)
                    {
                        TempData["message"] = "Login Successful";
                        TempData.Keep("message");
                        return RedirectToAction("BankPage", l);
                    }
                    else
                    {
                        ViewBag.attempt = "Login Failed. Please try again or if you are new then please Register";
                    }
                }
            }
            finally
            {
                // Close the data reader
                if (dr != null)
                {
                    dr.Close();
                }

                // Close the SQL connection
                if (con != null)
                {
                    con.Close();
                }
            }

            return View();
        }

        protected string GetConnectionString()
        {
            var datasource = @"KENOBI"; // your server
            var database = "BankSystem"; // your database name
            string connString = @"Data Source=" + datasource + ";Initial Catalog="
                                + database + ";Integrated Security=True";

            return connString;
        }

        public ActionResult Register()
        {
            // Get the connection string from the configuration file
            string constring = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            // Create a new SQL connection using the connection string
            SqlConnection con = new SqlConnection(GetConnectionString());

            // Create a new SQL data adapter with the query and connection
            SqlDataAdapter _da = new SqlDataAdapter("Select * From Branch", con);

            // Create a new data table to store the results
            DataTable _dt = new DataTable();

            // Fill the data table with the results from the data adapter
            _da.Fill(_dt);

            // Create a dynamic property on the ViewBag to hold the branch list
            // Convert the data table to a select list using the ToSelectList method
            ViewBag.BranchList = ToSelectList(_dt, "Id", "Name");

            // Return the view with the populated ViewBag
            return View();
        }

        [HttpPost]
        public ActionResult Register(login l)
        {
            // Redirect to the "BankPage" action with the login object
            return RedirectToAction("BankPage", l);
        }

        // Method to convert a DataTable to a SelectList
        private dynamic ToSelectList(DataTable dt, string v1, string v2)
        {
            // Create a list to store the SelectListItem objects
            List<SelectListItem> list = new List<SelectListItem>();

            try
            {
                // Iterate through each row in the DataTable
                foreach (DataRow row in dt.Rows)
                {
                    // Create a new SelectListItem object and populate its Text and Value properties
                    list.Add(new SelectListItem()
                    {
                        Text = row[v2].ToString(),
                        Value = row[v1].ToString()
                    });
                }

                // Create a new SelectList using the list of SelectListItem objects
                // The "Value" property is used as the selected value and the "Text" property is used as the display text
                return new SelectList(list, "Value", "Text");
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        public ActionResult BankPage(login l)
        {
            try
            {
                // Check if the login session is null and assign the username to the session
                if (Session["loginSession"] == null)
                {
                    Session["loginSession"] = l.username;
                }

                // Get the connection string from the configuration file
                string constring = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

                // Create a new SQL connection using the connection string
                SqlConnection con = new SqlConnection(GetConnectionString());
                con.Open();

                // Check if the "Name" property of the login object is not null
                if (l.Name != null)
                {
                    // Create a new SQL command for inserting into the bank tables
                    SqlCommand Cmd = new SqlCommand("INSERT_INTO_BANK_TABLES", con);
                    Cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the command
                    Cmd.Parameters.AddWithValue("@name", l.Name);
                    Cmd.Parameters.AddWithValue("@dob", l.DOB);
                    Cmd.Parameters.AddWithValue("@phone", l.Phone);
                    Cmd.Parameters.AddWithValue("@email", l.Email);
                    Cmd.Parameters.AddWithValue("@address", l.Address);
                    Cmd.Parameters.AddWithValue("@username", l.username);
                    Cmd.Parameters.AddWithValue("@password", l.password);
                    Cmd.Parameters.AddWithValue("@accType", l.AccountType);
                    Cmd.Parameters.AddWithValue("@branchId", l.Branch);

                    // Execute the command
                    Cmd.ExecuteNonQuery();

                    TempData["message"] = "Registration Successful";
                    TempData.Keep("message");
                }

                // Contact
                // Create a data adapter to fetch data from the database
                SqlDataAdapter _da = new SqlDataAdapter("Select B.Name, B.Description, B.IBAN, B.Phone From Branch B Inner Join Account A on B.Id = A.BranchId Inner Join Customer C on A.CustId = C.Id  Where C.Username = " + $" '{Session["loginSession"]}'", con);
                DataTable dt = new DataTable();
                _da.Fill(dt);

                // Create a list to store the branches
                List<Branches> br = new List<Branches>();

                // Iterate through each row in the DataTable and populate the list
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Branches brs = new Branches();
                    brs.Name = dt.Rows[i]["Name"].ToString();
                    brs.Description = dt.Rows[i]["Description"].ToString();
                    brs.IBAN = dt.Rows[i]["IBAN"].ToString();
                    brs.Contact = dt.Rows[i]["Phone"].ToString();
                    br.Add(brs);
                }

                // Assign the branches list to the login object
                l.Branchs = br;

                con.Close();

                // Return the view with the populated login object
                return View(l);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Statement()
        {
            // Return the Statement view
            return View();
        }

        [HttpPost]
        public ActionResult Statement(Statement st)
        {
            // Check if the entered dates are valid
            if (st.StartDate >= Convert.ToDateTime("1/1/0002 00:00:00 AM") && st.EndDate <= DateTime.Now)
            {
                // Call the tranrecord method to fetch the statement list
                st.StatementList = tranrecord(st.StartDate, st.EndDate);
            }
            else
            {
                // Set an error message in the ViewData object
                ViewData["wrongdates"] = "Please Enter valid Dates!";

                // Return the Statement view with the error message
                return View();
            }

            // Return the Statement view with the populated Statement object
            return View(st);
        }

        public List<Statemomentum> tranrecord(DateTime startDate, DateTime endDate)
        {
            // Create a new SQL connection using the connection string
            SqlConnection con = new SqlConnection(GetConnectionString());
            con.Open();

            // Create a new SQL data adapter with the query and connection
            SqlDataAdapter _da = new SqlDataAdapter("Select T.TranDate, T.TranType, T.Amount From Transactions T Inner Join Account A On T.AccId = A.Id Inner Join Customer C On A.CustId = C.Id Where C.Username = " + $" '{Session["loginSession"]}' and T.TranDate Between '{startDate}' And '{endDate}'", con);
            DataTable dt = new DataTable();
            _da.Fill(dt);

            // Create a list to store the statement records
            List<Statemomentum> st = new List<Statemomentum>();

            // Iterate through each row in the DataTable and populate the list
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Statemomentum s = new Statemomentum();
                s.TranDate = Convert.ToDateTime(dt.Rows[i]["TranDate"]);
                s.TranType = dt.Rows[i]["TranType"].ToString();
                s.Amount = Convert.ToInt32(dt.Rows[i]["Amount"]);
                st.Add(s);
            }

            con.Close();

            // Return the statement records list
            return st;
        }

        public ActionResult Balance()
        {
            // Create a new instance of the login class
            login l = new login();

            // Get the connection string from the configuration file
            string constring = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            // Create a new SQL connection using the connection string
            SqlConnection con = new SqlConnection(GetConnectionString());

            // Check if the session value for the username exists
            string username = Session["loginSession"]?.ToString();

            if (username != null)
            {
                // Construct the query to retrieve the balance for the logged-in customer
                string query = $"SELECT A.Balance FROM Account A INNER JOIN Customer C ON A.CustId = C.Id WHERE C.Username = '{username}'";

                // Create a new SQL data adapter with the query and connection
                SqlDataAdapter _da = new SqlDataAdapter(query, con);
                DataTable dt = new DataTable();
                _da.Fill(dt);

                // Check if any rows were returned
                if (dt.Rows.Count > 0)
                {
                    // Retrieve the balance value from the first row
                    string bal = dt.Rows[0]["Balance"].ToString();

                    // Store the balance value in TempData
                    TempData["balance"] = bal;
                    TempData.Keep("balance");
                }
            }

            // Redirect to the BankPage action
            return RedirectToAction("BankPage");
        }

        public ActionResult Transfer()
        {
            // Return the Transfer view
            return View();
        }

        [HttpPost]
        public ActionResult Transfer(string AccountNumber, string IBAN, string AccountHolder, float Amount)
        {
            // Get the connection string from the configuration file
            string constring = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            // Create a new SQL connection using the connection string
            SqlConnection con = new SqlConnection(constring);
            con.Open();

            // Check the validity of account details
            SqlDataAdapter _da = new SqlDataAdapter("Select * From Customer C Inner Join Account A on C.Id = A.CustId Inner Join Branch B on A.BranchId = B.Id  Where A.AccNumber = " + $" '{AccountNumber}' and B.IBAN = '{IBAN}' and C.Name = '{AccountHolder}'", con);
            DataSet ds = new DataSet();
            _da.Fill(ds);

            if (ds != null && ds.Tables[0].Rows.Count != 0)
            {
                // Perform balance check
                SqlCommand oCmd = new SqlCommand("Select Balance from Account A Inner Join Customer C on C.Id = A.CustId Where C.Username = @username", con);
                oCmd.Parameters.AddWithValue("@username", Session["loginSession"]);
                SqlDataReader dr = oCmd.ExecuteReader();

                if (dr.Read())
                {
                    int bal = Convert.ToInt32(dr["Balance"]);

                    if (bal >= Amount)
                    {
                        dr.Close();

                        // Update transaction in the database
                        SqlCommand Cmd = new SqlCommand("UPDATE_TRANSACTION", con);
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Parameters.AddWithValue("@Username", Session["loginSession"]);
                        Cmd.Parameters.AddWithValue("@AccNumber", AccountNumber);
                        Cmd.Parameters.AddWithValue("@IBAN", IBAN);
                        Cmd.Parameters.AddWithValue("@AccHolder", AccountHolder);
                        Cmd.Parameters.AddWithValue("@Amount", Amount);
                        Cmd.Parameters.Add("@text", SqlDbType.Char, 500);
                        Cmd.Parameters["@text"].Direction = ParameterDirection.Output;

                        Cmd.ExecuteNonQuery();

                        if (ViewBag.text == null)
                        {
                            // Add transaction record to the database
                            SqlCommand cmds = new SqlCommand("ADD_TRANSACTION_RECORD", con);
                            cmds.CommandType = CommandType.StoredProcedure;
                            cmds.Parameters.AddWithValue("@Username", Session["loginSession"]);
                            cmds.Parameters.AddWithValue("@AccNumber", AccountNumber);
                            cmds.Parameters.AddWithValue("@Amount", Amount);

                            cmds.ExecuteNonQuery();

                            ViewBag.text = "Transaction Successful.";
                        }
                    }
                    else
                    {
                        ViewBag.text = "You don't have enough Balance!";
                    }
                }
            }
            else
            {
                ViewBag.text = "Please Check User Credentials!";
            }

            // Close the connection
            con.Close();

            // Return the view
            return View();
        }
    }
}