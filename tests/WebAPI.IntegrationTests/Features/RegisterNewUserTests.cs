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
using WebAPI.Controllers;
using Moq;

namespace WebAPI.IntegrationTests.Features;

[TestFixture]
public class RegisterNewUserTests
{
    private readonly UserController _userController;

    public RegisterNewUserTests()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.Development.json").Build();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        var dbOptions = new DbContextOptionsBuilder<UserDbContext>().UseSqlServer(connectionString).Options;
        var dbContext = new UserDbContext(dbOptions);
        var repository = new UserRepository(dbContext);
        var mapper = AutoMapperConfig.Initialize();

        var service = new UserService(repository, mapper);
        var logger = Mock.Of<ILogger<UserController>>();

        _userController = new UserController(logger, service);
    }

    [SetUp]
    public async Task Init()
    {
        var initialInstance = new UserRegisterDto
        {
            Email = "test@gmail.com",
            FirstName = "Lorem",
            LastName = "Ipsum",
            BirthDate = new DateTime(2002, 1, 20),
        };
        
        await _userController.RegisterNewUser(initialInstance).ConfigureAwait(false);
    }

    [Test]
    public async Task UniqueEmailShouldReturnHTTP200()
    {
        var randomNumber = new Random().Next(10, 1000000);
        var testInstance = new UserRegisterDto
        {
            Email = $"test{randomNumber}@gmail.com",
            FirstName = "Lorem",
            LastName = "Ipsum",
            BirthDate = new DateTime(2002, 1, 20),
        };

        var response = await _userController.RegisterNewUser(testInstance).ConfigureAwait(false);
        Assert.That(response, Is.TypeOf<OkObjectResult>());
    }

    [Test]
    public async Task ExistingEmailShouldReturnHTTP400()
    {
        var testInstance = new UserRegisterDto
        {
            Email = $"test@gmail.com",
            FirstName = "Lorem",
            LastName = "Ipsum",
            BirthDate = new DateTime(2002, 1, 20),
        };
        var response = await _userController.RegisterNewUser(testInstance).ConfigureAwait(false);
        Assert.That(response, Is.TypeOf<BadRequestResult>());
    }
    
    [Test]
    public async Task InvalidModelShouldReturnHTTP400()
    {
        var testInstance = new UserRegisterDto
        {
            Email = "test@@@gmail.com",
            FirstName = "Lorem",
            LastName = "Ipsum",
            BirthDate = new DateTime(2002, 1, 20),
        };
        var response = await _userController.RegisterNewUser(testInstance).ConfigureAwait(false);
        Assert.That(response, Is.TypeOf<BadRequestResult>());
    }
}