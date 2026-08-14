using MiniCRM.Models;
using MiniCRM.Repositories;
using MiniCRM.ViewModels;

namespace MiniCRM.Services
{
    public class UserActivityLogService : IUserActivityLogService
    {
        private readonly IUserActivityLogRepository _userActivityLogRepository;

        public UserActivityLogService(IUserActivityLogRepository userActivityLogRepository)
        {
            _userActivityLogRepository = userActivityLogRepository;
        }

        public void Log(int? userId , string action, string? description)
        {
            var log = new UserActivityLog
            {
                UserId = userId,
                Action = action,
                Description = description,
                CreatedDate = DateTime.Now,

            };
            _userActivityLogRepository.Insert(log);
        }
        public List<UserActivityLogListViewModel> GetAll()
        {
            return _userActivityLogRepository
                .GetAll();
        }
    }
}
