using ModifiedProject.DTO;

namespace ModifiedProject.Services
{
    public interface IEmployee
    {
        Task<List<EmployeeDto>> GetAllEmployeesAsync(int pageNumber, int pageSize);
        Task<EmployeeDto?> GetEmployeeByIdAsync(int id);
        Task<EmployeeDto> AddEmployeeAsync(EmployeeDto employeeDto, IFormFile? image);
        Task<EmployeeDto?> UpdateEmployeeAsync(int id, EmployeeUpdateDto employeeDto, IFormFile? image);
        Task<EmployeeDto?> DeleteEmployeeAsync(int id);
        Task<List<EmployeeBasicDto>> GetAllEmployeeBasicInfoAsync(int pageNumber, int pageSize, string? searchTerm);
    }
}