namespace SmtpToHttp.Models;

public class MailMessage
{
    public string FromAddress { get; set; }
    public string FromName { get; set; }
    public List<To> To { get; set; }
    public List<Cc> Cc { get; set; }
    public List<Bcc> Bcc { get; set; }
    public string Subject { get; set; }
    public JsonDocument Message { get; set; }
    public DateTimeOffset Date { get; set; }
}

public class To
{
    public string Name { get; set; }
    public string Address { get; set; }
}

public class Cc
{
    public string Name { get; set; }
    public string Address { get; set; }
}

public class Bcc
{
    public string Name { get; set; }
    public string Address { get; set; }
}