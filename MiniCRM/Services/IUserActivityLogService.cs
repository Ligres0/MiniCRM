using MiniCRM.ViewModels;

namespace MiniCRM.Services
{
    public interface IUserActivityLogService
    {
        void Log(
            int? userId,
            string action,
            string? description);
        List<UserActivityLogListViewModel> GetAll();
    }
}