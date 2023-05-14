using StarkovGroupTest.DataBase;
using StarkovGroupTest.Logic;

var repository = new Repository();

var flags = args.Select((x, i) => new { Index = i, Value = x })
                .Where(iv => iv.Index % 2 == 0)
                .Select(pair => new { Key = pair.Value, Value = args[pair.Index + 1] })
                .ToDictionary(x => x.Key, y => y.Value); ;

var action = flags["-a"];
if (action.Equals("import"))
{
    var filePath = flags["-p"];
    var isExist = File.Exists(filePath);
    if (isExist)
    {
        var tsvHelper = new TsvConverter(filePath);
        var importType = flags["-t"];
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
else if (action.Equals("display"))
{
    var isInt = int.TryParse(flags["-d"], out var depId);
    if (isInt && depId > 0)
    {
        var display = new DisplayManager();
        display.DisplayOneDepartment(depId);
    }
    else
    {
        Console.WriteLine($"Некорректный номер подразделения: {flags["-d"]}");
    }
}
else
{
    Console.WriteLine($"Некорректное действие {action}");
}