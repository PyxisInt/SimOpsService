using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PrabalGhosh.Utilities;
using SimOps.Models.Authentication;
using SimOpsService.Interfaces;
using SimOpsService.Models;
using SimOpsService.Repository;


namespace SimOpsService.Services;

public class AuthManager : IAuthManager
{
    private AuthenticationOptions _authOptions;
    private readonly byte[] _secret;
    private SimOpsContext _db;
    

    public AuthManager(AuthenticationOptions authOptions, SimOpsContext db)
    {
        _authOptions = authOptions;
        _secret = Encoding.ASCII.GetBytes(_authOptions.InternalAuth.JwtConfig.Secret);
        _db = db;
    }
    
    public async Task<LoginResult> GenerateToken(string userName, Claim[] claims)
    {
        var shouldAddAudienceClaim =
            string.IsNullOrWhiteSpace(claims?.FirstOrDefault(
                x => x.Type == JwtRegisteredClaimNames.Aud)?.Value);
        var expiresAt = DateTime.UtcNow.AddMinutes(_authOptions.InternalAuth.JwtConfig.AccessTokenExpiration);
        var jwtToken = new JwtSecurityToken(
            _authOptions.InternalAuth.JwtConfig.Issuer,
            shouldAddAudienceClaim ? _authOptions.InternalAuth.JwtConfig.Audience : string.Empty,
            claims,
            expires: expiresAt,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(_secret), SecurityAlgorithms.HmacSha256Signature)
        );
        var accessToken = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        var refreshToken = new RefreshToken
        {
            Username = userName,
            Token = GenerateRefreshToken(),
            ExpireAt = DateTime.UtcNow.AddMinutes(_authOptions.InternalAuth.JwtConfig.RefreshTokenExpiration)
        };
        await AddOrUpdateRefreshToken(refreshToken);
        return new LoginResult()
        {
            Username    = userName,
            ExpiresAt = expiresAt,
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiry = refreshToken.ExpireAt
        };
    }

    private async Task AddOrUpdateRefreshToken(RefreshToken refreshToken)
    {
        var found = await _db.RefreshTokens
            .FirstOrDefaultAsync(r => r.Username.ToLower().Equals(refreshToken.Username.ToLower()));
        if (found.IsNotNull())
        {
            //exists...update it
            found.ExpireAt = refreshToken.ExpireAt;
            found.Token = refreshToken.Token;
        }
        else
        {
            _db.RefreshTokens.Add(refreshToken);
        }

        await _db.SaveChangesAsync();
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var randomNumberGenerator = RandomNumberGenerator.Create())
        {
            randomNumberGenerator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}