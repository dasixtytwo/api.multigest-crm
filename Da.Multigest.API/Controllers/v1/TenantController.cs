using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedClassLibrary.Contract.Tenants;
using SharedClassLibrary.DTOs.Tenants;
using SharedClassLibrary.Models.Tenants;
using System.Net;

namespace Da.Multigest.API.Controllers.v1
{
    [ApiVersion(1)]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize]
    public class TenantController : ControllerBase
    {
        private readonly ITenantFacade _tenantRepository;
        private readonly ILogger<TenantController> _logger;

        public TenantController(ITenantFacade tenatRepository, ILogger<TenantController> logger)
        {
            _logger = logger;
            _tenantRepository = tenatRepository;
        }

        [HttpGet("{tenantName}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Tenant>> GetTenantByName(string tenantName)
        {
            try
            {
                // string tenantName = $"{name[0].ToString().ToUpper()}{name.Substring(1)}";
                Tenant response = await _tenantRepository.GetTenantByNameAsync(tenantName.ToLower());

                if (response == null)
                {
                    _logger.LogInformation($"Tenant not found. {DateTime.Now}");
                    return StatusCode((int)HttpStatusCode.NotFound);
                }

                // log message
                _logger.LogInformation($"Tenant retrieved");
                return Ok(response);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("id/{tenantId}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<Tenant>> GetTenantByTenantId(string tenantId)
        {
            try
            {
                Tenant tenant = await _tenantRepository.GetTenantByIdAsync(tenantId);

                if (tenant == null)
                {
                    _logger.LogInformation($"Tenant not found. {DateTime.Now}");
                    return StatusCode((int)HttpStatusCode.NotFound);
                }

                // log message
                _logger.LogInformation($"Tenant retrieved");
                return Ok(tenant);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CreateNewTenantAsync([FromBody] TenantRequest request)
        {
            try
            {
                var response = await _tenantRepository.CreateNewTenantAsync(request);



                if (response == null)
                {
                    _logger.LogInformation($"Tenant not found. {DateTime.Now}");
                    return StatusCode((int)HttpStatusCode.NotFound);
                }

                // log message
                _logger.LogInformation($"Tenant retrieved");
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
