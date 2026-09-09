using MovieVerse.Dtos.Profiles;
using MovieVerse.Requests.Profiles;

namespace MovieVerse.Services.Interfaces;

public interface IUserProfileService
{
    Task<MyProfileReturnDto>
        GetMineAsync(Guid userId);

    Task<UserProfileReturnDto>
        GetByUserNameAsync(
            string userName);

    Task<List<ProfileActivityItemDto>>
        GetMyActivityAsync(
            Guid userId);

    Task UpdateAsync(
        Guid userId,
        ProfileUpdateRequest request);

    Task DeleteProfileImageAsync(
        Guid userId);
}