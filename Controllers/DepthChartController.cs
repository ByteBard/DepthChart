using DepthChart01.Controllers.Models;
using DepthChart01.Controllers.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Xml.Linq;

namespace DepthChart01.Controllers
{
    [Route("api/DepthChart")]
    [ApiController]
    public class DepthChartController : ControllerBase
    {
        private readonly string _defalutTeamName = "TBB";
        private readonly IMongoCollection<Player> _playerCollection;
        private readonly IMongoCollection<PlayerPosition> _positionCollection;
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
            _positionCollection = database.GetCollection<PlayerPosition>("position");
            _teamCollection = database.GetCollection<Team>("team");
            _teamPositionCollection = database.GetCollection<TeamPosition>("team_position");

        }

        [HttpPost("AddPlayerToDepthChart/{position}/{name}/{depth}")]
        public async Task<ActionResult> AddPlayerToDepthChart(string position, string name, int? depth)
        {
            var okMsg = "Add Player To Depth Chart OK!";
            var faliMsg = "Add Player To Depth Chart Fail!";
            var teamfiler = Builders<Team>.Filter.Eq(x => x.Name, _defalutTeamName);
            var team = _teamCollection.Find(teamfiler).FirstOrDefaultAsync().Result;
            if (team != null) 
            {
                var teamPlayerPro = new TeamPlayerProvider(team, _playerCollection, _teamPositionCollection);
                var positionTeamFilter = Builders<TeamPosition>.Filter.Eq(x => x.TeamId, team.TeamId);
                var positionNameFilter = Builders<TeamPosition>.Filter.Eq(x => x.Name, position);
                var positionFilter = Builders<TeamPosition>.Filter.And(positionTeamFilter, positionNameFilter);
                var teamPosition = _teamPositionCollection.Find(positionFilter).FirstOrDefaultAsync().Result;
                var player = teamPlayerPro.GetPlayerByName(name);
                if (player != null) 
                {
                    var depthOperPro = new DepthOperationProvider(teamPosition, new Player[] { });
                    var updatedTeamPosition = depthOperPro.AddPlayerToDepthChart(player, depth);
                    if (updatedTeamPosition != null) 
                    {
                        var filerDefinition = Builders<TeamPosition>.Filter.Eq(x => x.TeamPositionId, updatedTeamPosition.TeamPositionId);
                        await _teamPositionCollection.ReplaceOneAsync(filerDefinition, updatedTeamPosition);
                        return Ok(okMsg);
                    }
                }
            }
            return Ok(faliMsg);
        }
    }
}
