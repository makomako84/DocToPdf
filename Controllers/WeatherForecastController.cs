using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DocToPdf.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly HtmlToPdfService _htmlToPdfService;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, HtmlToPdfService htmlToPdfService)
        {
            _logger = logger;
            _htmlToPdfService = htmlToPdfService;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        /// <summary>
        /// Создать pdf из docx
        /// </summary>
        [HttpPost("pdf-from-docx")]
        public async Task<IActionResult> CreatePdfFromDocx(IFormFile file)
        {
            var pdf = (byte[])await _htmlToPdfService.CreatePdf(file);
            System.IO.File.WriteAllBytes("download.pdf", pdf);
            return Ok();
            //return File(pdf, "application/octet-stream", DateTime.Now + "download.pdf");
        }
    }
}
