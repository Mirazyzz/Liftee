using Liftee.Models;

namespace Liftee.Services
{
    public class ProjectsService
    {
        private static List<Project> _projects = new List<Project>();

        public static List<Project> Projects
        {
            get => _projects;
        }

        public static void AddProjects(IEnumerable<Project> projects)
        {
            if(_projects.Count > 0)
            {
                _projects.Clear();
            }

            foreach(var project in projects)
            {
                var projectExists = _projects.Where(p => p.PropertyName == project.PropertyName).FirstOrDefault();

                if (projectExists != null)
                {
                    projectExists.SerialNumbers.Add(project.SerialNumber);
                }
                else
                {
                    _projects.Add(project);
                }
            }
        }
    }
}
