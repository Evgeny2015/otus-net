using System.Threading.Tasks;
using System;

namespace Pcf.Administration.WebHost
{
    public interface IEmployeeService
    {
        Task<bool> UpdateAppliedPromocodesAsync(Guid id);
    }
}
