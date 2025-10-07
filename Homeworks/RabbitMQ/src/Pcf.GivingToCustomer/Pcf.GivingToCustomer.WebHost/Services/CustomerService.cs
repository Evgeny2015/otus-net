using Grpc.Core;
using System.Threading.Tasks;
using Pcf.GivingToCustomer.Core.Abstractions.Repositories;
using Customer = Pcf.GivingToCustomer.Core.Domain.Customer;
using GrpcCustomer = grpc.Customer;
using System.Linq;


namespace Pcf.GivingToCustomer.WebHost.Services
{
    public class CustomerService(IRepository<Customer> customerRepository) : GrpcCustomer.CustomerBase
    {     
        public override async Task<grpc.CustomerResponseList> GetAll(grpc.Empty request, ServerCallContext context)
        {
            var customers = await customerRepository.GetAllAsync();
            var res = customers.Select(x => new grpc.CustomerShortResponse
            {
                Id = x.Id.ToString(),
                Email = x.Email,
                FirstName = x.FirstName,
                LastName = x.LastName
            }).ToList();

            var response = new grpc.CustomerResponseList();
            response.List.AddRange(res);

            return response;
            
        }

        public override async Task<grpc.CustomerResponse> Get(grpc.CustomerRequest request, ServerCallContext context)
        {
            var customer = await customerRepository.GetByIdAsync(new System.Guid(request.Id));

            var response = new grpc.CustomerResponse { 
                Id = customer.Id.ToString(),
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName
            };
                        
            response.Preferences.AddRange(
                customer.Preferences.Select(x => new grpc.PreferenceResponse {
                    Id = x.Preference.Id.ToString(),
                    Name = x.Preference.Name
                }));
            

            return response;
        }

        public override async Task<grpc.CustomerShortResponse> Create(grpc.CreateCustomerRequest request, ServerCallContext context)
        {
            var customer = new Customer
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
            };

            await customerRepository.AddAsync(customer);

            var response = new grpc.CustomerShortResponse
            {
                Id = customer.Id.ToString(),
                Email = customer.Email,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            return response;
        }

        public override async Task<grpc.Empty> Delete(grpc.DeleteCustomerRequest request, ServerCallContext context)
        {
            var customer = await customerRepository.GetByIdAsync(new System.Guid(request.Id));

            if (customer != null)
            {
                await customerRepository.DeleteAsync(customer);
            }                          

            return new grpc.Empty();
        }

        public override async Task<grpc.Empty> Update(grpc.EditCustomerRequest request, ServerCallContext context)
        {
            var id = new System.Guid(request.Id);
            var customer = await customerRepository.GetByIdAsync(id);

            if (customer != null)
            {
                customer.FirstName = request.FirstName;
                customer.LastName = request.LastName;
                customer.Email = request.Email;

                await customerRepository.UpdateAsync(customer);
            }

            return new grpc.Empty();
        }
    }
}
