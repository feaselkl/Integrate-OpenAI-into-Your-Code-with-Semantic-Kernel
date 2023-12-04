using Microsoft.SemanticKernel;
using System.IO;
using System.Text.Json;
using Microsoft.SemanticKernel.Plugins.Memory;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;

var azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AzureOpenAIEndpoint");
var azureOpenAIKey = Environment.GetEnvironmentVariable("AzureOpenAIKey");

// Create a new kernel builder
var builder = new KernelBuilder();
builder.WithAzureOpenAIChatCompletionService("gpt-4", azureOpenAIEndpoint, azureOpenAIKey);
var kernel = builder.Build();

var memoryBuilder = new MemoryBuilder();
memoryBuilder.WithAzureOpenAITextEmbeddingGenerationService("text-embedding-ada-002", azureOpenAIEndpoint, azureOpenAIKey);
memoryBuilder.WithMemoryStore(new VolatileMemoryStore());
var memory = memoryBuilder.Build();

const string memoryCollectionName = "JoeFriday";
string filePath = "Dossier.json";
string jsonString = File.ReadAllText(filePath);
var dossier = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);

var context = kernel.CreateNewContext();

// Plugins don't need to be saved in a folder--we can create them inline.
string skPrompt = @"
ChatBot can have a conversation with you about any topic.
It can give explicit instructions or say 'I don't know' if it does not have an answer.

Information about the person we are discussing based on that person's dossier:
";

Console.WriteLine("Loading the data banks with information.");
var i = 1;
foreach (var entry in dossier)
{
    skPrompt += "- {{$fact" + i + "}} {{recall $fact" + i + "}}\n";
    await memory.SaveInformationAsync(
        collection: memoryCollectionName,
        description: entry.Value,
        text: entry.Value,
        id: entry.Key
    );
    var fact = $"fact{i}";
    context.Variables[fact] = entry.Key;
    Console.WriteLine($"  Entry {i++} saved");
}

skPrompt += @"
Chat:
{{$history}}
User: {{$userInput}}
ChatBot: ";

Console.WriteLine(skPrompt);

// Bring these memories into the kernel
kernel.ImportFunctions(new TextMemoryPlugin(memory));
var chatFunction = kernel.CreateSemanticFunction(skPrompt, requestSettings: new OpenAIRequestSettings { MaxTokens = 200, Temperature = 0.8 });

context.Variables[TextMemoryPlugin.CollectionParam] = memoryCollectionName;
context.Variables[TextMemoryPlugin.RelevanceParam] = "0.8";

while (true)
{

    var history = "";
    context.Variables["history"] = history;
    Func<string, Task> Chat = async (string input) => {
        // Save new message in the context variables
        context.Variables["userInput"] = input;

        // Process the user message and get an answer
        var answer = await chatFunction.InvokeAsync(context);

        // Append the new interaction to the chat history
        history += $"\nUser: {input}\nChatBot: {answer.GetValue<string>()}\n";
        context.Variables["history"] = history;
        
        // Show the bot response
        Console.WriteLine("ChatBot: " + context);
    };

    Console.WriteLine("What would you like to know about the person?");
    var search = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(search))
    {
        break;
    }

    Console.WriteLine("===========================\n" +
                    "Query: " + search + "\n");

    await Chat(search);
}
