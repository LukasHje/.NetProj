using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnvironmentCrime.Models
{
    public interface IECrimeRepository
    {
        IQueryable<Department> Departments { get; }
        IQueryable<ErrandStatus> ErrandStatuses { get; }
        IQueryable<Employee> Employees { get; }
        IQueryable<Errand> Errands { get; }

        


        //create
        public Errand SaveErrand(Errand errand);

        //update
        Errand CoordinatorUpdate(int errandId, string departmentId);
        Errand ManagerUpdate(int errandId, string employeeId);
        Errand ManagerNoAction(int errandId, string reason);
        Errand InvestigatorUpdate(int errandId, string statusId, string events, string information, string sampleName, string picName);

        //get details for specific errand
        Task<Errand> GetErrandDetail(int id);
        
        //returns the errands with names joined on the id's
        IQueryable<MyErrand> CoordinatorErrands();
        IQueryable<MyErrand> ManagerErrands();
        IQueryable<Employee> DepartmentEmployees();
        IQueryable<MyErrand> InvestigatorErrands();

        //delete
        Errand DeleteErrand(int errandId);
    }
}
