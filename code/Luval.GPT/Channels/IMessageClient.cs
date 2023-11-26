namespace Luval.GPT.Channels
{
    public interface IMessageClient
    {
        Task<ChannelMessageResponse> SendTextMessageAsync(string senderId, string messageBody, CancellationToken cancellationToken);
    }
}