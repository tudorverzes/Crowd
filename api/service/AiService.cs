using api.config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using Tokenizers.HuggingFace.Tokenizer;

namespace api.service;

public class AiService : IAiService, IDisposable
{
    private readonly InferenceSession? _session;
    private readonly Tokenizer? _tokenizer;
    private readonly ILogger<AiService> _logger;
    
    private bool _isInitialized;

    public AiService(IOptions<AiSettings> aiSettings, ILogger<AiService> logger)
    {
        _logger = logger;
        
        try
        {
            if (string.IsNullOrWhiteSpace(aiSettings.Value.ModelPath) || string.IsNullOrWhiteSpace(aiSettings.Value.TokenizerPath))
            {
                _logger.LogError("AiSettings configuration is missing ModelPath or TokenizerPath");
                return;
            }

            var modelPath = Path.Combine(Directory.GetCurrentDirectory(), aiSettings.Value.ModelPath);
            if (!File.Exists(modelPath))
            {
                _logger.LogError($"AI model file not found at: {modelPath}");
                return;
            }

            _session = new InferenceSession(modelPath);

            var tokenizerPath = Path.Combine(Directory.GetCurrentDirectory(), aiSettings.Value.TokenizerPath);
            if (!File.Exists(tokenizerPath))
            {
                _logger.LogError($"AI tokenizer file not found at: {tokenizerPath}");
                _session?.Dispose();
                return;
            }

            _tokenizer = Tokenizer.FromFile(tokenizerPath);
            _isInitialized = true;
            _logger.LogInformation("AI Service initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to initialize AI Service: {ex.Message}");
            _session?.Dispose();
        }
    }

    public float[] GenerateVector(string text)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(text)) return GenerateFallbackVector();
            
            if (!_isInitialized || _session == null || _tokenizer == null)
            {
                _logger.LogWarning("AI Service not initialized. Using fallback vector generation.");
                return GenerateFallbackVector();
            }
            
            var encodings = _tokenizer.Encode(text, addSpecialTokens: true, includeTypeIds: true, includeAttentionMask: true).First();
            
            var sequenceLength = encodings.Ids.Count;
            
            var inputIds = new DenseTensor<long>(new[] { 1, sequenceLength });
            var attentionMask = new DenseTensor<long>(new[] { 1, sequenceLength });
            var tokenTypeIds = new DenseTensor<long>(new[] { 1, sequenceLength });

            for (var i = 0; i < sequenceLength; i++)
            {
                inputIds[0, i] = encodings.Ids[i];
                attentionMask[0, i] = encodings.AttentionMask[i];
                tokenTypeIds[0, i] = encodings.TypeIds[i]; 
            }

            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor("input_ids", inputIds),
                NamedOnnxValue.CreateFromTensor("attention_mask", attentionMask),
                NamedOnnxValue.CreateFromTensor("token_type_ids", tokenTypeIds)
            };

            using var results = _session.Run(inputs);

            var outputTensor = results.First().AsTensor<float>();

            var vectorSize = outputTensor.Dimensions[2];
            var finalVector = new float[vectorSize];

            for (var i = 0; i < sequenceLength; i++)
            {
                for (int j = 0; j < vectorSize; j++)
                {
                    finalVector[j] += outputTensor[0, i, j];
                }
            }

            for (var j = 0; j < vectorSize; j++)
            {
                finalVector[j] /= sequenceLength;
            }

            return finalVector;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generating vector: {ex.Message}. Using fallback vector generation.");
            return GenerateFallbackVector();
        }
    }
    
    public float[] MergeVectorsWithWeights(List<(float[] Vector, float Weight)> components)
    {
        if (components.Count == 0) return new float[384];

        var vectorSize = components.First().Vector.Length;
        var finalVector = new float[vectorSize];
        float totalWeight = 0;

        foreach (var component in components)
        {
            totalWeight += component.Weight;
            for (int i = 0; i < vectorSize; i++)
            {
                finalVector[i] += component.Vector[i] * component.Weight;
            }
        }

        for (var i = 0; i < vectorSize; i++)
        {
            finalVector[i] /= totalWeight;
        }
        
        var magnitude = (float)Math.Sqrt(finalVector.Sum(x => x * x));
        if (!(magnitude > 0)) return finalVector;
        {
            for (var i = 0; i < vectorSize; i++)
            {
                finalVector[i] /= magnitude;
            }
        }

        return finalVector;
    }

    /// <summary>
    /// Generates a fallback vector when AI model is not available.
    /// Uses a simple hash-based approach to generate a consistent 384-dimensional vector.
    /// </summary>
    private float[] GenerateFallbackVector(string text = "")
    {
        const int vectorSize = 384;
        var vector = new float[vectorSize];

        if (string.IsNullOrWhiteSpace(text))
        {
            return vector;
        }

        var hash = text.GetHashCode();
        var random = new Random(hash);

        for (var i = 0; i < vectorSize; i++)
        {
            vector[i] = (float)(random.NextDouble() - 0.5) * 2;
        }

        return vector;
    }

    public void Dispose()
    {
        _session?.Dispose();
        _tokenizer?.Dispose();
    }
}