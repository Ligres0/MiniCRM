using MiniCRM.Models;
using MiniCRM.ViewModels;

namespace MiniCRM.Repositories
{
    public interface IUserActivityLogRepository
    {
        void Insert(UserActivityLog log);
        List<UserActivityLogListViewModel> GetAll();
    }
}