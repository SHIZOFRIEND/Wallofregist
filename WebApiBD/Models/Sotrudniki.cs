using System.Text.Json.Serialization;

namespace WebApiBD.Models
{
    public class Sotrudniki
    {
        [JsonIgnore]
        public int IDSotrydnika { get; set; }
        public int IDPolzovateliaDlyaAvtorizacii { get; set; }
        public int IDRoli { get; set; }
        public string Imya { get; set; }
        public string Familia { get; set; }
        public string Otchestvo { get; set; }
        public string NumberPhone { get; set; }
        public string Pochta { get; set; }
        public string SeriaPasporta { get; set; }
        public string NomerPasporta { get; set; }
    }
}
