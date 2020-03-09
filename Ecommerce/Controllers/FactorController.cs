using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Helpers.OptionEnums;
using Ecommerce.Models;
using Ecommerce.Net;
using Ecommerce.Net.OptionEnums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Ecommerce.Controllers
{
   
    public class FactorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<ApplicationUser> _userManager;

        public FactorController(ApplicationDbContext context, IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _serviceProvider = serviceProvider;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Factor()
        {
            List<Product> products = new List<Product>();


            if (Request.Cookies["S#$51%^Lm*A!2@m"] != null)
            {
                string cookicontent = Request.Cookies["S#$51%^Lm*A!2@m"].ToString();
                string[] requestedproduct = cookicontent.Split(',');
                requestedproduct = requestedproduct.Where(r => r != "").ToArray();

                products = (from p in _context.Products
                            where requestedproduct.Contains(p.Id.ToString())
                            select new Product
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Price = (int)p.Price,
                                ImageName = p.ImageName,

                            }).ToList();

            }

            ViewBag.imagepath = "/upload/normalimage/";
            var query_fullName =
                (from u in _context.Users where u.Id == _userManager.GetUserId(HttpContext.User) select u)
                .SingleOrDefault();
            ViewBag.fullname = query_fullName.Firstname + " " + query_fullName.Lastname;



            return View(products);

        }

        [HttpPost]
        public async Task<IActionResult> Factor()
        {
            var TotalPrice = TempData["Total"];
            var idproducts_ = TempData["IdPoducts"];
            int[] a = (int[])idproducts_;
            List<Product> product = new List<Product>();
            foreach (var VARIABLE in a)
            {
                var select = _context.Products.Where(p => p.Id == VARIABLE).FirstOrDefault();
                product.Add(select);
            }

            Models.Factor factor = new Factor();
            using (var db = _serviceProvider.GetRequiredService<ApplicationDbContext>())
            {
                factor.TotalPrice = (decimal)TotalPrice;
                factor.TotalDisCount = product.Sum(p => p.OldPrice) - product.Sum(p => p.Price);
                factor.Tax = (decimal)0.09;
                factor.FinalPrice = (factor.TotalDisCount - (decimal)TotalPrice) * factor.Tax;
                factor.IdUser = _userManager.GetUserId(User);

                var currentDay = DateTime.Now;
                PersianCalendar pcalender = new PersianCalendar();
                int year = pcalender.GetYear(currentDay);
                int month = pcalender.GetMonth(currentDay);
                int day = pcalender.GetDayOfMonth(currentDay);
                string ShamsiDate = string.Format("{0:yyyy/MM/dd}", Convert.ToDateTime(year + "/" + month + "/" + day));

                string GetTime = string.Format("{0:HH:mm:ss}",
                    Convert.ToDateTime(currentDay.TimeOfDay.Hours + ":" + currentDay.TimeOfDay.Minutes + ":" +
                                       currentDay.TimeOfDay.Seconds));

                string DateTimes = ShamsiDate + "|" + GetTime;

                factor.Date = Convert.ToDateTime(DateTimes);

                db.Add(factor);
                await db.SaveChangesAsync();
            }

            TempData["Notif"] = Notification.ShowNotif(MessageType.Add, type: ToastType.green);
            return View();
        }

        [AllowAnonymous]
      
        [HttpGet]
        public IActionResult requestedproduct()
        {

            List<Product> products = new List<Product>();


            if (Request.Cookies["S#$51%^Lm*A!2@m"] != null)
            {
                string cookicontent = Request.Cookies["S#$51%^Lm*A!2@m"].ToString();
                string[] requestedproduct = cookicontent.Split(',');
                requestedproduct = requestedproduct.Where(r => r != "").ToArray();

                products = (from p in _context.Products
                            where requestedproduct.Contains(p.Id.ToString())
                            select new Product
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Price = (int)p.Price,
                                ImageName = p.ImageName,

                            }).ToList();

            }

            ViewBag.imagepath = "/upload/normalimage/";
            var query_fullName =
                (from u in _context.Users where u.Id == _userManager.GetUserId(HttpContext.User) select u)
                .SingleOrDefault();
            ViewBag.fullname = query_fullName.Firstname + " " + query_fullName.Lastname;

            var select = _context.Cars.ToList();

            ViewBag.Cars = select;

            var select2 = _context.Categories.ToList();

            ViewBag.Categories = select2;

            return View(products);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> requestedproduct(int TotalPrice)
        {

            List<Product> product = new List<Product>();
            //foreach (var VARIABLE in IdProducts)
            //{
            //    var select = _context.Products.Where(a => a.Id == VARIABLE).FirstOrDefault();
            //    product.Add(select);
            //}

            Factor factor = new Factor();
            using (var db = _serviceProvider.GetRequiredService<ApplicationDbContext>())
            {
                factor.TotalPrice = TotalPrice;
                factor.TotalDisCount = product.Sum(p => p.OldPrice) - product.Sum(p => p.Price);
                factor.Tax = (decimal)0.09;
                factor.FinalPrice = (factor.TotalDisCount - TotalPrice) * factor.Tax;
                factor.IdUser = _userManager.GetUserId(User);

                var currentDay = DateTime.Now;
                PersianCalendar pcalender = new PersianCalendar();
                int year = pcalender.GetYear(currentDay);
                int month = pcalender.GetMonth(currentDay);
                int day = pcalender.GetDayOfMonth(currentDay);
                string ShamsiDate = string.Format("{0:yyyy/MM/dd}", Convert.ToDateTime(year + "/" + month + "/" + day));

                string GetTime = string.Format("{0:HH:mm:ss}",
                    Convert.ToDateTime(currentDay.TimeOfDay.Hours + ":" + currentDay.TimeOfDay.Minutes + ":" +
                                       currentDay.TimeOfDay.Seconds));

                string DateTimes = ShamsiDate + "|" + GetTime;

                factor.Date = Convert.ToDateTime(DateTimes);

                db.Add(factor);
                await db.SaveChangesAsync();
            }

            TempData["Notif"] = Notification.ShowNotif(MessageType.Add, type: ToastType.green);

            var select = _context.Cars.ToList();

            ViewBag.Cars = select;

            var select2 = _context.Categories.ToList();

            ViewBag.Categories = select2;

            return View();
        }


    }
}