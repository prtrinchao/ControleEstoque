using ControleEstoque.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers
{
    [Authorize(Roles = "Gerente,Administrativo,Operador")]
    public class CadLocalProdutoController : Controller
    {
        private const int QtdMaxLinhaPorPagina = 5;


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecuperarLocalProduto(int id)
        {
            return Json(LocalProdutoModel.RecuperarPeloId(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Gerente,Administrativo")]
        public ActionResult ExcluirLocalProduto(int id)
        {

            return Json(LocalProdutoModel.ExcluirPeloId(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SalvarLocalProduto(LocalProdutoModel model)
        {
            var resultado = "OK";
            var mensagens = new List<string>();
            var idSalvo = string.Empty;
            if (!ModelState.IsValid)
            {
                resultado = "AVISO";
                mensagens = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
            }
            else
            {
                try
                {

                    var id = model.Salvar();

                    if (id > 0)
                    {
                        idSalvo = id.ToString();
                    }
                    else
                    {
                        resultado = "ERRO";
                    }
                }
                catch (Exception ex)
                {
                    resultado = "ERRO";
                }

            }
            return Json(new { Resultado = resultado, Mensagens = mensagens, IdSalvo = idSalvo });
        }

        public ActionResult Index()
        {

            ViewBag.ListaTamPag = new SelectList(new int[] { QtdMaxLinhaPorPagina, 10, 15, 20 }, QtdMaxLinhaPorPagina);
            ViewBag.QtdMaxLinhaPorPagina = QtdMaxLinhaPorPagina;
            ViewBag.PaginaAtual = 1;
            var qtdRegistros = LocalProdutoModel.QtdRegistros();
            ViewBag.QtdPaginas = (qtdRegistros > ViewBag.QtdMaxLinhaPorPagina) ? ((qtdRegistros / ViewBag.QtdMaxLinhaPorPagina) + ((qtdRegistros % ViewBag.QtdMaxLinhaPorPagina > 0) ? 1 : 0)) : 1;

            var lista = LocalProdutoModel.RecuperarLista(ViewBag.PaginaAtual, QtdMaxLinhaPorPagina);

            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult LocalProdutoPagina(int pagina, int maxPag, string filtro)
        {
            var lista = LocalProdutoModel.RecuperarLista(pagina, maxPag,filtro);

            return Json(lista);
        }


    }
}