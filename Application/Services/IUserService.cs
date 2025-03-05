using Domain.Entities;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IUserService
    {
        Task RegisterAsync(string username, string password);
        Task LoginAsync(string username, string password);
        Task ChangePasswordAsync(string oldPassword, string newPassword);
        Task LogoutAsync();
        Task ChangeStatusAsync(UserStatus newStatus);
        Task<List<User>> SearchUsersAsync(string keyword);
    }
}
