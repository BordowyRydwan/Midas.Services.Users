namespace Domain.Entities;

public class User
{
    public ulong Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime RegisterDate { get; set; }
    public DateTime BirthDate { get; set; }
    public string Description { get; set; }
    public Guid ProfileImage { get; set; }
}