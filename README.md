# Serenity AIHub SDK

Official .NET SDK for Serenity AIHub API.

## Installation

```bash
dotnet add package SubgenAI.Serenity
```

## Usage

### Using Dependency Injection

```csharp
// In your startup/program.cs
services.AddSerenityAIHub("your-api-key");

// In your service/controller
public class YourService
{
    private readonly ISerenityAIHubClient _client;

    public YourService(ISerenityAIHubClient client)
    {
        _client = client;
    }

    public async Task DoSomething()
    {
        var conversation = await _client.CreateConversation("assistantagent");

        List<ExecuteParameter> input = [];
        input.Add(new(
                "chatId",
                conversation.ChatId
            ));
        input.Add(new(
            "message",
            "Hello, how are you?"
        ));
        var response = await _client.Execute("assistantagent", input);
    }
}
```

### Direct Instantiation

```csharp
var client = SerenityAIHubClient.Create("your-api-key");
var conversation = await client.CreateConversation("assistantagent", null);
var response = await client.SendMessage("assistantagent", conversation.ChatId, "Hello!");
```