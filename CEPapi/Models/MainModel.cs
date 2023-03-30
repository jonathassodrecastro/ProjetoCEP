using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CEPapi.Models
{
    [Table("CEP")]
    public class MainModel
    {
        [Column("id")]
        [Display(Name = "ID")]
        public int id { get; set; }
        [Column("cep")]
        [Display(Name = "CEP")]
        public string cep { get; set; }

        [Column("logradouro")]
        [Display(Name = "Logradouro")]
        public string logradouro { get; set; }

        [Column("complemento")]
        [Display(Name = "Complemento")]
        public string complemento { get; set; }

        [Column("bairro")]
        [Display(Name = "Bairro")]
        public string bairro { get; set; }

        [Column("localidade")]
        [Display(Name = "Cidade")]
        public string localidade { get; set; }

        [Column("uf")]
        [Display(Name = "UF")]
        public string uf { get; set; }

        [Column("unidade")]
        [Display(Name = "Unidade")]
        public long unidade { get; set; }

        [Column("ibge")]
        [Display(Name = "Código IBGE do Município")]
        public int ibge { get; set; }

        [Column("gia")]
        [Display(Name = "Código GIA")]
        public string gia { get; set; }


    }
}
