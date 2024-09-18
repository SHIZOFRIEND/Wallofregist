using System.Text.Json.Serialization;
namespace WebApiBD.Models
{
    public class Polzovateli
    {
        public int IDPolzovateliaDlyaAvtorizacii { get; set; }
        public int IDRoli { get; set; }  
        public string Logini { get; set; }
        public string Paroli { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int TwoFactorAvtor { get; set; }
        public Role Roli { get; set; }  
    }

    public class Role
    {
        public int IDRoli { get; set; }
        public string NazvanieRoli { get; set; }
    }
}
