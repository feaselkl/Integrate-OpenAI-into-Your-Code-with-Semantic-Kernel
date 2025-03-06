using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

/* Step 3: Create the host and the kernel */
var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        config.AddUserSecrets<Program>();
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<Kernel>((_) =>
        {
            IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.AddAzureOpenAIChatCompletion(
                deploymentName: hostContext.Configuration["AzureOpenAI:DeploymentName"]!,
                endpoint: hostContext.Configuration["AzureOpenAI:Endpoint"]!,
                apiKey: hostContext.Configuration["AzureOpenAI:ApiKey"]!
            );
            kernelBuilder.Plugins.AddFromType<LightsPlugin>("Lights");

            return kernelBuilder.Build();
        });
    });
var host = builder.Build();
var kernel = host.Services.GetRequiredService<Kernel>();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

/* Step 4: Add .NET secrets to the configuration */
/* Command to run:
    dotnet user-secrets init
    dotnet user-secrets set "AzureOpenAI:DeploymentName" "your-deployment-name"
    dotnet user-secrets set "AzureOpenAI:Endpoint" "your-endpoint"
    dotnet user-secrets set "AzureOpenAI:ApiKey" "your-api-key"
*/

// Enable planning
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new() 
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

/* Step 5: have a conversation */
// Create a history store the conversation
var history = new ChatHistory();
var msg = "Please turn on the lamp";
Console.WriteLine("User > " + msg);
history.AddUserMessage(msg);

// Get the response from the AI
var result = await chatCompletionService.GetChatMessageContentAsync(
   history,
   executionSettings: openAIPromptExecutionSettings,
   kernel: kernel);

// Print the results
Console.WriteLine("Assistant > " + result);
// Add the message from the agent to the chat history
history.AddAssistantMessage(result.Content);

msg = "Turn on the porch light.";
Console.WriteLine("User > " + msg);
history.AddUserMessage(msg);
result = await chatCompletionService.GetChatMessageContentAsync(
   history,
   executionSettings: openAIPromptExecutionSettings,
   kernel: kernel);

Console.WriteLine("Assistant > " + result);
history.AddAssistantMessage(result.Content);
