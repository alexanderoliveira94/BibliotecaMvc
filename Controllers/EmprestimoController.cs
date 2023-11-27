using BibliotecaMvc.Models;
using Microsoft.AspNetCore.Mvc;
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
                        return View();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return View();
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

                if (IdLivro <= 0 || IdUsuario <= 0)
                {
                    TempData["MensagemErro"] = "IDs de Livro e Usuário devem ser maiores que zero.";
                    return RedirectToAction("Create");
                }

                using (HttpClient httpClient = new HttpClient())
                {
                    // Construir a URL com os parâmetros como query string
                    var apiUrl =
                        $"{uriBase}realizarEmprestimo?IdLivro={IdLivro}&IdUsuario={IdUsuario}";

                    // Enviar a solicitação POST para a API
                    HttpResponseMessage response = await httpClient.PostAsync(apiUrl, null);

                    // Verificar se a solicitação foi bem-sucedida
                    if (response.IsSuccessStatusCode)
                    {
                        var serialized = await response.Content.ReadAsStringAsync();
                        TempData["Mensagem"] = $"Empréstimo Id {serialized} realizado com sucesso!";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["MensagemErro"] =
                            $"Erro ao realizar o empréstimo. Resposta da API: {response.ReasonPhrase}";
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

        public async Task<ActionResult> RealizarDevolucao(int IdTransacao)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    // Construir a URL com os parâmetros como query string
                    var apiUrl = $"{uriBase}realizarDevolucao/{IdTransacao}";

                    // Faz a chamada ao método da API usando o HttpClient
                    HttpResponseMessage response = await httpClient.PutAsync(apiUrl, null);

                    // Verifica se a chamada foi bem-sucedida (status code 204 - NoContent)
                    if (response.IsSuccessStatusCode)
                    {
                        // Devolução bem-sucedida, redireciona ou retorna uma mensagem de sucesso
                        TempData["SucessoDevolucao"] = "Devolução realizada com sucesso.";
                    }
                    else
                    {
                        // Devolução não foi bem-sucedida, define uma mensagem de erro
                        TempData["ErroDevolucao"] = "Erro ao realizar a devolução.";
                    }
                }

                return RedirectToAction("Index"); // Redireciona para a página inicial, ajuste conforme necessário
            }
            catch (Exception ex)
            {
                // Trate exceções aqui, redirecione para uma página de erro, ou faça o que for apropriado para sua aplicação
                TempData["ErroDevolucao"] = "Erro ao realizar a devolução.";
                return RedirectToAction("Index", "Home"); // Redireciona para a página inicial, ajuste conforme necessário
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
