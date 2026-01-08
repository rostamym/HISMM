using HospitalAppointmentSystem.Domain.Entities;

namespace HospitalAppointmentSystem.Application.Common.Interfaces;

/// <summary>
/// Interface for JWT token generation and validation
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generate access token for a user
    /// </summary>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Generate refresh token
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Validate token and extract user ID
    /// </summary>
    Guid? ValidateToken(string token);
}
