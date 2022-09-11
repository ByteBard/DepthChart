using DepthChart01.Controllers.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace DepthChart01.Controllers
{
    [Route("api/Team")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly string _defalutTeamName = "TBB";
        private readonly IMongoCollection<Team> _teamCollection;
        public TeamController()
        {
            var dbHost = "localhost";
            var dbName = "depth_chart";
            var connectionString = $"mongodb://{dbHost}:27017/{dbName}";

            var mongoUrl = MongoUrl.Create(connectionString);
            var mongoClient = new MongoClient(mongoUrl);
            var database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            _teamCollection = database.GetCollection<Team>("team");

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Team>>> GetTeams()
        {
            return await _teamCollection.Find(Builders<Team>.Filter.Empty).ToListAsync();
        }

        [HttpGet("{name}")]
        public async Task<ActionResult<Team>> GetTeamByName(string name)
        {
            var filerDefinition = Builders<Team>.Filter.Eq(x => x.Name, name);
            return await _teamCollection.Find(filerDefinition).FirstOrDefaultAsync();
        }

        [HttpPost]
        public async Task<ActionResult> CreateTeam(Team team)
        {
            await _teamCollection.InsertOneAsync(team);
            return Ok();
        }
    }
}
