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
    public class PaisModel
    {
        public int Id { get; set; }
       
        [Required(ErrorMessage = "Preencha o nome.")]
        public string Nome { get; set; }
      
        [Required(ErrorMessage = "Preencha o Código.")]
        public string Codigo { get; set; }
        public bool Ativo { get; set; }
        public static List<PaisModel> RecuperarLista(int pagIni, int qtdReg, string filtro = "")
        {
            var retorno = new List<PaisModel>();

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
                    comando.CommandText = string.Format(" Select * from pais {0} order by nome offset {1} rows fetch next {2} rows only ", whereFiltro, pos, qtdReg);
                    var reader = comando.ExecuteReader();

                    while (reader.Read())
                    {
                        retorno.Add(new PaisModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Codigo = (string)reader["codigo"],
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
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "Select count(*) from pais";
                    retorno = (int)comando.ExecuteScalar();


                }
            }
            return retorno;

        }
        public static PaisModel RecuperarPeloId(int id)
        {
            PaisModel retorno = null;

            using (var conexao = new SqlConnection())
            {
                conexao.ConnectionString = ConfigurationManager.ConnectionStrings["principal"].ConnectionString;
                conexao.Open();

                using (var comando = new SqlCommand())
                {
                    comando.Connection = conexao;
                    comando.CommandText = "Select * from pais where id = @id";
                    comando.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    var reader = comando.ExecuteReader();

                    if (reader.Read())
                    {
                        retorno = new PaisModel
                        {
                            Id = (int)reader["id"],
                            Nome = (string)reader["nome"],
                            Codigo = (string)reader["codigo"],
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
                        comando.CommandText = "delete from pais where id =@id";
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
                        comando.CommandText = "insert into pais (nome,codigo,ativo) values (@nome,@codigo,@ativo);select convert(int,scope_identity())";
                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@codigo", SqlDbType.VarChar).Value = this.Codigo;
                        comando.Parameters.Add("@ativo", SqlDbType.Bit).Value = (this.Ativo ? 1 : 0);
                        retorno = (int)comando.ExecuteScalar();

                    }
                    else
                    {
                        comando.CommandText = "update grupo_produto set nome = @nome , codigo = @codigo, ativo = @ativo where id = @id";
                        comando.Parameters.Add("@nome", SqlDbType.VarChar).Value = this.Nome;
                        comando.Parameters.Add("@codigo", SqlDbType.VarChar).Value = this.Codigo;
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