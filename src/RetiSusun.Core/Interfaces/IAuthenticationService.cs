using RetiSusun.Data.Models;

namespace RetiSusun.Core.Interfaces;

public interface IAuthenticationService
{
    Task<User?> AuthenticateAsync(string username, string password);
    Task<User> RegisterUserAsync(string username, string password, string fullName, string email, string role, int businessId);
    Task<User> RegisterSupplierUserAsync(string username, string password, string fullName, string email, string role, int supplierId);
    Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
