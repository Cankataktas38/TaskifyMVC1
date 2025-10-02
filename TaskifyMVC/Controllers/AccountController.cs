using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using TaskifyMVC.Models;
namespace TaskifyMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IHttpClientFactory? _factory;
        public AccountController(IHttpClientFactory factory)
        {
            _factory = factory;
        }
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username,string password)
        {
            var client = _factory.CreateClient("TaskifyAPI");
            var payload = JsonConvert.SerializeObject(new { Username = username, Password = password });
            var res = await client.PostAsync("api/auth/login",new StringContent(payload,Encoding.UTF8, "application/json"));
            if (!res.IsSuccessStatusCode)
            {
                ViewBag.Error = "Giriş başarısız";
                return View();
            }
            var json = await res.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<dynamic>(json);
            string token = obj.token;
            HttpContext.Session.SetString("JWToken",token);
            return RedirectToAction("Index","Task");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JWToken");
            return RedirectToAction("Login");
        }
    }
}
