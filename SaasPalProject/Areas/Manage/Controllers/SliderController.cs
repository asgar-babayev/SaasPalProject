using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SaasPalProject.DAL;
using SaasPalProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SaasPalProject.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class SliderController : Controller
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(Context context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index()
        {
            return View(_context.Sliders.ToList());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost,AutoValidateAntiforgeryToken]

        public IActionResult Create(Slider slider)
        {
            if (!ModelState.IsValid) return View(slider);
            if (slider.ImageFile != null)
            {
                if (slider.ImageFile.ContentType != "image/jpeg" && slider.ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "Image can be only .jpeg or .png");
                    return View(slider);
                }
                if (slider.ImageFile.Length / 1024 > 2000)
                {
                    ModelState.AddModelError("ImageFile", "Image size must be lower than 2mb");
                    return View(slider);
                }

                string filename = slider.ImageFile.FileName;
                if (filename.Length > 64)
                {
                    filename.Substring(filename.Length - 64, 64);
                }
                string newFileName = Guid.NewGuid().ToString() + filename;
                string path = Path.Combine(_env.WebRootPath, "assets", "img", newFileName);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    slider.ImageFile.CopyTo(stream);
                }
                slider.Image = newFileName;
                _context.Sliders.Add(slider);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
