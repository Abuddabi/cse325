using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using Newtonsoft.Json;

var currentDirectory = Directory.GetCurrentDirectory();
var storesDirectory = Path.Combine(currentDirectory, "stores");

var salesTotalDir = Path.Combine(currentDirectory, "salesTotalDir");
Directory.CreateDirectory(salesTotalDir);

var salesFiles = FindFiles(storesDirectory);

var salesTotal = CalculateSalesTotal(salesFiles);

File.AppendAllText(Path.Combine(salesTotalDir, "totals.txt"), $"{salesTotal}{Environment.NewLine}");

GenerateSalesSummaryReport(salesFiles, storesDirectory, Path.Combine(salesTotalDir, "salesSummary.txt"));

IEnumerable<string> FindFiles(string folderName)
{
  List<string> salesFiles = new List<string>();

  var foundFiles = Directory.EnumerateFiles(folderName, "*", SearchOption.AllDirectories);

  foreach (var file in foundFiles)
  {
    var extension = Path.GetExtension(file);
    if (extension == ".json")
    {
      salesFiles.Add(file);
    }
  }

  return salesFiles;
}

double CalculateSalesTotal(IEnumerable<string> salesFiles)
{
  double salesTotal = 0;

  // Loop over each file path in salesFiles
  foreach (var file in salesFiles)
  {
    // Read the contents of the file
    string salesJson = File.ReadAllText(file);

    // Parse the contents as JSON
    SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

    // Add the amount found in the Total field to the salesTotal variable
    salesTotal += data?.Total ?? 0;
  }

  return salesTotal;
}

void GenerateSalesSummaryReport(IEnumerable<string> salesFiles, string storesDirectory, string outputPath)
{
  var currency = CultureInfo.GetCultureInfo("en-US");

  double totalSales = 0;
  var details = new StringBuilder();

  // Read each file once: add its total to the grand total AND to the details section.
  foreach (var file in salesFiles)
  {
    string salesJson = File.ReadAllText(file);
    SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

    // Skip files without a Total field.
    if (data?.Total is null)
    {
      continue;
    }

    double fileTotal = data.Total.Value;
    totalSales += fileTotal;

    // Show a short path relative to the stores folder instead of the full path.
    string shortName = Path.GetRelativePath(storesDirectory, file);
    details.AppendLine($"  {shortName}: {fileTotal.ToString("C", currency)}");
  }

  // Assemble the final report.
  var report = new StringBuilder();
  report.AppendLine("Sales Summary");
  report.AppendLine("----------------------------");
  report.AppendLine($" Total Sales: {totalSales.ToString("C", currency)}");
  report.AppendLine();
  report.AppendLine(" Details:");
  report.Append(details);

  File.WriteAllText(outputPath, report.ToString());
}

record SalesData(double? Total);
