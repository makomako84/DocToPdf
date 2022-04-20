using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Http;
using OpenXmlPowerTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using WkHtmlToPdfDotNet;
using WkHtmlToPdfDotNet.Contracts;
using System.Runtime.Loader;
using System.Reflection;
using System.Runtime.InteropServices;

namespace DocToPdf
{
    /// <summary>
    /// Сервис для работы с заказами
    /// </summary>
    public class HtmlToPdfService
    {
        public HtmlToPdfService()
        {
            // var architectureFolder = (IntPtr.Size == 8) ? "64 bit" : "32 bit";
            // var executingAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            // var wkHtmlToPdfPath = Path.Combine(executingAssemblyPath, $"wkhtmltox\\v0.12.4\\{architectureFolder}\\libwkhtmltox");

            // CustomAssemblyLoadContext context = new CustomAssemblyLoadContext();
            // context.LoadUnmanagedLibrary(wkHtmlToPdfPath);
        }

        public async Task<object> CreatePdf(IFormFile file)
        {            
            await CreateHtml(file);
            return await CreatePdfConverter(file);
        }

        private async Task CreateHtml(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);   

            //var source = Package.Open(file.OpenReadStream());            
            var document = WordprocessingDocument.Open(memoryStream, true);
            HtmlConverterSettings settings = new HtmlConverterSettings();
            XElement html = HtmlConverter.ConvertToHtml(document, settings);               
            var writer = File.CreateText("test.html");
            writer.WriteLine(html.ToString());
            writer.Dispose();    
        }

        private async Task<object> CreatePdfConverter(IFormFile file)
        {
            byte[] pdf = null;
            var converter = new SynchronizedConverter(new PdfTools());

            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Landscape,
                    PaperSize = PaperKind.A4,
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = File.ReadAllText(@"test.html"),
                        WebSettings = { DefaultEncoding = "utf-8" },
                        HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                        FooterSettings = { FontSize = 9, Right = "Page [page] of [toPage]" }
                    }
                }
            };

            
            pdf = converter.Convert(doc);

            return pdf;
        }

        private async Task<object> CreatePdfUtil(IFormFile file)
        {
            byte[] pdf = null;

            return pdf;
        }
    }
}
