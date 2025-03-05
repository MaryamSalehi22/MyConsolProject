using Application.Exceptions;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        public User CurrentUser { get; private set; }

        public UserService(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task RegisterAsync(string username, string password)
        {
            var user = new User { UserName = username };
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                throw new AppException("Registration Failed! " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        public async Task LoginAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                throw new AppException("Login Failed or the Password is wrong");

            CurrentUser = user;
        }
        public async Task ChangePasswordAsync(string oldPassword, string newPassword)
        {
            if (CurrentUser == null)
                throw new AppException("Please Login!");

            var result = await _userManager.ChangePasswordAsync(
                CurrentUser,
                oldPassword,
                newPassword
            );

            if (!result.Succeeded)
                throw new AppException("Password Changes Failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        public async Task LogoutAsync()
        {
            if (CurrentUser == null)
                throw new AppException("No one Login to your System");

            CurrentUser = null;
            Console.WriteLine("Logout Succsec‌!");
        }
        public async Task ChangeStatusAsync(UserStatus newStatus)
        {
            if (CurrentUser == null)
                throw new AppException("First Login to your Account!");

            if (CurrentUser.Status == newStatus)
                throw new AppException($"new status ({newStatus}) can't be the same as current status ({CurrentUser.Status}) ");

            CurrentUser.Status = newStatus;

            var result = await _userManager.UpdateAsync(CurrentUser);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new AppException($"Status change fail: {errors}");
            }

            CurrentUser = await _userManager.FindByIdAsync(CurrentUser.Id.ToString());
        }
        public async Task<List<User>> SearchUsersAsync(string keyword)
        {
            if (CurrentUser == null)
                throw new AppException("First Login to your Account!");

            return await _userManager.Users
                .Where(u => u.UserName.Contains(keyword))
                .ToListAsync();
        }
    }
}
