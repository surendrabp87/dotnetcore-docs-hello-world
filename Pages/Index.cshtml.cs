using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Runtime.InteropServices;

namespace dotnetcoresample.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
    
        public string OSVersion { get { return RuntimeInformation.OSDescription; } }
        public string Message { get; private set; }
    
        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    
        public void OnGet()
        {
            var environmentStage = _configuration["environment_stage"];
    
            if (!string.IsNullOrEmpty(environmentStage))
            {
                Message = _configuration["Message"];
            }
            else
            {
                Message = "Hello, World!";
            }
        }
    }
}


