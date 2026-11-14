using HomeApplianceRepair.Enums;

namespace HomeApplianceRepair.Interfaces;

/// <summary>
/// Core operations shared by every appliance handled by the repair center.
/// </summary>
public interface IRepairable
{
    string SerialNumber { get; }
    RepairStatus Status { get; }

    /// <summary>Moves the appliance from Received to UnderRepair.</summary>
    void StartRepair();

    /// <summary>Moves the appliance from UnderRepair to Repaired.</summary>
    void CompleteRepair();

    /// <summary>Hands the repaired appliance back to the customer.</summary>
    void Deliver();

    /// <summary>Calculates the repair cost (rules differ per appliance type).</summary>
    decimal CalculateRepairCost();

    /// <summary>One-line description used when listing appliances.</summary>
    string GetSummary();
}
