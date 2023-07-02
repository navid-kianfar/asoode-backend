using Asoode.Shared.Abstraction.Events.Membership;
using MassTransit;

namespace Asoode.Workers.Background.Consumers.Membership;

internal class UserCreatedConsumer : IConsumer<UserCreated>
{
    public Task Consume(ConsumeContext<UserCreated> context)
    {
        Console.WriteLine(context.Message.Username);
        return Task.CompletedTask;
    }
}