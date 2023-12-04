using System;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.Planners;
using Microsoft.SemanticKernel;
using System.Text.Json;

var azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AzureOpenAIEndpoint");
var azureOpenAIKey = Environment.GetEnvironmentVariable("AzureOpenAIKey");

// Create a new kernel builder
var builder = new KernelBuilder();
builder.WithAzureOpenAIChatCompletionService("gpt-4", azureOpenAIEndpoint, azureOpenAIKey);
var kernel = builder.Build();

var samplePlugins = kernel.ImportSemanticFunctionsFromDirectory(System.IO.Directory.GetCurrentDirectory(), "SamplePlugins");
var customPlugins = kernel.ImportSemanticFunctionsFromDirectory(System.IO.Directory.GetCurrentDirectory(), "CustomPlugins");
var planner = new SequentialPlanner(kernel);

async Task<string> GenerateLimerick(IKernel kernel, IDictionary<string, ISKFunction> samplePlugins)
{
    var prompt = "Jed is a man who is always late to work. Build a limerick about Jed.";
    var result = await kernel.RunAsync(prompt, samplePlugins["Limerick"]);
    return result.GetValue<string>();
}


async Task<string> GenerateExcuse(IKernel kernel, IDictionary<string, ISKFunction> samplePlugins)
{
    var prompt = "I need an excuse for why I cannot go to my neighbor's cat wedding. Please give me one because the idea of marrying two cats together is insanity.";
    var result = await kernel.RunAsync(prompt, samplePlugins["Excuses"]);
    return result.GetValue<string>();
}

async Task<string> GenerateExcuseInLimerickForm(IKernel kernel, SequentialPlanner planner)
{
    var prompt = "I need an excuse for why I was late for work today. Please make it in the form of a limerick.";
    var plan = await planner.CreatePlanAsync(prompt);
    var result = await kernel.RunAsync(prompt, plan);
    return result.GetValue<string>();
}

async Task<string> GenerateScoutingReportInLimerickForm(IKernel kernel, SequentialPlanner planner)
{
    var prompt = "Build a scouting report for Dawson Knox. Please make it in the form of a limerick.";
    var plan = await planner.CreatePlanAsync(prompt);
    var result = await kernel.RunAsync(prompt, plan);
    return result.GetValue<string>();
}

char inputCommand = ' ';
while (inputCommand != 'q')
{
    Console.Write(@"------------------------------------------------------
    This is a demonstration of Semantic Kernel plugins.
    Choose the demonstration you would like to see:
    1. Limerick
    2. Excuse
    3. Excuse in the form of a limerick
    4. Scouting report in the form of a limerick

    Press q to quit.

    Enter a command: ");
    inputCommand = Console.ReadKey().KeyChar;
    Console.WriteLine();

    if (inputCommand == 'q')
    {
        break;
    }

    var resultString = "";
    switch(inputCommand)
    {
        case '1':
            resultString = await GenerateLimerick(kernel, samplePlugins);
            break;
        case '2':
            resultString = await GenerateExcuse(kernel, samplePlugins);
            break;
        case '3':
            resultString = await GenerateExcuseInLimerickForm(kernel, planner);
            break;
        case '4':
            resultString = await GenerateScoutingReportInLimerickForm(kernel, planner);
            break;
        default:
            Console.WriteLine("Invalid command.");
            continue;
    }
    Console.WriteLine(resultString);
}
