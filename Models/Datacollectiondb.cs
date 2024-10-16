using System.Data;
using System.Data.SqlClient;

namespace recyclecollection.Models
{
    public class Datacollectiondb
    {
        SqlConnection con = new SqlConnection("Data Source=serveraddress;Initial Catalog=ROCK;Integrated Security=True");

        public string InsertCollection(Datacollection dc, out string msg)
        {
            msg = "";
            
            try
            {
                using (SqlCommand com = new SqlCommand("Sp_dataentry", con))
                {
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@Collection_id", dc.collection_id);

                    if (dc.date.HasValue)
                    {
                        if (dc.date < new DateTime(1753, 1, 1) || dc.date > new DateTime(9999, 12, 31))
                        {
                            msg = "date must be between 1/1/1753 and 12/31/9999.";
                            return msg; // Exit early if date is invalid
                        }
                        com.Parameters.AddWithValue("@Date", dc.date.Value); // Use Value to extract DateTime
                    }
                    else
                    {
                        com.Parameters.AddWithValue("@Date", DBNull.Value); // Handle null case
                    }

                    // Add other parameters to command
                    com.Parameters.AddWithValue("@Category", string.IsNullOrEmpty(dc.category) ? (object)DBNull.Value : dc.category);
                    com.Parameters.AddWithValue("@SubCategory", string.IsNullOrEmpty(dc.subcategory) ? (object)DBNull.Value : dc.subcategory);
                    com.Parameters.AddWithValue("@Weight", dc.weight);
                    com.Parameters.AddWithValue("@Recycle_Type", dc.recycle_type);
                    

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
