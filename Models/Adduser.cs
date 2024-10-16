namespace recyclecollection.Models
{
    public class Adduser
    {
        public int  User_id { get; set; }
        public required string  username { get; set; }

        public required string password { get; set; }

        public required string confirmpassword { get; set; }
    }
}
