﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebFrontend1.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGet()
        {
            // Call *mywebapi*, and display its response in the page
            using (var client = new System.Net.Http.HttpClient())
            {
                var request = new System.Net.Http.HttpRequestMessage();

                // A delay is a quick and dirty way to work around the fact that
                // the mywebapi service might not be immediately ready on startup.
                // See the text for some ideas on how you can improve this.
                // Uncomment if not using healthcheck (Visual Studio 17.13 or later)
                // await System.Threading.Tasks.Task.Delay(10000);

                // mywebapi is the service name, as listed in docker-compose.yml.
                // Docker Compose creates a default network with the services
                // listed in docker-compose.yml exposed as host names.
                // The port 8080 is exposed in the WebAPI Dockerfile.
                // If your WebAPI is exposed on port 80 (the default for HTTP, used
                // with earlier versions of the generated Dockerfile), change
                // or delete the port number here.

                // This is the API URL for the WebAPI service
                // request.RequestUri = new Uri("http://mywebapi-app-20250410135521.happydune-22375e61.centralus.azurecontainerapps.io/Counter");

                //This version uses the container environment in the URL
                request.RequestUri = new Uri("http://mywebapi-app-20250410135521.MyWebAPI-env-20250410135522.centralus.azurecontainerapps.io/Counter");
                
                // We should also be able to jusst put the container app name

                request.RequestUri = new Uri("http://mywebapi-app-20250410135521/Counter");

                var response = await client.SendAsync(request);
                string counter = await response.Content.ReadAsStringAsync();
                ViewData["Message"] = $"Counter value from cache :{counter}";
            }
        }
    }
   
}
