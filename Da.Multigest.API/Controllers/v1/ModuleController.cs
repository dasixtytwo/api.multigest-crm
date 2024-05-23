using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.Contract.Modules;
using System.Net;

namespace Da.Multigest.API.Controllers.v1;

[ApiVersion(1)]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class ModuleController : ControllerBase
{
    private readonly IModuleFacade _moduleRepository;
    private readonly ILogger<ModuleController> _logger;

    public ModuleController(IModuleFacade moduleRepository, ILogger<ModuleController> logger)
    {
        _moduleRepository = moduleRepository;
        _logger = logger;
    }

    [HttpGet("{moduleId}")]
    public IActionResult GetModule(Guid moduleId)
    {
        try
        {
            var response = _moduleRepository.GetModuleByIdAsync(moduleId);

            if (response == null)
            {
                _logger.LogWarning("No module found for this tenant");
                return StatusCode((int)HttpStatusCode.NotFound);
            }

            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}
