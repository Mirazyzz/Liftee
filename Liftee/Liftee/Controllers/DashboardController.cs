using Liftee.Helpers;
using Liftee.Services;
using Microsoft.AspNetCore.Mvc;

namespace Liftee.Controllers
{
    public class DashboardController : Controller
    {
        private readonly CsvParser _parser;

        public ComboBoxFor Categories { get; set; } = new ComboBoxFor();

        public DashboardController()
        {
            _parser = new CsvParser();
        }

        public IActionResult Index(string? category)
        {
            var totalProjects = ProjectsService.Projects.Count;
            ViewBag.TotalProjects = totalProjects;

            ViewBag.FinishedProjects = ProjectsService.Projects.Where(p => p.ProjectStatus == "Закрыт").Count();

            var inProcessProjects = ProjectsService.Projects.Where(p => p.ProjectStatus == "В процессе").Count();
            ViewBag.InProcessProjects = inProcessProjects;

            var problems = ProjectsService.Projects.Where(p => p.Issues != "Нет" && !string.IsNullOrEmpty(p.Issues)).Count();
            ViewBag.Problems = problems;

            ViewBag.ProjectStatuses = ProjectsService.Projects.Where(p => !string.IsNullOrEmpty(p.ProjectStatus))
                .GroupBy(p => p.ProjectStatus)
                .Select(k => new
                {
                    ProjectStatus = k.Key,
                    Count = k.Count()
                }).ToList();

            
            var filteredProject = ProjectsService.Projects;

            if (!string.IsNullOrEmpty(category))
            {
                if (category == "Finished")
                {
                    filteredProject = filteredProject.Where(p => p.ProjectStatus == "Закрыт").ToList();
                }
                else if (category == "InProccess")
                {
                    filteredProject = filteredProject.Where(p => p.ProjectStatus == "В процессе").ToList();
                }
                else if (category == "Problems")
                {
                    filteredProject = filteredProject.Where(p => p.Issues != "Нет" && !string.IsNullOrEmpty(p.Issues)).ToList();
                }
                else if (category == "All")
                {
                    filteredProject = ProjectsService.Projects;
                }
            }


            ViewBag.Projects = filteredProject;
            ViewBag.ProjectsCount = filteredProject.Count;

            return View(Categories);
        }
    }

    public class SplineChartData
    {
        public string month;
        public int newProjects;
        public int finishedProjects;

    }

    public class ComboBoxFor
    {
        public string Val { get; set; }
        public string[] Data { get; set; }

        public ComboBoxFor()
        {
            Data = new string[]
            {
                "Все",
                "Законченные",
                "В процессе",
                "Проблемы"
            };
        }
    }
}
