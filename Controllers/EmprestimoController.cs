using BibliotecaMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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
                // Verificar se os IDs são válidos
                if (IdLivro <= 0 || IdUsuario <= 0)
                {
                    TempData["MensagemErro"] = "IDs de Livro e Usuário devem ser maiores que zero.";
                    return RedirectToAction("Create");
                }

                // Log de informações
                Console.WriteLine($"Recebendo solicitação para realizar empréstimo - ID Livro: {IdLivro}, ID Usuário: {IdUsuario}");

                using (HttpClient httpClient = new HttpClient())
                {
                    // Criar um objeto anônimo com os IDs
                    var emprestimo = new { IdLivro = IdLivro, IdUsuario = IdUsuario };

                    // Log de informações
                    Console.WriteLine($"Enviando solicitação para API - ID Livro: {emprestimo.IdLivro}, ID Usuário: {emprestimo.IdUsuario}");

                    // Serializar o objeto em JSON
                    var content = new StringContent(JsonConvert.SerializeObject(emprestimo));

                    // Definir o cabeçalho de tipo de conteúdo
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    // Enviar a solicitação POST para a API
                    HttpResponseMessage response = await httpClient.PostAsync(uriBase + "realizarEmprestimo", content);

                    // Verificar se a solicitação foi bem-sucedida
                    if (response.IsSuccessStatusCode)
                    {
                        var serialized = await response.Content.ReadAsStringAsync();
                        TempData["Mensagem"] = $"Empréstimo Id {serialized} realizado com sucesso!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["MensagemErro"] = $"Erro ao realizar o empréstimo. Resposta da API: {response.ReasonPhrase}";
                        return RedirectToAction("Create");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log de erro
                Console.WriteLine($"Erro ao realizar o empréstimo: {ex.Message}");
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
        public async Task<ActionResult> Devolver(int IdLivro, int IdUsuario)
        {
            try
            {
                // Verificar se os IDs são válidos
                if (IdLivro <= 0 || IdUsuario <= 0)
                {
                    TempData["MensagemErro"] = "IDs de Livro e Usuário devem ser maiores que zero.";
                    return RedirectToAction("Index");
                }

                using (HttpClient httpClient = new HttpClient())
                {
                    var devolucao = new { IdLivro = IdLivro, IdUsuario = IdUsuario };
                    var content = new StringContent(JsonConvert.SerializeObject(devolucao));
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
                // Log de erro
                Console.WriteLine($"Erro ao realizar a devolução: {ex.Message}");
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
                // Log de erro
                Console.WriteLine($"Erro ao excluir o empréstimo: {ex.Message}");
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
