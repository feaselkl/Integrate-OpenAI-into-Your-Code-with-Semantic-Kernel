# Integrate OpenAI into Your Code with Semantic Kernel

This repository provides the supporting code for my presentation entitled [Integrate OpenAI into Your Code with Semantic Kernel](https://www.catallaxyservices.com/presentations/integrate-openai-into-your-code-with-semantic-kernel/).

## Running the Code

Most of the demos come from a TechExcel entitled [Integrating Azure PaaS and AI Services for Design Wins](https://microsoft.github.io/TechExcel-Integrating-Azure-PaaS-and-AI-Services-for-AI-Design-Wins/). This presentation uses a sub-selection of the total code in that training, but I recommend going through it if you'd like to re-create the the Contoso Suites demos.

To run the Lights demo app, you will need the following .NET user secrets:

- `AzureOpenAI:Endpoint`  -- The full URL of your Azure OpenAI endpoint. I recommend using a region like East US 2, though any region supporting GPT-4o will work.
- `AzureOpenAI:DeploymentName` -- The name of your GPT-4o deployment.
- `AzureOpenAI:ApiKey` -- One of the access keys for your Azure OpenAI service.

Review the **Cheat Sheets** folder for instructions on running each of the demos. Note that you will need a paid Azure subscription (rather than a free subscription) to go through these demos. You can also use the OpenAI service rather than Azure OpenAI, though some code changes will be necessary.
