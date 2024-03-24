namespace SharedClassLibrary.DTOs;

public class ServiceResponses
{
  public record class GeneralResponse(bool Flag, string tenantName, string Message);
  public record class LoginResponse(bool Flag, string tenantName, string Token, string Message);
}
