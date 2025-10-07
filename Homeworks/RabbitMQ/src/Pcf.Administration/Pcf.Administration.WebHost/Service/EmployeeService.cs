using Pcf.Administration.Core.Abstractions.Repositories;
using Pcf.Administration.Core.Domain.Administration;
using System;
using System.Threading.Tasks;

namespace Pcf.Administration.WebHost.Service
{
    public class EmployeeService(IRepository<Employee> employeeRepository) : IEmployeeService
    {
        public async Task<bool> UpdateAppliedPromocodesAsync(Guid id)
        {
            var employee = await employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return false;

            employee.AppliedPromocodesCount++;

            await employeeRepository.UpdateAsync(employee);

            return true;
        }
    }
}
