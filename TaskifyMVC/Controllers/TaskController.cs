using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using TaskifyMVC.Models;
namespace TaskifyMVC.Controllers
{
    public class TaskController : Controller
    {
        private readonly IHttpClientFactory? _factory;
        public TaskController(IHttpClientFactory factory)
        {
            _factory = factory;
        }
        private HttpClient GetClient()
        {
            var client = _factory.CreateClient("TaskifyAPI");
            var token = HttpContext.Session.GetString("JWToken");
            if(!string.IsNullOrEmpty(token) )
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
            }
            return client;
        }
        public async Task<IActionResult> Index()
        {
            var client = GetClient();
            var res = await client.GetAsync("api/tasks");
            List<TaskViewModel> tasks = new();
            if (res.IsSuccessStatusCode)
            {
               var json = await res.Content.ReadAsStringAsync();
                tasks = JsonConvert.DeserializeObject<List<TaskViewModel>>(json);
            }
            ViewBag.IsAuthenticated = !string.IsNullOrEmpty(HttpContext.Session.GetString("JWToken"));
                return View(tasks);
        }
        [HttpPost]
        public async Task<IActionResult> Create(TaskViewModel model)
        {
            var client = GetClient();
            var payload = JsonConvert.SerializeObject(new { Title = model.Title,Description = model.Description });
            await client.PostAsync("api/tasks", new StringContent(payload, Encoding.UTF8, "application/json"));
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var client = GetClient();
            await client.DeleteAsync($"api/tasks/{id}");
            return RedirectToAction("Index");
        }
    }
}
