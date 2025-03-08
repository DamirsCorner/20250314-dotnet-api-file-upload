using Microsoft.AspNetCore.Mvc;
using WebApiFileUpload.Models;

namespace WebApiFileUpload.Controllers;

[ApiController]
[Route("[controller]")]
public class UploadController(ILogger<UploadController> logger) : ControllerBase
{
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(ProblemDetails),
        StatusCodes.Status400BadRequest,
        "application/problem+json"
    )]
    public IActionResult UploadFile([FromForm] UploadRequest request)
    {
        logger.LogInformation(
            "Request received, title: {Title}, description: {Description}, files: {Files}",
            request.Title,
            request.Description,
            request.Files.Count
        );

        foreach (var file in request.Files)
        {
            logger.LogInformation(
                "File uploaded, filename: {Filename}, size: {Size}",
                file.FileName,
                file.Length
            );
        }
        return Accepted();
    }
}
