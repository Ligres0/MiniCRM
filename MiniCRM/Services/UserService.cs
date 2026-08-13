using Microsoft.AspNetCore.Identity;
using MiniCRM.Models;
using MiniCRM.Repositories;
using MiniCRM.ViewModels;

namespace MiniCRM.Services
{
    public class UserService : IUserService

    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IAuthorizationService _authorizationService;

        public UserService(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IAuthorizationService authorizationService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _authorizationService = authorizationService;
        }


        public bool CreateUser(UserCreateViewModel model, out string message)
        {
            if (_userRepository.UsernameExists(model.UserName))
            {
                message = "Username already exists. ";
                return false;
            }

            if (_userRepository.EmailExists(model.Email))
            {
                message = "Email already exists.";
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

        public List<UserListViewModel> GetAllWithRoles()
        {
            return _userRepository.GetAllWithRoles();
        }
        public UserRoleEditViewModel? GetUserRoleEditModel(int userId)

        {
            var users =
                _userRepository.GetAllWithRoles();

            var user =
                users.FirstOrDefault(
                    x => x.Id == userId);

            if (user == null)
            {
                return null;
            }

            var allRoles =
                _userRepository.GetAllRoles();

            var selectedRoleIds =
                _userRepository.GetUserRoleIds(
                    userId);

            return new UserRoleEditViewModel
            {
                UserId = user.Id,

                UserName = user.UserName,

                AllRoles = allRoles,

                SelectedRoleIds =
                    selectedRoleIds
            };
        }
        public bool UpdateUserRoles(
            int userId,
            List<int> roleIds,
            out string message)
        {
            if (userId <= 0)
            {
                message = "Invalid user.";
                return false;
            }

            roleIds ??= new List<int>();

            try
            {
                _userRepository.UpdateUserRoles(
                    userId,
                    roleIds);

                _authorizationService.ClearPermissionCache(
                    userId);

                message = "User roles updated successfully.";
                return true;
            }
            catch
            {
                message = "User roles could not be updated.";
                return false;
            }
        }
        public bool HasAnyRole(int userId)
        {
            return _userRepository.HasAnyRole(userId);
        }


    }
}
