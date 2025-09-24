using Microsoft.AspNetCore.Mvc;

namespace Chirp.WebService.Controllers;

[ApiController]
[Route("")]
public class CheepController : ControllerBase
{
    private readonly IDatabaseRepository<Cheep> _database;

    public CheepController()
    {
        _database = CSVDatabase<Cheep>.GetInstance();
    }

    [HttpPost("/cheep")]
    public IActionResult PostCheep([FromBody] Cheep cheep)
    {
        if (cheep == null)
        {
            return BadRequest("Cheep data is required");
        }

        if (string.IsNullOrEmpty(cheep.Author) || string.IsNullOrEmpty(cheep.Message))
        {
            return BadRequest("Author and Message are required");
        }

        _database.Store(cheep);
        return Ok();
    }

    [HttpGet("/cheeps")]
    public IActionResult GetCheeps()
    {
        var cheeps = _database.Read(int.MaxValue);
        return Ok(cheeps);
    }
}