﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace ProjetoCEP
{
    class Program
    {
        static void Main(string[] args)
        {
            


            //TODO: Implementar forma de fazer o usuário poder errar várias vezes o CEP informado
            //TODO: Melhorar validação do CEP.

            string cep;
            string result = string.Empty;


            do
            {
                Console.WriteLine("Informe um CEP válido. O CEP deve ter 8 dígitos e apenas números:");
                cep = Console.ReadLine();
            }
            while (!validaCep(cep));

            //

            //Exemplo CEP 13050020
            string viaCEPUrl = "https://viacep.com.br/ws/" + cep + "/json/";

            //TODO: Resolver dados com caracter especial no retorno do JSON 
            WebClient client = new WebClient();
            result = client.DownloadString(viaCEPUrl);



            JObject jsonRetorno = JsonConvert.DeserializeObject<JObject>(result);
         


            //TODO: Validar CEP existente
            string query = "INSERT INTO [dbo].[CEP] ([cep], [logradouro], [complemento], [bairro], [localidade], [uf], [unidade], [ibge], [gia]) VALUES (";
            query = query + "'" + jsonRetorno["cep"] + "'";
            query = query + ",'" + jsonRetorno["logradouro"] + "'";
            query = query + ",'" + jsonRetorno["complemento"] + "'";
            query = query + ",'" + jsonRetorno["bairro"] + "'";
            query = query + ",'" + jsonRetorno["localidade"] + "'";
            query = query + ",'" + jsonRetorno["uf"] + "'";
            query = query + ",'" + jsonRetorno["unidade"] + "'";
            query = query + ",'" + jsonRetorno["ibge"] + "'";
            query = query + ",'" + jsonRetorno["gia"] + "'" + ")";

            SqlConnection connection = new SqlConnection("Data Source=(localdb)\\LocalDB;Initial Catalog=CEP;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            SqlCommand sqlCommand = new SqlCommand(query, connection);

            sqlCommand.CommandType = CommandType.Text;

            try
            {
                connection.Open();

                sqlCommand.ExecuteNonQuery();
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


            Console.WriteLine();

            //TODO: Retornar os dados do CEP infomado no início para o usuário

            Console.WriteLine("Deseja visualizar todos os CEPs alguma UF? Se sim, informar UF, se não, informar sair.");
            string resposta = Console.ReadLine();

            if (resposta == "sair")
            {
                return;
            }

            if (resposta.Length > 2)
            {
                Console.WriteLine("UF inválida");
                resposta = Console.ReadLine();
            }

            connection = new SqlConnection("Data Source=(localdb)\\LocalDB;Initial Catalog=CEP;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");
            sqlCommand = new SqlCommand("Select * from CEP", connection);

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


    }
}
