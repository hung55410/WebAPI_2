using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebAPI_2.Models;

namespace WebAPI_2.Controllers
{
    public class HomeController : Controller
    {
        public async Task<ActionResult> Index()
        {
            ViewBag.Title = "Home Page";
            var list = await GetAllUser();
            if (list != null)
            {
                return View(list);// Nếu list user khác null thì trả về View có chứa list
            } else
            {
                ViewBag.Title = "No User Found"; // Khởi tạo giá trị mặc định cho ViewBag.Title
                return View(new List<Users>()); // Truyền vào danh sách rỗng
            }

        }

        private async Task<List<Users>> GetAllUser()   // Hàm Gọi API trả về list user
        {
            string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority +
                Request.ApplicationPath.TrimEnd('/') + "/";   // Lấy base uri của website
            using (var httpClient = new HttpClient())
            {
                HttpResponseMessage res = await httpClient.GetAsync(baseUrl + "api/User/GetAllUser");
                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<Users> list = new List<Users>();
                    list = await res.Content.ReadAsAsync<List<Users>>();
                    return list;
                }
                return null;
            }
        }
    }
}
