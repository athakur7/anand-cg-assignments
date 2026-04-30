using System.ComponentModel.DataAnnotations;

namespace ModifiedProject.DTO
{
    public class EmployeeDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter your firstname")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your lastname")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Please enter email id")]
        [EmailAddress(ErrorMessage = "Please enter valid email id")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Please enter your age")]
        [Range(1, 100, ErrorMessage = "Please enter your age between 1 to 100 only")]
        public int Age { get; set; }

        public string? ImagePath { get; set; }
    }
}