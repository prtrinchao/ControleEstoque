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
using System.ComponentModel.DataAnnotations;
using Microsoft.Ajax.Utilities;

namespace ControleEstoque.Web.Models
{
    public class UsuarioModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Informe o Login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Informe o Nome")]
        public string Nome { get; set; }
        public string Senha { get; set; }

        public List<PerfilModel> Perfis { get; set; }

        public UsuarioModel()
        {
            Perfis = new List<PerfilModel>();
        }
        public static UsuarioModel ValidarUsuario(string login, string senha)
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

                    var reader = comando.ExecuteReader();

                    if (reader.Read())
                    {

                        retorno = new UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Login = (string)reader["login"],
                            Nome = (string)reader["nome"]
                        };

                    }

                }
            }
            return retorno;

        }

        public static List<UsuarioModel> RecuperarLista(int pagIni, int qtdReg, string filtro = "")
        {
            var retorno = new List<UsuarioModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new SqlCommand())
                {
                    var pos = (pagIni - 1) * qtdReg;
                    var whereFiltro = "";
                    if (!string.IsNullOrEmpty(filtro))
                    {
                        whereFiltro = string.Format("where lower(nome) like '%{0}%'", filtro.ToLower());
                    }

                    comando.Connection = conexao;
                    comando.CommandText = string.Format(" Select * from usuario {0} order by nome offset {1} rows fetch next {2} rows only ",whereFiltro, pos, qtdReg);

                    var reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        retorno.Add(new UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Login = (string)reader["login"],
                            Nome = (string)reader["nome"]

                        });
                    }

                }
            }
            return retorno;

        }

        public static int QtdRegistros()
        {
            var retorno = 0;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "Select count(*) from usuario";
                    retorno = (int)comando.ExecuteScalar();


                }
            }
            return retorno;

        }

        public static UsuarioModel RecuperarPeloId(int id)
        {
            UsuarioModel retorno = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "Select * from usuario where id = @id";
                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    var reader = comando.ExecuteReader();

                    if (reader.Read())
                    {
                        retorno = new UsuarioModel
                        {
                            Id = (int)reader["id"],
                            Login = (string)reader["login"],
                            Nome = (string)reader["nome"],
                            Senha = (string)reader["senha"]
                        };
                    }

                }
            }
            return retorno;

        }

        public static bool ResetSenha(int id, string newSenha)
        {
            var retorno = false;


            if (RecuperarPeloId(id) != null)
            {
                using (var conexao = new SqlConnection())
                {
                    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                    conexao.Open();

                    using (var comando = new SqlCommand())
                    {
                        comando.Connection = conexao;
                        comando.CommandText = "update usuario set senha = @senha where id =@id";
                        comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
                        comando.Parameters.Add("@senha", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(newSenha);
                        retorno = (comando.ExecuteNonQuery() > 0);

                    }
                }

            }

            return retorno;

        }



        public static bool ExcluirPeloId(int id)
        {
            var retorno = false;


            if (RecuperarPeloId(id) != null)
            {
                using (var conexao = new SqlConnection())
                {
                    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                    conexao.Open();

                    using (var comando = new SqlCommand())
                    {
                        comando.Connection = conexao;
                        comando.CommandText = "delete from usuario where id =@id";
                        comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
                        retorno = (comando.ExecuteNonQuery() > 0);

                    }
                }

            }

            return retorno;

        }

        public int Salvar()
        {
            var retorno = 0;
            var model = RecuperarPeloId(this.Id);


            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();


                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;

                    if (model == null)
                    {
                        comando.CommandText = "insert into usuario (login,nome,senha) values (@login,@nome,@senha);select convert(int,scope_identity())";
                        comando.Parameters.Add("@login", SqlDbType.VarChar).Value = this.Login;
                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@senha", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(this.Senha);
                        retorno = (int)comando.ExecuteScalar();

                    }
                    else
                    {
                        comando.CommandText = "update usuario set login = @login , nome = @nome " +
                         (!string.IsNullOrEmpty(this.Senha) ? ", senha = @senha" : "") +
                            " where id = @id";
                        comando.Parameters.Add("@login", SqlDbType.VarChar).Value = this.Login;
                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        if (!string.IsNullOrEmpty(this.Senha))
                        {
                            comando.Parameters.Add("@senha", SqlDbType.VarChar).Value = CriptoHelper.HashMD5(this.Senha);

                        }
                        comando.Parameters.Add("@id", SqlDbType.Int).Value = this.Id;

                        if (comando.ExecuteNonQuery() > 0)
                        {
                            retorno = this.Id;

                        }


                    }

                }
            }


            return retorno;


        }
        public string RecuperarPerfisUsuario()
        {
            var retorno = string.Empty;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "Select * from perfil p join usuario_perfil up on up.id_perfil = p.id  where up.id_usuario = @id";
                    comando.Parameters.Add("@id", SqlDbType.Int).Value = this.Id;
                    var reader = comando.ExecuteReader();

                    if (reader.Read())
                    {
                        retorno += (retorno != string.Empty ? ";" : "") + (string)reader["nome"];

                    }

                }
            }
            return retorno;

        }

        public List<PerfilModel> CarregarPerfisUsuario()
        {
            Perfis.Clear();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "select p.id, p.nome from usuario_perfil up join perfil p on up.id_perfil = p.id where up.id_usuario = @id";
                    comando.Parameters.Add("@id", SqlDbType.Int).Value = this.Id;
                    var reader = comando.ExecuteReader();

                    if (reader.Read())
                    {
                        Perfis.Add(new PerfilModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"]
                        });
                    }

                }
            }
            return Perfis;

        }

    }



}
