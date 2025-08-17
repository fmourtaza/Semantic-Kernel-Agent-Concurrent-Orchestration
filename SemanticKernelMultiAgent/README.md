# Semantic Kernel Multi-Agent Concurrent Orchestration

A .NET 9 demonstration of concurrent AI agent orchestration using Microsoft Semantic Kernel. This project showcases how to deploy multiple specialized AI agents that work simultaneously, dramatically reducing response time while enriching the quality of answers through diverse expertise.

## 🚀 Features

- **True Parallel Execution**: Multiple AI agents run concurrently using `Task.WhenAll()`
- **Specialized Expert Personas**: Each agent has tailored instructions for specific domains
- **Performance Optimization**: Concurrent processing reduces total response time
- **Production-Ready Architecture**: Built with stable Semantic Kernel APIs
- **Real-time Performance Monitoring**: Transparent timing and feedback
- **Robust Error Handling**: Individual agent failures don't crash the system
- **Multiple AI Service Support**: Azure OpenAI, OpenAI, and Ollama compatibility

## 🏗️ Architecture

The demo creates specialized expert agents (Physics Expert and Chemistry Expert) that simultaneously process the same question from their unique perspectives. Instead of sequential queries, the system leverages parallel execution to provide comprehensive insights faster.

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   User Query    │───▶│  Orchestrator    │───▶│  Expert Agents  │
└─────────────────┘    └──────────────────┘    └─────────────────┘
                                │                        │
                                │                        ▼
                                │               ┌─────────────────┐
                                │               │ Physics Expert  │
                                │               └─────────────────┘
                                │                        │
                                │                        ▼
                                │               ┌─────────────────┐
                                │               │Chemistry Expert │
                                │               └─────────────────┘
                                │                        │
                                ▼                        │
                       ┌─────────────────┐              │
                       │  Aggregated     │◀─────────────┘
                       │  Results        │
                       └─────────────────┘
```

## 🛠️ Prerequisites

- **.NET 9.0** or later
- **AI Service Access** (choose one):
  - Azure OpenAI account with API key
  - OpenAI API key
  - Local Ollama installation

## 📦 Installation

1. **Clone the repository**:
   ```powershell
   git clone https://github.com/fmourtaza/Semantic-Kernel-Agent-Concurrent-Orchestration.git
   cd Semantic-Kernel-Agent-Concurrent-Orchestration/SemanticKernelMultiAgent
   ```

2. **Restore dependencies**:
   ```powershell
   dotnet restore
   ```

3. **Configure AI Service**: Open `Program.cs` and uncomment one of the configuration options in `CreateKernelWithChatCompletion()`:

   **Option 1: Azure OpenAI (Recommended for Enterprise)**
   ```csharp
   builder.AddAzureOpenAIChatCompletion(
       deploymentName: "your-deployment-name",
       endpoint: "your-azure-endpoint",
       apiKey: "your-api-key");
   ```

   **Option 2: OpenAI**
   ```csharp
   builder.AddOpenAIChatCompletion(
       modelId: "gpt-4o",
       apiKey: "your-openai-api-key");
   ```

   **Option 3: Ollama (Local)**
   ```csharp
   builder.AddOllamaChatCompletion(
       modelId: "llama3.1",
       endpoint: new Uri("http://localhost:11434"));
   ```

## 🚀 Usage

**Build and run the application**:
```powershell
dotnet build
dotnet run
```

**Sample Output**:
```
🚀 Starting Semantic Kernel Multi-Expert Demo
=============================================
✅ Kernel created successfully!
✅ Chat completion service ready!
✅ Experts created: Physics Expert and Chemistry Expert

🤔 Question: What is temperature?
⏳ Asking both experts concurrently...

🤖 Physics Expert is thinking...
🤖 Chemistry Expert is thinking...
✅ Physics Expert responded in 1.23s
✅ Chemistry Expert responded in 1.45s

📋 CONCURRENT EXPERT RESPONSES:
===============================

🔬 Physics Expert:
─────────────────────────────
Temperature is a measure of the average kinetic energy of particles in a substance...

⏱️  Response time: 1.23 seconds

🔬 Chemistry Expert:
─────────────────────────────
From a chemistry perspective, temperature affects molecular motion and reaction rates...

⏱️  Response time: 1.45 seconds

✅ All experts responded! Total time: 1.45 seconds
   (Concurrent execution - both ran in parallel!)
```

## 🔧 Configuration Options

### Environment Variables (Recommended)
For better security, set environment variables instead of hardcoding API keys:

```powershell
$env:OPENAI_API_KEY = "your-api-key"
$env:AZURE_OPENAI_ENDPOINT = "your-azure-endpoint"
$env:AZURE_OPENAI_DEPLOYMENT = "your-deployment-name"
```

Then uncomment the environment variable configuration in `Program.cs`.

### Customizing Expert Agents
You can easily modify or add new expert personas by editing the expert creation section:

```csharp
var dataScientist = new ExpertAgent("Data Scientist", 
    "You are an expert data scientist. Analyze questions from a statistical and " +
    "machine learning perspective, focusing on data patterns and insights.");
```

## 📊 Performance Benefits

**Sequential vs Concurrent Execution**:
- **Sequential**: Expert 1 (1.2s) + Expert 2 (1.4s) = **2.6s total**
- **Concurrent**: max(Expert 1 (1.2s), Expert 2 (1.4s)) = **1.4s total**
- **Performance Gain**: ~46% faster response time

## 🧪 Extending the Demo

### Adding More Experts
```csharp
var experts = new[]
{
    new ExpertAgent("Physics Expert", "Physics-focused analysis..."),
    new ExpertAgent("Chemistry Expert", "Chemistry-focused analysis..."),
    new ExpertAgent("Biology Expert", "Biology-focused analysis..."),
    new ExpertAgent("Mathematics Expert", "Mathematical analysis...")
};

var tasks = experts.Select(expert => 
    GetExpertResponse(chatService, kernel, expert, question));
var responses = await Task.WhenAll(tasks);
```

### Interactive Question Loop
```csharp
while (true)
{
    Console.Write("\nEnter your question (or 'quit' to exit): ");
    string question = Console.ReadLine();
    if (question?.ToLower() == "quit") break;
    
    // Process question with all experts...
}
```

## 📚 Dependencies

- **Microsoft.SemanticKernel** (v1.61.0): Core SK framework
- **Microsoft.SemanticKernel.Connectors.OpenAI** (v1.61.0): OpenAI connectivity

## 🤝 Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is open source and available under the [MIT License](LICENSE).

## 🙏 Acknowledgments

- **Microsoft Semantic Kernel Team** for the excellent AI orchestration framework
- **OpenAI** for providing powerful language models
- **.NET Community** for continuous innovation in AI development

## 📞 Support

If you encounter any issues or have questions:
- Open an issue on GitHub
- Check the [Semantic Kernel documentation](https://learn.microsoft.com/semantic-kernel/)
- Join the [Semantic Kernel Discord community](https://aka.ms/SKDiscord)

---

**Happy AI Orchestrating! 🤖✨**
