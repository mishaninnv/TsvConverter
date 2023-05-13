using StarkovGroupTest.DataBase;
using StarkovGroupTest.Models;

namespace StarkovGroupTest.Logic;

internal class DisplayManager
{
    private Repository _repository;

    public DisplayManager() 
    {
        _repository = new Repository();
    }

    /// <summary>
    /// Полный вывод данных по всем подразделениям с сотрудниками и должностями в консоль.
    /// </summary>
    public void AllDisplay()
    {  
        var depData = GetNestingDepartmentCollection();
        foreach (var dep in depData)
        {
            DisplayDepartment(dep);
        }   
    }

    /// <summary>
    /// Вывод данных по одному подразделению с его работниками (должностями) и 
    /// иерархию вышестоящих подразделений без сотрудников.
    /// </summary>
    /// <param name="depId"> Идентификатор подразделения. </param>
    public void DisplayOneDepartment(int depId)
    {
        var stack = new Stack<DepartmentModel>();
        SetStackDepartment(stack, depId);

        foreach (var dep in stack)
        {
            if (dep.ID == depId)
            {
                DisplayDepartment(dep);
            }
            else 
            {
                DisplayDepartmentInfo(dep);
            }
        }
    }

    /// <summary>
    /// Вывод данных подразделения. Информация подразделения, руководитель, сотрудники.
    /// </summary>
    /// <param name="dep"> Модель подразделения для вывода. </param>
    private void DisplayDepartment(DepartmentModel dep)
    {
        DisplayDepartmentInfo(dep);

        var employees = _repository.GetEmployees(dep.ID);
        DisplayBossInfo(dep, employees);

        foreach (var employee in employees)
        {
            DisplayEmployeeInfo(employee, dep.Nested, '-');
        }
    }

    /// <summary>
    /// Вывод информации подразделения (Id, Name).
    /// </summary>
    /// <param name="dep"> Модель подразделения для вывода. </param>
    private void DisplayDepartmentInfo(DepartmentModel dep) => 
        Console.WriteLine($"{GetEqualSings(dep.Nested)}{dep.ID}-{dep.Name}");

    /// <summary>
    /// Вывод информации по руководителю подразделения.
    /// </summary>
    /// <param name="dep"> Модель подразделения руководителя. </param>
    /// <param name="employees"> Список сотрудников подразделения. </param>
    private void DisplayBossInfo(DepartmentModel dep, List<EmployeeModel> employees)
    {
        if (dep.ManagerID != null)
        {
            var boss = _repository.GetEmployee((int)dep.ManagerID);
            if (boss != null)
            {
                DisplayEmployeeInfo(boss, dep.Nested, '*');
                employees.Remove(boss);
            }
        }
    }

    /// <summary>
    /// Вывод информации по сотруднику (ID, FullName, JobTitle).
    /// </summary>
    /// <param name="employee"></param>
    /// <param name="nested"></param>
    /// <param name="prefix"></param>
    private void DisplayEmployeeInfo(EmployeeModel employee, int nested, char prefix)
    {
        var jobTitle = _repository.GetJobTitle(employee.JobTitle);
        Console.WriteLine($"{GetSpaces(nested)}{prefix}ID:{employee.ID}, Full name:{employee.FullName} ({jobTitle.Name})");
    }

    /// <summary>
    /// Получение списка подразделений с вложенностью.
    /// </summary>
    /// <returns> Список всех подразделений. </returns>
    private List<DepartmentModel> GetNestingDepartmentCollection()
    {
        var result = new List<DepartmentModel>();
        var data = _repository.GetDepartments();
        foreach (var department in data) 
        {
            var nested = 1;
            SetNested(ref nested, department.ParentID);
            department.Nested = nested;
            result.Add(department);
        }
        return result.OrderBy(x => x.Nested).ThenBy(x => x.Name).ToList();
    }

    /// <summary>
    /// Установка вложенности подразделения. (рекурсия)
    /// </summary>
    /// <param name="nested"> Текущая вложенность. </param>
    /// <param name="depId"> Id подразделения. </param>
    private void SetNested(ref int nested, int? depId)
    {
        if (depId != null)
        {
            nested++;
            var parentDep = _repository.GetDepartment((int)depId);
            SetNested(ref nested, parentDep.ParentID);
        }
    }

    /// <summary>
    /// Установить стек подразделений от текущего до верхнего.
    /// </summary>
    /// <param name="stack"> Текущий стек подразделений. </param>
    /// <param name="depId"> Id подразделения. </param>
    private void SetStackDepartment(Stack<DepartmentModel> stack, int? depId)
    {
        if (depId != null)
        {
            var parentDep = _repository.GetDepartment((int)depId);
            var nested = 1;
            SetNested(ref nested, parentDep.ParentID);
            parentDep.Nested = nested;
            stack.Push(parentDep);
            SetStackDepartment(stack, parentDep.ParentID);
        }
    }

    private string GetSpaces(int nested) => string.Concat(Enumerable.Repeat(" ", nested - 1));
    private string GetEqualSings(int nested) => string.Concat(Enumerable.Repeat("=", nested));
}
