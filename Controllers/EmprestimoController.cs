using Microsoft.AspNetCore.Mvc;
using BibliotecaMvc.Models;
using System.Net.Http.Headers;
using Newtonsoft.Json;


namespace BibliotecaMvc.Controllers
{
    public class EmprestimoController : Controller
    {
        private readonly string uriBase = "http://localhost:5175/api/Emprestimo/";

        private async Task<EmprestismoDeLivros> ObterEmprestimoPorIdAsync(int? idTransacao)
        {
            if (idTransacao == null)
            {
                return null;
            }

            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = await httpClient.GetAsync(uriBase + $"obterEmprestimoPorId/{idTransacao}");

                if (response.IsSuccessStatusCode)
                {
                    var serialized = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<EmprestismoDeLivros>(serialized);
                }
                else
                {
                    TempData["MensagemErro"] = response.ReasonPhrase;
                    return null;
                }
            }
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.GetAsync(uriBase + "obterTodosOsEmprestimos");

                    if (response.IsSuccessStatusCode)
                    {
                        var serialized = await response.Content.ReadAsStringAsync();
                        var listaEmprestimos = JsonConvert.DeserializeObject<IEnumerable<EmprestismoDeLivros>>(serialized);

                        return View(listaEmprestimos);
                    }
                    else
                    {
                        TempData["MensagemErro"] = response.ReasonPhrase;
                        return View(); // Retorna a view sem dados em caso de erro.
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return View(); // Retorna a view sem dados em caso de exceção.
            }
        }

        [HttpGet]
        public async Task<ActionResult> Details(int? IdTransacao)
        {
            var emprestimo = await ObterEmprestimoPorIdAsync(IdTransacao);

            if (emprestimo == null)
            {
                return NotFound();
            }

            return View(emprestimo);
        }

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(int IdLivro, int IdUsuario)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var emprestimo = new EmprestismoDeLivros
                    {
                        IdLivro = IdLivro,
                        IdUsuario = IdUsuario
                        // Você pode definir as outras propriedades, se necessário
                    };

                    var content = new StringContent(JsonConvert.SerializeObject(emprestimo));
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    HttpResponseMessage response = await httpClient.PostAsync(uriBase + "realizarEmprestimo", content);

                    if (response.IsSuccessStatusCode)
                    {
                        var serialized = await response.Content.ReadAsStringAsync();
                        TempData["Mensagem"] = $"Empréstimo Id {serialized} realizado com sucesso!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["MensagemErro"] = "Ocorreu um erro ao realizar o empréstimo.";
                        return RedirectToAction("Create");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Create");
            }
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int? IdTransacao)
        {
            var emprestimo = await ObterEmprestimoPorIdAsync(IdTransacao);

            if (emprestimo == null)
            {
                TempData["MensagemErro"] = "Empréstimo não encontrado.";
                return RedirectToAction("Index");
            }

            return View(emprestimo);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(int IdLivro, int IdUsuario)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var emprestimo = new { IdLivro = IdLivro, IdUsuario = IdUsuario };
                    var content = new StringContent(JsonConvert.SerializeObject(emprestimo));
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    HttpResponseMessage response = await httpClient.PutAsync(uriBase + $"realizarDevolucao", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Mensagem"] = $"Devolução do Empréstimo realizada com sucesso!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["MensagemErro"] = response.ReasonPhrase;
                        return RedirectToAction("Index"); // Ou redirecione para a página desejada em caso de erro.
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }


        [HttpPost]
        public async Task<ActionResult> Delete(int IdTransacao)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.DeleteAsync(uriBase + $"realizarDevolucao/{IdTransacao}");

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Mensagem"] = $"Empréstimo Id {IdTransacao} removido com sucesso!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["MensagemErro"] = response.ReasonPhrase;
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
