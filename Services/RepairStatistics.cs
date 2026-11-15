using HomeApplianceRepair.Enums;

namespace HomeApplianceRepair.Services;

/// <summary>
/// Static class that keeps global counters for the whole repair center.
/// Only the project itself (internal methods) can update the counters;
/// the outside world can only read the static properties.
/// </summary>
public static class RepairStatistics
{
    private static readonly Dictionary<RepairStatus, int> Counts =
        Enum.GetValues<RepairStatus>().ToDictionary(status => status, _ => 0);

    private static int _totalAppliances;
    private static decimal _totalRevenue;

    /// <summary>Number of appliances ever registered.</summary>
    public static int TotalAppliances => _totalAppliances;

    /// <summary>Appliances that are still being repaired.</summary>
    public static int UnderRepairCount => Counts[RepairStatus.UnderRepair];

    /// <summary>Appliances whose repair is finished (repaired or already delivered).</summary>
    public static int RepairedCount => Counts[RepairStatus.Repaired] + Counts[RepairStatus.Delivered];

    /// <summary>Sum of the cost of all completed repairs.</summary>
    public static decimal TotalRepairRevenue => _totalRevenue;

    public static int CountByStatus(RepairStatus status) => Counts[status];

    internal static void RecordCreated()
    {
        _totalAppliances++;
        Counts[RepairStatus.Received]++;
    }

    internal static void RecordTransition(RepairStatus oldStatus, RepairStatus newStatus)
    {
        Counts[oldStatus]--;
        Counts[newStatus]++;
    }

    internal static void RecordRevenue(decimal amount) => _totalRevenue += amount;
}
