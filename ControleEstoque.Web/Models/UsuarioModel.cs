using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using ControleEstoque.Web.Helpers;
using System.Web.UI.WebControls;
using System.Data;
using System.Security.Principal;

namespace ControleEstoque.Web.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string Nome { get; set; }
        public static UsuarioModel validarUsuario(string login, string senha)
        {
            UsuarioModel retorno = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "Select * from usuario where  login = @login and senha=@senha";
                    comando.Parameters.Add("@login", SqlDbType.VarChar).Value = login;
                    comando.Parameters.Add("@senha", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(senha);

                    var reder = comando.ExecuteReader();

                    if (reder.Read())
                    {

                        retorno = new  UsuarioModel{
                            Id = (int)reder["id"],
                            Login = (string)reder["login"],
                            Nome = (string)reder["nome"]
                        };

                    }

                }
            }
            return retorno;

        }
    }
}