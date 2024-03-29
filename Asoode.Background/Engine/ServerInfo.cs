using Asoode.Core.Contracts.General;

namespace Asoode.Background.Engine;

internal class ServerInfo : IServerInfo
{
    public string ContentRootPath { get; set; }
    public string EmailsRootPath { get; set; }
    public string FilesRootPath { get; set; }
    public string I18nRootPath { get; set; }
    public string RootPath { get; set; }
    public string SmsRootPath { get; set; }
    public bool IsDevelopment { get; set; }
    public string ReportsRootPath { get; set; }
}