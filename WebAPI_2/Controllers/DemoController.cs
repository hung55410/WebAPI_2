using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAPI_2.Controllers
{
    public class DemoController : Controller
    {
        // GET: Demo
        public ActionResult Index()
        {
            return View();
        }
        public string Get()
        {
            return "Chào mừng bạn đến với WebAPI Rest";
        }

        public List<string> Get(int id)
        {
            return new List<string>()
            {
                "data 1",
                "data 2"
            };
        }
    }
}