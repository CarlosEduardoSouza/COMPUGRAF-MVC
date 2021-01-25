using MvcContatos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace MvcContatos.Controllers
{
    public class PessoasController : Controller
    {
        // GET: Pessoas
        public ActionResult Index(int? pagina)
        {
            int paginaTamanho = 10;
            int paginaNumero = (pagina ?? 1);

            IEnumerable<PessoaViewModel> pessoas = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:64314/api/");

                //HTTP GET
                var responseTask = client.GetAsync("pessoas");
                responseTask.Wait();
                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<PessoaViewModel>>();
                    readTask.Wait();
                    pessoas = readTask.Result;
                }
                else
                {
                    pessoas = Enumerable.Empty<PessoaViewModel>();
                    ModelState.AddModelError(string.Empty, "Erro no servidor. Contate o Administrador.");
                }
                return View(pessoas.ToPagedList(paginaNumero, paginaTamanho));
            }
        }

        [HttpGet]
        public ActionResult create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult create(PessoaViewModel pessoa)
        {
            //if (pessoa == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            if (pessoa.CEP != null && pessoa.CPF != null && pessoa.Telefone != null)
            {
                pessoa.CEP = pessoa.CEP.Replace("-", "").Trim();
                pessoa.CPF = pessoa.CPF.Replace(".", "").Replace("-", "").Trim();
                pessoa.Telefone = pessoa.Telefone.Replace("(", "").Replace(")", "").Replace("-", "").Trim();
            }

            if (ModelState.IsValid)
            {

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:64314/api/");
                    //HTTP POST
                    var postTask = client.PostAsJsonAsync<PessoaViewModel>("pessoas", pessoa);
                    postTask.Wait();
                    var result = postTask.Result;

                    //VERIFICA O RETORNO => SE JÁ EXISTE CPF CADASTRADO
                    if (!result.IsSuccessStatusCode)
                    {
                        ModelState.AddModelError("CPF", "Ja existe um CPF cadastrado com esse número");
                    }

                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            //ModelState.AddModelError(string.Empty, "Erro no Servidor. Contacte o Administrador.");
            return View(pessoa);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PessoaViewModel pessoa = null;
            using (var client = new HttpClient())
            {  
                client.BaseAddress = new Uri("http://localhost:64314/api/pessoas");
                //HTTP GET
                var responseTask = client.GetAsync("?id=" + id.ToString());
                responseTask.Wait();
                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<PessoaViewModel>();
                    readTask.Wait();
                    pessoa = readTask.Result;
                }
            }
            return View(pessoa);
        }

        [HttpPost]
        public ActionResult Edit(PessoaViewModel pessoa)
        {
            //if (pessoa == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}

            if (pessoa.CEP != null && pessoa.CPF != null && pessoa.Telefone != null)
            {
                pessoa.CEP = pessoa.CEP.Replace("-", "").Trim();
                pessoa.CPF = pessoa.CPF.Replace(".", "").Replace("-", "").Trim();
                pessoa.Telefone = pessoa.Telefone.Replace("(", "").Replace(")", "").Replace("-", "").Trim();
            }

            if (ModelState.IsValid)
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:64314/api/");
                    //HTTP PUT
                    var putTask = client.PutAsJsonAsync<PessoaViewModel>("pessoas", pessoa);
                    putTask.Wait();
                    var result = putTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(pessoa);
          
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PessoaViewModel pessoa = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:64314/api/");
                //HTTP DELETE
                var deleteTask = client.DeleteAsync("pessoas/" + id.ToString());
                deleteTask.Wait();
                var result = deleteTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(pessoa);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PessoaViewModel pessoa = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:64314/api/pessoas");

                //HTTP GET
                var responseTask = client.GetAsync("?id=" + id.ToString());
                responseTask.Wait();
                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<PessoaViewModel>();
                    readTask.Wait();
                    pessoa = readTask.Result;
                }
            }
            return View(pessoa);
        }
    }
}
