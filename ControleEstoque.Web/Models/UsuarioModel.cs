using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;

namespace ControleEstoque.Web.Models
{
    public class UsuarioModel
    {
        public static bool validarUsuario(string login, string senha)
        {
            var returno = false;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = string.Format(
                        "Select count(*) from usuario where  login = '{0}' and senha='{1}'", login, senha);

                    returno = ((int)comando.ExecuteScalar() > 0);

                }
            }
            return returno;

        }
    }
}