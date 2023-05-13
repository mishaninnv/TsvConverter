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
            if (_context.JobTitle.FirstOrDefault(x => x.Name.Equals(jobTitleModel.Name)) != null)
            {
                continue;
            }
            _context.JobTitle.Add(jobTitleModel);
            _context.SaveChanges();
        }
    }

    public void AddDepartments(List<DepartmentModel> departmentModels)
    {
        foreach (var departmentModel in departmentModels)
        {
            var department = _context.Departments.Where(x => x.Name == departmentModel.Parent).FirstOrDefault();
            var employee = _context.Employees.Where(x => x.FullName.Equals(departmentModel.Manager)).FirstOrDefault();
            if (department != null || string.IsNullOrWhiteSpace(departmentModel.Parent))
            {
                departmentModel.ParentID = department?.ID ?? null;
                departmentModel.ManagerID = employee?.ID ?? null;

                var existing = _context.Departments.FirstOrDefault(x => x.Name.Equals(departmentModel.Name) &&
                                                                       x.ParentID.Equals(departmentModel.ParentID));
                if (existing != null)
                {
                    existing.ManagerID = departmentModel.ManagerID;
                }
                else 
                {
                    _context.Departments.Add(departmentModel);
                }
                _context.SaveChanges();
            }
            else 
            {
                Console.Error.WriteLine($"Запись: name - {departmentModel.Name}, " +
                                        $"manager - {departmentModel.Manager}, " +
                                        $"parent - {departmentModel.Parent} --- не обработана.");
            }
        }
    }

    public void AddEmployees(List<EmployeeModel> employeeModels)
    {
        foreach (var employeeModel in employeeModels)
        {
            var department = _context.Departments.Where(x => x.Name.Equals(employeeModel.DepartmentName)).FirstOrDefault();
            var jobTitle = _context.JobTitle.Where(x => x.Name.Equals(employeeModel.JobTitleName)).FirstOrDefault();

            if (department != null && jobTitle != null)
            {
                employeeModel.Department = department.ID;
                employeeModel.JobTitle = jobTitle.ID;

                var existing = _context.Employees.FirstOrDefault(x => x.FullName.Equals(employeeModel.FullName));
                if (existing != null)
                {
                    existing.Password = employeeModel.Password;
                    existing.Department = employeeModel.Department;
                }
                else
                {
                    _context.Employees.Add(employeeModel);
                }
                _context.SaveChanges();
            }
            else
            {
                Console.Error.WriteLine($"Запись name - {employeeModel.FullName}, " +
                                        $"department - {employeeModel.DepartmentName}, " +
                                        $"job title - {employeeModel.JobTitleName}, " +
                                        $"login - {employeeModel.Login} не обработана.");
            }
        }
    }

    public List<DepartmentModel> GetDepartments() => 
        _context.Departments.OrderBy(x => x.ID).ThenBy(x => x.Name).ToList();

    public DepartmentModel GetDepartment(int id) => 
        _context.Departments.FirstOrDefault(x => x.ID == id) ?? new DepartmentModel();


    public List<EmployeeModel> GetEmployees(int departmentId) =>
        _context.Employees.Where(x => x.Department == departmentId)
                          .OrderBy(x => x.FullName)
                          .ToList();


    public EmployeeModel GetEmployee(int employeeId) =>
        _context.Employees.FirstOrDefault(x => x.ID == employeeId) ?? new EmployeeModel();

    public JobTitleModel GetJobTitle(int jobId) =>
        _context.JobTitle.FirstOrDefault(x => x.ID == jobId) ?? new JobTitleModel();
}