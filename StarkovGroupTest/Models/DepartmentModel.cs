namespace StarkovGroupTest.Models;

internal class DepartmentModel
{
    public int ID { get; set; }
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;


    public int? ParentID { get; set; }
    public DepartmentModel Department { get; set; } = null!;
    public ICollection<DepartmentModel> Departments { get; } = new List<DepartmentModel>();

    public int? ManagerID { get; set; }
    public EmployeeModel Employee { get; set; } = null!;

    public ICollection<EmployeeModel> Employees { get; } = new List<EmployeeModel>();

    #region Игнорируемые поля
    public string Parent { get; set; } = null!;
    public string Manager { get; set; } = null!;
    public int Nested { get; set; }
    #endregion
}
