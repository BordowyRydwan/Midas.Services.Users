using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using MockQueryable.Moq;
using Moq;

namespace Infrastructure.UnitTests.Repositories;

[TestFixture]
public class UpdateUserEmailTests
{
    private readonly IUserRepository _repository;

    private IList<User> _data = new List<User>
    {
        new()
        {
            Id = 1,
            Email = "test@test.pl",
            FirstName = "Test 1",
            LastName = "Test 1",
            BirthDate = DateTime.UtcNow,
            RegisterDate = DateTime.UtcNow,
        }
    };

    public UpdateUserEmailTests()
    {
        var mockContext = new Mock<UserDbContext>();
        var mockData = _data.AsQueryable().BuildMockDbSet();

        mockContext.Setup(x => x.Users).Returns(mockData.Object);

        _repository = new UserRepository(mockContext.Object);
    }

    [TearDown]
    public void ClearUserPassword()
    {
        _data.First().Email = "test@test.pl";
    }

    [Test]
    public async Task ShouldReturnTrue_OnExistingEmail()
    {
        var email = _data.First().Email;
        var newEmail = "test2@test.pl";

        await _repository.UpdateUserEmail(email, newEmail);
        Assert.That(_data.First().Email, Is.EqualTo(newEmail));
    }

    [Test]
    public async Task ShouldReturnFalse_OnNotExistingEmail()
    {
        var email = "testkljkljkj@test.pl";
        var newEmail = "test2@test.pl";
        var result = _repository.UpdateUserEmail;

        Assert.That(async () => await result(email, newEmail), Throws.Exception.TypeOf<UserException>());
    }
}
