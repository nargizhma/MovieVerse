namespace MovieVerse.Dtos.Writers;

public class WriterCreateDto
{
    public string FullName { get; set; } = null!;

    public IFormFile? ProfileImage { get; set; }

    public string? Biography { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? BirthPlace { get; set; }

    public DateTime? DeathDate { get; set; }

    public string? DeathPlace { get; set; }

    public decimal? HeightInMeters { get; set; }

    public string? AlternativeName { get; set; }

    public string? Nickname { get; set; }

    public string? Spouse { get; set; }

    public string? Children { get; set; }

    public string? Parents { get; set; }

    public string? Relatives { get; set; }

    public string? OtherWorks { get; set; }

    public string? Trivia { get; set; }

    public string? Quote { get; set; }

    public string? Trademark { get; set; }
}