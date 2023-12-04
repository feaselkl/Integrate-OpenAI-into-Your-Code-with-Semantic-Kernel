using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.SemanticKernel.Planners;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel;
using System.Text.Json;
using System.ComponentModel.DataAnnotations;

namespace _04_WebApp.Pages;

public class SemanticPluginsModel : PageModel
{
    private readonly ILogger<SemanticPluginsModel> _logger;

    public SemanticPluginsModel(ILogger<SemanticPluginsModel> logger)
    {
        _logger = logger;
        InputMessage = "If my investment of 2130.23 dollars increased by 23%, how much would I have after I spent $5 on a latte?";
    }

    [BindProperty, MaxLength(300), Required]
    public string InputMessage {get; set;}
    [BindProperty, Required]
    public bool AsIanMalcolm {get; set;}
    public string OutputMessage {get; private set;} = "";

    private async Task<KernelResult> TalkToIan(string request)
    {
        var azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AzureOpenAIEndpoint");
        var azureOpenAIKey = Environment.GetEnvironmentVariable("AzureOpenAIKey");

        var builder = new KernelBuilder();
        builder.WithAzureOpenAIChatCompletionService("gpt-4", azureOpenAIEndpoint, azureOpenAIKey);
        var kernel = builder.Build();

        var mathPlugins = kernel.ImportFunctions(new Plugins.MathPlugin.Math(), "MathPlugin");
        var customPlugins = kernel.ImportSemanticFunctionsFromDirectory("Plugins", "Semantic");
        var planner = new SequentialPlanner(kernel);

        var plan = await planner.CreatePlanAsync(request);

        _logger.LogInformation(JsonSerializer.Serialize(plan, new JsonSerializerOptions { WriteIndented = true }));

        // Execute the plan
        return await kernel.RunAsync(plan);
    }
    public void OnPost()
    {
        if (ModelState.IsValid)
        {
            var request = InputMessage;
            if (AsIanMalcolm)
            {
                request += " Please respond as though you were Dr. Ian Malcolm.";
            }
            OutputMessage = TalkToIan(request).Result.GetValue<string>()!.Trim();
        }
    }
}

