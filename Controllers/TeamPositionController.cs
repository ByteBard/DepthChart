using DepthChart01.Controllers.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace DepthChart01.Controllers
{
    [Route("api/TeamPosition")]
    [ApiController]
    public class TeamPositionController : ControllerBase
    {
        private readonly IMongoCollection<TeamPosition> _teamPositionCollection;
        private readonly IMongoCollection<Team> _teamCollection;
        public TeamPositionController()
        {
            var dbHost = "localhost";
            var dbName = "depth_chart";
            var connectionString = $"mongodb://{dbHost}:27017/{dbName}";

            var mongoUrl = MongoUrl.Create(connectionString);
            var mongoClient = new MongoClient(mongoUrl);
            var database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            _teamCollection = database.GetCollection<Team>("team");
            _teamPositionCollection = database.GetCollection<TeamPosition>("team_position");

        }

        [HttpPost("AddPlayerToDepthChart/{player}/{position}")]
        public async Task<ActionResult> AddPlayerToDepthChart(string player, int position)
        {
            //await _teamCollection.InsertOneAsync(team);
            return Ok();
        }
    }
}
