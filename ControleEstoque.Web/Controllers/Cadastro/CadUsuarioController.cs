using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ControleEstoque.Web.Models;




namespace ControleEstoque.Web.Controllers
{

    [Authorize(Roles = "Gerente")]
    public class CadUsuarioController : Controller
    {
        private const int QtdMaxLinhaPorPagina = 5;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RecuperarUsuario(int id)
        {
            var ret = UsuarioModel.RecuperarPeloId(id);
            ret.CarregarPerfisUsuario();
            return Json(ret);
        }

        [HttpPost]       
        [ValidateAntiForgeryToken]
        public ActionResult ExcluirUsuario(int id)
        {

            return Json(UsuarioModel.ExcluirPeloId(id));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SalvarUsuario(UsuarioModel model)
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

        [HttpPost]
        [Authorize(Roles = "Gerente;Administrativo;Operador")]
        [ValidateAntiForgeryToken]
        public ActionResult ResetSenhaUsuario(int id, string senha)
        {
            return Json(UsuarioModel.ResetSenha(id, senha));
        }

        [Authorize]
        public ActionResult Index()
        {
            ViewBag.ListaTamPag = new SelectList(new int[] { QtdMaxLinhaPorPagina, 10, 15, 20 }, QtdMaxLinhaPorPagina);
            ViewBag.QtdMaxLinhaPorPagina = QtdMaxLinhaPorPagina;
            ViewBag.PaginaAtual = 1;
            var qtdRegistros = UsuarioModel.QtdRegistros();
            ViewBag.QtdPaginas = (qtdRegistros > ViewBag.QtdMaxLinhaPorPagina) ? ((qtdRegistros / ViewBag.QtdMaxLinhaPorPagina) + ((qtdRegistros % ViewBag.QtdMaxLinhaPorPagina > 0) ? 1 : 0)) : 1;

            ViewBag.ListarPerfil = PerfilModel.ListarPerfil();

            var lista = UsuarioModel.RecuperarLista(ViewBag.PaginaAtual, QtdMaxLinhaPorPagina);


            return View(lista);
        }

        public JsonResult UsuarioPagina(int pagina, int maxPag, string filtro)
        {
            var lista = UsuarioModel.RecuperarLista(pagina, maxPag,filtro);

            return Json(lista);
        }



    }
}