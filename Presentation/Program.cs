using Application.Exceptions;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Presentation
{
    internal class Program
    {
        private readonly IUserService _userService;

        public Program(IUserService userService)
        {
            _userService = userService; 
        }
        static async Task Main(string[] args)
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            var userService = serviceProvider.GetService<IUserService>();

            while (true)
            {
                Console.Write("> ");
                var input = Console.ReadLine();
                var commandArgs = ParseCommand(input);

                try
                {
                    switch (commandArgs["instruction"])
                    {
                        case "Register":
                            await userService.RegisterAsync(commandArgs["--username"], commandArgs["--password"]);
                            Console.WriteLine("Succes Registration!");
                            break;

                        case "Login":
                            await userService.LoginAsync(commandArgs["--username"], commandArgs["--password"]);
                            Console.WriteLine("Succsec Login!");
                            break;

                        case "ChangePassword":
                            await userService.ChangePasswordAsync(
                                commandArgs["--old"],
                                commandArgs["--new"]
                            );
                            Console.WriteLine("Succsec PasswordChange!");
                            break;

                        case "Change":
                            var newStatus = commandArgs["--status"] == "available"
                                ? UserStatus.Available
                                : UserStatus.NotAvailable;
                            await userService.ChangeStatusAsync(newStatus);
                            break;

                        case "Search":
                            var users = await userService.SearchUsersAsync(commandArgs["--username"]);
                            Console.WriteLine("Result:");
                            for (int i = 0; i < users.Count; i++)
                                Console.WriteLine($"{i + 1}- {users[i].UserName} status: {users[i].Status}");
                            break;

                        case "Logout":
                            await userService.LogoutAsync();
                            break;
                    }
                }
                catch (AppException ex)
                {
                    Console.WriteLine($"Alert: {ex.Message}");
                }
            }
        }
        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(); 

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer("Data Source=DESKTOP-DCVH6RQ\\MSSQLSERVER2;Initial Catalog=ConsoleProjectDb;TrustServerCertificate=True;Integrated Security=SSPI"));

            services.AddIdentity<User, IdentityRole<Guid>>( options =>
    {
                options.Password.RequireDigit = false; 
                options.Password.RequireLowercase = false; 
                options.Password.RequireUppercase = false; 
                options.Password.RequireNonAlphanumeric = false; 
                options.Password.RequiredLength = 4; 
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IUserService,UserService>();
        }

        private static Dictionary<string, string> ParseCommand(string input)
        {
            var args = input.Split(' ');
            var command = new Dictionary<string, string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--"))
                    command[args[i]] = args[++i];
                else
                    command["instruction"] = args[i];
            }
            return command;
        }
    }
}
