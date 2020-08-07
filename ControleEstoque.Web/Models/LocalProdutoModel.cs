using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace ControleEstoque.Web.Models
{
    public class LocalProdutoModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Preencha o nome.")]
        public string Nome { get; set; }

        public bool Ativo { get; set; }

        public static List<LocalProdutoModel> RecuperarLista(int pagIni, int qtdReg, string filtro ="")
        {
            var retorno = new List<LocalProdutoModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
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
                    comando.CommandText = string.Format(" Select * from local_produto {0} order by nome offset {1} rows fetch next {2} rows only ",whereFiltro, pos, qtdReg);
                    var reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        retorno.Add(new LocalProdutoModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Ativo = (bool)reader["ativo"]

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
                conexao.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "Select count(*) from local_produto";
                    retorno = (int)comando.ExecuteScalar();


                }
            }
            return retorno;

        }


        public static LocalProdutoModel RecuperarPeloId(int id)
        {
            LocalProdutoModel retorno = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "Select * from local_produto where id = @id";
                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    var reader = comando.ExecuteReader();

                    if (reader.Read())
                    {
                        retorno = new LocalProdutoModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Ativo = (bool)reader["ativo"]

                        };
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
                        comando.CommandText = "delete from local_Produto where id =@id";
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
                        comando.CommandText = "insert into local_produto (nome,ativo) values (@nome,@ativo);select convert(int,scope_identity())";
                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@ativo", SqlDbType.Bit).Value = (this.Ativo ? 1 : 0);
                        retorno = (int)comando.ExecuteScalar();

                    }
                    else
                    {
                        comando.CommandText = "update local_produto set nome = @nome , ativo = @ativo where id = @id";
                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@ativo", SqlDbType.Bit).Value = (this.Ativo ? 1 : 0);
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

    }
}