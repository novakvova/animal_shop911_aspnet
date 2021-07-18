using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAppSite.Domain;
using WebAppSite.Models;

namespace WebAppSite.Controllers
{
    public class AnimalController : Controller
    {
        private readonly AppEFContext _context;

        public AnimalController(AppEFContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<AnimalsViewModel> model =
                _context.Animals.Select(x => new AnimalsViewModel
                {
                    Id=x.Id,
                    BirthDay = x.DateBirth,
                    Image=x.Image,
                    Name=x.Name
                }).ToList();
                
                //new List<AnimalsViewModel>();
            //model.Add(new AnimalsViewModel
            //{
            //    Id=1,
            //    BirthDay=DateTime.Now,
            //    Image="1.jpg",
            //    Name="Барсік Клим"
            //});
            //model.Add(new AnimalsViewModel
            //{
            //    Id = 2,
            //    BirthDay = DateTime.Now,
            //    Image = "2.jpg",
            //    Name = "Кусачий"
            //});
            return View(model);
        }
    }
}
