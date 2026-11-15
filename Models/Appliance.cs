using HomeApplianceRepair.Enums;
using HomeApplianceRepair.Events;
using HomeApplianceRepair.Interfaces;
using HomeApplianceRepair.Services;
using HomeApplianceRepair.Utilities;

namespace HomeApplianceRepair.Models;

/// <summary>
/// Abstract base class for every appliance. It implements <see cref="IRepairable"/>,
/// holds the shared data and leaves the cost calculation to the derived classes.
/// </summary>
public abstract class Appliance : IRepairable
{
    // ---- Private fields (encapsulation) ----
    private readonly string _serialNumber;          // never changes after creation
    private readonly string _brand;
    private readonly string _customerName;
    private readonly DateTime _receivedDate;
    private readonly DateTime _expectedDeliveryDate;
    private string _faultDescription;
    private RepairStatus _status;
    private decimal _partsCost;
    private decimal _laborHours;

    /// <summary>Hourly labor price (same for all appliances).</summary>
    protected const decimal HourlyRate = 10m;

    /// <summary>Fixed diagnosis fee charged for every appliance.</summary>
    protected const decimal InspectionFee = 15m;

    /// <summary>Raised every time the repair status changes.</summary>
    public event ApplianceStatusChangedHandler? StatusChanged;

    protected Appliance(string serialNumber, string brand, string customerName,
                        string faultDescription, DateTime receivedDate, DateTime expectedDeliveryDate)
    {
        if (!Validator.IsValidSerialNumber(serialNumber))
            throw new ArgumentException("Serial number must look like RF-10234.", nameof(serialNumber));
        if (!Validator.IsValidText(brand))
            throw new ArgumentException("Brand is required.", nameof(brand));
        if (!Validator.IsValidText(customerName))
            throw new ArgumentException("Customer name is required.", nameof(customerName));
        if (!Validator.IsValidText(faultDescription))
            throw new ArgumentException("Fault description is required.", nameof(faultDescription));
        if (!Validator.IsValidDeliveryDate(receivedDate, expectedDeliveryDate))
            throw new ArgumentException("Expected delivery date can not be before the received date.",
                                        nameof(expectedDeliveryDate));

        _serialNumber = serialNumber;
        _brand = brand;
        _customerName = customerName;
        _faultDescription = faultDescription;
        _receivedDate = receivedDate;
        _expectedDeliveryDate = expectedDeliveryDate;
        _status = RepairStatus.Received;

        RepairStatistics.RecordCreated();
    }

    // ---- Read-only properties (no setter): fixed after construction ----
    public string SerialNumber => _serialNumber;
    public string Brand => _brand;
    public string CustomerName => _customerName;
    public DateTime ReceivedDate => _receivedDate;
    public DateTime ExpectedDeliveryDate => _expectedDeliveryDate;

    /// <summary>Current status. It can only change through the repair methods.</summary>
    public RepairStatus Status => _status;

    /// <summary>True once the repair is finished: financial data is then locked.</summary>
    public bool IsLocked => _status == RepairStatus.Repaired || _status == RepairStatus.Delivered;

    // ---- Properties with controlled write access ----
    public string FaultDescription
    {
        get => _faultDescription;
        set
        {
            EnsureEditable();
            if (!Validator.IsValidText(value))
                throw new ArgumentException("Fault description is required.");
            _faultDescription = value;
        }
    }

    public decimal PartsCost
    {
        get => _partsCost;
        set
        {
            EnsureEditable();
            if (!Validator.IsValidAmount(value))
                throw new ArgumentOutOfRangeException(nameof(value), "Parts cost can not be negative.");
            _partsCost = value;
        }
    }

    public decimal LaborHours
    {
        get => _laborHours;
        set
        {
            EnsureEditable();
            if (!Validator.IsValidAmount(value))
                throw new ArgumentOutOfRangeException(nameof(value), "Labor hours can not be negative.");
            _laborHours = value;
        }
    }

    /// <summary>Human readable type name, defined by each derived class.</summary>
    public abstract string ApplianceType { get; }

    /// <summary>Each appliance type has its own cost rules.</summary>
    public abstract decimal CalculateRepairCost();

    // ---- Helpers for derived classes ----
    protected decimal LaborCost => _laborHours * HourlyRate;

    /// <summary>Common part of the cost: inspection + parts + labor.</summary>
    protected decimal CalculateBaseCost() => InspectionFee + _partsCost + LaborCost;

    /// <summary>Derived classes call this before changing their own editable data.</summary>
    protected void EnsureEditable()
    {
        if (IsLocked)
            throw new InvalidOperationException("The repair is finished; data can not be modified anymore.");
    }

    // ---- Repair workflow ----
    public void StartRepair()
    {
        if (_status != RepairStatus.Received)
            throw new InvalidOperationException("Only a received appliance can start repair.");
        ChangeStatus(RepairStatus.UnderRepair);
    }

    public void CompleteRepair()
    {
        if (_status != RepairStatus.UnderRepair)
            throw new InvalidOperationException("Only an appliance under repair can be completed.");
        ChangeStatus(RepairStatus.Repaired);
        RepairStatistics.RecordRevenue(CalculateRepairCost());
    }

    public void Deliver()
    {
        if (_status != RepairStatus.Repaired)
            throw new InvalidOperationException("Only a repaired appliance can be delivered.");
        ChangeStatus(RepairStatus.Delivered);
    }

    /// <summary>True when the appliance is not repaired yet and the expected date has passed.</summary>
    public bool IsOverdue(DateTime now) => !IsLocked && now.Date > _expectedDeliveryDate.Date;

    public virtual string GetSummary() =>
        $"[{ApplianceType}] {Brand} | SN: {SerialNumber} | Customer: {CustomerName} | " +
        $"Status: {Status} | Cost: {CalculateRepairCost():0.00}";

    private void ChangeStatus(RepairStatus newStatus)
    {
        RepairStatus oldStatus = _status;
        _status = newStatus;
        RepairStatistics.RecordTransition(oldStatus, newStatus);
        OnStatusChanged(new ApplianceStatusChangedEventArgs(this, oldStatus, newStatus));
    }

    protected virtual void OnStatusChanged(ApplianceStatusChangedEventArgs e) =>
        StatusChanged?.Invoke(this, e);
}
