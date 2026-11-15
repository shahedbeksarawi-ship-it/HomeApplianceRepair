namespace HomeApplianceRepair.Models;

/// <summary>Air conditioner: on-site work makes labor more expensive, plus gas and outdoor unit work.</summary>
public class AirConditioner : Appliance
{
    private const decimal GasRefillPrice = 40m;
    private const decimal OutdoorUnitPrice = 60m;
    private const decimal HighCapacityFee = 30m;
    private const int HighCapacityBtu = 18000;
    private const decimal LaborFactor = 1.25m; // work at height / installation difficulty

    private readonly int _capacityBtu;
    private bool _gasRefillNeeded;
    private bool _outdoorUnitRepaired;

    public AirConditioner(string serialNumber, string brand, string customerName, string faultDescription,
                          DateTime receivedDate, DateTime expectedDeliveryDate, int capacityBtu)
        : base(serialNumber, brand, customerName, faultDescription, receivedDate, expectedDeliveryDate)
    {
        if (capacityBtu <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacityBtu), "Capacity must be positive.");
        _capacityBtu = capacityBtu;
    }

    public override string ApplianceType => "AirConditioner";

    public int CapacityBtu => _capacityBtu;

    public bool GasRefillNeeded
    {
        get => _gasRefillNeeded;
        set { EnsureEditable(); _gasRefillNeeded = value; }
    }

    public bool OutdoorUnitRepaired
    {
        get => _outdoorUnitRepaired;
        set { EnsureEditable(); _outdoorUnitRepaired = value; }
    }

    public override decimal CalculateRepairCost()
    {
        decimal cost = InspectionFee + PartsCost + LaborCost * LaborFactor;
        if (_gasRefillNeeded) cost += GasRefillPrice;
        if (_outdoorUnitRepaired) cost += OutdoorUnitPrice;
        if (_capacityBtu > HighCapacityBtu) cost += HighCapacityFee;
        return cost;
    }

    public override string GetSummary() =>
        base.GetSummary() + $" | {CapacityBtu} BTU | Gas refill: {GasRefillNeeded}";
}
