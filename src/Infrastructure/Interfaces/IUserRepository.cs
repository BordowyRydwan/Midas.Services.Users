using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IUserRepository
{
    public Task<ulong> AddNewUser(User user);
    public Task<User> GetUserByEmail(string email);
    public Task<User> GetUserById(ulong id);
    public Task<bool> UpdateUserData(User user);
    public Task UpdateUserEmail(string from, string to);
}