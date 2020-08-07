using ControleEstoque.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ControleEstoque.Web.Controllers.Cadastro
{
    public class CadUnidadeMedidaController : Controller
    {
        private const int QtdMaxLinhaPorPagina = 5;


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult RecuperarUnidadeMedida(int id)
        {
            return Json(UnidadeMedidaModel.RecuperarPeloId(id));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult ExcluirUnidadeMedida(int id)
        {

            return Json(UnidadeMedidaModel.ExcluirPeloId(id));
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult SalvarUnidadeMedida(UnidadeMedidaModel model)
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


        [Authorize]
        public ActionResult Index()
        {

            ViewBag.ListaTamPag = new SelectList(new int[] { QtdMaxLinhaPorPagina, 10, 15, 20 }, QtdMaxLinhaPorPagina);
            ViewBag.QtdMaxLinhaPorPagina = QtdMaxLinhaPorPagina;
            ViewBag.PaginaAtual = 1;
            var qtdRegistros = UnidadeMedidaModel.QtdRegistros();
            ViewBag.QtdPaginas = (qtdRegistros > ViewBag.QtdMaxLinhaPorPagina) ? ((qtdRegistros / ViewBag.QtdMaxLinhaPorPagina) + ((qtdRegistros % ViewBag.QtdMaxLinhaPorPagina > 0) ? 1 : 0)) : 1;

            var lista = UnidadeMedidaModel.RecuperarLista(ViewBag.PaginaAtual, QtdMaxLinhaPorPagina);

            return View(lista);
        }

        public JsonResult UnidadeMedidaPagina(int pagina, int maxPag, string filtro)
        {
            var lista = UnidadeMedidaModel.RecuperarLista(pagina, maxPag,filtro);

            return Json(lista);
        }


    }
}