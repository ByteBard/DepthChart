using DepthChart01.Controllers.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace DepthChart01.Controllers
{
    [Route("api/TeamPlayer")]
    [ApiController]
    public class PlayerControler : ControllerBase
    {
        private readonly IMongoCollection<Player> _playerCollection;
        private readonly IMongoCollection<Team> _teamCollection;
        private readonly string _defalutTeamName = "TBB";
        public PlayerControler()
        {
            var dbHost = "localhost";
            var dbName = "depth_chart";
            var connectionString = $"mongodb://{dbHost}:27017/{dbName}";

            var mongoUrl = MongoUrl.Create(connectionString);
            var mongoClient = new MongoClient(mongoUrl);
            var database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            _playerCollection = database.GetCollection<Player>("player");
            _teamCollection = database.GetCollection<Team>("team");
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

        [HttpPost("CreatePlayer/{name}/{number}")]
        public async Task<ActionResult> CreatePlayer(string name, string number)
        {
            Regex regex = new Regex(@"^\d+$");
            var result = regex.IsMatch(number);
            if (!result || string.IsNullOrWhiteSpace(name)) return Ok("name is empty or number is not digit");

            var filerDefinition = Builders<Team>.Filter.Eq(x => x.Name, _defalutTeamName);
            var team = await _teamCollection.Find(filerDefinition).FirstOrDefaultAsync();
            var playerNameFilter = Builders<Player>.Filter.Eq(x => x.Name, name);
            var players = _playerCollection.Find(playerNameFilter).ToList();
            if(players.Any(x => x.TeamPlayerNumbers.Any(x => x.TeamId == team.TeamId))) return Ok("Player already exist!");
            var teamPlayerNumber = new TeamPlayerNumber
            {
                TeamId = team.TeamId,
                Number = number
            };

            var newPlayer = new Player()
            {
                Name = name,
                TeamPlayerNumbers = new TeamPlayerNumber[] { teamPlayerNumber }
            };

            await _playerCollection.InsertOneAsync(newPlayer);
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
