# Cybersecurity Awareness Chatbot — Part 2

A Windows Forms desktop application called **CyberSafe**. It gives student-friendly cybersecurity guidance through a graphical interface and remembers a small amount of information during the current session.

## Relationship to the earlier Part 1 feedback

The Part 1 marking feedback shared earlier refers to the separate RaceDay event-management project (ERD, API endpoint plan and SQL Server database). Those database-specific requirements do not belong to this Cybersecurity Awareness Chatbot brief, so they have not been mixed into this project. Relevant general lessons have been applied: keep the repository organised, include clear documentation and all required assets, store the GitHub Actions workflow under `.github/workflows/`, and maintain meaningful commits and version tags.

## Features

- Dark, spaced Windows Forms interface with topic buttons and a chat area.
- Voice greeting loaded from `Assets/voice.wav` and a shield/ASCII-art identity asset.
- Keyword recognition for passwords, scams, privacy, phishing, malware and public Wi-Fi.
- Randomised tips stored in `Dictionary<string, List<string>>` collections.
- Follow-up conversation support, including “give me another tip”, “explain more” and “tell me more”.
- Session memory for a user's name and favourite cybersecurity topic.
- Simple sentiment detection for worried, frustrated and curious messages.
- Empty-input and unknown-input handling.
- Object-oriented structure, a generic `List<ChatMessage>`, dictionaries and a response delegate/event.

## Requirements

- Windows 10 or Windows 11.
- .NET 8 SDK (Windows desktop workload / Windows Forms support).
- Visual Studio 2022 or newer with the **.NET desktop development** workload, or the .NET CLI on Windows.

## Run in Visual Studio

1. Extract the project folder.
2. Open `CybersecurityAwarenessChatbot.csproj` in Visual Studio.
3. Allow the project to restore/build if prompted.
4. Set the project as the startup project.
5. Press **F5** or click **Start**.
6. Use the topic buttons or type a question into the message box.

## Run using the .NET CLI

Open a terminal in this folder on a Windows machine and run:

```powershell
dotnet restore
dotnet build
dotnet run
```

This is a Windows Forms application and will not run as a desktop GUI on Linux or macOS.

## Try these example messages

| Purpose | Example | Expected behaviour |
|---|---|---|
| Password keyword | `Tell me about password safety` | Shows a password safety tip. |
| Scam keyword + sentiment | `I'm worried about online scams` | Responds supportively and provides a scam tip. |
| Privacy memory | `I'm interested in privacy` | Saves the preferred topic for the session and gives a tip. |
| Name memory | `My name is Sam` | Greets Sam and remembers the name for the session. |
| Recall | `What is my name?` | Recalls the name if it was entered earlier in the session. |
| Random responses | `Give me another phishing tip` | Selects a phishing tip from a list. |
| Follow-up | `Explain more` | Continues with the current topic. |
| Unknown input | `zebra moon rocket` | Gives a safe default response without closing. |
| Empty input | Send an empty message | Prompts the user to type a message. |

## Project structure

```text
CybersecurityAwarenessChatbot/
├── Assets/
│   ├── ascii-art.txt
│   ├── cyber-shield.png
│   └── voice.wav
├── Docs/
│   ├── Part2-Report.md
│   ├── Test-Plan.md
│   └── Video-Presentation-Script.md
├── Models/
│   └── ChatMessage.cs
├── Services/
│   └── ChatbotEngine.cs
├── .github/workflows/
│   └── build.yml
├── MainForm.cs
├── Program.cs
├── CybersecurityAwarenessChatbot.csproj
└── README.md
```

## Memory and privacy

The chatbot's memory is in-memory only. It is not saved to a database or uploaded to a server. Closing the app clears the session memory. Do not enter real passwords, banking information or other sensitive personal information.

## Troubleshooting

- If Visual Studio says the target framework is unavailable, install the .NET 8 SDK and the .NET desktop development workload.
- If the greeting does not play, check that `Assets/voice.wav` exists and that system volume is enabled.
- If the logo is missing, check that `Assets/cyber-shield.png` exists. The text and chat functions do not depend on the logo.
- If the application closes unexpectedly, run it from Visual Studio and check the Error List / Output window.

## GitHub and submission checklist

- [ ] Push the complete project folder to your own GitHub repository.
- [ ] Confirm the source, README and three assets are visible in GitHub.
- [ ] Confirm the build workflow runs successfully on GitHub Actions.
- [ ] Keep at least six meaningful commits; suggested messages are in `Docs/Part2-Report.md`.
- [ ] Create at least two releases/tags (the rubric recommends three clear version tags). Use the release notes in `Docs/Release-Notes.md` as a guide.
- [ ] Record your own screen and voice-over demonstrating the code and features. Upload as **Unlisted** to YouTube.
- [ ] Replace the video placeholder below with your actual unlisted YouTube URL.
- [ ] Submit only your GitHub repository URL on ARC, and submit the YouTube unlisted link as instructed by your lecturer.



