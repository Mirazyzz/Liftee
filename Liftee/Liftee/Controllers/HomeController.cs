using Liftee.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ExcelDataReader;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Features;

namespace Liftee.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment hostingEnv;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostingEnv)
        {
            _logger = logger;
            this.hostingEnv = hostingEnv;
        }

        public IActionResult Index()
        {
            List<Project> projects = new List<Project>();
            return View(projects);
        }

        [HttpPost]
        public IActionResult Privacy(IFormFile file, [FromServices] IWebHostEnvironment hostingEnvironment)
        {
            string fileName = $"{hostingEnvironment.WebRootPath}\\files\\{file.FileName}";

            using(FileStream fs = System.IO.File.Create(fileName))
            {
                file.CopyTo(fs);
                fs.Flush();
            }

            var projects = GetProjects(file.FileName);

            return View(nameof(Index), projects);
        }

        public IActionResult Privacy()
        {
            List<Project> projects = new List<Project>();

            return View(projects);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<Project> GetProjects(string fName)
        {
            List<Project> projects = new List<Project>();
            var fileName = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\files"}\\{fName}";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using var stream = System.IO.File.OpenRead(fileName);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            while (reader.Read())
            {
                projects.Add(new Project()
                {
                    Contract = reader.GetValue(0).ToString(),
                    PropertyName = reader.GetValue(1).ToString(),
                    ContactDetails = reader.GetValue(2).ToString(),
                    ContractStatus = reader.GetValue(3).ToString(),
                    SerialNumber = reader.GetValue(4).ToString(),
                    InstallationStartDate = reader.GetValue(5).ToString(),
                    Fitter = reader.GetValue(6).ToString(),
                    InstallationStatus = reader.GetValue(7).ToString(),
                    InstallationFinishDate = reader.GetValue(8).ToString(),
                    Issues = reader.GetValue(9).ToString(),
                    Document = reader.GetValue(10).ToString(),
                    ElevatorPassport = reader.GetValue(11).ToString(),
                    ProjectStatus = reader.GetValue(12).ToString()
                });
            }

            return projects;
        }

        [AcceptVerbs("Post")]
        public IActionResult OnPostSave(IList<IFormFile> UploadFiles)
        {
            try
            {
                foreach (var file in UploadFiles)
                {
                    if (UploadFiles != null)
                    {
                        var filename = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        filename = hostingEnv.WebRootPath + $@"\{filename}";
                        if (!System.IO.File.Exists(filename))
                        {
                            using (FileStream fs = System.IO.File.Create(filename))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }
                        }
                        else
                        {
                            Response.Clear();
                            Response.StatusCode = 204;
                            Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "File already exists.";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Response.Clear();
                Response.ContentType = "application/json; charset=utf-8";
                Response.StatusCode = 204;
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = "No Content";
                Response.HttpContext.Features.Get<IHttpResponseFeature>().ReasonPhrase = e.Message;
            }
            return Content("");
        }
    }
}