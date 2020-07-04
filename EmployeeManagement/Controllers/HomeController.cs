using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Controllers
{
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly IEmployee _employee;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly ILogger logger;

        public HomeController(IEmployee employee,
            IWebHostEnvironment webHostEnvironment,
            ILogger<HomeController> logger)
        {
            _employee = employee;
            this.webHostEnvironment = webHostEnvironment;
            this.logger = logger;
        }

        [Route("")]
        [Route("[action]")]
        [Route("~/")]
        public ViewResult Index()
        {
            var model = _employee.GetAllEmployee();
            return View(model);
        }

        [Route("[action]/{id}")]
        public ViewResult Details(int? id)
        {
            //throw new Exception("Error in details view.");
            logger.LogTrace("");
            logger.LogDebug("");

            Employee employee = _employee.GetEmployee(id.Value);

            if(employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id.Value);
            }

            HomeDetailsViewModel homeDetailsViewModel = new HomeDetailsViewModel()
            {
                Employee = employee,
                PageTitle = "Employee Details"
            };
            
            return View(homeDetailsViewModel);
        }

        [HttpGet]
        [Route("[action]")]
        public ViewResult Create()
        {
            return View();
        }

        [HttpGet]
        [Route("[action]")]
        public ViewResult Edit(int id)
        {
            Employee employee = _employee.GetEmployee(id);
            EmployeeEditViewModel employeeEditView = new EmployeeEditViewModel{
                Id = employee.Id,
                Name = employee.Name,
                Department = employee.Department,
                Email = employee.Email,
                ExistingPhotoPath = employee.PhotoPath
            };

            return View(employeeEditView);
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employee.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;
                
                if(model.Photo != null)
                {
                    if(model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(webHostEnvironment.WebRootPath
                                                    , "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    employee.PhotoPath = ProcessUploadedFile(model);
                }

                _employee.Update(employee);
                return RedirectToAction("details", employee);
            }

            return View();
        }

        [HttpPost]
        [Route("[action]")]
        public IActionResult Create(EmployeeCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);

                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
                };

                _employee.Add(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }

            return View();
        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                string uploadFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}
