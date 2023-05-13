using StarkovGroupTest.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace StarkovGroupTest.Logic;

/// <summary>
/// Класс конвертирующий TSV файл в рабочую модель
/// </summary>
internal class TsvConverter
{
    private TextInfo _textInfo;
    private List<string[]> _data;

    public TsvConverter(string filePath)
    {
        _textInfo = new CultureInfo("ru-RU").TextInfo;
        _data = LoadDataFromTsv(filePath);
    }

    /// <summary>
    /// Конвертировать TSV в  <see cref="List{}"/> where T: <see cref="JobTitleModel"/>.
    /// </summary>
    /// <returns> Cписок <see cref="List{}"/> where T: <see cref="JobTitleModel"/>. </returns>
    public List<JobTitleModel> ConvertToListJobTitleModels() =>
           _data.Select(line =>
                        new JobTitleModel() { Name = _textInfo.ToTitleCase(_textInfo.ToLower(line[0])) })
                .ToList();

    /// <summary>
    /// Конвертировать TSV в  <see cref="List{}"/> where T: <see cref="EmployeeModel"/>.
    /// </summary>
    /// <returns> Cписок <see cref="List{}"/> where T: <see cref="EmployeeModel"/>. </returns>
    public List<EmployeeModel> ConvertToEmployeeModel() =>
           _data.Select(line => 
                        new EmployeeModel()
                        {
                            DepartmentName = _textInfo.ToTitleCase(_textInfo.ToLower(line[0])),
                            FullName = _textInfo.ToTitleCase(_textInfo.ToLower(line[1])),
                            Login = line[2],
                            Password = line[3],
                            JobTitleName = _textInfo.ToTitleCase(_textInfo.ToLower(line[4]))
                        })
                .ToList();

    /// <summary>
    /// Конвертировать TSV в  <see cref="List{}"/> where T: <see cref="DepartmentModel"/>.
    /// </summary>
    /// <returns> Cписок <see cref="List{}"/> where T: <see cref="DepartmentModel"/>. </returns>
    public List<DepartmentModel> ConvertToDepartmentModel() =>
           _data.Select(line => 
                        new DepartmentModel()
                        {
                            Name = _textInfo.ToTitleCase(_textInfo.ToLower(line[0])),
                            Parent = _textInfo.ToTitleCase(_textInfo.ToLower(line[1])),
                            Manager = _textInfo.ToTitleCase(_textInfo.ToLower(line[2])),
                            Phone = line[3]
                        })
                .ToList();

    /// <summary>
    /// Загрузка данных из TSV файла.
    /// </summary>
    /// <param name="filePath"> Полный путь к файлу. </param>
    /// <returns> Cписок <see cref="List{}"/> where T: <see cref="string[]"/>.</returns>
    private List<string[]> LoadDataFromTsv(string filePath)
    {
        var data = File.ReadAllLines(filePath);

        return data.Skip(1)
                   .Select(line =>
                           line.Split('\t')
                               .Select(x => Regex.Replace(x, "\\s{2,}", " ").Trim())
                               .ToArray())
                   .ToList();
    }
}
