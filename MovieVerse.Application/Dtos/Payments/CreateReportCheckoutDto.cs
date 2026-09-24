namespace MovieVerse.Dtos.Payments;

public class CreateReportCheckoutDto
{

    public string ReportType { get; set; }
        = null!;


    public string? PersonType { get; set; }

    public Guid? PersonId { get; set; }
}