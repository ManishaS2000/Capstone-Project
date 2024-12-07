namespace recyclecollection.Models
{
    public class UserLogin
    {
        public required string User_id { get; set; }

        public required string Password { get; set; }

        public int UserType { get; set; } // 0 for User, 1 for Admin
    }
}
