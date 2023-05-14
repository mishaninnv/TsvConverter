using StarkovGroupTest.DataBase;
using StarkovGroupTest.Logic;

var repository = new Repository();

try
{
    if (!args.Any() || args.Count() % 2 != 0)
    {
        throw new ArgumentException("Переданы некорректные данные.");
    }

    var flags = args.Distinct()
                    .Select((x, i) => new { Index = i, Value = x })
                    .Where(iv => iv.Index % 2 == 0)
                    .Select(pair => new { Key = pair.Value, Value = args[pair.Index + 1] })
                    .ToDictionary(x => x.Key, y => y.Value);

    if (!flags.ContainsKey("-a"))
    {
        throw new ArgumentException("Не передан режим работы приложения. -a ");
    }

    var action = flags["-a"];
    if (action.Equals("import"))
    {
        if (!flags.ContainsKey("-p") || !flags.ContainsKey("-t"))
        {
            throw new ArgumentException("Отсутствуют уточняющие данные. -p | -t ");
        }

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
                throw new ArgumentException($"Указан недопустимый тип импорта - {importType}");
            }
        }
        else
        {
            throw new ArgumentException($"Файл по заданному пути не найден - {filePath}");
        }
        var display = new DisplayManager();
        display.AllDisplay();
    }
    else if (action.Equals("display"))
    {
        if (!flags.ContainsKey("-d"))
        {
            throw new ArgumentException("Уточните id подразделения. -d");
        }

        var isInt = int.TryParse(flags["-d"], out var depId);
        if (isInt && depId > 0 && depId <= repository.GetDepartments().Count)
        {
            var display = new DisplayManager();
            display.DisplayOneDepartment(depId);
        }
        else
        {
            throw new ArgumentException($"Некорректный номер подразделения: {flags["-d"]}");
        }
    }
    else
    {
        throw new ArgumentException($"Некорректное действие {action}");
    }
}
catch(Exception ex)
{
    Console.WriteLine(ex.Message);
}