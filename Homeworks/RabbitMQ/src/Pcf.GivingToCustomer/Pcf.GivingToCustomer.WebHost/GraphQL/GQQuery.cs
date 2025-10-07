using System.Collections.Generic;
using HotChocolate;
using HotChocolate.Data;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using Microsoft.EntityFrameworkCore;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.DataAccess;

namespace Pcf.GivingToCustomer.WebHost.GraphQL
{
    [ExtendObjectType(OperationTypeNames.Query)]
    public class GQQuery
    {        
        [UsePaging]
        [UseSorting]
        public List<Customer> GetCustomers([Service] DataContext dbContext) => [.. dbContext.Customers.Include(x => x.Preferences)];

        public List<Preference> GetPreferences([Service] DataContext dbContext) => [.. dbContext.Preferences];

        public List<PromoCode> GetPromoCodes([Service] DataContext dbContext) => [.. dbContext.PromoCodes];
    }
}
