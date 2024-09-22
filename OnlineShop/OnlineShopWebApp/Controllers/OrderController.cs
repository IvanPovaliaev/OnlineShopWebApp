using Microsoft.AspNetCore.Mvc;
using OnlineShopWebApp.Models;
using System;

namespace OnlineShopWebApp.Controllers
{
    public class OrderController : Controller
    {
        private Guid _userId = new Guid("74f1f6b5-083a-4677-8f68-8255caa77965"); //Временный guid для тестирования

        [HttpPost]
        public IActionResult Register(Order order)
        {
            order.UserId = _userId;
            return View(order);
        }
    }
}
