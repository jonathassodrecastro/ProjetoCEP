using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ProjetoCEP.Model
{
    class ConexaoDB
    {
        //string de conexão
        private static SqlConnection Conexao;
        private string strconexao = ConfigurationManager.ConnectionStrings["ConexaoLocalDB"].ConnectionString;

        public static SqlConnection ObterConexao { get { return Conexao; } }

        public ConexaoDB() { Conexao = new SqlConnection(strconexao); }
    }
}
