using HomeApplianceRepair.Enums;
using HomeApplianceRepair.Events;
using HomeApplianceRepair.Interfaces;
using HomeApplianceRepair.Models;

namespace HomeApplianceRepair.Services;

/// <summary>
/// Main class of the system. It stores the appliances in a List of the base type
/// and processes them polymorphically.
/// </summary>
public class RepairCenter
{
    private readonly List<Appliance> _appliances = new();

    /// <summary>Forwards the status-changed events of all registered appliances.</summary>
    public event ApplianceStatusChangedHandler? ApplianceStatusChanged;

    /// <summary>Raised by <see cref="CheckDelays"/> for every late appliance.</summary>
    public event DeliveryDelayedHandler? DeliveryDelayed;

    public IReadOnlyList<Appliance> Appliances => _appliances;

    public void AddAppliance(Appliance appliance)
    {
        if (_appliances.Any(a => a.SerialNumber == appliance.SerialNumber))
            throw new InvalidOperationException($"Serial number {appliance.SerialNumber} is already registered.");

        appliance.StatusChanged += OnApplianceStatusChanged;
        _appliances.Add(appliance);
    }

    public Appliance? FindBySerial(string serialNumber) =>
        _appliances.FirstOrDefault(a => a.SerialNumber == serialNumber);

    public IEnumerable<Appliance> GetByStatus(RepairStatus status) =>
        _appliances.Where(a => a.Status == status);

    /// <summary>Appliances whose repair is finished.</summary>
    public IEnumerable<Appliance> GetRepaired() =>
        _appliances.Where(a => a.Status == RepairStatus.Repaired || a.Status == RepairStatus.Delivered);

    /// <summary>Appliances that are still in the center (not repaired yet).</summary>
    public IEnumerable<Appliance> GetStillInMaintenance() =>
        _appliances.Where(a => a.Status == RepairStatus.Received || a.Status == RepairStatus.UnderRepair);

    /// <summary>Polymorphism: each object computes its own cost.</summary>
    public decimal CalculateTotalCost() => _appliances.Sum(a => a.CalculateRepairCost());

    /// <summary>Polymorphism: each object prints its own summary.</summary>
    public void PrintAllStatuses()
    {
        foreach (IRepairable item in _appliances)
            Console.WriteLine("  " + item.GetSummary());
    }

    /// <summary>Raises <see cref="DeliveryDelayed"/> for each appliance that is late.</summary>
    public void CheckDelays(DateTime now)
    {
        foreach (Appliance appliance in _appliances.Where(a => a.IsOverdue(now)))
        {
            int daysLate = (now.Date - appliance.ExpectedDeliveryDate.Date).Days;
            DeliveryDelayed?.Invoke(appliance, daysLate);
        }
    }

    private void OnApplianceStatusChanged(object? sender, ApplianceStatusChangedEventArgs e) =>
        ApplianceStatusChanged?.Invoke(this, e);
}
