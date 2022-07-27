using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;


namespace SimpliFi
{
    /// <summary>
    /// Summary description for Usage
    /// </summary>
    public class UsageData
    {
        public string conn = ConfigurationManager.ConnectionStrings["ConStr"].ConnectionString;

        public void LogUsage(Usage usage)
        {
            try
            {
                //Create connection and open it.
                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();

                    //create command object to pass the connection and other information
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    //set command type as stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    //pass the stored procedure name
                    cmd.CommandText = "LogUsage";

                    //pass the parameter to stored procedure
                    cmd.Parameters.Add(new SqlParameter("@LDAP", SqlDbType.VarChar)).Value = usage.LDAP;
                    cmd.Parameters.Add(new SqlParameter("@ProgramName", SqlDbType.VarChar)).Value = usage.ProgramName;
                    cmd.Parameters.Add(new SqlParameter("@ProgramType", SqlDbType.VarChar)).Value = usage.ProgramType;
                    cmd.Parameters.Add(new SqlParameter("@ValidatedOn", SqlDbType.DateTime)).Value = usage.ValidatedOn;
                    cmd.Parameters.Add(new SqlParameter("@ProofsCount", SqlDbType.Int)).Value = usage.ProofsCount;
                    cmd.Parameters.Add(new SqlParameter("@HTMLString", SqlDbType.NText)).Value = string.Empty;
                    cmd.Parameters.Add(new SqlParameter("@TimeTaken", SqlDbType.Int)).Value = usage.TimeTaken;

                    //Execute the query
                    int res = cmd.ExecuteNonQuery();

                    con.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public DataSet GetUsage(string LDAP, DateTime fromDate, DateTime toDate, string programType)
        {
            try
            {
                //Create connection and open it.
                using (SqlConnection con = new SqlConnection(conn))
                {
                    DataSet ds = new DataSet("UsageDetails");

                    con.Open();

                    //create command object to pass the connection and other information
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    //set command type as stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    //pass the stored procedure name
                    cmd.CommandText = "GetUsage";

                    //pass the parameter to stored procedure
                    cmd.Parameters.Add(new SqlParameter("@LDAP", SqlDbType.VarChar)).Value = LDAP;
                    cmd.Parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime)).Value = fromDate;
                    cmd.Parameters.Add(new SqlParameter("@ToDate", SqlDbType.DateTime)).Value = toDate;
                    cmd.Parameters.Add(new SqlParameter("@ProgramType", SqlDbType.VarChar)).Value = programType;

                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;

                    da.Fill(ds);

                    return ds;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DataSet GetUsageByLDAPS(DateTime fromDate, DateTime toDate)
        {
            try
            {
                //Create connection and open it.
                using (SqlConnection con = new SqlConnection(conn))
                {
                    DataSet ds = new DataSet("GetUsageByLDAPs");

                    con.Open();

                    //create command object to pass the connection and other information
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    //set command type as stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    //pass the stored procedure name
                    cmd.CommandText = "GetUsageByLDAPs";

                    //pass the parameter to stored procedure
                    cmd.Parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime)).Value = fromDate;
                    cmd.Parameters.Add(new SqlParameter("@ToDate", SqlDbType.DateTime)).Value = toDate;

                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;

                    da.Fill(ds);

                    return ds;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DataSet GetUsageByProgramTypes(DateTime fromDate, DateTime toDate)
        {
            try
            {
                //Create connection and open it.
                using (SqlConnection con = new SqlConnection(conn))
                {
                    DataSet ds = new DataSet("GetUsageByProgramTypes");

                    con.Open();

                    //create command object to pass the connection and other information
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    //set command type as stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    //pass the stored procedure name
                    cmd.CommandText = "GetUsageByProgramTypes";

                    //pass the parameter to stored procedure
                    cmd.Parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime)).Value = fromDate;
                    cmd.Parameters.Add(new SqlParameter("@ToDate", SqlDbType.DateTime)).Value = toDate;

                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;

                    da.Fill(ds);

                    return ds;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool CheckUserAccess(string LDAP, string password)
        {
            try
            {
                //Create connection and open it.
                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();

                    //create command object to pass the connection and other information
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    //set command type as stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    //pass the stored procedure name
                    cmd.CommandText = "CheckUserAccess";

                    //pass the parameter to stored procedure
                    cmd.Parameters.Add(new SqlParameter("@LDAP", SqlDbType.VarChar)).Value = LDAP;
                    cmd.Parameters.Add(new SqlParameter("@Password", SqlDbType.VarChar)).Value = password;

                    //Execute the query
                    bool isAllowed = Convert.ToBoolean(cmd.ExecuteScalar());

                    con.Close();

                    return isAllowed;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string GetUserPassword(string LDAP)
        {
            try
            {
                //Create connection and open it.
                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();

                    //create command object to pass the connection and other information
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    //set command type as stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    //pass the stored procedure name
                    cmd.CommandText = "GetUserPassword";

                    //pass the parameter to stored procedure
                    cmd.Parameters.Add(new SqlParameter("@LDAP", SqlDbType.VarChar)).Value = LDAP;

                    //Execute the query
                    string password = Convert.ToString(cmd.ExecuteScalar());

                    con.Close();

                    return password;
                }
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public DataSet GetUsers()
        {
            try
            {
                //Create connection and open it.
                using (SqlConnection con = new SqlConnection(conn))
                {
                    DataSet ds = new DataSet("dsUsers");

                    con.Open();

                    //create command object to pass the connection and other information
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    //set command type as stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    //pass the stored procedure name
                    cmd.CommandText = "GetUsers";

                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;

                    da.Fill(ds);

                    return ds;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int UpdateUserAccess(string LDAP, int access)
        {
            try
            {
                //Create connection and open it.
                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();

                    //create command object to pass the connection and other information
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    //set command type as stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    //pass the stored procedure name
                    cmd.CommandText = "AddUpdateUserAccess";

                    //pass the parameter to stored procedure
                    cmd.Parameters.Add(new SqlParameter("@LDAP", SqlDbType.VarChar)).Value = LDAP;
                    cmd.Parameters.Add(new SqlParameter("@IsAllowed", SqlDbType.Bit)).Value = access;

                    //Execute the query
                    int id = Convert.ToInt32(cmd.ExecuteScalar());

                    con.Close();

                    return id;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public List<string> GetURLS()
        {
            try
            {
                //Create connection and open it.
                using (SqlConnection con = new SqlConnection(conn))
                {
                    List<string> lstURLS = new List<string>();

                    con.Open();

                    //create command object to pass the connection and other information
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    //set command type as stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    //pass the stored procedure name
                    cmd.CommandText = "GetURLS";

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        lstURLS.Add(reader["URL"].ToString());
                    }

                    return lstURLS;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public DataSet GetWhitelistedURLS()
        {
            try
            {
                //Create connection and open it.
                using (SqlConnection con = new SqlConnection(conn))
                {
                    DataSet ds = new DataSet("dsURLS");

                    con.Open();

                    //create command object to pass the connection and other information
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    //set command type as stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    //pass the stored procedure name
                    cmd.CommandText = "GetURLS";

                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;

                    da.Fill(ds);

                    return ds;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int InsertURL(string url)
        {
            try
            {
                //Create connection and open it.
                using (SqlConnection con = new SqlConnection(conn))
                {
                    con.Open();

                    //create command object to pass the connection and other information
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    //set command type as stored procedure
                    cmd.CommandType = CommandType.StoredProcedure;

                    //pass the stored procedure name
                    cmd.CommandText = "InsertURl";

                    //pass the parameter to stored procedure
                    cmd.Parameters.Add(new SqlParameter("@URL", SqlDbType.VarChar)).Value = url;

                    //Execute the query
                    int res = cmd.ExecuteNonQuery();

                    con.Close();

                    return res;
                }
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
