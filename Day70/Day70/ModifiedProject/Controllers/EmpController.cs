using Microsoft.AspNetCore.Mvc;
using ModifiedProject.DTO;
using ModifiedProject.Services;

namespace ModifiedProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpController : ControllerBase
    {
        private readonly IEmployee _service;

        public EmpController(IEmployee service)
        {
            _service = service;
        }

        // ✅ GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int size = 5)
        {
            return Ok(await _service.GetAllEmployeesAsync(page, size));
        }

        // ✅ GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var emp = await _service.GetEmployeeByIdAsync(id);
            return emp == null ? NotFound("Employee not found") : Ok(emp);
        }

        // ✅ CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] EmployeeDto dto, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.AddEmployeeAsync(dto, image);
            return Ok(result);
        }

        // ✅ UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] EmployeeUpdateDto dto, IFormFile? image)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateEmployeeAsync(id, dto, image);
            return result == null ? NotFound("Employee not found") : Ok(result);
        }

        // ✅ DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteEmployeeAsync(id);
            return result == null ? NotFound("Employee not found") : Ok(result);
        }

        // ✅ SEARCH + BASIC DATA
        [HttpGet("basic")]
        public async Task<IActionResult> GetBasic(int page = 1, int size = 5, string? search = null)
        {
            return Ok(await _service.GetAllEmployeeBasicInfoAsync(page, size, search));
        }
    }
}