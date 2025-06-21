using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using RL.Backend.Commands;
using RL.Backend.Models;
using RL.Data;
using RL.Data.DataModels;

namespace RL.Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class ProceduresController : ControllerBase
{
    private readonly ILogger<ProceduresController> _logger;
    private readonly RLContext _context;
    private readonly IMediator _mediator;
    public ProceduresController(ILogger<ProceduresController> logger, RLContext context, IMediator mediator)
    {
        _logger = logger;
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    [HttpGet]
    [EnableQuery]
    public IEnumerable<Procedure> Get()
    {
        return _context.Procedures;
    }

    [HttpPost("AddUserToProcedurePlan")]
    public async Task<IActionResult> AddUserToProcedurePlan(AddUserToProcedurePlanCommand command, CancellationToken token)
    {
        var response = await _mediator.Send(command, token);
        return response.ToActionResult();
    }

    [HttpPost("RemoveUserFromProcedurePlan")]
    public async Task<IActionResult> RemoveUserToProcedurePlan(RemoveUserFromProcedurePlanCommand command, CancellationToken token)
    {
        var response = await _mediator.Send(command, token);
        return response.ToActionResult();
    }
}
