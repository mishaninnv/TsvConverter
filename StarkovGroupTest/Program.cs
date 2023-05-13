using StarkovGroupTest.DataBase;
using StarkovGroupTest.Logic;

var repository = new Repository();

var action = args[0];
if (action.Equals("import"))
{
    if (args.Length == 3)
    {
        var filePath = args[1];
        var isExist = File.Exists(filePath);
        if (isExist)
        {
            var tsvHelper = new TsvConverter(filePath);
            var importType = args[2];
            if (importType.ToLower().Equals("departments"))
            {
                var dep = tsvHelper.ConvertToDepartmentModel();
                repository.AddDepartments(dep);
            }
            else if (importType.ToLower().Equals("employees"))
            {
                var emp = tsvHelper.ConvertToEmployeeModel();
                repository.AddEmployees(emp);
            }
            else if (importType.ToLower().Equals("jobtitles"))
            {
                var job = tsvHelper.ConvertToListJobTitleModels();
                repository.AddJobTitles(job);
            }
            else
            {
                Console.WriteLine($"Указан недопустимый тип импорта - {importType}");
            }
        }
        else
        {
            Console.WriteLine($"Файл по заданному пути не найден - {filePath}");
        }
        var display = new DisplayManager();
        display.AllDisplay();
    }
    else 
    {
        Console.WriteLine("Недопустимое количество параметров.");
    }
}
else if (action.Equals("display"))
{
    var isInt = int.TryParse(args[1], out var depId);
    if (isInt && depId > 0 )
    {
        var display = new DisplayManager();
        display.DisplayOneDepartment(depId);
    }
    else
    {
        Console.WriteLine($"Некорректный номер подразделения: {args[1]}");
    }
}
else 
{
    Console.WriteLine($"Некорректное действие {action}");
}