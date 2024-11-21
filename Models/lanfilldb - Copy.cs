using System.Data.SqlClient;
using System.Data;

namespace recyclecollection.Models
{
    public class lanfilldb
    {
        SqlConnection con = new SqlConnection("Data Source=NW59287;Initial Catalog=RECYCLEMANAGEMENT;Integrated Security=True");


        public string Insertlandfill(landfill dg, out string msg)
        {
            msg = "";
            try
            {
                SqlCommand com = new SqlCommand("Sp_AddLandfill", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@landfill_id", dg.landfill_id);
                com.Parameters.Add("@Date", SqlDbType.DateTime).Value = (object)dg.Date ?? DBNull.Value;
                com.Parameters.AddWithValue("@WEIGHT", dg.weight);
                com.Parameters.AddWithValue("@EXPENSE", dg.Expense);
                com.Parameters.AddWithValue("@COMPANYNAME", dg.COMPANYNAME ?? (object)DBNull.Value);
               
                con.Open();
                com.ExecuteNonQuery();
                con.Close();
                msg = "ok";
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
