# Home Appliance Repair Center (C# / OOP)

Assignment TIC_IPG203_F25 - Object-Oriented Programming and Design.

Run: `dotnet run` (requires .NET SDK 8 or later).

| Principle | Where |
|---|---|
| Abstraction | `Interfaces/IRepairable.cs`, `Models/Appliance.cs` (abstract) |
| Inheritance / Polymorphism | `Refrigerator`, `WashingMachine`, `AirConditioner`, `Services/RepairCenter.cs` |
| Encapsulation | private fields + properties, read-only `SerialNumber`, `EnsureEditable()` |
| Delegates & Events | `Events/ApplianceEvents.cs`, `StatusChanged`, `DeliveryDelayed` |
| Static | `Utilities/Validator.cs`, `Services/RepairStatistics.cs` |
