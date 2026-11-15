namespace HomeApplianceRepair.Models;

/// <summary>Refrigerator: expensive parts (compressor) and gas refill.</summary>
public class Refrigerator : Appliance
{
    private const decimal CompressorPrice = 120m;
    private const decimal GasRefillPrice = 30m;

    private readonly int _capacityLiters;
    private bool _compressorReplaced;
    private bool _gasRefillNeeded;

    public Refrigerator(string serialNumber, string brand, string customerName, string faultDescription,
                        DateTime receivedDate, DateTime expectedDeliveryDate, int capacityLiters)
        : base(serialNumber, brand, customerName, faultDescription, receivedDate, expectedDeliveryDate)
    {
        if (capacityLiters <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacityLiters), "Capacity must be positive.");
        _capacityLiters = capacityLiters;
    }

    public override string ApplianceType => "Refrigerator";

    public int CapacityLiters => _capacityLiters;

    public bool CompressorReplaced
    {
        get => _compressorReplaced;
        set { EnsureEditable(); _compressorReplaced = value; }
    }

    public bool GasRefillNeeded
    {
        get => _gasRefillNeeded;
        set { EnsureEditable(); _gasRefillNeeded = value; }
    }

    public override decimal CalculateRepairCost()
    {
        decimal cost = CalculateBaseCost();
        if (_compressorReplaced) cost += CompressorPrice;
        if (_gasRefillNeeded) cost += GasRefillPrice;
        return cost;
    }

    public override string GetSummary() =>
        base.GetSummary() + $" | {CapacityLiters} L | Compressor replaced: {CompressorReplaced}";
}
