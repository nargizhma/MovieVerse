using MovieVerse.Dtos.Profiles;

public interface IUserProfileService
{
    Task<MyProfileReturnDto> GetMineAsync(
        Guid userId);

    Task<UserProfileReturnDto> GetByUserNameAsync(
        string userName);

    Task<List<ProfileActivityItemDto>>
        GetMyActivityAsync(
            Guid userId);

    Task UpdateAsync(
        Guid userId,
        ProfileUpdateDto dto);

    Task DeleteProfileImageAsync(
        Guid userId);
}