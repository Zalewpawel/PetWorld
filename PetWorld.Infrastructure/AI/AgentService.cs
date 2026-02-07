using Microsoft.Extensions.AI;
using PetWorld.Domain.Entities;

namespace PetWorld.Infrastructure.AI;

/// <summary>
/// AI-powered agent service implementing the Writer-Critic pattern.
/// Processes user questions through iterative refinement using GPT-4o.
/// </summary>
public class AgentService
{
    private readonly IChatClient _chatClient;
    private const int MaxIterations = 3;

    public AgentService(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }

    /// <summary>
    /// Processes a user question using the Writer-Critic agent pattern.
    /// The Writer agent generates responses while the Critic agent validates them.
    /// Iterates up to 3 times until an approved answer is generated.
    /// </summary>
    /// <param name="question">The user's question.</param>
    /// <param name="products">Available products for context.</param>
    /// <returns>A tuple containing the final answer, iteration count, and detailed agent log.</returns>
    public async Task<(string Answer, int Iterations, string AgentLog)> ProcessQuestionAsync(string question, List<Product> products)
    {
        string productsContext = BuildProductsContext(products);
        
        string currentAnswer = "";
        int iterations = 0;
        bool isApproved = false;
        var agentLog = new System.Text.StringBuilder();

        while (!isApproved && iterations < MaxIterations)
        {
            iterations++;
            agentLog.AppendLine($"\n=== ITERACJA {iterations} ===");

            agentLog.AppendLine($"[WRITER] Generuje odpowiedź na pytanie: {question}");
            currentAnswer = await CallWriterAsync(question, productsContext, currentAnswer);
            agentLog.AppendLine($"[WRITER] Odpowiedź: {currentAnswer}");

            agentLog.AppendLine($"[CRITIC] Ocenia odpowiedź...");
            var criticFeedback = await CallCriticAsync(currentAnswer, productsContext);
            agentLog.AppendLine($"[CRITIC] Feedback: {criticFeedback}");

            if (criticFeedback.Contains("APPROVED", StringComparison.OrdinalIgnoreCase))
            {
                isApproved = true;
                agentLog.AppendLine($"✓ Odpowiedź zatwierdzona!");
            }
            else if (iterations < MaxIterations)
            {
                agentLog.AppendLine($"✗ Wymaga poprawy, próba {iterations + 1}/{MaxIterations}");
            }
        }

        if (!isApproved)
        {
            currentAnswer = "Przepraszamy, nie byliśmy w stanie wygenerować satysfakcjonującej odpowiedzi. Proszę spróbuj zadać pytanie jeszcze raz.";
            agentLog.AppendLine($"\n⚠️ Limit iteracji ({MaxIterations}) osiągnięty. Zwrócono domyślną odpowiedź.");
        }

        return (currentAnswer, iterations, agentLog.ToString());
    }

    /// <summary>
    /// Builds formatted product catalog context for AI prompts.
    /// </summary>
    /// <param name="products">List of available products.</param>
    /// <returns>Formatted string containing product information.</returns>
    private string BuildProductsContext(List<Product> products)
    {
        if (products == null || !products.Any())
            return "Brak produktów w katalogu.";

        var lines = new System.Collections.Generic.List<string>
        {
            "KATALOG PRODUKTÓW PETWORLD:",
            "=============================="
        };

        foreach (var product in products.OrderBy(p => p.Category))
        {
            lines.Add($"- {product.Name} (ID: {product.Id}, {product.Category}) - {product.Price} zł: {product.Description}");
        }

        return string.Join("\n", lines);
    }

    /// <summary>
    /// Invokes the Writer agent to generate or refine an answer.
    /// </summary>
    /// <param name="question">The user's question.</param>
    /// <param name="context">Product catalog context.</param>
    /// <param name="previousAttempt">Previous answer attempt if refining.</param>
    /// <returns>Generated answer text.</returns>
    private async Task<string> CallWriterAsync(string question, string context, string previousAttempt)
    {
        var prompt = previousAttempt == ""
            ? BuildWriterPrompt(question, context)
            : BuildWriterPromptWithFeedback(question, context, previousAttempt);

        try
        {
            var messages = new List<Microsoft.Extensions.AI.ChatMessage> { new(Microsoft.Extensions.AI.ChatRole.User, prompt) };
            var response = await _chatClient.GetResponseAsync(messages);
            return response.Text ?? "Nie udało się wygenerować odpowiedzi.";
        }
        catch (Exception ex)
        {
            return $"Błąd przy generowaniu odpowiedzi: {ex.Message}";
        }
    }

    /// <summary>
    /// Builds the initial Writer agent prompt.
    /// </summary>
    private string BuildWriterPrompt(string question, string context)
    {
        return $@"Jesteś doradcą klienta w sklepie zoologicznym PetWorld. 
Twoja rola to generowanie pomocnych i profesjonalnych odpowiedzi na pytania klientów.

{context}

Pytanie klienta: {question}

Wytyczne:
1. Odwołuj się tylko do produktów z katalogu powyżej
2. Podawaj dokładne ceny i opisy
3. Rekomenduj produkty na podstawie pytania
4. Bądź uprzejmy i profesjonalny
5. Jeśli produktu nie ma, powiedz o tym
6. Linkuj produkty jako markdown: [Nazwa](/products/ID)

Udziel odpowiedzi:";
    }

    /// <summary>
    /// Builds the Writer agent prompt with feedback for refinement.
    /// </summary>
    private string BuildWriterPromptWithFeedback(string question, string context, string previousAttempt)
    {
        return $@"Jesteś doradcą klienta w sklepie zoologicznym PetWorld.

{context}

Pytanie klienta: {question}

Twoja poprzednia odpowiedź została zwrócona do poprawy. Spróbuj jeszcze raz, biorąc pod uwagę możliwe błędy:
- Czy produkty rzeczywiście istnieją w katalogu?
- Czy ceny są poprawne?
- Czy rekomendacje są sensowne?
- Czy linki do produktów mają format [Nazwa](/products/ID)?

Poprzednia próba: {previousAttempt}

Udziel ulepszonej odpowiedzi:";
    }

    /// <summary>
    /// Invokes the Critic agent to validate an answer.
    /// </summary>
    /// <param name="answer">The answer to validate.</param>
    /// <param name="context">Product catalog context.</param>
    /// <returns>Validation feedback or "APPROVED" if valid.</returns>
    private async Task<string> CallCriticAsync(string answer, string context)
    {
        var prompt = $@"Jesteś kontrolerem jakości w sklepie PetWorld. Twoja rola to weryfikacja odpowiedzi doradcy.

{context}

Odpowiedź do weryfikacji: '{answer}'

Sprawdź:
1. Czy wszystkie wymienione produkty rzeczywiście istnieją w katalogu?
2. Czy wszystkie ceny podane są poprawne?
3. Czy odpowiedź jest kompletna i profesjonalna?
4. Czy zrekomendowane produkty pasują do pytania?

Jeśli wszystko jest OK, odpowiedz DOKŁADNIE: APPROVED

Jeśli coś wymaga poprawy, wypisz krótko (max 2-3 zdania) co poprawić:";

        try
        {
            var messages = new List<Microsoft.Extensions.AI.ChatMessage> { new(Microsoft.Extensions.AI.ChatRole.User, prompt) };
            var response = await _chatClient.GetResponseAsync(messages);
            return response.Text ?? "Brak feedbacku";
        }
        catch (Exception ex)
        {
            return $"Błąd przy ocenie: {ex.Message}";
        }
    }
}