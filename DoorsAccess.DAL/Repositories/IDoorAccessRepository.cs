using System;
using System.Threading.Tasks;

namespace DoorsAccess.DAL.Repositories
{
    public interface IDoorAccessRepository
    {
        Task<bool> CanAccess(long userId, long doorId, DateTime date);
    }
}