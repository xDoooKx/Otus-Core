using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Abstractions.Tools;
using PromoCodeFactory.Core.Domain.Administration;
using PromoCodeFactory.DataAccess.Data;
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
        /// Создаем нового сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<string>> CreateEmployee(EmployeeCreate newEmployee)
        {
            if (!newEmployee.Validate()) return BadRequest("Объект сотрудника представлен неверно.");

            Func<EmployeeCreate, Employee> mapping = newEmployee => new Employee
            {
                FirstName = newEmployee.FirstName,
                LastName = newEmployee.LastName,
                Email = newEmployee.Email,
                Id = Guid.NewGuid(),
                Roles = new List<Role>() { FakeDataFactory.Roles.Where(x => x.Name == "Admin").FirstOrDefault() }
            };
            var _newEmployee = mapping(newEmployee);

            await _employeeRepository.AddAsync(_newEmployee);

            return Ok("Новый сотрудник успешно создан.");
        }

        /// <summary>
        /// Удаляем какого-то сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<string>> DeleteEmployee(Guid id)
        {
            if (!await _employeeRepository.DeleteAsync(id)) return BadRequest("Удалить сотрудника не удалось.");

            return Ok("Сотрудник успешно удалён.");
        }

        /// <summary>
        /// Апдейтим какого-то сотрудника
        /// </summary>
        /// <returns></returns>
        [HttpPatch("{id:guid}")]
        public async Task<ActionResult<string>> UpdateEmployee(Guid id, EmployeeUpdate updateEmployee)
        {
            if (!updateEmployee.Validate()) return BadRequest("Объект сотрудника представлен неверно.");

            var employee = await _employeeRepository.GetByIdAsync(id);

            if (employee == null) return BadRequest("Сотрудник не найден.");

            Func<EmployeeUpdate, Employee> mapping = updEmployee => new Employee
            {
                FirstName = updEmployee.FirstName,
                LastName = updEmployee.LastName,
                Email = updEmployee.Email,
                Id = employee.Id,
                Roles = employee.Roles
            };
            var _updEmployee = mapping(updateEmployee);

            if (!await _employeeRepository.UpdateAsync(id, _updEmployee)) return BadRequest("Не удалось обновить сотрудника.");

            return Ok("Сотрудник успешно обновлён.");
        }
    }
}