using System.Globalization;
using System.Text.RegularExpressions;

namespace HomeApplianceRepair.Utilities;

/// <summary>
/// Static helper class that validates input data (serial numbers, dates, text, amounts).
/// </summary>
public static class Validator
{
    // Example of a valid serial number: RF-10234 (2-3 letters, dash, 4-8 digits)
    private static readonly Regex SerialPattern =
        new(@"^[A-Z]{2,3}-\d{4,8}$", RegexOptions.Compiled);

    public static bool IsValidSerialNumber(string? serialNumber) =>
        !string.IsNullOrWhiteSpace(serialNumber) && SerialPattern.IsMatch(serialNumber);

    public static bool IsValidText(string? text) => !string.IsNullOrWhiteSpace(text);

    public static bool IsValidAmount(decimal amount) => amount >= 0;

    /// <summary>The expected delivery date can not be earlier than the received date.</summary>
    public static bool IsValidDeliveryDate(DateTime received, DateTime expected) =>
        expected.Date >= received.Date;

    /// <summary>Parses a date written as yyyy-MM-dd.</summary>
    public static bool TryParseDate(string? text, out DateTime date) =>
        DateTime.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture,
                               DateTimeStyles.None, out date);
}
