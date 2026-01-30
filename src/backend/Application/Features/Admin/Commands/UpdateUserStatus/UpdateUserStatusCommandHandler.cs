using HospitalAppointmentSystem.Application.Common.Interfaces;
using HospitalAppointmentSystem.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HospitalAppointmentSystem.Application.Features.Admin.Commands.UpdateUserStatus;

/// <summary>
/// Handler for UpdateUserStatusCommand
/// </summary>
public class UpdateUserStatusCommandHandler : IRequestHandler<UpdateUserStatusCommand, Result<bool>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateUserStatusCommandHandler> _logger;

    public UpdateUserStatusCommandHandler(
        IApplicationDbContext context,
        ILogger<UpdateUserStatusCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(
        UpdateUserStatusCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Updating user status - UserId: {UserId}, IsActive: {IsActive}",
                request.UserId, request.IsActive);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
            {
                _logger.LogWarning("User not found with ID: {UserId}", request.UserId);
                return Result<bool>.Failure("User not found");
            }

            // Update status using domain methods
            if (request.IsActive)
            {
                user.Activate();
            }
            else
            {
                user.Deactivate();
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User status updated successfully - UserId: {UserId}, IsActive: {IsActive}",
                request.UserId, request.IsActive);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user status - UserId: {UserId}", request.UserId);
            return Result<bool>.Failure($"Failed to update user status: {ex.Message}");
        }
    }
}
