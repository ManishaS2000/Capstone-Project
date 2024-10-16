using System.Data;
using System.Data.SqlClient;

namespace recyclecollection.Models
{
    public class Revenuedb
    {
        SqlConnection con = new SqlConnection("Data Source=serveraddress;Initial Catalog=ROCK;Integrated Security=True");
        public string InsertRevenue(Revenue rs, out string msg)
        {
            msg = "";

            try
            {
                using (SqlCommand com = new SqlCommand("Sp_Revenue", con))
                {
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@Revenue_id", rs.Revenue_id);

                    // Handle Sales_date parameter
                    if (rs.date.HasValue) // Check if Sales_date has a value
                    {
                        if (rs.date < new DateTime(1753, 1, 1) || rs.date > new DateTime(9999, 12, 31))
                        {
                            msg = "date must be between 1/1/1753 and 12/31/9999.";
                            return msg; // Exit early if date is invalid
                        }
                        com.Parameters.AddWithValue("@date", rs.date.Value); // Use Value to extract DateTime
                    }
                    else
                    {
                        com.Parameters.AddWithValue("@date", DBNull.Value); // Handle null case
                    }

                    // Add other parameters to command
                    com.Parameters.AddWithValue("@Category", string.IsNullOrEmpty(rs.Category) ? (object)DBNull.Value : rs.Category);
                    com.Parameters.AddWithValue("@SubCategory", string.IsNullOrEmpty(rs.SubCategory) ? (object)DBNull.Value : rs.SubCategory);
                    com.Parameters.AddWithValue("@Weight", rs.Weight);
                    com.Parameters.AddWithValue("@Revenue", rs.sales);
                    com.Parameters.AddWithValue("@BuyerName", string.IsNullOrEmpty(rs.BuyerName) ? (object)DBNull.Value : rs.BuyerName);
                    

                    // Open connection and execute command
                    con.Open();
                    com.ExecuteNonQuery();
                    msg = "OK";
                }
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }

            return msg;
        }

    }
}
