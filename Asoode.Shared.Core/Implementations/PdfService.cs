using Asoode.Shared.Abstraction.Contracts;
using WkHtmlToPdfDotNet;

namespace Asoode.Shared.Core.Implementations;

internal class PdfService : IPdfService
{
    private readonly SynchronizedConverter _converter;
    private readonly ILoggerService _loggerService;

    public PdfService(ILoggerService loggerService)
    {
        _loggerService = loggerService;
        _converter = new SynchronizedConverter(new PdfTools());
    }
    
    public async Task<Stream?> Generate(string html)
    {
        try
        {
            var pdf = _converter.Convert(new HtmlToPdfDocument()
            {
                GlobalSettings =
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings() { Top = 10 }
                },
                Objects =
                {
                    new ObjectSettings()
                    {
                        PagesCount = true,
                        HtmlContent = html,
                        WebSettings = { DefaultEncoding = "utf-8" },
                    },
                }
            });
            if (pdf == null) return null;
            var stream = new MemoryStream(pdf);
            stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
        catch (Exception e)
        {
            await _loggerService.Error(e.Message, "PdfService.Generate", e);
            return null;
        }
    }
}