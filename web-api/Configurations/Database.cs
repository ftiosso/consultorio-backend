namespace web_api.Configurations
{
    public class Database
    {
        public static string GetConnectionString()
        {
            return System.Configuration.ConfigurationManager
                .ConnectionStrings["consultorio"].ConnectionString;
        }      
    }
}