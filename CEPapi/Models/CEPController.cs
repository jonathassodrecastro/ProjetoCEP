using Microsoft.EntityFrameworkCore;

namespace CEPapi.Models
{
    public class CEPController
    {
        //[Id] INT IDENTITY(1, 1) NOT NULL,

        //      [cep]         CHAR(9)       NULL,
        //        [logradouro] NVARCHAR(500) NULL,
        //        [complemento] NVARCHAR(500) NULL,
        //        [bairro] NVARCHAR(500) NULL,
        //        [localidade] NVARCHAR(500) NULL,
        //        [uf] CHAR(2)       NULL,
        //        [unidade]     BIGINT NULL,
        //        [ibge]        INT NULL,
        //        [gia]         NVARCHAR(500) NULL

        public int Id { get; set; } 
        public string Cep { get; set; } 
        public string Logradouro { get; set; }  
        public string Complemento { get; set; } 
        public string Bairro { get; set; }  
        public string Localidade { get; set; }
        public string Uf { get; set; }  
        public int Ibge { get; set; }   
        public string Gia { get; set; } 



    }
}
