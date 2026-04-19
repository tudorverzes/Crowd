namespace api.service;

public interface IAiService
{
	float[] GenerateVector(string text);
	float[] MergeVectorsWithWeights(List<(float[] Vector, float Weight)> components);
}