using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;

namespace Phoenix.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BotStatsController : ControllerBase
    {
        private readonly ILogger<BotStatsController> _logger;
        private readonly FirestoreDb _dbContext;

        public BotStatsController(ILogger<BotStatsController> logger, FirestoreDb firestoreDb)
        {
            _logger = logger;
            _dbContext = firestoreDb;
        }

        [HttpGet]
        public async Task<bool> Get()
        {
            CollectionReference collection = _dbContext.Collection("users");
            DocumentReference document = await collection.AddAsync(new { Name = new { First = "Ada", Last = "Lovelace" }, Born = 1815 });

            return true;
        }
    }
}
