using System.Threading.Channels;
using LapTrinh_Web.Services.Interfaces;
using LapTrinh_Web.Services.Models;

namespace LapTrinh_Web.Services.Implementations;

public sealed class EmailDispatchQueue : IEmailDispatchQueue
{
    private readonly Channel<EmailDispatchItem> _channel = Channel.CreateBounded<EmailDispatchItem>(
        new BoundedChannelOptions(500)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false
        });

    public ValueTask QueueOtpEmailAsync(string toEmail, string otpCode, CancellationToken cancellationToken = default)
    {
        var item = new EmailDispatchItem(toEmail, otpCode);
        return _channel.Writer.WriteAsync(item, cancellationToken);
    }

    public IAsyncEnumerable<EmailDispatchItem> DequeueAllAsync(CancellationToken cancellationToken = default)
        => _channel.Reader.ReadAllAsync(cancellationToken);
}
