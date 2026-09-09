namespace MovieVerse.Abstractions.Identity;

public interface IIdentityService
{
    Task<IdentityUserInfo?>
        FindByIdAsync(Guid userId);

    Task<IdentityUserInfo?>
        FindByEmailAsync(string email);

    Task<IdentityUserInfo?>
        FindByUserNameAsync(string userName);

    Task<List<IdentityUserInfo>>
        GetAllUsersAsync();

    Task<IdentityOperationResult>
        CreateAsync(
            string email,
            string userName,
            string password);

    Task<bool> CheckPasswordAsync(
        Guid userId,
        string password);

    Task<IReadOnlyList<string>>
        GetRolesAsync(Guid userId);

    Task<IdentityOperationResult>
        AddToRoleAsync(
            Guid userId,
            string role);

    Task<IdentityOperationResult>
        RemoveFromRolesAsync(
            Guid userId,
            IEnumerable<string> roles);

    Task<int> CountUsersAsync();

    Task<int> CountUsersInRoleAsync(
        string role);
}

public class IdentityUserInfo
{
    public Guid Id { get; set; }

    public string UserName { get; set; }
        = string.Empty;

    public string Email { get; set; }
        = string.Empty;

    public string? DisplayName { get; set; }

    public string? Bio { get; set; }

    public string? ProfileImageUrl { get; set; }

    public List<string> Roles { get; set; }
        = [];
}

public class IdentityOperationResult
{
    public bool Succeeded { get; set; }

    public Guid? UserId { get; set; }

    public List<string> Errors { get; set; }
        = [];
}