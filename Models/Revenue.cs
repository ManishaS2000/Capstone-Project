using System.ComponentModel.DataAnnotations;

namespace recyclecollection.Models
{
    public class Revenue
    {
        public int Revenue_id { get; set; }



        public DateTime? date { get; set; }


        public string Category { get; set; }

        public string SubCategory { get; set; }

        public double Weight { get; set; }


        public required decimal sales { get; set; }


        public string BuyerName { get; set; }

    }
}
