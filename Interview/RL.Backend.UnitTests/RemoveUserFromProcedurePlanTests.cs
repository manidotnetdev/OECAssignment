using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RL.Backend.Commands;
using RL.Backend.Commands.Handlers.Plans;
using RL.Backend.Exceptions;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.UnitTests;

[TestClass]
public class RemoveUserFromProcedurePlanTests
{
    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task RemoveUserFromProcedurePlan_InvalidPlanId_ReturnsBadRequest(int planId)
    {
        // Arrange
        var context = DbContextHelper.CreateContext();
        var handler = new RemoveUserFromProcedurePlanCommandHandler(context);
        var request = new RemoveUserFromProcedurePlanCommand
        {
            PlanId = planId,
            ProcedureId = 1
        };

        // Act
        var result = await handler.Handle(request, default);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Exception.Should().BeOfType<BadRequestException>();
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(int.MinValue)]
    public async Task RemoveUserFromProcedurePlan_InvalidProcedureId_ReturnsBadRequest(int procedureId)
    {
        // Arrange
        var context = DbContextHelper.CreateContext();
        var handler = new RemoveUserFromProcedurePlanCommandHandler(context);
        var request = new RemoveUserFromProcedurePlanCommand
        {
            PlanId = 1,
            ProcedureId = procedureId
        };

        // Act
        var result = await handler.Handle(request, default);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Exception.Should().BeOfType<BadRequestException>();
    }

    [TestMethod]
    public async Task RemoveUserFromProcedurePlan_RemoveAllUsers_Succeeds()
    {
        // Arrange
        var context = DbContextHelper.CreateContext();
        var planId = 1;
        var procedureId = 2;

        context.PlanProcedureUsers.AddRange(
            new PlanProcedureUser { PlanId = planId, ProcedureId = procedureId, UserId = 100 },
            new PlanProcedureUser { PlanId = planId, ProcedureId = procedureId, UserId = 101 }
        );
        await context.SaveChangesAsync();

        var handler = new RemoveUserFromProcedurePlanCommandHandler(context);
        var request = new RemoveUserFromProcedurePlanCommand
        {
            PlanId = planId,
            ProcedureId = procedureId
            // UserId is null => remove all
        };

        // Act
        var result = await handler.Handle(request, default);

        // Assert
        var remaining = await context.PlanProcedureUsers
            .Where(ppu => ppu.PlanId == planId && ppu.ProcedureId == procedureId)
            .ToListAsync();

        result.Succeeded.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
        remaining.Should().BeEmpty();
    }

    [TestMethod]
    public async Task RemoveUserFromProcedurePlan_RemoveSpecificUser_Succeeds()
    {
        // Arrange
        var context = DbContextHelper.CreateContext();
        var planId = 2;
        var procedureId = 3;
        var userId = 200;

        context.PlanProcedureUsers.AddRange(
            new PlanProcedureUser { PlanId = planId, ProcedureId = procedureId, UserId = userId },
            new PlanProcedureUser { PlanId = planId, ProcedureId = procedureId, UserId = 201 }
        );
        await context.SaveChangesAsync();

        var handler = new RemoveUserFromProcedurePlanCommandHandler(context);
        var request = new RemoveUserFromProcedurePlanCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = userId
        };

        // Act
        var result = await handler.Handle(request, default);

        // Assert
        var remaining = await context.PlanProcedureUsers
            .Where(ppu => ppu.PlanId == planId && ppu.ProcedureId == procedureId)
            .ToListAsync();

        result.Succeeded.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
        remaining.Should().ContainSingle(x => x.UserId == 201);
    }

    [TestMethod]
    public async Task RemoveUserFromProcedurePlan_NoUserExists_StillSucceeds()
    {
        // Arrange
        var context = DbContextHelper.CreateContext();
        var planId = 10;
        var procedureId = 20;

        var handler = new RemoveUserFromProcedurePlanCommandHandler(context);
        var request = new RemoveUserFromProcedurePlanCommand
        {
            PlanId = planId,
            ProcedureId = procedureId,
            UserId = 999 // not existing
        };

        // Act
        var result = await handler.Handle(request, default);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
    }
}
