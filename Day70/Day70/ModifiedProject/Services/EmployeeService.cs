using Microsoft.EntityFrameworkCore;
using ModifiedProject.DTO;
using ModifiedProject.Models;

namespace ModifiedProject.Services
{
    public class EmployeeService : IEmployee
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmployeeService(ApplicationDbContext context,
            IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext!.Request;
            return $"{request.Scheme}://{request.Host}";
        }

        private EmployeeDto MapEmployeeToDto(Employee e)
        {
            string baseUrl = GetBaseUrl();

            return new EmployeeDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                Age = e.Age,
                ImagePath = string.IsNullOrEmpty(e.ImagePath)
                    ? $"{baseUrl}/uploads/default.jpg"
                    : $"{baseUrl}{e.ImagePath}"
            };
        }

        private async Task<string> SaveImageAsync(IFormFile file)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(path, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return "/uploads/" + fileName;
        }

        public async Task<List<EmployeeDto>> GetAllEmployeesAsync(int page, int size)
        {
            var list = await _context.Employees
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return list.Select(MapEmployeeToDto).ToList();
        }

        public async Task<EmployeeDto?> GetEmployeeByIdAsync(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            return emp == null ? null : MapEmployeeToDto(emp);
        }

        public async Task<EmployeeDto> AddEmployeeAsync(EmployeeDto dto, IFormFile? image)
        {
            var emp = new Employee
            {
                FirstName = dto.FirstName!,
                LastName = dto.LastName!,
                Email = dto.Email!,
                Age = dto.Age,
                ImagePath = "/uploads/default.jpg"
            };

            if (image != null)
                emp.ImagePath = await SaveImageAsync(image);

            await _context.Employees.AddAsync(emp);
            await _context.SaveChangesAsync();

            return MapEmployeeToDto(emp);
        }

        public async Task<EmployeeDto?> UpdateEmployeeAsync(int id, EmployeeUpdateDto dto, IFormFile? image)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return null;

            emp.FirstName = dto.FirstName!;
            emp.LastName = dto.LastName!;
            emp.Email = dto.Email!;
            emp.Age = dto.Age;

            if (image != null)
                emp.ImagePath = await SaveImageAsync(image);

            await _context.SaveChangesAsync();

            return MapEmployeeToDto(emp);
        }

        public async Task<EmployeeDto?> DeleteEmployeeAsync(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return null;

            _context.Employees.Remove(emp);
            await _context.SaveChangesAsync();

            return MapEmployeeToDto(emp);
        }

        public async Task<List<EmployeeBasicDto>> GetAllEmployeeBasicInfoAsync(int page, int size, string? search)
        {
            var query = _context.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e =>
                    e.FirstName.Contains(search) ||
                    e.LastName.Contains(search) ||
                    e.Email.Contains(search));
            }

            var list = await query.Skip((page - 1) * size).Take(size).ToListAsync();

            string baseUrl = GetBaseUrl();

            return list.Select(e => new EmployeeBasicDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                Email = e.Email,
                ImageUrl = $"{baseUrl}{e.ImagePath}"
            }).ToList();
        }
    }
}
