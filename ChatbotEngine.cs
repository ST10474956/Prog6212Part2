using System.Text.RegularExpressions;

namespace CybersecurityAwarenessChatbot.Services;

/// <summary>Creates context-aware cybersecurity replies for the GUI.</summary>
public sealed class ChatbotEngine
{
    // Delegate demonstrates a named callback type for delivering bot replies.
    public delegate void ResponseReadyHandler(string response);
    public event ResponseReadyHandler? ResponseReady;

    private readonly Random _random = new();
    private readonly Dictionary<string, List<string>> _topicResponses;
    private readonly Dictionary<string, string> _memory = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> _conversationHistory = new();
    private string _currentTopic = "general";
    private string _lastTip = "";

    public IReadOnlyDictionary<string, string> Memory => _memory;
    public IReadOnlyList<string> ConversationHistory => _conversationHistory;

    public ChatbotEngine()
    {
        _topicResponses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
        {
            ["password"] = new()
            {
                "Use a long, unique password for every account. A password manager can help you keep track of them.",
                "Turn on multi-factor authentication where it is available, and never share your one-time codes.",
                "Avoid names, birthdays and common words. A passphrase made from several unrelated words is easier to remember and harder to guess."
            },
            ["scam"] = new()
            {
                "Pause before paying or sharing details. Verify the person or company using contact details from its official website.",
                "Be suspicious of urgent messages promising prizes or threatening account closure. Urgency is a common scam tactic.",
                "Never send banking PINs or one-time passwords to someone who contacts you unexpectedly."
            },
            ["privacy"] = new()
            {
                "Review app permissions and turn off access to your location, microphone or contacts when it is not needed.",
                "Share less personal information publicly, and check who can see your posts and profile details.",
                "Use privacy settings, install updates and think carefully before clicking consent or tracking prompts."
            },
            ["phishing"] = new()
            {
                "Check the sender and the full web address before clicking. A familiar logo does not prove a message is genuine.",
                "Do not open unexpected attachments or sign in through links in alarming messages. Visit the official website yourself.",
                "If a message asks for passwords, banking details or urgent payment, verify it through a separate trusted channel."
            },
            ["malware"] = new()
            {
                "Install apps from trusted sources, keep software updated and avoid unknown attachments or downloads.",
                "Use reputable security software and back up important files. Do not ignore unusual pop-ups or device behaviour.",
                "If you suspect malware, disconnect from the network if appropriate and get help from a trusted support person."
            },
            ["wifi"] = new()
            {
                "Avoid sensitive transactions on public Wi-Fi when possible. Use your mobile network or a trusted secure connection.",
                "Check the network name with the venue and turn off automatic connection to unknown Wi-Fi networks.",
                "Keep your device updated and use secure websites. Public Wi-Fi names can be copied by attackers."
            }
        };
    }

    /// <summary>Clears temporary memory and conversation context for a fresh session.</summary>
    public void ResetSession()
    {
        _memory.Clear();
        _conversationHistory.Clear();
        _currentTopic = "general";
        _lastTip = string.Empty;
    }

    public string Reply(string input)
    {
        string message = (input ?? string.Empty).Trim();
        if (message.Length == 0) return Publish("Please type a message first. You can ask about passwords, scams, privacy, phishing, malware or Wi-Fi.");
        _conversationHistory.Add($"You: {message}");
        string lower = message.ToLowerInvariant();

        // Store simple personal details and a preferred cybersecurity topic.
        var nameMatch = Regex.Match(message, @"\b(?:my name is|call me|i am called|i'm called)\s+([A-Za-z][A-Za-z'-]{1,30})", RegexOptions.IgnoreCase);
        if (nameMatch.Success)
        {
            _memory["name"] = nameMatch.Groups[1].Value;
            return Publish($"Nice to meet you, {_memory["name"]}! I will remember your name during this session. Which online safety topic would you like to explore?");
        }
        if (lower.Contains("i'm interested in") || lower.Contains("i am interested in") || lower.Contains("my favourite topic is") || lower.Contains("my favorite topic is"))
        {
            string topic = DetectTopic(lower) ?? "cybersecurity";
            _memory["interest"] = topic;
            _currentTopic = topic;
            return Publish($"Great! I will remember that your favourite cybersecurity topic is {topic} during this session. {GetTopicTip(topic)}");
        }
        if (lower.Contains("what is my name") || lower.Contains("do you remember my name"))
            return Publish(_memory.TryGetValue("name", out string? name) ? $"Your name is {name}. I have remembered it for this session." : "You have not told me your name yet. Try saying, 'My name is Sam'.");