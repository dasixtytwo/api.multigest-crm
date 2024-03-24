using Da.Multigest.API.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SharedClassLibrary.Contract;
using SharedClassLibrary.Contract.Tenants;
using SharedClassLibrary.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static SharedClassLibrary.DTOs.ServiceResponses;

namespace Da.Multigest.API.Repositories;

public class AccountRepository(AppDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config, ITenantFacade _tenantRepository) : IUserAccount
{
	public async Task<GeneralResponse> CreateAccount(string tenant, UserDTO userDTO)
	{
		if (userDTO is null) return new GeneralResponse(false, null!, "Model is empty");

		var tenantResponse = await _tenantRepository.GetTenantByNameAsync(tenant);
		var newUser = new ApplicationUser()
		{
			Name = userDTO.Name,
			Email = userDTO.Email,
			PasswordHash = userDTO.Password,
			UserName = userDTO.Email,
			TenantId = tenantResponse.Id
		};

		// check if the tenantId already has an admin;
		var hasTenantAdmin = checkTenanthasAdminRoleAsync(tenantResponse.Id);

		var user = await userManager.FindByEmailAsync(newUser.Email);
		if (user is not null) return new GeneralResponse(false, null!, "User registered already");

		var createUser = await userManager.CreateAsync(newUser!, userDTO.Password);
		if (!createUser.Succeeded) return new GeneralResponse(false, null!, "Error occured.. please try again");

		if (!hasTenantAdmin)
		{
			await roleManager.CreateAsync(new IdentityRole() { Name = "Admin" });
			await userManager.AddToRoleAsync(newUser, "Admin");
			return new GeneralResponse(true, tenant, "Account Created");
		}
		else
		{
			var checkUser = await roleManager.FindByNameAsync("User");
			if (checkUser is null)
				await roleManager.CreateAsync(new IdentityRole() { Name = "User" });

			await userManager.AddToRoleAsync(newUser, "User");
			return new GeneralResponse(true, tenant, "Account Created");
		}
	}

	public async Task<LoginResponse> LoginAccount(LoginDTO loginDTO)
	{
		if (loginDTO == null)
			return new LoginResponse(false, null!, null!, "Login container is empty");

		var getUser = await userManager.FindByEmailAsync(loginDTO.Email);
		if (getUser is null)
			return new LoginResponse(false, null!, null!, "User not found");

		bool checkUserPasswords = await userManager.CheckPasswordAsync(getUser, loginDTO.Password);
		if (!checkUserPasswords)
			return new LoginResponse(false, null!, null!, "Invalid email/password");

		var getUserRole = await userManager.GetRolesAsync(getUser);
		var userSession = new UserSession(getUser.Id, getUser.Name, getUser.Email, getUserRole.First(), getUser.TenantId);

		var tenantResponse = await _tenantRepository.GetTenantByIdAsync(getUser.TenantId!);

		string token = GenerateToken(userSession);
		return new LoginResponse(true, tenantResponse.Name, token!, "Login completed");
	}

	private bool checkTenanthasAdminRoleAsync(string tenantId)
	{
		var userCont = dbContext.Users.Where(t => t.TenantId == tenantId).ToList();

		if (userCont.Count == 0) return false;

		return true;
	}

	private string GenerateToken(UserSession user)
	{
		var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:SecretKey"]!));
		var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
		var userClaims = new[]
		{
		new Claim(ClaimTypes.NameIdentifier, user.Id),
		new Claim(ClaimTypes.Name, user.Name),
		new Claim(ClaimTypes.Email, user.Email),
		new Claim(ClaimTypes.Role, user.Role),
		new Claim(ClaimConstants.TenantId, user.TenantId ?? string.Empty)
	};
		var token = new JwtSecurityToken(
			issuer: config["Jwt:Issuer"],
			audience: config["Jwt:Audience"],
			claims: userClaims,
			expires: DateTime.Now.AddDays(1),
			signingCredentials: credentials
			);
		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}
