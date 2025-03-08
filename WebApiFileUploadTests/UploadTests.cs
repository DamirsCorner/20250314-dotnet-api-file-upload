using System.Net;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.StaticFiles;
using Shouldly;

namespace WebApiFileUploadTests;

public class UploadTests
{
    private WebApplicationFactory<Program> factory;

    [SetUp]
    public void Setup()
    {
        factory = new WebApplicationFactory<Program>();
    }

    [TearDown]
    public void TearDown()
    {
        factory.Dispose();
    }

    [Test]
    public async Task CallWithoutRequiredMetadataFails()
    {
        using var httpClient = factory.CreateClient();
        var content = CreateMultipartContent(null, "Description", Array.Empty<string>());
        var response = await httpClient.PostAsync("/Upload", content);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task CallWithMetadataWithoutFilesSucceeds()
    {
        using var httpClient = factory.CreateClient();
        var content = CreateMultipartContent("Title", "Description", Array.Empty<string>());
        var response = await httpClient.PostAsync("/Upload", content);

        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);
    }

    [Test]
    public async Task CallWithMetadataAndFilesSucceeds()
    {
        using var httpClient = factory.CreateClient();
        var content = CreateMultipartContent(
            "Title",
            "Description",
            ["Files/img.png", "Files/img.jpg"]
        );
        var response = await httpClient.PostAsync("/Upload", content);

        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);
    }

    private MultipartFormDataContent CreateMultipartContent(
        string? title,
        string? description,
        string[] files
    )
    {
        var content = new MultipartFormDataContent();
        if (!string.IsNullOrEmpty(title))
        {
            content.Add(new StringContent(title), "Title");
        }
        if (!string.IsNullOrEmpty(description))
        {
            content.Add(new StringContent(description), "Description");
        }

        var contentTypeProvider = new FileExtensionContentTypeProvider();
        foreach (var file in files)
        {
            var streamContent = new StreamContent(File.OpenRead(file));
            contentTypeProvider.TryGetContentType(file, out var contentType);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(
                contentType ?? "application/octet-stream"
            );
            content.Add(streamContent, "Files", file);
        }
        return content;
    }
}
