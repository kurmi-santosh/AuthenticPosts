using Microsoft.AspNetCore.Mvc;

namespace Consumer.Controllers;

public class HomeController : Controller
{
    public Task<string> Bad()
    {
        var client = new HttpClient() { BaseAddress = new Uri("http://localhost:5131") };
        return client.GetStringAsync($"/home/{Guid.NewGuid()}");
    }

    public Task<string> Simple([FromServices] IHttpClientFactory factory)
    {
        var client = factory.CreateClient();
        client.BaseAddress = new Uri("http://localhost:5131");

        return client.GetStringAsync($"/home/{Guid.NewGuid()}");
    }


}

