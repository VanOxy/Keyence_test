namespace SalesPlatform.Application.Auth.Commands.Login;

public record LoginResult(string Token, string FullName, string Email, string Role);
