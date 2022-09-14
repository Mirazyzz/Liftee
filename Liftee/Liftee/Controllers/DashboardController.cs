﻿using Liftee.Helpers;
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

        public IActionResult Index(ComboBoxFor model)
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

            // New Projects
            //var newProjects = ProjectsService.Projects.GroupBy(p => DateTime.Parse(p.InstallationStartDate).Month)
            //    .Select(p => new SplineChartData()
            //    {
            //        month = DateTime.Parse(p.First().InstallationStartDate).ToString("MMM"),
            //        newProjects = p.Count()
            //    })
            //    .ToList();

            //var finishedProjects = ProjectsService.Projects.GroupBy(p => DateTime.Parse(p.InstallationFinishDate).Month)
            //    .Select(p => new SplineChartData()
            //    {
            //        month = DateTime.Parse(p.First().InstallationFinishDate).ToString("MMM"),
            //        finishedProjects = p.Count()
            //    })
            //    .ToList();

            //var startDate = new DateTime(DateTime.Now.Year, 1, 1);

            //string[] summary = Enumerable.Range(0, DateTime.Now.Month)
            //    .Select(i => startDate.AddMonths(i).ToString("MMM"))
            //    .ToArray();

            //ViewBag.SplineChartData = from month in summary
            //                          join newProject in newProjects on month equals newProject.month into newProjectMonthJoined
            //                          from newProject in newProjectMonthJoined.DefaultIfEmpty()
            //                          join finishedProject in finishedProjects on month equals finishedProject.month into finishedProjecMonthJoined
            //                          from finishedProject in finishedProjecMonthJoined.DefaultIfEmpty()
            //                          select new
            //                          {
            //                              month = month,
            //                              newProjects = newProject == null ? 0 : newProject.newProjects,
            //                              finishedProjects = finishedProject == null ? 0 : finishedProject.finishedProjects
            //                          };

            var filteredProject = ProjectsService.Projects;

            if(model?.Val == "Законченные")
            {
                filteredProject = filteredProject.Where(p => p.ProjectStatus == "Закрыт").ToList();
            }
            else if(model?.Val == "В процессе")
            {
                filteredProject = filteredProject.Where(p => p.ProjectStatus == "В процессе").ToList();
            }
            else if(model?.Val == "Проблемы")
            {
                filteredProject = filteredProject.Where(p => p.Issues != "Нет" && !string.IsNullOrEmpty(p.Issues)).ToList();
            }
            else if(model?.Val == "Все")
            {
                filteredProject = ProjectsService.Projects;
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
