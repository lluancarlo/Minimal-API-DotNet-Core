using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Minimal.Application.Common.Interfaces;
using Minimal.Application.Common.Results;

namespace Minimal.Infrastructure.Services;

public sealed class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ServiceResult<string> GenerateToken(Guid userId, string email, ICollection<string> roles)
    {
        var jwtSecret = _configuration["Jwt:Secret"];
        if (string.IsNullOrEmpty(jwtSecret))
            throw new MissingAppSettingsConfigurationException("Jwt:Secret");

        var jwtIssuer = _configuration["Jwt:Issuer"];
        if (string.IsNullOrEmpty(jwtIssuer))
            throw new MissingAppSettingsConfigurationException("Jwt:Issuer is null");

        var jwtAudience = _configuration["Jwt:Audience"];
        if (string.IsNullOrEmpty(jwtAudience))
            throw new MissingAppSettingsConfigurationException("Jwt:Audience is null");

        var jwtExpires = _configuration["Jwt:ExpireInHours"];
        if (string.IsNullOrEmpty(jwtExpires))
            throw new MissingAppSettingsConfigurationException("Jwt:ExpireInHours is null");

        if (!int.TryParse(jwtExpires, out var expireInHours))
            throw new MissingAppSettingsConfigurationException("Jwt:ExpireInHours should be a int value");

        var claims = new List<Claim> {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email)
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role.ToString()));

        var keyInBytes = Encoding.UTF8.GetBytes(jwtSecret);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(expireInHours),
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(keyInBytes), SecurityAlgorithms.HmacSha256)
        );

        return ServiceResult<string>.Ok(new JwtSecurityTokenHandler().WriteToken(token));
    }
}

class MissingAppSettingsConfigurationException(string? msg = null) : Exception(msg);