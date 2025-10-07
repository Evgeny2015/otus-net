using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.DataAccess.EntityFramework;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController
        : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _mapper;
        public CustomersController(IMapper mapper, DataContext dataContext)
        {
            this._mapper = mapper;
            this._dataContext = dataContext;
        }
        /// <summary>
        /// Получение списка всех клиентов
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<CustomerShortResponse>> GetCustomersAsync()
        {            
            var customerSet = await _dataContext.Set<Customer>().ToListAsync();
            var result = _mapper.Map<List<CustomerShortResponse>>(customerSet);

            return Ok(result);
        }

        /// <summary>
        /// Получение клиента вместе с выданными ему промомкодами по id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {            
            var customerSet = _dataContext.Set<Customer>();
            var customer = await customerSet.FindAsync(id);

            var result = _mapper.Map<Customer, CustomerResponse>(customer);

            return Ok(result);
        }

        /// <summary>
        /// Добавление нового клиента вместе с его предпочтениями
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {            
            var customers = _dataContext.Set<Customer>();
            var preferences = _dataContext.Set<Preference>();

            var customer = new Customer
            {
                Id = new Guid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,                
            };

            var customerPreferences = request.PreferenceIds.Select(async x => 
                new CustomerPreference 
                { 
                    Customer = customer,
                    Preference = await preferences.FindAsync(x)
                });

            // добавление нового клиента
            await customers.AddAsync(customer);

            // добавление предпочтений клиента
            await _dataContext.Set<CustomerPreference>().AddRangeAsync(await Task.WhenAll(customerPreferences));

            await _dataContext.SaveChangesAsync();

            return Ok(customer.Id);
        }

        /// <summary>
        /// Обновление данных клиента вместе с его предпочтениями
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            var customers = _dataContext.Set<Customer>();
            var preferences = _dataContext.Set<Preference>();
            var customerPreference = _dataContext.Set<CustomerPreference>();

            var customer = new Customer
            {
                Id = id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
            };

            var customerPreferences = request.PreferenceIds.Select(async x =>
                new CustomerPreference
                {
                    Customer = customer,
                    Preference = await preferences.FindAsync(x)
                });

            // обновление клиента
            await customers.AddAsync(customer);

            // удаление прежних предпочтений клиента            
            customerPreference.RemoveRange(customerPreference.Where(x => x.CustomerId == id));

            // добавление предпочтений клиента
            await customerPreference.AddRangeAsync(await Task.WhenAll(customerPreferences));
            
            await _dataContext.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// Удаление клиента вместе с выданными ему промокодами
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {            
            var customerSet = _dataContext.Set<Customer>();
            var customer = await customerSet.FindAsync(id);

            if (customer != null)
            {
                // удаление промокодов происходит каскадно после удаления клиента
                
                // удаление клиента 
                customerSet.Remove(customer);

                await _dataContext.SaveChangesAsync();
            }

            return Ok();
        }
    }
}