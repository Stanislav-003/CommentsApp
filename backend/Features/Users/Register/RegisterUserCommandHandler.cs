using backend.Abstractions.Messaging;
using backend.Contracts.Responses;
using backend.Database;
using backend.Models;
using backend.Options;
using backend.Shared.Errors;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static backend.Utils.EncodeUtils;

namespace backend.Features.Users.Register;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, RegisterResponse>
{
    private readonly ApplicationDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public RegisterUserCommandHandler(
        ApplicationDbContext context,
        IOptions<JwtSettings> jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<RegisterResponse> Handle(RegisterUserCommand command, CancellationToken ct)
    {
        if (command.Password != command.ConfirmPassword)
            throw new AppException("PasswordsNotMatches");

        var securityKey = _jwtSettings.SecurityKey;

        if (string.IsNullOrEmpty(securityKey))
            throw new AppException("SecurityKeyIsNotConfigured");

        var user = new User
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
        };

        var hashedPass = EncodeUtilsRfc2898.Hash(command.Password + securityKey);

        user.Credential = new Credential
        {
            Email = command.Email,
            Password = hashedPass,
            Role = command.Role
        };

        await _context.Users.AddAsync(user, ct);

        var token = GenerateJwtToken(user);

        await _context.SaveChangesAsync(ct);

        return new RegisterResponse(token, DateTimeOffset.UtcNow.AddDays(1).ToUnixTimeSeconds());
    }

    public string GenerateJwtToken(User user)
    {
        var securityKey = _jwtSettings.SecurityKey;

        if (securityKey == null)
            throw new AppException("SecurityKeyIsNotConfigured");

        var claims = new List<Claim>
        {
            new Claim("UserId", user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.FirstName),
            new Claim(ClaimTypes.Role, user.Credential.Role.ToString())
        };

        var jwt = new JwtSecurityToken(
            issuer: _jwtSettings.JwtIssuer,
            audience: _jwtSettings.JwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(securityKey)),
                SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
