using Application.Dto;

namespace Application.Interfaces;

public interface IUserService : IInternalService
{
    public Task<UserRegisterReturnDto> RegisterNewUser(UserRegisterDto user);
    public Task<bool> UpdateUserData(UserUpdateDto user);
    public Task UpdateUserEmail(UserUpdateEmailDto user);
    public Task<UserDto> GetUserByEmail(string email);
    public Task<UserDto> GetUserById(ulong id);
}