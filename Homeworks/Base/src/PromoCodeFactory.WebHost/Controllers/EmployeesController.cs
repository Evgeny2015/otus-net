using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.WebHost.Models;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Сотрудники
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IRepository<Employee> _employeeRepository;        

        public EmployeesController(IRepository<Employee> employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Получить данные всех сотрудников
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<List<EmployeeShortResponse>> GetEmployeesAsync()
        {
            var employees = await _employeeRepository.GetAllAsync();

            var employeesModelList = employees.Select(x =>
                new EmployeeShortResponse()
                {
                    Id = x.Id,
                    Email = x.Email,
                    FullName = x.FullName,
                }).ToList();

            return employeesModelList;
        }

        /// <summary>
        /// Получить данные сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<EmployeeResponse>> GetEmployeeByIdAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null)
                return NotFound();

            var employeeModel = new EmployeeResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                Roles = employee.Roles.Select(x => new RoleItemResponse()
                {
                    Name = x.Name,
                    Description = x.Description
                }).ToList(),
                FullName = employee.FullName,
                AppliedPromocodesCount = employee.AppliedPromocodesCount
            };

            return employeeModel;
        }

        /// <summary>
        /// Добавить нового сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPost("EmployeeCreateRequest")]
        public async Task<ActionResult<EmployeeShortResponse>> AddEmployeeAsync([FromBody] EmployeeCreateRequest emp)
        {
            var employee = new Employee()
            {
                Id = Guid.NewGuid(),
                Email = emp.Email,
                FirstName = emp.FirstName,
                LastName = emp.LastName,
            };

            employee = await _employeeRepository.AddAsync(employee);
            
            var employeeShort = new EmployeeShortResponse()
            {
                Id = employee.Id,
                Email = employee.Email,
                FullName = employee.FullName,
            };

            return Ok(employeeShort);
        }

        /// <summary>
        /// Удалить сотрудника по Id
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DelEmployeeByIdAsync(Guid id)
        {            
            await _employeeRepository.DeleteByIdAsync(id);
            return Ok();
        }

        /// <summary>
        /// Редактирование сотрудника
        /// </summary>
        /// <param name="emp"></param>
        /// <returns></returns>
        [HttpPut("EmployeeUpdateRequest")]
        public async Task<IActionResult> UpdateEmployeeAsync(EmployeeUpdateRequest emp)
        {
            var employee = new Employee() { 
                Id = emp.Id,
                Email = emp.Email,
                FirstName = emp.FirstName,  
                LastName = emp.LastName,
            };

            await _employeeRepository.UpdateAsync(employee);
            return Ok();
        }
    }
}