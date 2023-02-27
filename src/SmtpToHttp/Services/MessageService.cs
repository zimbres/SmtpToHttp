namespace SmtpToHttp.Services;

public sealed class MessageService : MessageStore
{
    private readonly HttpService _httpService;

    public MessageService(HttpService httpService)
    {
        _httpService = httpService;
    }

    public override async Task<SmtpResponse> SaveAsync(ISessionContext context, IMessageTransaction transaction,
        ReadOnlySequence<byte> buffer, CancellationToken cancellationToken)
    {
        await using var stream = new MemoryStream();

        var position = buffer.GetPosition(0);
        while (buffer.TryGet(ref position, out var memory))
        {
            await stream.WriteAsync(memory, cancellationToken);
        }

        stream.Position = 0;

        var mime = await MimeMessage.LoadAsync(stream, cancellationToken);

        if (mime.Body.ContentType.MimeType == "text/html")
        {
            var response = new SmtpResponse(SmtpReplyCode.CommandUnrecognized, "Body must be plain text. Got html.");
            return response;
        }

        var tolist = new List<To>();
        foreach (var item in mime.To.Mailboxes)
        {
            var to = new To
            {
                Address = item.Address,
                Name = item.Name
            };
            tolist.Add(to);
        }

        var cclist = new List<Cc>();
        foreach (var item in mime.Cc.Mailboxes)
        {
            var cc = new Cc
            {
                Address = item.Address,
                Name = item.Name
            };
            cclist.Add(cc);
        }

        var bcclist = new List<Bcc>();
        foreach (var item in mime.Bcc.Mailboxes)
        {
            var bcc = new Bcc
            {
                Address = item.Address,
                Name = item.Name
            };
            bcclist.Add(bcc);
        }

        JsonDocument body = default!;
        try
        {
            body = JsonDocument.Parse(mime.TextBody);
        }
        catch (Exception)
        {
            body = JsonDocument.Parse(JsonSerializer.Serialize(mime.HtmlBody));
        }

        var message = new MailMessage
        {
            FromName = mime.From.Mailboxes.First().Name,
            FromAddress = mime.From.Mailboxes.First().Address,
            To = tolist,
            Cc = cclist,
            Bcc = bcclist,
            Subject = mime.Subject,
            Message = body,
            Date = mime.Date
        };

        await _httpService.PostAsync(message);

        return SmtpResponse.Ok;
    }
}
