using DepthChart01.Controllers.Models;
using DepthChart01.Controllers.Providers;
using DepthChart01.Controllers.Transport;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Xml.Linq;

namespace DepthChart01.Controllers
{
    [Route("api/TeamPosition")]
    [ApiController]
    public class TeamPositionController : ControllerBase
    {
        private readonly string _defalutTeamName = "TBB";
        private readonly IMongoCollection<TeamPosition> _teamPositionCollection;
        private readonly IMongoCollection<Player> _playerCollection;
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
            _playerCollection = database.GetCollection<Player>("player");
        }

        [HttpPost("RemovePlayerFromDepthChart/{position}/{name}")]
        public async Task<TeamPlayer[]> RemovePlayerFromDepthChartAsync(string position, string name)
        {
            var filerDefinition = Builders<Team>.Filter.Eq(x => x.Name, _defalutTeamName);
            var team = await _teamCollection.Find(filerDefinition).FirstOrDefaultAsync();
            if (team != null)
            {
                var positionTeamFilter = Builders<TeamPosition>.Filter.Eq(x => x.TeamId, team.TeamId);
                var positionNameFilter = Builders<TeamPosition>.Filter.Eq(x => x.Name, position);
                var positionFilter = Builders<TeamPosition>.Filter.And(positionTeamFilter, positionNameFilter);
                var existingTeamPosition = _teamPositionCollection.Find(positionFilter).FirstOrDefaultAsync().Result;
                if (existingTeamPosition == null) return new TeamPlayer[] { };
                var existingPlayerPositions = existingTeamPosition.PlayerPositions.ToList();
                if (existingPlayerPositions.Any())
                {
                    var positionDic = existingPlayerPositions.ToDictionary(x => x.PlayerId, x => x.Order);
                    var playerNameFilter = Builders<Player>.Filter.Eq(x => x.Name, name);
                    var playerTeamFilter = Builders<Player>.Filter.Eq(x => x.CurrentTeamId, team.TeamId);
                    var playerFilter = Builders<Player>.Filter.And(playerNameFilter, playerTeamFilter);
                    var player = _playerCollection.Find(playerFilter).FirstOrDefault();
                    var deletedPlayers = DepthOperationProvider.GetRemovedPlayerFromPosition(player, existingPlayerPositions);

                    //position updated, then commit
                    existingTeamPosition.PlayerPositions = existingPlayerPositions.ToArray();
                    await _teamPositionCollection.ReplaceOneAsync(positionFilter, existingTeamPosition);
                    if (deletedPlayers != null && deletedPlayers.Any()) return new TeamPlayer[] { TeamPlayer.FromModel(deletedPlayers[0], team, positionDic, existingTeamPosition) };
                }
            }

            return new TeamPlayer[] { };
        }

        [HttpPost("CreateTeamPosition/{position}")]
        public async Task<ActionResult> CreateTeamPosition(string position)
        {

            var filerDefinition = Builders<Team>.Filter.Eq(x => x.Name, _defalutTeamName);
            var team = await _teamCollection.Find(filerDefinition).FirstOrDefaultAsync();
            if (team != null)
            {

                var positionTeamFilter = Builders<TeamPosition>.Filter.Eq(x => x.TeamId, team.TeamId);
                var positionNameFilter = Builders<TeamPosition>.Filter.Eq(x => x.Name, position);
                var positionFilter = Builders<TeamPosition>.Filter.And(positionTeamFilter, positionNameFilter);
                var existingTeamPosition = _teamPositionCollection.Find(positionFilter).FirstOrDefaultAsync().Result;
                if (existingTeamPosition != null) return Ok("postition already exist!");

                var teamPosition = new TeamPosition
                {
                    TeamId = team.TeamId,
                    Name = position,
                    PlayerPositions = new PlayerPosition[] { }
                };
                await _teamPositionCollection.InsertOneAsync(teamPosition);
            }

            return Ok("Create Team Position ok!");
        }
    }
}
