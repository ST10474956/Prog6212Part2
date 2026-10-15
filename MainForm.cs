using System.Media;
using CybersecurityAwarenessChatbot.Models;
using CybersecurityAwarenessChatbot.Services;

namespace CybersecurityAwarenessChatbot
{
    public sealed class MainForm : Form
    {
        private readonly ChatbotEngine _bot = new();
        private readonly List<ChatMessage> _messages = new();
        private readonly RichTextBox _chat = new();
        private readonly TextBox _input = new();
        private readonly Label _status = new();
        private readonly string _assetFolder = Path.Combine(AppContext.BaseDirectory, "Assets");
        private SoundPlayer? _voicePlayer;
        private PictureBox? _logo;

        public MainForm()
        {
            Text = "CyberSafe | Cybersecurity Awareness Chatbot";
            MinimumSize = new Size(900, 620);
            Size = new Size(1100, 760);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(15, 23, 42);
            ForeColor = Color.FromArgb(226, 232, 240);
            Font = new Font("Segoe UI", 10F);

            BuildInterface();

            _bot.ResponseReady += _ =>
            {
                if (!IsDisposed && IsHandleCreated)
                    BeginInvoke((Action)(() => _status.Text = "Response generated."));
            };

            FormClosed += (_, _) =>
            {
                _voicePlayer?.Dispose();
                _logo?.Image?.Dispose();
                _logo?.Dispose();
            };

            AddMessage(
                "CyberSafe",
                "Welcome! I'm CyberSafe, your cybersecurity awareness chatbot. I can help with passwords, scams, privacy, phishing, malware and Wi-Fi. What would you like to learn today?");

            // Play the greeting only when the voice asset exists.
            PlayGreeting();
        }

        private void BuildInterface()
        {
            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                Padding = new Padding(16),
                BackColor = BackColor
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 250));
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            Controls.Add(root);

