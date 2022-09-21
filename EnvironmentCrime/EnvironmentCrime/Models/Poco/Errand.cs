using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.Graph;

namespace EnvironmentCrime.Models
{
    public class Errand
    {
        public int ErrandID { get; set; }
        public String RefNumber { get; set; }

        [Required(ErrorMessage = "Du måste fylla i var du upptäckte brottet")]
        public String Place { get; set; }

        [Required(ErrorMessage = "Du måste fylla i typ av brott")]
        public String TypeOfCrime { get; set; }

        [Required(ErrorMessage = "Du måste fylla i när du upptäckte brottet")]
        [UIHint("Date")]
        public DateTime DateOfObservation { get; set; }
        public String Observation { get; set; }
        public String InvestigatorInfo { get; set; }
        public String InvestigatorAction { get; set; }

        [Required(ErrorMessage = "Du måste fylla i ditt namn")]
        public String InformerName { get; set; }

        [Required(ErrorMessage = "Du måste fylla i ditt telefonnummer")]
        public String InformerPhone { get; set; }
        public String StatusId { get; set; }
        public String DepartmentId { get; set; }
        public String EmployeeId { get; set; }
        public ICollection<Sample> Samples { get; set; }
        public ICollection<Picture> Pictures { get; set; }

    }
}
