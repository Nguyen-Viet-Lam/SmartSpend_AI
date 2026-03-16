using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web_Project.Models.Dtos.Finance;
using Web_Project.Services.AI;

namespace Web_Project.Controllers
{
    [Authorize]
    [Route("api/ai")]
    public class SmartInputController : ApiControllerBase
    {
        private readonly ISmartInputService _smartInputService;

        public SmartInputController(ISmartInputService smartInputService)
        {
            _smartInputService = smartInputService;
        }

        [HttpPost("smart-input")]
        public async Task<ActionResult<SmartInputResponse>> Parse([FromBody] SmartInputRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result = await _smartInputService.ParseAsync(request.Input, cancellationToken);
            return Ok(result);
        }
    }
}
