using Google.Cloud.Firestore;

namespace Phoenix.API.Service
{
    public class BotStatsService(ILogger<BotStatsService> logger, FirestoreDb firestoreDb)
    {
        private readonly ILogger<BotStatsService> _logger = logger;
        private readonly FirestoreDb _firestoreDb = firestoreDb;
    }
}
