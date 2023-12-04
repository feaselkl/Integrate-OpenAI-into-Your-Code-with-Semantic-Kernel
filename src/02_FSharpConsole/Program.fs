open System
open System.IO
open Microsoft.SemanticKernel
open Microsoft.SemanticKernel.Connectors.AI.OpenAI

let azureOpenAIEndpoint = Environment.GetEnvironmentVariable("AzureOpenAIEndpoint")
let azureOpenAIKey = Environment.GetEnvironmentVariable("AzureOpenAIKey")

let builder = (new KernelBuilder()).WithAzureOpenAIChatCompletionService(
    "gpt-4",                      // Azure OpenAI Deployment Name
    azureOpenAIEndpoint,
    azureOpenAIKey)
let kernel = builder.Build()

let prompt = "Matt Milano is the starting weakside linebacker for the Buffalo Bills. What are his key traits, strengths, and areas of his game that need improvement?"
let rs = new OpenAIRequestSettings()
rs.MaxTokens <- 500
let playerLookup = kernel.CreateSemanticFunction(prompt, rs)

let output =
    kernel.RunAsync(prompt, playerLookup)
    |> Async.AwaitTask
    |> Async.RunSynchronously

printfn "%O" (output)
