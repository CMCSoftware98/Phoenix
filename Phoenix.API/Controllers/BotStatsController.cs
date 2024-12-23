using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;

namespace Phoenix.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BotStatsController : ControllerBase
    {
        private readonly ILogger<BotStatsController> _logger;

        public BotStatsController(ILogger<BotStatsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<bool> Get()
        {
            FirestoreDb firestoreDb = await FirestoreDb.CreateAsync("phoenix-430818");

            CollectionReference collection = firestoreDb.Collection("users");
            DocumentReference document = await collection.AddAsync(new { Name = new { First = "Ada", Last = "Lovelace" }, Born = 1815 });

            return true;
        }
    }
}
