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

        public IActionResult Index(string? category, ComboBoxFor filters)
        {
            // Setup Widget data
            var totalProjects = ProjectsService.Projects.Count;
            ViewBag.TotalProjects = totalProjects;

            var finishedProjects = ProjectsService.Projects.Where(p => p.ProjectStatus == "Закрыт").Count();
            ViewBag.FinishedProjects = finishedProjects;

            var inProcessProjects = ProjectsService.Projects.Where(p => p.ProjectStatus == "В процессе").Count();
            ViewBag.InProcessProjects = inProcessProjects;

            var problems = ProjectsService.Projects.Where(p => p.Issues != "Нет" && !string.IsNullOrEmpty(p.Issues)).Count();
            ViewBag.Problems = problems;

            var debts = ProjectsService.Projects.Where(p => p.MutualSettlement == "Долг").Count();
            ViewBag.Debts = debts;

            // Get values for Filters
            var fitters = ProjectsService.Projects.Where(p => !string.IsNullOrEmpty(p.Fitter))
                .OrderBy(p => p.Fitter)
                .Select(p => p.Fitter)
                .Distinct()
                .ToArray();
            var documents = ProjectsService.Projects.Where(p => !string.IsNullOrEmpty(p.Document))
                .OrderBy(p => p.Document)
                .Select(p => p.Document)
                .Distinct()
                .ToArray();

            Categories.FitterData = fitters;
            Categories.DocumentData = documents;
            
            // Filtering
            var filteredProjects = ProjectsService.Projects;

            if (!string.IsNullOrEmpty(category))
            {
                if (category == "Finished")
                {
                    filteredProjects = filteredProjects.Where(p => p.ProjectStatus == "Закрыт").ToList();
                }
                else if (category == "Debts")
                {
                    filteredProjects = filteredProjects.Where(p => p.MutualSettlement == "Долг").ToList();
                }
                else if (category == "Problems")
                {
                    filteredProjects = filteredProjects.Where(p => p.Issues != "Нет" && !string.IsNullOrEmpty(p.Issues)).ToList();
                }
                else if (category == "All")
                {
                    filteredProjects = ProjectsService.Projects;
                }
            }

            if(filters != null)
            {
                if(filters.FitterValue != null)
                {
                    filteredProjects = filteredProjects.Where(p => p.Fitter == filters.FitterValue).ToList();
                }
                
                if(filters.DocumentValue != null)
                {
                    filteredProjects = filteredProjects.Where(p => p.Document == filters.DocumentValue).ToList();
                }
            }

            ViewBag.Projects = filteredProjects;
            ViewBag.ProjectsCount = filteredProjects.Count;

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
        public string CategoryValue { get; set; }
        public string[] CategoryData { get; set; }

        public string FitterValue { get; set; }
        public string[] FitterData { get; set; }

        public string DocumentValue { get; set; }
        public string[] DocumentData { get; set; }

        public ComboBoxFor()
        {
            CategoryData = new string[]
            {
                "Все",
                "Законченные",
                "В процессе",
                "Проблемы"
            };
        }
    }
}
