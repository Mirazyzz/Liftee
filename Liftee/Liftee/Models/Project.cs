namespace Liftee.Models
{
    public class Project
    {
        public string Id { get; set; } = "";
        public string Contract { get; set; } = "";
        public string PropertyName { get; set; } = "";
        public string ContactDetails { get; set; } = "";
        public string ContractStatus { get; set; } = "";
        public string GP { get; set; } = "";
        public string ElevatorType { get; set; } = "";
        public string SerialNumber { get; set; } = "";
        public string InstallationStartDate { get; set; } = "";
        public string Fitter { get; set; } = "";
        public string InstallationStatus { get; set; } = "";
        public string InstallationFinishDate { get; set; } = "";
        public string Issues { get; set; } = "";
        public string Document { get; set; } = "";
        public string ElevatorPassport { get; set; } = "";
        public string ProjectStatus { get; set; } = "";
        public string MutualSettlement { get; set; } = "";

        public List<string> SerialNumbers { get; set; }

        public bool IsDue
        {
            get
            {
                if(DateTime.TryParse(InstallationFinishDate, out DateTime expectedDate))
                {
                    if(expectedDate < DateTime.UtcNow && ProjectStatus != "Закрыт")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                return false;
            }
        }

        public Project()
        {
            SerialNumbers = new List<string>();
        }
    }
}
