namespace MovieVerse.Dtos.Reviews;

public class ReviewReturnDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string UserName { get; set; } = null!;

    public string? DisplayName { get; set; }

    public decimal Rating { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }
}
