using Microsoft.AspNetCore.Mvc;
using BibliotecaMvc.Models;
using System.Net.Http.Headers;
using Newtonsoft.Json;


namespace BibliotecaMvc.Controllers
{
    public class LivroController : Controller
    {
        private readonly string uriBase = "http://localhost:5175/api/Livros/";

        [HttpGet]
        public async Task<ActionResult> IndexAsync(string searchTerm)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(uriBase + "obterTodosOsLivros");

                if (string.IsNullOrEmpty(searchTerm))
                {
                    // Buscar todos os livros se o termo de busca estiver vazio
                    response = await httpClient.GetAsync(uriBase + "obterTodosOsLivros");
                }
                else
                {
                    // Buscar livros com base no termo de busca
                    response = await httpClient.GetAsync(uriBase + $"buscarLivros?termoBusca={searchTerm}");
                }



                if (response.IsSuccessStatusCode)
                {
                    var serialized = await response.Content.ReadAsStringAsync();
                    var listaLivros = JsonConvert.DeserializeObject<IEnumerable<Livro>>(serialized);

                    return View(listaLivros);
                }
                else
                {
                    TempData["MensagemErro"] = response.ReasonPhrase;
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<ActionResult> DetailsAsync(int? IdLivro)
        {
            if (IdLivro == null)
            {
                return NotFound();
            }

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(uriBase + $"obterLivroPorid?id={IdLivro}");

            if (response.IsSuccessStatusCode)
            {
                var serialized = await response.Content.ReadAsStringAsync();
                var livro = JsonConvert.DeserializeObject<Livro>(serialized);

                return View(livro);
            }
            else
            {
                TempData["MensagemErro"] = response.ReasonPhrase;
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync([Bind("Titulo, Autor, Categoria")] Livro livro)
        {
            try
            {
                if (string.IsNullOrEmpty(livro.Titulo) || string.IsNullOrEmpty(livro.Autor) || string.IsNullOrEmpty(livro.Categoria))
                {
                    if (string.IsNullOrEmpty(livro.Titulo))
                    {
                        TempData["MensagemErro"] = "O Título do livro não pode ser vazio ou nulo.";
                    }
                    else if (string.IsNullOrEmpty(livro.Autor))
                    {
                        TempData["MensagemErro"] = "O Autor do livro não pode ser vazio ou nulo.";
                    }
                    else if (string.IsNullOrEmpty(livro.Categoria))
                    {
                        TempData["MensagemErro"] = "A Categoria do livro não pode ser vazia ou nula.";
                    }

                    return RedirectToAction("Create");
                }

                HttpClient httpClient = new HttpClient();
                var content = new StringContent(JsonConvert.SerializeObject(livro));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await httpClient.PostAsync(uriBase + "adicionarLivro", content);

                if (response.IsSuccessStatusCode)
                {
                    var serialized = await response.Content.ReadAsStringAsync();
                    TempData["Mensagem"] = $"Livro {livro.Titulo}, Id {serialized} adicionado com sucesso!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["MensagemErro"] = "Ocorreu um erro ao adicionar o livro, ele já existe.";
                    return RedirectToAction("Create");
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Create");
            }
        }


        [HttpGet]
        public async Task<ActionResult> EditAsync(int? IdLivro)
        {
            

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(uriBase + $"obterLivroPorid?id={IdLivro}");

            if (response.IsSuccessStatusCode)
            {
                var serialized = await response.Content.ReadAsStringAsync();
                var livro = JsonConvert.DeserializeObject<Livro>(serialized);

                return View(livro);
            }
            else
            {
                TempData["MensagemErro"] = response.ReasonPhrase;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditAsync(Livro livro)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var content = new StringContent(JsonConvert.SerializeObject(livro));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await httpClient.PutAsync(uriBase + $"atualizarLivroPorId/{livro.IdLivro}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Mensagem"] = $"Livro {livro.Titulo}, Id {livro.IdLivro} atualizado com sucesso!";
                    // Corrija aqui de "id" para "IdLivro"
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["MensagemErro"] = response.ReasonPhrase;
                    // Corrija aqui de "id" para "IdLivro"
                    return RedirectToAction("Edit", new { IdLivro = livro.IdLivro });
                }
            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = ex.Message;
                return RedirectToAction("Index");
            }
        }



        [HttpPost]
        public async Task<ActionResult> DeleteAsync(int IdLivro)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.DeleteAsync(uriBase + $"{IdLivro}");

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Mensagem"] = $"Livro Id {IdLivro} removido com sucesso!";
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
