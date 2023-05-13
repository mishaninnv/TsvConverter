namespace StarkovGroupTest.Models;

internal class JobTitleModel
{
    public int ID { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<EmployeeModel> Employees { get; } = new List<EmployeeModel>();
}
