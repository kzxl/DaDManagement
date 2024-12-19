namespace Models
{
    public partial class dbDaDDataContext
    {              
        public static string connectionString = "server=192.168.15.40\\SQLEXPRESS;database=DaDManagement;user id = testing;password=268479#Kzx;Connection Timeout=30";

        public static dbDaDDataContext New()
        {
            return new dbDaDDataContext(connectionString);
        }
    }
}