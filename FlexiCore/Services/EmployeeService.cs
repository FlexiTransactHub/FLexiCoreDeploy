using FlexiCore.Models;
using FlexiCore.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<List<Employee>> GetEmployeesByBranchAsync(int branchId)
        {
            return await _employeeRepository.GetEmployeesByBranchAsync(branchId);
        }

        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            return await _employeeRepository.GetEmployeeByIdAsync(id);
        }

        public async Task CreateEmployeeAsync(Employee employee, int branchId)
        {
            if (string.IsNullOrEmpty(employee.Name))
            {
                throw new ArgumentException("Employee name is required.");
            }

            employee.BranchId = branchId;
            await _employeeRepository.AddEmployeeAsync(employee);
            await _employeeRepository.SaveChangesAsync();
        }

        public async Task UpdateEmployeeAsync(Employee employee)
        {
            if (string.IsNullOrEmpty(employee.Name))
            {
                throw new ArgumentException("Employee name is required.");
            }

            await _employeeRepository.UpdateEmployeeAsync(employee);
            await _employeeRepository.SaveChangesAsync();
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            var employee = await _employeeRepository.GetEmployeeByIdAsync(id);
            if (employee == null)
            {
                throw new ArgumentException("Employee not found.");
            }

            await _employeeRepository.DeleteEmployeeAsync(employee);
            await _employeeRepository.SaveChangesAsync();
        }
    }
}