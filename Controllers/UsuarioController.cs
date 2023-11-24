using Microsoft.AspNetCore.Mvc;
using BibliotecaMvc.Models;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Net;

namespace BibliotecaMvc.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly string uriBase = "http://localhost:5175/api/Usuarios/";

        [HttpGet]
        public async Task<ActionResult> IndexAsync()
        {
            try
            {

                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(uriBase + "obterTodosUsuarios");

                if (response.IsSuccessStatusCode)
                {
                    var serialized = await response.Content.ReadAsStringAsync();
                    var listaUsuarios = JsonConvert.DeserializeObject<IEnumerable<Usuario>>(serialized);

                    return View(listaUsuarios);
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
        public async Task<ActionResult> DetailsAsync(int? IdUsuario)
        {
            if (IdUsuario == null)
            {
                return NotFound();
            }

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(uriBase + $"obterUsuarioPorId/{IdUsuario}");

            if (response.IsSuccessStatusCode)
            {
                var serialized = await response.Content.ReadAsStringAsync();
                var usuario = JsonConvert.DeserializeObject<Usuario>(serialized);

                return View(usuario);
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
        public async Task<ActionResult> CreateAsync([Bind("NomeUsuario")] Usuario usuario)
        {
            try
            {
                if (string.IsNullOrEmpty(usuario.NomeUsuario))
                {
                    TempData["MensagemErro"] = "O Nome do usuário não pode ser vazio ou nulo.";
                    return RedirectToAction("Create");
                }

                HttpClient httpClient = new HttpClient();
                var content = new StringContent(JsonConvert.SerializeObject(usuario));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                HttpResponseMessage response = await httpClient.PostAsync(uriBase + "adicionarUsuario", content);

                if (response.IsSuccessStatusCode)
                {
                    var serialized = await response.Content.ReadAsStringAsync();
                    TempData["Mensagem"] = $"Usuário {usuario.NomeUsuario}, Id {serialized} adicionado com sucesso!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["MensagemErro"] = "Ocorreu um erro ao adicionar o usuário, ele já existe.";
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
        public async Task<ActionResult> Edit(int? IdUsuario)
        {
            if (IdUsuario == null)
            {
                return NotFound();
            }

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(uriBase + $"obterUsuarioPorId/{IdUsuario}");

            if (response.IsSuccessStatusCode)
            {
                var serialized = await response.Content.ReadAsStringAsync();
                var usuario = JsonConvert.DeserializeObject<Usuario>(serialized);

                return View(usuario);
            }
            else
            {
                TempData["MensagemErro"] = response.ReasonPhrase;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Usuario usuario)
        {
            try
            {
                // Verifica se o novo nome do usuário já existe
                using (HttpClient httpClientCheck = new HttpClient())
                {
                    HttpResponseMessage responseCheck = await httpClientCheck.GetAsync(uriBase + $"verificarUsuarioExistente/{usuario.NomeUsuario}");

                    if (responseCheck.IsSuccessStatusCode)
                    {
                        // Adiciona um erro ao ModelState
                        ModelState.AddModelError("NomeUsuario", "O nome do usuário já está cadastrado para outro usuário.");
                        return View(usuario);
                    }
                    else if (responseCheck.StatusCode == HttpStatusCode.NotFound)
                    {
                        // Continua com a atualização se o nome do usuário não existir
                        using (HttpClient httpClient = new HttpClient())
                        {
                            var content = new StringContent(JsonConvert.SerializeObject(usuario));
                            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                            HttpResponseMessage response = await httpClient.PutAsync(uriBase + $"atualizarUsuario/{usuario.IdUsuario}", content);

                            if (response.IsSuccessStatusCode)
                            {
                                TempData["Mensagem"] = $"Usuário {usuario.NomeUsuario}, Id {usuario.IdUsuario} atualizado com sucesso!";

                                // Adiciona uma mensagem de sucesso para ser exibida via JavaScript
                                TempData["MensagemJS"] = "success";

                                return RedirectToAction("Index");
                            }
                            else
                            {
                                TempData["MensagemErro"] = response.ReasonPhrase;
                                return RedirectToAction("Edit", new { IdUsuario = usuario.IdUsuario });
                            }
                        }
                    }
                    else
                    {
                        TempData["MensagemErro"] = "Erro ao verificar o nome do usuário.";
                        return RedirectToAction("Edit", new { IdUsuario = usuario.IdUsuario });
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
        public async Task<ActionResult> Delete(int IdUsuario)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    HttpResponseMessage response = await httpClient.DeleteAsync(uriBase + $"{IdUsuario}");

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Mensagem"] = $"Livro Id {IdUsuario} removido com sucesso!";
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

