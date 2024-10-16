using System.Data.SqlClient;
using System.Data;

namespace recyclecollection.Models
{
    public class AddUserdb
    {
        SqlConnection con = new SqlConnection("Data Source=serveraddress;Initial Catalog=ROCK;Integrated Security=True");

        
        public string ADDUSER(Adduser ar, out string msg)
        {
            
            msg = "";
            try
            {
                SqlCommand com = new SqlCommand("sp_Addusers", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@User_Id", ar.User_id);
                com.Parameters.AddWithValue("@UserName", ar.username);
                com.Parameters.AddWithValue("@Password", ar.password);
               
                con.Open();
                com.ExecuteNonQuery();
                con.Close();
                msg = "OK";
                return msg;
            }
            catch (Exception ex)
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                msg = ex.Message;
                return msg;
            }
        }
        }
    }
