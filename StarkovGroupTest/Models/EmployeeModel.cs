namespace StarkovGroupTest.Models;

internal class EmployeeModel
{
    public int ID { get; set; }
    public string FullName { get; set; } = null!;
    public string Login { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string DepartmentName { get; set; } = null!;
    public string JobTitleName { get; set; } = null!;

    public int JobTitle { get; set; }
    public JobTitleModel JobTitleModel { get; set; } = null!;

    public int Department { get; set; }
    public DepartmentModel DepartmentModel { get; set; } = null!;

    public ICollection<DepartmentModel> Departments { get; } = new List<DepartmentModel>();
}
