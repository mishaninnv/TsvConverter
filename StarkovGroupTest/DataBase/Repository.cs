using Microsoft.EntityFrameworkCore;
using StarkovGroupTest.Models;

namespace StarkovGroupTest.DataBase;

internal class Repository
{
    private Context _context;

    public Repository()
    { 
        _context = new Context();
    }

    public void AddJobTitles(List<JobTitleModel> jobTitleModels)
    {
        foreach (var jobTitleModel in jobTitleModels)
        {
            var existJobTitle = _context.JobTitle.AsNoTracking()
                                                 .FirstOrDefault(x => x.Name.Equals(jobTitleModel.Name));
            if (existJobTitle != null)
            {
                continue;
            }
            _context.JobTitle.Add(jobTitleModel);
        }
        Save();
    }

    public void AddDepartments(List<DepartmentModel> departmentModels)
    {
        foreach (var departmentModel in departmentModels)
        {
            var parentDep = _context.Departments.AsNoTracking()
                                                 .FirstOrDefault(x => x.Name == departmentModel.Parent);

            if (parentDep != null || string.IsNullOrWhiteSpace(departmentModel.Parent))
            {
                var employee = _context.Employees.AsNoTracking()
                                                 .FirstOrDefault(x => x.FullName.Equals(departmentModel.Manager));

                departmentModel.ManagerID = employee?.ID ?? null;
                departmentModel.ParentID = parentDep?.ID ?? null;

                var existDep = _context.Departments.AsNoTracking()
                                                   .FirstOrDefault(x => x.Name.Equals(departmentModel.Name) &&
                                                                        x.ParentID.Equals(departmentModel.ParentID));
                if (existDep != null)
                {
                    existDep.ManagerID = departmentModel.ManagerID;
                }
                else 
                {
                    _context.Departments.Add(departmentModel);
                }
                Save();
            }
            else 
            {
                Console.Error.WriteLine($"Запись: name - {departmentModel.Name}, " +
                                        $"manager - {departmentModel.Manager}, " +
                                        $"parent - {departmentModel.Parent} --- не обработана.");
            }
        }
    }

    public void AddEmployees(List<EmployeeModel> empModels)
    {
        for (var i = 0; i < empModels.Count; i++)
        {
            var department = _context.Departments.AsNoTracking()
                                                 .FirstOrDefault(x => x.Name.Equals(empModels[i].DepartmentName));
            var jobTitle = _context.JobTitle.AsNoTracking()
                                            .FirstOrDefault(x => x.Name.Equals(empModels[i].JobTitleName));

            if (department != null && jobTitle != null)
            {
                empModels[i].Department = department.ID;
                empModels[i].JobTitle = jobTitle.ID;

                var existEmp = _context.Employees.AsNoTracking()
                                                 .FirstOrDefault(x => x.FullName.Equals(empModels[i].FullName));
                if (existEmp != null)
                {
                    existEmp.Password = empModels[i].Password;
                    existEmp.Department = empModels[i].Department;
                }
                else
                {
                    _context.Employees.Add(empModels[i]);
                }

                if (i % 1000 == 0)
                {
                    Save();
                }
            }
            else
            {
                Console.Error.WriteLine($"Запись name - {empModels[i].FullName}, " +
                                        $"department - {empModels[i].DepartmentName}, " +
                                        $"job title - {empModels[i].JobTitleName}, " +
                                        $"login - {empModels[i].Login} не обработана.");
            }
        }
        Save();
    }

    public List<DepartmentModel> GetDepartments() => 
        _context.Departments.AsNoTracking()
                            .OrderBy(x => x.ID)
                            .ThenBy(x => x.Name)
                            .ToList();

    public DepartmentModel GetDepartment(int id) => 
        _context.Departments.AsNoTracking()
                            .FirstOrDefault(x => x.ID == id) ?? new DepartmentModel();


    public List<EmployeeModel> GetEmployees(int departmentId) =>
        _context.Employees.AsNoTracking()
                          .Where(x => x.Department == departmentId)
                          .OrderBy(x => x.FullName)
                          .ToList();


    public EmployeeModel GetEmployee(int employeeId) =>
        _context.Employees.AsNoTracking()
                          .FirstOrDefault(x => x.ID == employeeId) ?? new EmployeeModel();

    public JobTitleModel GetJobTitle(int jobId) =>
        _context.JobTitle.AsNoTracking()
                         .FirstOrDefault(x => x.ID == jobId) ?? new JobTitleModel();

    private void Save() => _context.SaveChanges();
}