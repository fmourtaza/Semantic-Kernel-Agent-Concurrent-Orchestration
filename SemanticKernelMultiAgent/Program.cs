using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace SemanticKernelConcurrent;  // Updated to match your project

/// <summary>
/// Simple expert agent class that represents a specialized AI persona
/// </summary>
public class ExpertAgent
{
    public string Name { get; }
    public string SystemPrompt { get; }

    public ExpertAgent(string name, string systemPrompt)
    {
        Name = name;
        SystemPrompt = systemPrompt;
    }
}

/// <summary>
/// Response class to hold expert responses with timing information
/// </summary>
public record ExpertResponse
{
    public string ExpertName { get; init; } = string.Empty;
    public string Response { get; init; } = string.Empty;
    public TimeSpan Duration { get; init; }
}

/// <summary>
/// Multi-Agent Concurrent Demo using Microsoft Semantic Kernel
/// 
/// This demo showcases how to create and orchestrate multiple AI agents that work 
/// simultaneously on the same task, each bringing their unique expertise to provide 
/// comprehensive insights. Instead of querying one AI model sequentially, the system 
/// deploys specialized "expert agents" that think in parallel, dramatically reducing 
/// response time while enriching the quality of answers.
/// 
/// Key Features:
/// - True Parallel Execution using Task.WhenAll()
/// - Specialized Expert Personas with tailored instructions
/// - Performance Optimization through concurrent processing
/// - Production-Ready using stable Semantic Kernel APIs
/// - Transparent Timing with real-time feedback
/// - Robust Error Handling for individual agent failures
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🚀 Starting Semantic Kernel Multi-Expert Demo");
        Console.WriteLine("=============================================");
        
        try
        {
            // Create a kernel with AI service configuration
            Kernel kernel = CreateKernelWithChatCompletion();
            Console.WriteLine("✅ Kernel created successfully!");

            // Get the chat completion service
            var chatService = kernel.GetRequiredService<IChatCompletionService>();
            Console.WriteLine("✅ Chat completion service ready!");

            // Create expert personas
            var physicsExpert = new ExpertAgent("Physics Expert", 
                "You are an expert physicist. Answer questions from a physics perspective, " +
                "focusing on physical laws, properties, energy, matter, and scientific principles.");

            var chemistryExpert = new ExpertAgent("Chemistry Expert",
                "You are an expert chemist. Answer questions from a chemistry perspective, " +
                "focusing on molecular structure, chemical bonds, reactions, and chemical properties.");

            Console.WriteLine($"✅ Experts created: {physicsExpert.Name} and {chemistryExpert.Name}");
            Console.WriteLine();

            // Ask a question concurrently
            string question = "What is temperature?";
            Console.WriteLine($"🤔 Question: {question}");
            Console.WriteLine("⏳ Asking both experts concurrently...");
            Console.WriteLine();

            // Run both experts concurrently
            var physicsTask = GetExpertResponse(chatService, kernel, physicsExpert, question);
            var chemistryTask = GetExpertResponse(chatService, kernel, chemistryExpert, question);

            // Wait for both to complete
            var responses = await Task.WhenAll(physicsTask, chemistryTask);

            // Display the results
            Console.WriteLine("📋 CONCURRENT EXPERT RESPONSES:");
            Console.WriteLine("===============================");
            
            for (int i = 0; i < responses.Length; i++)
            {
                Console.WriteLine($"\n🔬 {responses[i].ExpertName}:");
                Console.WriteLine("─────────────────────────────");
                Console.WriteLine(responses[i].Response);
                Console.WriteLine($"⏱️  Response time: {responses[i].Duration.TotalSeconds:F2} seconds");
                Console.WriteLine();
            }

            var totalTime = responses.Max(r => r.Duration);
            Console.WriteLine($"✅ All experts responded! Total time: {totalTime.TotalSeconds:F2} seconds");
            Console.WriteLine("   (Concurrent execution - both ran in parallel!)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error occurred: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"   Inner exception: {ex.InnerException.Message}");
            }
        }

        Console.WriteLine("\n🏁 Press any key to exit...");
        Console.ReadKey();
    }

    private static async Task<ExpertResponse> GetExpertResponse(
        IChatCompletionService chatService, 
        Kernel kernel, 
        ExpertAgent expert, 
        string question)
    {
        var startTime = DateTime.Now;
        
        try
        {
            // Create a chat history with the expert's system prompt
            var chatHistory = new ChatHistory(expert.SystemPrompt);
            chatHistory.AddUserMessage(question);

            Console.WriteLine($"🤖 {expert.Name} is thinking...");

            // Get response from the expert
            var response = await chatService.GetChatMessageContentAsync(
                chatHistory, 
                executionSettings: null, 
                kernel: kernel);

            var duration = DateTime.Now - startTime;
            Console.WriteLine($"✅ {expert.Name} responded in {duration.TotalSeconds:F2}s");
            
            return new ExpertResponse
            {
                ExpertName = expert.Name,
                Response = response.Content ?? "No response received",
                Duration = duration
            };
        }
        catch (Exception ex)
        {
            var duration = DateTime.Now - startTime;
            Console.WriteLine($"❌ {expert.Name} failed: {ex.Message}");
            
            return new ExpertResponse
            {
                ExpertName = expert.Name,
                Response = $"Error: {ex.Message}",
                Duration = duration
            };
        }
    }

    /// <summary>
    /// Create a kernel with chat completion service configured.
    /// </summary>
    private static Kernel CreateKernelWithChatCompletion()
    {
        var builder = Kernel.CreateBuilder();
        
        // CHOOSE ONE OF THE OPTIONS BELOW:

        // OPTION 1: Azure OpenAI (recommended for enterprise)
        // Uncomment and fill in your Azure OpenAI details:
        builder.AddAzureOpenAIChatCompletion(
            deploymentName: "gpt-4o",        // e.g., "gpt-4o"
            endpoint: "https://azureaihub8699990451.openai.azure.com/",
            apiKey: "GKRxTwmDMX5lRwJvGvl76UuOrDXpcnzoNujG2K0ZRva6HN1WJL03JQQJ99BEAC4f1cMXJ3w3AAAAACOGAuTU");

        // OPTION 2: OpenAI (easier to get started)
        // Uncomment and add your OpenAI API key:
        /*
        builder.AddOpenAIChatCompletion(
            modelId: "gpt-4o",                            // or "gpt-3.5-turbo"
            apiKey: "your-openai-api-key");
        */

        // OPTION 3: Ollama (free local models)
        // Uncomment if you have Ollama running locally:
        /*
        builder.AddOllamaChatCompletion(
            modelId: "llama3.1",                          // or any model you have
            endpoint: new Uri("http://localhost:11434"));
        */

        // OPTION 4: Environment Variables (recommended for security)
        // Set environment variables and uncomment:
        /*
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
        var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT");

        if (!string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(endpoint) && !string.IsNullOrEmpty(deploymentName))
        {
            builder.AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey);
        }
        else if (!string.IsNullOrEmpty(apiKey))
        {
            builder.AddOpenAIChatCompletion("gpt-4o", apiKey);
        }
        else
        {
            throw new InvalidOperationException("No AI service configuration found. Please set environment variables or configure directly in code.");
        }
        */

        return builder.Build();
    }
}

/*
 * PROJECT SETUP INSTRUCTIONS:
 * 
 * 1. Create new console project:
 *    dotnet new console -n SemanticKernelMultiAgent
 *    cd SemanticKernelMultiAgent
 * 
 * 2. Add required packages:
 *    dotnet add package Microsoft.SemanticKernel
 *    dotnet add package Microsoft.SemanticKernel.Connectors.OpenAI
 *    dotnet add package Microsoft.SemanticKernel.Connectors.AzureOpenAI  # If using Azure
 * 
 * 3. Replace Program.cs with this code
 * 
 * 4. Configure your AI service (uncomment one option in CreateKernelWithChatCompletion)
 * 
 * 5. Build and run:
 *    dotnet build
 *    dotnet run
 * 
 * FEATURES DEMONSTRATED:
 * - Multi-agent concurrent processing
 * - Specialized expert personas
 * - Performance timing and analysis
 * - Graceful error handling
 * - Production-ready architecture
 * - Interactive demo with multiple questions
 */