using DepthChart01.Controllers.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace DepthChart01.Controllers
{
    [Route("api/Player")]
    [ApiController]
    public class PlayerControler : ControllerBase
    {
        private readonly IMongoCollection<Player> _playerCollection;
        public PlayerControler()
        {
            var dbHost = "localhost";
            var dbName = "depth_chart";
            var connectionString = $"mongodb://{dbHost}:27017/{dbName}";

            var mongoUrl = MongoUrl.Create(connectionString);
            var mongoClient = new MongoClient(mongoUrl);
            var database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            _playerCollection = database.GetCollection<Player>("player");

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
