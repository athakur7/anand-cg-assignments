using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace FunctionApp1;

public class Function1
{
    private readonly ILogger<Function1> _logger;

    public Function1(ILogger<Function1> logger)
    {
        _logger = logger;
    }

    [Function("Function1")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var xText = req.Query["x"].ToString();
        if (string.IsNullOrWhiteSpace(xText) || !int.TryParse(xText, out var x))
        {
            return new BadRequestObjectResult("Missing or invalid 'x' query parameter");
        }

        var yText = req.Query["y"].ToString();
        if (string.IsNullOrWhiteSpace(yText) || !int.TryParse(yText, out var y))
        {
            return new BadRequestObjectResult("Missing or invalid 'y' query parameter");
        }

        int result = x + y;

        return new OkObjectResult(result);
    }
}
