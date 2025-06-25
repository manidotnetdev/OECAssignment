using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Exceptions;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Commands.Handlers.Plans;

public class RemoveUserFromProcedurePlanCommandHandler : IRequestHandler<RemoveUserFromProcedurePlanCommand, ApiResponse<Unit>>
{
    private readonly RLContext _context;
    private readonly ILogger<RemoveUserFromProcedurePlanCommandHandler> _logger;
    public RemoveUserFromProcedurePlanCommandHandler(RLContext context, ILogger<RemoveUserFromProcedurePlanCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApiResponse<Unit>> Handle(RemoveUserFromProcedurePlanCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (request.PlanId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid PlanId"));

            if (request.ProcedureId < 1)
                return ApiResponse<Unit>.Fail(new BadRequestException("Invalid ProcedureId"));

            if (request.UserId is null)
            {
                // Remove ALL users for the given Plan + Procedure
                var assignments = await _context.PlanProcedureUsers
                    .Where(ppu => ppu.PlanId == request.PlanId && ppu.ProcedureId == request.ProcedureId)
                    .ToListAsync(cancellationToken);

                if (!assignments.Any())
                    return ApiResponse<Unit>.Succeed(Unit.Value); // nothing to delete, still succeed

                _context.PlanProcedureUsers.RemoveRange(assignments);
            }
            else
            {
                // Remove specific user
                var assignment = await _context.PlanProcedureUsers.FirstOrDefaultAsync(ppu =>
                    ppu.PlanId == request.PlanId &&
                    ppu.ProcedureId == request.ProcedureId &&
                    ppu.UserId == request.UserId.Value,
                    cancellationToken);

                if (assignment == null)
                    return ApiResponse<Unit>.Succeed(Unit.Value); // nothing to delete, still succeed

                _context.PlanProcedureUsers.Remove(assignment);
            }

            await _context.SaveChangesAsync(cancellationToken);
            return ApiResponse<Unit>.Succeed(Unit.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user from procedure plan. PlanId: {PlanId}, ProcedureId: {ProcedureId}, UserId: {UserId}",
                request.PlanId, request.ProcedureId, request.UserId);
            return ApiResponse<Unit>.Fail(ex);
        }
    }

}
