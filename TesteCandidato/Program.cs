using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace ProjetoCEP
{
    class Program
    {
        static void Main(string[] args)
        {
            bool sair = true;
            do
            {
                string cep;
                string result;

                do
                {
                    Console.WriteLine("Informe um CEP válido. O CEP deve ter 8 dígitos e apenas números:");
                    cep = Console.ReadLine();
                }
                while (!validaCep(cep));


                string viaCEPUrl = "https://viacep.com.br/ws/" + cep + "/json/";

                //TODO: Resolver dados com caracter especial no retorno do JSON 
                WebClient client = new WebClient();
                result = client.DownloadString(viaCEPUrl);

                JObject jsonRetorno = JsonConvert.DeserializeObject<JObject>(trataCaracteres(result, viaCEPUrl));

                //valida se um cep realmente existe ou se apenas está no formato correto
                while (!buscaCEP(jsonRetorno))
                {
                    Console.WriteLine("O CEP está no formato correto, porém não é um CEP existente.");
                    cep = Console.ReadLine();
                }


                //TODO: Retornar os dados do CEP infomado no início para o usuário

                Console.WriteLine("Deseja visualizar todos os CEPs alguma UF? Se sim, informar UF, se não, digite: 'novo' para cadastrar um novo endereço.");
                string resposta = Console.ReadLine();

                buscaCEPporUF(resposta);

                Console.WriteLine("Para encerrar o programa digite: sair. Para novo endereço digite qualquer tecla.");
                string encerra = Console.ReadLine();

                if (encerra == "sair") { sair = false; }
            } while (sair);
        }

        //Função para receber o CEP, validar e tratar o erro
        public static bool validaCep(string cep)
        {
            if (cep.Length != 8)
            {
                Console.WriteLine("CEP Inválido. O CEP contém 8 dígitos.");
                return false;
            }
            if (!int.TryParse(cep, out int _))
            {
                Console.WriteLine("CEP inválido: deve conter somente números.");
                return false;
            }
            return true;
        }

        public static bool buscaCEP(JObject jsonRetorno)
        {
            if (jsonRetorno == null || jsonRetorno.Count <= 0)
            {
                return false;
            }
            //TODO: Validar CEP existente
            string query = "SELECT * FROM [dbo].[CEP] WHERE cep = "+"'"+jsonRetorno["cep"]+"'";

            SqlConnection connection = new SqlConnection("Data Source=(localdb)\\LocalDB;Initial Catalog=CEP;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            SqlCommand sqlCommand = new SqlCommand(query, connection);

            try
            {
                connection.Open();

                SqlDataReader reader = sqlCommand.ExecuteReader();

                if (reader.HasRows)
                {
                    while(reader.Read()) {

                        string endereco;
                        string cep = reader.GetString(reader.GetOrdinal("cep"));
                        string logradouro = reader.GetString(reader.GetOrdinal("logradouro"));
                        string bairro = reader.GetString(reader.GetOrdinal("bairro"));
                        string cidade = reader.GetString(reader.GetOrdinal("localidade"));
                        string estado = reader.GetString(reader.GetOrdinal("estado"));

                        endereco = $"Endereço cadastrado anteriormente: CEP:{cep} - {logradouro} - {bairro} - {cidade}/{estado}";
                        Console.WriteLine(endereco);
                    }

                    connection.Close();
                }
                else {

                    

                    query = "INSERT INTO [dbo].[CEP] ([cep], [logradouro], [complemento], [bairro], [localidade], [uf], [unidade], [ibge], [gia]) VALUES (";
                    query = query + "'" + jsonRetorno["cep"] + "'";
                    query = query + ",'" + jsonRetorno["logradouro"] + "'";
                    query = query + ",'" + jsonRetorno["complemento"] + "'";
                    query = query + ",'" + jsonRetorno["bairro"] + "'";
                    query = query + ",'" + jsonRetorno["localidade"] + "'";
                    query = query + ",'" + jsonRetorno["uf"] + "'";
                    query = query + ",'" + jsonRetorno["unidade"] + "'";
                    query = query + ",'" + jsonRetorno["ibge"] + "'";
                    query = query + ",'" + jsonRetorno["gia"] + "'" + ")";

                    //sqlCommand = new SqlCommand(query, connection);

                    sqlCommand.CommandType = CommandType.Text;

                    sqlCommand.ExecuteNonQuery();

                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                connection.Close();
            }

            
            sqlCommand.Dispose();
            connection.Close();
            connection.Dispose();


            Console.WriteLine(jsonRetorno);

            return true;
        }

        public static void buscaCEPporUF(string resposta)
        {
            if (resposta == "novo")
            {
                return;
            }
            if (resposta.Length > 2)
            {
                Console.WriteLine("UF inválida");
                resposta = Console.ReadLine();
            }

            SqlConnection connection = new SqlConnection("Data Source=(localdb)\\LocalDB;Initial Catalog=CEP;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            SqlCommand sqlCommand = new SqlCommand("Select * from CEP", connection);

            SqlDataAdapter adapter = new SqlDataAdapter();
            DataSet ds = new DataSet();
            DataView dv;
            sqlCommand.CommandType = CommandType.Text;

            try
            {
                connection.Open();
                adapter.SelectCommand = sqlCommand;
                adapter.Fill(ds, "Create DataView");
                adapter.Dispose();

                dv = ds.Tables[0].DefaultView;

                for (int i = 0; i < dv.Count; i++)
                {
                    if (dv[i]["uf"].ToString() == resposta)
                    {
                        if (i == 0)
                        {
                            Console.Write(dv[i]["cep"]);
                        }
                        else
                        {
                            Console.Write(";" + dv[i]["cep"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            finally
            {
                connection.Close();
            }

            sqlCommand.Dispose();
            connection.Close();
            connection.Dispose();

            Console.ReadLine();
        }

        public static string trataCaracteres(string result, string viaCEPurl)
        {
            result = HttpUtility.HtmlDecode(result);
            string url = viaCEPurl;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:58.0) Gecko/20100101 Firefox/58.0";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            result = reader.ReadToEnd();

            return result;
        }

    }
}
