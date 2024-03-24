using SharedClassLibrary.DTOs;
using static SharedClassLibrary.DTOs.ServiceResponses;

namespace SharedClassLibrary.Contract;

public interface IUserAccount
{
  Task<GeneralResponse> CreateAccount(string tenant, UserDTO userDTO);
  Task<LoginResponse> LoginAccount(LoginDTO loginDTO);
}
