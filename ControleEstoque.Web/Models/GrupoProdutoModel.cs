
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;






namespace ControleEstoque.Web.Models
{
    public class GrupoProdutoModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Preencha o nome.")]
        public string Nome { get; set; }

        public bool Ativo { get; set; }

        public static List<GrupoProdutoModel> RecuperarLista()
        {
            var returno = new List<GrupoProdutoModel>();

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "Select * from grupo_produto order by nome";
                    var reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        returno.Add(new GrupoProdutoModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Ativo = (bool)reader["ativo"]

                        });
                    }

                }
            }
            return returno;

        }

        public static GrupoProdutoModel RecuperarPeloId(int id)
        {
            GrupoProdutoModel retorno = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = string.Format("Select * from grupo_produto where id = {0}", id);
                    var reader = comando.ExecuteReader();

                    if (reader.Read())
                    {
                        retorno = new GrupoProdutoModel
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
                        comando.CommandText = string.Format("delete from grupo_produto where id = {0}", id);
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
                        comando.CommandText = string.Format("insert into grupo_produto (nome,ativo) values ('{0}',{1});select convert(int,scope_identity())", this.Nome, this.Ativo ? 1 : 0);
                        retorno = (int)comando.ExecuteScalar();

                    }
                    else
                    {
                        comando.CommandText = string.Format("update grupo_produto set nome = '{1}' , ativo = {2} where id = {0}", this.Id, this.Nome, this.Ativo ? 1 : 0);
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