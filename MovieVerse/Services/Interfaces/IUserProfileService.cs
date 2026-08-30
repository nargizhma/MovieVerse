using MovieVerse.Dtos.Profiles;

namespace MovieVerse.Services.Interfaces;

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