using AutoMapper;
using Bogus;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebAppSite.Domain;
using WebAppSite.Domain.Entities.Catalog;
using WebAppSite.Models;

namespace WebAppSite.Controllers
{
    public class AnimalController : Controller
    {
        private readonly AppEFContext _context;
        private readonly IMapper _mapper;

        public AnimalController(AppEFContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;

            //GenataAnimal();
        }

        private void GenataAnimal()
        {
            int n = 1000;
            var endDate = DateTime.Now;
            var startDate = new DateTime(endDate.Year - 2,
                endDate.Month, endDate.Day);

            var faker = new Faker<Animal>("uk")
                .RuleFor(x => x.Name, f => f.Person.FullName)
                .RuleFor(x => x.DateBirth, f => f.Date.Between(startDate, endDate))
                .RuleFor(x => x.Image, f => f.Image.PicsumUrl())
                .RuleFor(x => x.Price, f => Decimal.Parse(f.Commerce.Price(100M, 500M)))
                .RuleFor(x => x.DateCreate, DateTime.Now);


            for (int i = 0; i < n; i++)
            {
                var animal = faker.Generate();
                _context.Animals.Add(animal);
                _context.SaveChanges();
            }
        }
        public IActionResult Index(SearchHomeIndexModel search, int page=1)
        {
            int showItems = 10;
            var query = _context.Animals.AsQueryable();
            if(!string.IsNullOrEmpty(search.Name))
            {
                query = query.Where(x => x.Name.Contains(search.Name));
            }
            HomeIndexModel model = new HomeIndexModel();

            //кількість записів, які ми знайшли загально
            int countItems = query.Count();
            var pageCount = (int)Math.Ceiling(countItems/(double)showItems);
            if (pageCount == 0) pageCount = 1;

            int skipItems = (page - 1) * showItems;

            query = query.Skip(skipItems).Take(showItems);

            model.Animals = query
                .Select(x=>_mapper.Map<AnimalsViewModel>(x))
                .ToList();
            model.Search = search;
            model.Page = page;
            model.PageCount = pageCount;
                
            return View(model);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(AnimalCreateViewModel model)
        {

            if(!ModelState.IsValid)
                return View(model);
            string fileName = ""; 
            if (model.Image!=null)
            {
                var ext = Path.GetExtension(model.Image.FileName);
                fileName = Path.GetRandomFileName() + ext;
                var dir = Path.Combine(Directory.GetCurrentDirectory(), "images");
                
                var filePath = Path.Combine(dir, fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await model.Image.CopyToAsync(stream);
                }
            }
            DateTime dt = DateTime.Parse(model.BirthDay, new CultureInfo("uk-UA"));
            Animal animal = new Animal
            {
                Name=model.Name,
                DateBirth=dt,
                Image= fileName,
                Price=model.Price,
                DateCreate=DateTime.Now
            };
            _context.Animals.Add(animal);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Delete(long id)
        {
            Thread.Sleep(2000);
            var item =_context.Animals.SingleOrDefault(x => x.Id == id);
            if(item!=null)
            {
                //_context.Remove(item);
                _context.Animals.Remove(item);
                _context.SaveChanges();
            }    
            return Ok();
        }
    }
}
