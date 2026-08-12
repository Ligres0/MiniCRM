using Microsoft.AspNetCore.Identity;
using MiniCRM.Models;
using MiniCRM.Repositories;
using MiniCRM.ViewModels;

namespace MiniCRM.Services
{
    public class UserService : IUserService

    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository, PasswordHasher<User> passwordHasher) 
        {
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<User>();
        }


        public bool CreateUser(UserCreateViewModel model, out string message)
        {
            if (_userRepository.UsernameExists(model.UserName))
            {
                message = "Username already exists. ";
                return false;
            }

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                IsActive = true,
                CreatedDate = DateTime.Now,
            };

            user.PasswordHash = _passwordHasher.HashPassword(
                user,
                model.Password);

            int userId = _userRepository.Insert(user);

            if (userId <= 0)
            {
                message = "User could not be created.";
                return false;
            }
            message = "User created successfully. ";

            return true;
        }

        }
}
