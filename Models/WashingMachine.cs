namespace HomeApplianceRepair.Models;

/// <summary>Washing machine: motor/leak repairs and a surcharge for big drums.</summary>
public class WashingMachine : Appliance
{
    private const decimal MotorPrice = 80m;
    private const decimal LeakFixPrice = 25m;
    private const int HeavyDrumKg = 9;
    private const decimal HeavyDrumFactor = 1.10m;

    private readonly int _drumCapacityKg;
    private bool _motorReplaced;
    private bool _hasWaterLeak;

    public WashingMachine(string serialNumber, string brand, string customerName, string faultDescription,
                          DateTime receivedDate, DateTime expectedDeliveryDate, int drumCapacityKg)
        : base(serialNumber, brand, customerName, faultDescription, receivedDate, expectedDeliveryDate)
    {
        if (drumCapacityKg <= 0)
            throw new ArgumentOutOfRangeException(nameof(drumCapacityKg), "Drum capacity must be positive.");
        _drumCapacityKg = drumCapacityKg;
    }

    public override string ApplianceType => "WashingMachine";

    public int DrumCapacityKg => _drumCapacityKg;

    public bool MotorReplaced
    {
        get => _motorReplaced;
        set { EnsureEditable(); _motorReplaced = value; }
    }

    public bool HasWaterLeak
    {
        get => _hasWaterLeak;
        set { EnsureEditable(); _hasWaterLeak = value; }
    }

    public override decimal CalculateRepairCost()
    {
        decimal cost = CalculateBaseCost();
        if (_motorReplaced) cost += MotorPrice;
        if (_hasWaterLeak) cost += LeakFixPrice;
        if (_drumCapacityKg >= HeavyDrumKg) cost *= HeavyDrumFactor; // heavy-duty surcharge
        return cost;
    }

    public override string GetSummary() =>
        base.GetSummary() + $" | {DrumCapacityKg} kg | Motor replaced: {MotorReplaced}";
}
