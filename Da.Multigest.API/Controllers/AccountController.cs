using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.Contract;
using SharedClassLibrary.DTOs;
using static SharedClassLibrary.DTOs.ServiceResponses;

namespace Da.Multigest.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(IUserAccount userAccount) : ControllerBase
{
	[HttpPost("register")]
	public async Task<ActionResult<GeneralResponse>> Register([FromHeader] string tenant, UserDTO userDTO)
	{
		var response = await userAccount.CreateAccount(tenant, userDTO);
		return Ok(response);
	}

	[HttpPost("login")]
	public async Task<ActionResult<LoginResponse>> Login(LoginDTO loginDTO)
	{
		var response = await userAccount.LoginAccount(loginDTO);
		return Ok(response);
	}
}
