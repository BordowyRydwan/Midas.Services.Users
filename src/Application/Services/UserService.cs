using Application.Dto;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities;
using Domain.Exceptions;
using Infrastructure.Interfaces;
using Midas.Services;
using UserRegisterDto = Application.Dto.UserRegisterDto;
using UserRegisterReturnDto = Application.Dto.UserRegisterReturnDto;
using UserUpdateEmailDto = Application.Dto.UserUpdateEmailDto;


namespace Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IAuthorizationClient _authorizationClient;

    public UserService(IUserRepository userRepository, IMapper mapper, IAuthorizationClient authorizationClient)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _authorizationClient = authorizationClient;
    }
    
    public async Task<UserRegisterReturnDto> RegisterNewUser(UserRegisterDto user)
    {
        var userEntity = _mapper.Map<UserRegisterDto, User>(user);
        var returnModel = new UserRegisterReturnDto { Email = user.Email };
        
        try
        {
            returnModel.Id = await _userRepository.AddNewUser(userEntity).ConfigureAwait(false);
        }
        catch (UserException)
        {
            return null;
        }

        return returnModel;
    }

    public async Task<bool> UpdateUserData(UserUpdateDto user)
    {
        var userEntity = _mapper.Map<UserUpdateDto, User>(user);
        var updateResult = await _userRepository.UpdateUserData(userEntity);

        return updateResult;
    }

    public async Task UpdateUserEmail(UserUpdateEmailDto user)
    {
        try
        {
            var mappedModel = _mapper.Map<UserUpdateEmailDto, Midas.Services.UserUpdateEmailDto>(user);

            await _userRepository.UpdateUserEmail(user.OldEmail, user.NewEmail);
            await _authorizationClient.UpdateUserEmailAsync(mappedModel);
        }
        catch (UserException e)
        {
            Console.Write(e);
        }
        catch (Exception e)
        {
            await _userRepository.UpdateUserEmail(user.NewEmail, user.OldEmail);
            Console.Write(e);
        }
    }

    public async Task<UserDto> GetUserByEmail(string email)
    {
        var entity = await _userRepository.GetUserByEmail(email).ConfigureAwait(false);
        var dto = _mapper.Map<User, UserDto>(entity);

        return dto;
    }
    
    public async Task<UserDto> GetUserById(ulong id)
    {
        var entity = await _userRepository.GetUserById(id).ConfigureAwait(false);
        var dto = _mapper.Map<User, UserDto>(entity);

        return dto;
    }
}