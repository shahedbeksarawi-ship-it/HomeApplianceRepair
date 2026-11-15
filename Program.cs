using HomeApplianceRepair.Enums;
using HomeApplianceRepair.Models;
using HomeApplianceRepair.Services;
using HomeApplianceRepair.Utilities;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var center = new RepairCenter();

// Subscribe to the events (delegates)
center.ApplianceStatusChanged += (sender, e) =>
{
    Console.WriteLine($"  [EVENT] {e.Appliance.ApplianceType} {e.Appliance.SerialNumber}: " +
                      $"{e.OldStatus} -> {e.NewStatus} ({e.ChangedAt:HH:mm:ss})");
    if (e.NewStatus == RepairStatus.Repaired)
        Console.WriteLine($"  [NOTIFY] Repair completed - call {e.Appliance.CustomerName}.");
};
center.DeliveryDelayed += (appliance, daysLate) =>
    Console.WriteLine($"  [DELAY] {appliance.ApplianceType} {appliance.SerialNumber} is {daysLate} day(s) late " +
                      $"(customer: {appliance.CustomerName}).");

DateTime today = DateTime.Today;

Console.WriteLine("=== 1. Validation (static class) ===");
Console.WriteLine($"  IsValidSerialNumber(\"RF-10234\") = {Validator.IsValidSerialNumber("RF-10234")}");
Console.WriteLine($"  IsValidSerialNumber(\"123\")      = {Validator.IsValidSerialNumber("123")}");
try
{
    _ = new Refrigerator("bad", "LG", "Test", "No cooling", today, today.AddDays(2), 300);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"  Rejected invalid appliance: {ex.Message}");
}

Console.WriteLine("\n=== 2. Registering appliances ===");
var fridge = new Refrigerator("RF-10234", "Samsung", "Ahmad", "Not cooling",
                              today.AddDays(-2), today.AddDays(3), 450);
var washer = new WashingMachine("WM-20455", "Bosch", "Layla", "Water leak and strange noise",
                                today.AddDays(-9), today.AddDays(-3), 9);   // already late
var ac = new AirConditioner("AC-30871", "Gree", "Omar", "Warm air",
                            today.AddDays(-1), today.AddDays(4), 24000);
var fridge2 = new Refrigerator("RF-10999", "Toshiba", "Sara", "Door seal broken",
                               today, today.AddDays(2), 300);

center.AddAppliance(fridge);
center.AddAppliance(washer);
center.AddAppliance(ac);
center.AddAppliance(fridge2);
center.PrintAllStatuses();

Console.WriteLine("\n=== 3. Repair workflow ===");
fridge.StartRepair();
fridge.PartsCost = 35m;
fridge.LaborHours = 3m;
fridge.CompressorReplaced = true;
fridge.CompleteRepair();

washer.StartRepair();
washer.PartsCost = 20m;
washer.LaborHours = 2m;
washer.HasWaterLeak = true;

ac.StartRepair();
ac.PartsCost = 15m;
ac.LaborHours = 2m;
ac.GasRefillNeeded = true;
ac.CompleteRepair();
ac.Deliver();

Console.WriteLine("\n=== 4. Encapsulation: modifying a finished repair ===");
try
{
    fridge.PartsCost = 1m;   // locked after CompleteRepair()
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"  Blocked: {ex.Message}");
}
// fridge.SerialNumber = "X";  // compile error: the property is read-only

Console.WriteLine("\n=== 5. Delay check (event) ===");
center.CheckDelays(DateTime.Now);

Console.WriteLine("\n=== 6. All appliances (polymorphism) ===");
center.PrintAllStatuses();
Console.WriteLine($"  Total cost of all appliances: {center.CalculateTotalCost():0.00}");

Console.WriteLine("\n=== 7. Repaired vs still in maintenance ===");
Console.WriteLine("  Repaired:");
foreach (Appliance a in center.GetRepaired()) Console.WriteLine("    - " + a.SerialNumber);
Console.WriteLine("  Still in maintenance:");
foreach (Appliance a in center.GetStillInMaintenance()) Console.WriteLine("    - " + a.SerialNumber);

Console.WriteLine("\n=== 8. Static statistics ===");
Console.WriteLine($"  Total appliances : {RepairStatistics.TotalAppliances}");
Console.WriteLine($"  Under repair     : {RepairStatistics.UnderRepairCount}");
Console.WriteLine($"  Repaired         : {RepairStatistics.RepairedCount}");
Console.WriteLine($"  Revenue so far   : {RepairStatistics.TotalRepairRevenue:0.00}");
