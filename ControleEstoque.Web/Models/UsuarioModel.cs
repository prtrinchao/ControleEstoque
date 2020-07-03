using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Configuration;
using ControleEstoque.Web.Helpers;
using System.Web.UI.WebControls;
using System.Data;

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
                    comando.CommandText = "Select count(*) from usuario where  login = @login and senha=@senha";
                    comando.Parameters.Add("@login", SqlDbType.VarChar).Value = login;
                    comando.Parameters.Add("@senha", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(senha);

                    returno = ((int)comando.ExecuteScalar() > 0);

                }
            }
            return returno;

        }
    }
}