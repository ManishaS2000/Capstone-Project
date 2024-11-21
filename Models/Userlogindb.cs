using System.Data;
using System.Data.SqlClient;

namespace recyclecollection.Models
{
    public class Userlogindb
    {
        SqlConnection con = new SqlConnection("Data Source=NW59287;Initial Catalog=RECYCLEMANAGEMENT;Integrated Security=True");

        public int Login(UserLogin ad)
        {
            try
            {
                using (SqlCommand com = new SqlCommand("sp_CheckUserLogin", con))
                {
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@User_id", ad.User_id);
                    com.Parameters.AddWithValue("@Password", ad.Password);
                    com.Parameters.AddWithValue("@Usertype", 0);

                    SqlParameter oblogin = new SqlParameter
                    {
                        ParameterName = "@Isvalid",
                        SqlDbType = SqlDbType.Bit,
                        Direction = ParameterDirection.Output
                    };
                    com.Parameters.Add(oblogin);

                    con.Open();
                    com.ExecuteNonQuery();
                    int res = Convert.ToInt32(oblogin.Value);
                    return res;
                }
            }
            catch (SqlException ex)
            {
                // Handle SQL exceptions
                Console.WriteLine($"SQL Error: {ex.Message}");
                return -1; // Or some other error code
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"Error: {ex.Message}");
                return -1; // Or some other error code
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close(); // Ensure the connection is closed
                }
            }
        }





    }
}
