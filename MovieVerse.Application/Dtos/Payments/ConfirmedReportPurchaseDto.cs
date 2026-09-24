namespace MovieVerse.Dtos.Payments;

public class ConfirmedReportPurchaseDto
{
    public Guid PurchaseId { get; set; }

    public string ReportType { get; set; }
        = null!;

    public string? PersonType { get; set; }

    public Guid? PersonId { get; set; }
}