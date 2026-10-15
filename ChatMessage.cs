namespace CybersecurityAwarenessChatbot.Models;

/// <summary>Represents one message shown in the conversation.</summary>
public sealed class ChatMessage
{
    public string Sender { get; }
    public string Text { get; }
    public DateTime TimeSent { get; }

    public ChatMessage(string sender, string text)
    {
        Sender = sender;
        Text = text;
        TimeSent = DateTime.Now;
    }
}
