using ExcelDataReader;
using Liftee.Models;
using Liftee.Services;
using Microsoft.AspNetCore.Mvc;

namespace Liftee.Controllers
{
    public class UpdateController : Controller
    {
        public IActionResult Index()
        {
            List<Project> projects = new List<Project>();
            return View(projects);
        }

        [HttpPost]
        public IActionResult Index(IFormFile file, [FromServices] IWebHostEnvironment hostingEnvironment)
        {
            if(file == null)
            {
                return View();
            }

            string fileName = $"{hostingEnvironment.WebRootPath}\\files\\{file.FileName}";

            using (FileStream fs = System.IO.File.Create(fileName))
            {
                file.CopyTo(fs);
                fs.Flush();
            }

            var projects = GetProjects(file.FileName);

            return View(nameof(Index), projects);
        }

        private static List<Project> GetProjects(string fName)
        {
            List<Project> projects = new();
            var fileName = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\files"}\\{fName}";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using var stream = System.IO.File.OpenRead(fileName);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            while (reader.Read())
            {
                projects.Add(new Project()
                {
                    Id = reader.GetValue(0)?.ToString(),
                    Contract = reader.GetValue(1)?.ToString(),
                    PropertyName = reader.GetValue(2)?.ToString(),
                    ContactDetails = reader.GetValue(3)?.ToString(),
                    ContractStatus = reader.GetValue(4)?.ToString(),
                    GP = reader.GetValue(5)?.ToString(),
                    ElevatorType = reader.GetValue(6)?.ToString(),
                    SerialNumber = reader.GetValue(7)?.ToString(),
                    InstallationStartDate = reader.GetValue(8)?.ToString(),
                    Fitter = reader.GetValue(9)?.ToString(),
                    InstallationStatus = reader.GetValue(10)?.ToString(),
                    InstallationFinishDate = reader.GetValue(11)?.ToString(),
                    Issues = reader.GetValue(12)?.ToString(),
                    Document = reader.GetValue(13)?.ToString(),
                    ElevatorPassport = reader.GetValue(14)?.ToString(),
                    ProjectStatus = reader.GetValue(15)?.ToString(),
                    MutualSettlement = reader.GetValue(16)?.ToString()
                });
            }

            ProjectsService.AddProjects(projects.Skip(1));

            return projects;
        }
    }
}
