using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using RetiSusun.Core.Interfaces;
using RetiSusun.Data;
using RetiSusun.Data.Models;

namespace RetiSusun.Core.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly RetiSusunDbContext _context;

    public AuthenticationService(RetiSusunDbContext context)
    {
        _context = context;
    }

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        var user = await _context.Users
            .Include(u => u.Business)
            .Include(u => u.Supplier)
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

        if (user == null)
            return null;

        if (!VerifyPassword(password, user.PasswordHash))
            return null;

        user.LastLoginDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> RegisterUserAsync(string username, string password, string fullName, string email, string role, int businessId)
    {
        // Check if username already exists
        if (await _context.Users.AnyAsync(u => u.Username == username))
            throw new InvalidOperationException("Username already exists");

        var user = new User
        {
            Username = username,
            PasswordHash = HashPassword(password),
            FullName = fullName,
            Email = email,
            Role = role,
            AccountType = "Business",
            BusinessId = businessId,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User> RegisterSupplierUserAsync(string username, string password, string fullName, string email, string role, int supplierId)
    {
        // Check if username already exists
        if (await _context.Users.AnyAsync(u => u.Username == username))
            throw new InvalidOperationException("Username already exists");

        var user = new User
        {
            Username = username,
            PasswordHash = HashPassword(password),
            FullName = fullName,
            Email = email,
            Role = role,
            AccountType = "Supplier",
            SupplierId = supplierId,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        if (!VerifyPassword(oldPassword, user.PasswordHash))
            return false;

        user.PasswordHash = HashPassword(newPassword);
        await _context.SaveChangesAsync();

        return true;
    }

    public string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == hash;
    }
}
