using FlexiCore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FlexiCore.Services
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetEmployeesByBranchAsync(int branchId);
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task CreateEmployeeAsync(Employee employee, int branchId);
        Task UpdateEmployeeAsync(Employee employee);
        Task DeleteEmployeeAsync(int id);
    }
}