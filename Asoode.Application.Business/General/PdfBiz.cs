using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Asoode.Core.Contracts.General;
using Asoode.Core.Contracts.Logging;
using Asoode.Core.Contracts.Storage;
using Asoode.Core.Primitives;
using Asoode.Core.Primitives.Enums;
using Asoode.Core.ViewModels.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Asoode.Business.General
{
    internal class PdfBiz : IPdfBiz
    {
        private readonly IServiceProvider _serviceProvider;

        public PdfBiz(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        
        private Task<int> RunProcessAsync(string command, string args)
        {
            var tcs = new TaskCompletionSource<int>();

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Exited += (sender, args) =>
            {
                tcs.SetResult(process.ExitCode);
                process.Dispose();
            };

            process.Start();

            return tcs.Task;
        }
        
        private string RunProcess(string command, string args)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (string.IsNullOrEmpty(error)) return output;
            return error;
        }
        
        public async Task<OperationResult<bool>> FromHtml(string html, string destination)
        {
            try
            {
                var fullPath = $"{_serviceProvider.GetService<IServerInfo>().FilesRootPath}{destination}";
                var dir = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                var htmlFile = fullPath + ".html";
                await File.WriteAllTextAsync(htmlFile, html, Encoding.UTF8);
                var command = $"{htmlFile} {fullPath}";
                RunProcess("wkhtmltopdf", command);
                await _serviceProvider.GetService<IUploadProvider>().Upload(new StoreViewModel
                {
                    FilePath = fullPath,
                    Section = UploadSection.Pdf,
                    RecordId = Guid.NewGuid()
                });
                File.Delete(htmlFile);
                return OperationResult<bool>.Success();
            }
            catch (Exception ex)
            {
                await _serviceProvider.GetService<IErrorBiz>().LogException(ex);
                return OperationResult<bool>.Failed();
            }
        }
    }
}