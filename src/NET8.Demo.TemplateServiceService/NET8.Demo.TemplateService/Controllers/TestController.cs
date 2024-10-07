using Microsoft.AspNetCore.Mvc;

namespace NET8.Demo.TemplateService.Controllers;

[Route("api/test")]
public class TestController : ApiControllerBase
{
    private static readonly string[] Names = ["Alice", "Bob", "Charlie", "David", "Eva", "Frank", "Grace"];

    [HttpGet]
    public IActionResult GetRandomData()
    {
        var random = new Random();
        var randomName = Names[random.Next(Names.Length)];
        var randomNumber = random.Next(1, 100);

        return Ok(new
        {
            Name = randomName,
            Number = randomNumber
        });
    }
}
