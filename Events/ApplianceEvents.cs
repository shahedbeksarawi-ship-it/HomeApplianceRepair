using HomeApplianceRepair.Enums;
using HomeApplianceRepair.Models;

namespace HomeApplianceRepair.Events;

/// <summary>Delegate used to notify that an appliance changed its repair status.</summary>
public delegate void ApplianceStatusChangedHandler(object? sender, ApplianceStatusChangedEventArgs e);

/// <summary>Delegate used to notify that an appliance missed its expected delivery date.</summary>
public delegate void DeliveryDelayedHandler(Appliance appliance, int daysLate);

/// <summary>Data carried by the status-changed event (immutable).</summary>
public class ApplianceStatusChangedEventArgs : EventArgs
{
    public ApplianceStatusChangedEventArgs(Appliance appliance, RepairStatus oldStatus, RepairStatus newStatus)
    {
        Appliance = appliance;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        ChangedAt = DateTime.Now;
    }

    public Appliance Appliance { get; }
    public RepairStatus OldStatus { get; }
    public RepairStatus NewStatus { get; }
    public DateTime ChangedAt { get; }
}
