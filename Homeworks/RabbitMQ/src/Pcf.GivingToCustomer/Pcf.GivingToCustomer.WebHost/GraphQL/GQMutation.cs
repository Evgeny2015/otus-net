using HotChocolate;
using HotChocolate.Types;
using Pcf.GivingToCustomer.Core.Domain;
using Pcf.GivingToCustomer.DataAccess;
using System.Threading.Tasks;

namespace Pcf.GivingToCustomer.WebHost.GraphQL
{
    [ExtendObjectType(OperationTypeNames.Mutation)]
    public class GQMutation
    {
        public async Task<Customer> Customer([Service] DataContext dbContext, string email, string firstName, string lastName)
        {
            var customer = new Customer
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };
            dbContext.Customers.Add(customer);
            await dbContext.SaveChangesAsync();

            return customer;
        }
    }
}
