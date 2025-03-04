using ContosoSuitesWebAPI.Entities;

namespace ContosoSuitesWebAPI.Services
{
    public interface IVectorizationService
    {
        Task<float[]> GetEmbeddings(string text); 

        Task<List<VectorSearchResult>> ExecuteVectorSearch(float[] queryVector, int max_results = 0, double minimum_similarity_score = 0.8);
    }
}
