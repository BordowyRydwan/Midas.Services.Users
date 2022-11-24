using Application.Dto;
using Application.Mappings;
using Application.Services;
using Domain.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WebAPI.Controllers;

namespace WebAPI.IntegrationTests.Features;

[TestFixture]
public class UpdateUserEmailTests
{
    private readonly UserController _userController;

    private ulong _id;
    private string _email;

    public UpdateUserEmailTests()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var dbOptions = new DbContextOptionsBuilder<UserDbContext>().UseSqlServer(connectionString).Options;
        var dbContext = new UserDbContext(dbOptions);
        var repository = new UserRepository(dbContext);
        var mapper = AutoMapperConfig.Initialize();

        var userService = new UserService(repository, mapper, null);
        var userLogger = Mock.Of<ILogger<UserController>>();
        
        _userController = new UserController(userLogger, userService);
    }

    [SetUp]
    public async Task Init()
    {
        var randomNumber = new Random().Next(1000, 1000000);
        var initialInstance = new UserRegisterDto
        {
            Email = $"test{randomNumber}@gmail.com",
            FirstName = "Lorem",
            LastName = "Ipsum",
            BirthDate = new DateTime(2002, 1, 20),
        };

        var result = await _userController.RegisterNewUser(initialInstance).ConfigureAwait(false) as OkObjectResult;
        _id = (result.Value as UserRegisterReturnDto).Id;
        _email = initialInstance.Email;
    }
    
    [Test]
    public async Task ShouldUpdateData_ForExistingUser()
    {
        var randomNumber = new Random().Next(1000, 1000000);
        var dto = new UserUpdateEmailDto
        {
            OldEmail = _email,
            NewEmail = $"testxyz{randomNumber}@email.com"
        };

        var result = await _userController.UpdateUserEmail(dto).ConfigureAwait(false);
        
        Assert.That(result, Is.InstanceOf<OkResult>());
    }


    [Test]
    public async Task ShouldNotUpdateData_ForNotExistingUser()
    {
        var dto = new UserUpdateEmailDto
        {
            OldEmail = _email + "salt",
            NewEmail = "someemail"
        };
        var result = await _userController.UpdateUserEmail(dto).ConfigureAwait(false);
        
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }
}