            var side = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 41, 59),
                Padding = new Padding(16)
            };
            side.Paint += (_, e) =>
            {
                using var pen = new Pen(Color.FromArgb(51, 65, 85));
                e.Graphics.DrawRectangle(pen, 0, 0, side.Width - 1, side.Height - 1);
            };
            root.Controls.Add(side, 0, 0);

            var sideFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = side.BackColor
            };
            side.Controls.Add(sideFlow);

            _logo = new PictureBox
            {
                Width = 150,
                Height = 120,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = side.BackColor,
                Margin = new Padding(10, 4, 10, 4)
            };

            string logoPath = Path.Combine(_assetFolder, "cyber-shield.png");
            if (File.Exists(logoPath))
            {
                // Clone the image so the file is not left locked by Image.FromFile.
                using var loadedImage = Image.FromFile(logoPath);
                _logo.Image = new Bitmap(loadedImage);
            }
            sideFlow.Controls.Add(_logo);

            sideFlow.Controls.Add(new Label
            {
                Text = "CYBERSAFE",
                AutoSize = true,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(56, 189, 248),
                Margin = new Padding(0, 8, 0, 4)
            });

            sideFlow.Controls.Add(new Label
            {
                Text = "A student-friendly guide to safer digital habits.",
                AutoSize = true,
                MaximumSize = new Size(205, 0),
                ForeColor = Color.FromArgb(203, 213, 225),
                Margin = new Padding(0, 0, 0, 18)
            });

            sideFlow.Controls.Add(new Label
            {
                Text = "TRY A TOPIC",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184),
                Margin = new Padding(0, 0, 0, 8)
            });

            foreach (string topic in new[]
            {
                "Password safety", "Scam awareness", "Online privacy",
                "Phishing tips", "Malware safety", "Public Wi-Fi"
            })
            {
                var button = MakeButton(topic, Color.FromArgb(51, 65, 85), Color.White, 205, 38);
                button.Margin = new Padding(0, 0, 0, 7);
                button.Click += (_, _) => SendText(topic);
                sideFlow.Controls.Add(button);
            }

            string artPath = Path.Combine(_assetFolder, "ascii-art.txt");
            string artText = File.Exists(artPath)
                ? File.ReadAllText(artPath)
                : "CYBERSAFE\r\nCYBERSECURITY AWARENESS";

            sideFlow.Controls.Add(new Label
            {
                Text = artText,
                AutoSize = false,
                Width = 215,
                Height = 105,
                ForeColor = Color.FromArgb(34, 211, 238),
                Font = new Font("Consolas", 6.5F, FontStyle.Bold),
                Margin = new Padding(0, 14, 0, 4)
            });

            var voice = MakeButton("▶ Play voice greeting", Color.FromArgb(14, 116, 144), Color.White, 205, 38);
            voice.Click += (_, _) => PlayGreeting();
            sideFlow.Controls.Add(voice);

            var clear = MakeButton("Clear conversation", Color.FromArgb(71, 85, 105), Color.White, 205, 38);
            clear.Click += (_, _) =>
            {
                _chat.Clear();
                _messages.Clear();
                _bot.ResetSession();
                AddMessage("CyberSafe", "Conversation cleared. I can help with passwords, scams, privacy, phishing, malware and Wi-Fi.");
                _status.Text = "Ready • Your saved details stay in this session only.";
            };
            sideFlow.Controls.Add(clear);

            var main = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(18, 6, 0, 0),
                BackColor = BackColor
            };
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 66));
            main.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            root.Controls.Add(main, 1, 0);

            main.Controls.Add(new Label
            {
                Text = "Cybersecurity Awareness Chatbot",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 19, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft
            }, 0, 0);

            _chat.Dock = DockStyle.Fill;
            _chat.ReadOnly = true;
            _chat.BorderStyle = BorderStyle.None;
            _chat.BackColor = Color.FromArgb(30, 41, 59);
            _chat.ForeColor = Color.FromArgb(226, 232, 240);
            _chat.Font = new Font("Segoe UI", 11F);
            _chat.Padding = new Padding(12);
            main.Controls.Add(_chat, 0, 1);

            var entry = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                Padding = new Padding(0, 12, 0, 4)
            };
            entry.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            entry.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115));

            _input.Dock = DockStyle.Fill;
            _input.Font = new Font("Segoe UI", 11F);
            _input.BackColor = Color.FromArgb(30, 41, 59);
            _input.ForeColor = Color.White;
            _input.BorderStyle = BorderStyle.FixedSingle;
            _input.PlaceholderText = "Type a question… (e.g. I'm worried about scams)";
            _input.KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Enter && !e.Shift)
                {
                    e.SuppressKeyPress = true;
                    SendText(_input.Text);
                }
            };
            entry.Controls.Add(_input, 0, 0);

            var send = MakeButton("Send message", Color.FromArgb(2, 132, 199), Color.White, 105, 40);
            send.Dock = DockStyle.Fill;
            send.Click += (_, _) => SendText(_input.Text);
            entry.Controls.Add(send, 1, 0);
            main.Controls.Add(entry, 0, 2);

            _status.Dock = DockStyle.Fill;
            _status.Text = "Ready • Your saved details stay in this session only.";
            _status.ForeColor = Color.FromArgb(148, 163, 184);
            _status.Font = new Font("Segoe UI", 9F);
            main.Controls.Add(_status, 0, 3);
        }

        private static Button MakeButton(string text, Color background, Color foreground, int width, int height)
        {
            return new Button
            {
                Text = text,
                Width = width,
                Height = height,
                BackColor = background,
                ForeColor = foreground,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private void SendText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                _status.Text = "Please enter a message first.";
                return;
            }

            string userText = text.Trim();
            AddMessage("You", userText);
            _input.Clear();
            _status.Text = "CyberSafe is preparing a response…";

            try
            {
                string response = _bot.Reply(userText);
                AddMessage("CyberSafe", response);
                _status.Text = _bot.Memory.Count > 0
                    ? $"Session memory: {string.Join(", ", _bot.Memory.Keys)}"
                    : "Ready • Your saved details stay in this session only.";
            }
            catch (Exception ex)
            {
                _status.Text = "Sorry, a response could not be generated.";
                AddMessage("CyberSafe", "I ran into a problem while preparing a response. Details: " + ex.Message);
            }
        }

        private void AddMessage(string sender, string text)
        {
            _messages.Add(new ChatMessage(sender, text));

            _chat.SelectionStart = _chat.TextLength;
            _chat.SelectionColor = sender == "You"
                ? Color.FromArgb(56, 189, 248)
                : Color.FromArgb(74, 222, 128);
            _chat.SelectionFont = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            _chat.AppendText($"{sender}  ·  {DateTime.Now:HH:mm}{Environment.NewLine}");

            _chat.SelectionColor = Color.FromArgb(226, 232, 240);
            _chat.SelectionFont = new Font("Segoe UI", 10.5F);
            _chat.AppendText(text + Environment.NewLine + Environment.NewLine);

            _chat.SelectionStart = _chat.TextLength;
            _chat.ScrollToCaret();
        }

        private void PlayGreeting()
        {
            try
            {
                string wav = Path.Combine(_assetFolder, "voice.wav");
                if (!File.Exists(wav))
                {
                    _status.Text = "Voice file not found. Restore Assets/voice.wav and try again.";
                    return;
                }

                _voicePlayer?.Stop();
                _voicePlayer?.Dispose();
                _voicePlayer = new SoundPlayer(wav);
                _voicePlayer.Load();
                _voicePlayer.Play();
                _status.Text = "Playing voice greeting…";
            }
            catch (Exception ex)
            {
                _status.Text = "Could not play the greeting: " + ex.Message;
            }
        }
    }
}
