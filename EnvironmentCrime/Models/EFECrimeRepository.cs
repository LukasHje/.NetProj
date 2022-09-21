using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EnvironmentCrime.Models
{
    public class EFECrimeRepository : IECrimeRepository
    {
        private ECrimeDbContext context;
        private IHttpContextAccessor contextAcc;

        //konstruktor
        public EFECrimeRepository(ECrimeDbContext ctx, IHttpContextAccessor cont)
        {
            context = ctx;
            contextAcc = cont;
        }

        public IQueryable<Department> Departments => context.Departments;
        public IQueryable<Employee> Employees => context.Employees;
        public IQueryable<Errand> Errands => context.Errands.Include(e => e.Samples).Include(e => e.Pictures);
        public IQueryable<ErrandStatus> ErrandStatuses => context.ErrandStatuses;

        public Task<Errand> GetErrandDetail(int id)
        {
            return Task.Run(() =>
            {
                var errandDetail = Errands.Where(singleErrand => singleErrand.ErrandID == id).First();
                return errandDetail;
            });
        }

        //get coordinator errands with names joined on id's
        public IQueryable<MyErrand> CoordinatorErrands()
        {
            var errandList = from err in Errands
                             join stat in ErrandStatuses on err.StatusId equals stat.StatusId
                             join dep in Departments on err.DepartmentId equals dep.DepartmentId
                             into departmentErrand
                             from deptE in departmentErrand.DefaultIfEmpty()
                             join em in Employees on err.EmployeeId equals em.EmployeeId
                             into employeeErrand
                             from empE in employeeErrand.DefaultIfEmpty()
                             orderby err.RefNumber descending
                             
                             select new MyErrand
                             {
                                 DateOfObservation = err.DateOfObservation,
                                 ErrandId = err.ErrandID,
                                 RefNumber = err.RefNumber,
                                 TypeOfCrime = err.TypeOfCrime,
                                 StatusName = stat.StatusName,
                                 DepartmentName = (err.DepartmentId == null ? "ej tillsatt" : deptE.DepartmentName),
                                 EmployeeName = (err.EmployeeId == null ? "ej tillsatt" : empE.EmployeeName)
                             };
            return errandList;
        }

        //get investigator errands with names joined on id's
        public IQueryable<MyErrand> InvestigatorErrands()
        {
            var userName = contextAcc.HttpContext.User.Identity.Name;

            var errandList = from err in Errands where (err.EmployeeId).Contains(userName)
                             join stat in ErrandStatuses on err.StatusId equals stat.StatusId
                             join dep in Departments on err.DepartmentId equals dep.DepartmentId
                             into departmentErrand
                             from deptE in departmentErrand.DefaultIfEmpty()
                             join em in Employees on err.EmployeeId equals em.EmployeeId
                             into employeeErrand
                             from empE in employeeErrand.DefaultIfEmpty()
                             orderby err.RefNumber descending

                             select new MyErrand
                             {
                                 DateOfObservation = err.DateOfObservation,
                                 ErrandId = err.ErrandID,
                                 RefNumber = err.RefNumber,
                                 TypeOfCrime = err.TypeOfCrime,
                                 StatusName = stat.StatusName,
                                 DepartmentName = deptE.DepartmentName,
                                 EmployeeName = empE.EmployeeName
                             };
            return errandList;
        }

        //get manager errands with names joined on id's
        public IQueryable<MyErrand> ManagerErrands()
        {
            var userName = contextAcc.HttpContext.User.Identity.Name;
            var user = context.Employees.FirstOrDefault(usr => usr.EmployeeId == userName);

            var errandList = from err in Errands where (err.DepartmentId == user.DepartmentId)
                             join stat in ErrandStatuses on err.StatusId equals stat.StatusId
                             join dep in Departments on err.DepartmentId equals dep.DepartmentId
                             into departmentErrand
                             from deptE in departmentErrand.DefaultIfEmpty()
                             join em in Employees on err.EmployeeId equals em.EmployeeId
                             into employeeErrand
                             from empE in employeeErrand.DefaultIfEmpty()
                             orderby err.RefNumber descending

                             select new MyErrand
                             {
                                 DateOfObservation = err.DateOfObservation,
                                 ErrandId = err.ErrandID,
                                 RefNumber = err.RefNumber,
                                 TypeOfCrime = err.TypeOfCrime,
                                 StatusName = stat.StatusName,
                                 DepartmentName = deptE.DepartmentName,
                                 EmployeeName = (err.EmployeeId == null ? "ej tillsatt" : empE.EmployeeName)
                             };
            return errandList;
        }

        //get employees that belongs to a department, however does not include manager
        public IQueryable<Employee> DepartmentEmployees()
        {
            var userName = contextAcc.HttpContext.User.Identity.Name;
            var user = context.Employees.FirstOrDefault(usr => usr.EmployeeId == userName);

            var employeeList = from em in Employees where (em.DepartmentId == user.DepartmentId && em.EmployeeId != userName)
                             join dep in Departments on em.DepartmentId equals dep.DepartmentId
                             into departmentErrand
                             from deptE in departmentErrand.DefaultIfEmpty()

                             select new Employee
                             {
                                 EmployeeName = em.EmployeeName,
                                 EmployeeId = em.EmployeeId
                             };
            return employeeList;
        }

        //create = save errandobject
        public Errand SaveErrand(Errand errand)
        {
            if (errand.ErrandID == 0)
            {
                Sequence dbEntry = context.Sequences.FirstOrDefault(s => s.Id == 1);
                errand.RefNumber = "2020-45-" + dbEntry.CurrentValue;
                errand.StatusId = "S_A";
                context.Errands.Add(errand);
                dbEntry.CurrentValue++;

                context.SaveChanges();
            }
            return errand;
        }

        //Coordinator updates errandobject
        public Errand CoordinatorUpdate(int ErrandID, string DepartmentId)
        {
            Errand dbEntry = context.Errands.FirstOrDefault(s => s.ErrandID == ErrandID);
            //All the variables that you'd want to update is located here. Not variables such as errandId or dateOfObservation.
            if (dbEntry != null && !DepartmentId.Equals("Välj avdelning"))
            {
                if (DepartmentId.Equals("D00"))
                {
                    dbEntry.DepartmentId = ""; // button "spara" is pressed while "småstads kommun" have been selected as a department
                    context.SaveChanges();
                }
                else
                {

                    dbEntry.DepartmentId = DepartmentId; // a department has been selected and the button "spara" has been pressed
                    context.SaveChanges();
                }
            }
            return dbEntry;
        }

        //Manager updates errandobject
        public Errand ManagerUpdate(int ErrandID, string EmployeeId)
        {
            Errand dbEntry = context.Errands.FirstOrDefault(s => s.ErrandID == ErrandID);
            //All the variables that you'd want to update is located here. Not variables such as errandId or dateOfObservation.
            if (dbEntry != null && !EmployeeId.Equals("Välj handläggare"))
            {
                dbEntry.EmployeeId = EmployeeId; // a investigator get's responsibility for the errand

                dbEntry.InvestigatorInfo = ""; // if the manager changes his mind and wants an investigator on the errand, the old reason should be removed
                dbEntry.StatusId = "S_A"; // in the case that was mentioned above, the manager has decided to take action then the status should be "inrapporterad" (S_A) and not "ingen åtgärd" (S_B)
                context.SaveChanges();
            }
            return dbEntry;
        }

        //Manager noAction update on errandObject
        public Errand ManagerNoAction(int ErrandID, string reason)
        {
            Errand dbEntry = context.Errands.FirstOrDefault(s => s.ErrandID == ErrandID);
            //All the variables that you'd want to update is located here. Not variables such as errandId or dateOfObservation.
            if (dbEntry != null)
            {
                dbEntry.EmployeeId = ""; //if the manager decides to take no action on the errand there should be no investigator on the errand
                dbEntry.InvestigatorInfo = reason; // reasons for not handeling errand
                dbEntry.StatusId = "S_B"; // manager choosed not to take an action for this errand
                context.SaveChanges();
            }
            return dbEntry;
        }

        //Investigator updates errandobject
        public Errand InvestigatorUpdate(int ErrandID, string StatusId, string events, string information, string sampleName, string picName)
        {
            Errand dbEntry = context.Errands.FirstOrDefault(s => s.ErrandID == ErrandID);
            //All the variables that you'd want to update is located here. Not variables such as errandId or dateOfObservation.
            if (dbEntry != null && !StatusId.Equals("S_A") && !StatusId.Equals("S_B") && !StatusId.Equals("Välj"))
            {
                dbEntry.StatusId = StatusId; // Status changes from either "in-progress" or "done"

                if (picName != null)
                {
                    Picture picture = new Picture
                    {
                        PictureName = picName,
                        ErrandId = ErrandID
                    };
                    context.Pictures.Add(picture);
                }
                if (sampleName != null)
                {
                    Sample sample = new Sample
                    {
                        SampleName = sampleName,
                        ErrandId = ErrandID
                    };
                    context.Samples.Add(sample);
                }

                if (information != null)
                {
                    dbEntry.InvestigatorInfo += " | " + information; // some information has been added to this errand (appends to the dbEntry)
                }
                if (events != null)
                {
                    dbEntry.InvestigatorAction += " | " + events; // some events has been done to this errand (appends to the dbEntry)
                }

                context.SaveChanges();
            }
            return dbEntry;
        }


        //delete = delete errandobject
        public Errand DeleteErrand(int id)
        {
            Errand dbEntry = context.Errands.FirstOrDefault(s => s.ErrandID == id);
            if (dbEntry != null)
            {
                context.Errands.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
    }
}
