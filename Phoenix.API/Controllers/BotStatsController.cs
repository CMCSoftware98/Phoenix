using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using Phoenix.Shared.Models;

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
        public async Task<ActionResult> Get()
        {
            try
            {
                CollectionReference collection = _dbContext.Collection("users");
                DocumentReference document = await collection.AddAsync(new { Name = new { First = "Ada", Last = "Lovelace" }, Born = 1815 });

                return Ok(true);
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }



        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] MatchResult matchResult)
        {
            try
            {
                CollectionReference collection = _dbContext.Collection("MatchResults");


                Query query = collection.WhereEqualTo("Url", matchResult.Url);

                QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

                if(querySnapshot.Documents.Any())
                {
                    return Ok("Document already exists");
                }

                DocumentReference document = await collection.AddAsync(matchResult);

                return Ok();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }
    }
}
