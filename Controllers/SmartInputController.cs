using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSpendAI.Models.Dtos.Finance;
using SmartSpendAI.Services.AI;

namespace SmartSpendAI.Controllers
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

            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var result = await _smartInputService.ParseAsync(request.Input, userId.Value, cancellationToken);
            return Ok(result);
        }

        [HttpPost("learn-from-correction")]
        public async Task<IActionResult> LearnFromCorrection([FromBody] LearnFromCorrectionRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var userId = GetCurrentUserId();
            if (userId is null)
            {
                return Unauthorized();
            }

            try
            {
                await _smartInputService.LearnFromCorrectionAsync(
                    request.Input,
                    userId.Value,
                    request.CorrectedCategoryId,
                    cancellationToken);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(new { message = "Da cap nhat hoc may theo chinh sua cua ban." });
        }
    }
}
