namespace StudentInquiryAssistanceAPI.DTOs.Courses;

public class CourseDto
{
    public int CourseId { get; set; }

    public string CourseName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string InstructorName { get; set; } = string.Empty;

    public string Duration { get; set; } = string.Empty;

    public decimal FeesAmount { get; set; }
}
