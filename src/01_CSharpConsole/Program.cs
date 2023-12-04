using System;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.AI.OpenAI;

var azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AzureOpenAIEndpoint");
var azureOpenAIKey = Environment.GetEnvironmentVariable("AzureOpenAIKey");

// Create a new kernel builder
var builder = new KernelBuilder();
builder.WithAzureOpenAIChatCompletionService("gpt-4", azureOpenAIEndpoint, azureOpenAIKey);
var kernel = builder.Build();

char inputCommand = ' ';
while (inputCommand != 'q')
{
    Console.Write(@"------------------------------------------------------
    This is a demonstration of Semantic Kernel integrating with Azure Open AI's ChatGPT-4 service.
    We will ask for a scouting report for one of the following players. Enter the number of the player you want to scout.
    1. Josh Allen
    2. Gabriel Davis
    3. Stefon Diggs
    4. Dalton Kincaid

    Press q to quit.

    Enter a command: ");
    inputCommand = Console.ReadKey().KeyChar;
    Console.WriteLine();

    if (inputCommand == 'q')
    {
        break;
    }

    var playerName = "";
    switch(inputCommand)
    {
        case '1':
            playerName = "Josh Allen";
            break;
        case '2':
            playerName = "Gabriel Davis";
            break;
        case '3':
            playerName = "Stefon Diggs";
            break;
        case '4':
            playerName = "Dalton Kincaid";
            break;
        default:
            Console.WriteLine("Invalid command.");
            continue;
    }

    // Send the command to the kernel
    var promptText = $"{playerName} is a player for the Buffalo Bills. What are his key traits, strengths, and areas of his game that need improvement? Also, tell me when your last update was so I know how up to date your information is.";
    var fn = kernel.CreateSemanticFunction(promptText, requestSettings: new OpenAIRequestSettings { MaxTokens = 500 });
    var output = await kernel.RunAsync(promptText, fn);

    Console.WriteLine(output);
}
