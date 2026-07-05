# CSE 325 — Week 01 Notes

## 1. Web API evidence (ContosoPizza)

My additional record: **"4 Cheese"** (Id 3), added to the seeded Pizzas list in `Services/PizzaService.cs`.
Requests run against `http://localhost:5268/Pizza`.

### GET /Pizza — list (existing pizzas + my "4 Cheese")
```
HTTP/1.1 200 OK
[{"id":1,"name":"Classic Italian","isGlutenFree":false},
 {"id":2,"name":"Veggie","isGlutenFree":true},
 {"id":3,"name":"4 Cheese","isGlutenFree":false}]
```

### POST /Pizza — add a pizza
Body: `{"name":"Hawaiian","isGlutenFree":false}`
```
HTTP/1.1 201 Created
Location: http://localhost:5268/Pizza/4
```

### PUT /Pizza/4 — update it
Body: `{"id":4,"name":"Hawaiian Deluxe","isGlutenFree":true}`
```
HTTP/1.1 204 No Content
```

### DELETE /Pizza/4 — remove it
```
HTTP/1.1 204 No Content
```

### GET /Pizza/4 — confirm deletion
```
HTTP/1.1 404 Not Found
```

## 2. Sales summary function (workWithFiles)

Added to `week01/workWithFiles/Program.cs`:

```csharp
void GenerateSalesSummaryReport(IEnumerable<string> salesFiles, string storesDirectory, string outputPath)
{
  var currency = CultureInfo.GetCultureInfo("en-US");

  double totalSales = 0;
  var details = new StringBuilder();

  foreach (var file in salesFiles)
  {
    string salesJson = File.ReadAllText(file);
    SalesData? data = JsonConvert.DeserializeObject<SalesData?>(salesJson);

    // Skip files without a Total field (e.g. salestotals.json).
    if (data?.Total is null)
    {
      continue;
    }

    double fileTotal = data.Total.Value;
    totalSales += fileTotal;

    string shortName = Path.GetRelativePath(storesDirectory, file);
    details.AppendLine($"  {shortName}: {fileTotal.ToString("C", currency)}");
  }

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
```

Generated output (`salesTotalDir/salesSummary.txt`):
```
Sales Summary
----------------------------
 Total Sales: $2,012.20

 Details:
  sales.json: $88.88
  204/sales.json: $88.88
  203/sales.json: $99.00
  202/sales.json: $1,234.22
  201/sales.json: $501.22
```
