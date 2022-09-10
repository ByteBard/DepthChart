using DepthChart01.Controllers.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace DepthChart01.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepthChartController : ControllerBase
    {
        private readonly IMongoCollection<Player> _playerCollection;
        private readonly IMongoCollection<Position> _positionCollection;
        private readonly IMongoCollection<Team> _teamCollection;
        private readonly IMongoCollection<TeamPosition> _teamPositionCollection;
        public DepthChartController()
        {
            var dbHost = "localhost";
            var dbName = "depth_chart";
            var connectionString = $"mongodb://{dbHost}:27017/{dbName}";

            var mongoUrl = MongoUrl.Create(connectionString);
            var mongoClient = new MongoClient(mongoUrl);
            var database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            _playerCollection = database.GetCollection<Player>("player");
            _positionCollection = database.GetCollection<Position>("position");
            _teamCollection = database.GetCollection<Team>("team");
            _teamPositionCollection = database.GetCollection<TeamPosition>("team_position");

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayers()
        {
            return await _playerCollection.Find(Builders<Player>.Filter.Empty).ToListAsync();
        }

        [HttpGet("{playerId}")]
        public async Task<ActionResult<IEnumerable<Player>>> GetPlayerById(string playerId)
        {
            var filerDefinition = Builders<Player>.Filter.Eq(x => x.PlayerId, playerId);
            return await _playerCollection.Find(filerDefinition).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> CreatePlayer(Player player)
        {
            await _playerCollection.InsertOneAsync(player);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Update(Player player) 
        {
            var filerDefinition = Builders<Player>.Filter.Eq(x => x.PlayerId, player.PlayerId);
            await _playerCollection.ReplaceOneAsync(filerDefinition, player);
            return Ok();
        }

        [HttpDelete("{playerId}")]
        public async Task<ActionResult> Delete(string playerId)
        {
            var filerDefinition = Builders<Player>.Filter.Eq(x => x.PlayerId, playerId);
            await _playerCollection.DeleteOneAsync(filerDefinition);
            return Ok();
        }
    }
}
