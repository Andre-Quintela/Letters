using Letters.Application.DTOs;
using Letters.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Letters.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EssayController : ControllerBase
{
    private readonly IEssayService _essayService;

    public EssayController(IEssayService essayService)
    {
        _essayService = essayService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateEssay([FromBody] EssayRequestDto request)
    {
        try
        {
            var essay = await _essayService.CreateEssayAsync(request);
            return Ok(essay);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{essayId}/correct")]
    public async Task<IActionResult> CorrectEssay(Guid essayId)
    {
        try
        {
            var correctedEssay = await _essayService.CorrectEssayAsync(essayId);
            return Ok(correctedEssay);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("{essayId}")]
    public async Task<IActionResult> GetEssayById(Guid essayId)
    {
        try
        {
            var essay = await _essayService.GetEssayByIdAsync(essayId);
            return Ok(essay);
        }
        catch (Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserEssays(Guid userId)
    {
        try
        {
            var essays = await _essayService.GetUserEssaysAsync(userId);
            return Ok(essays);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
