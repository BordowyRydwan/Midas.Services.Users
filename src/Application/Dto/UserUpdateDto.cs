namespace Application.Dto;

public class UserUpdateDto
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
    public string Description { get; set; }
    public Guid ProfileImage { get; set; }
}