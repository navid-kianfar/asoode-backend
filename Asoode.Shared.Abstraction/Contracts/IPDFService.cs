namespace Asoode.Shared.Abstraction.Contracts;

public interface IPdfService
{
    Task<Stream?> Generate(string html);
}