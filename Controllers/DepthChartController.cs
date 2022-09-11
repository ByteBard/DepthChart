using DepthChart01.Controllers.Models;
using DepthChart01.Controllers.Providers;
using DepthChart01.Controllers.Transport;
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
            _teamCollection = database.GetCollection<Team>("team");
            _teamPositionCollection = database.GetCollection<TeamPosition>("team_position");

        }

        [HttpGet("GetFullDepthChart")]
        public TeamPlayer[] GetFullDepthChart()
        {
            var teamfiler = Builders<Team>.Filter.Eq(x => x.Name, _defalutTeamName);
            var team = _teamCollection.Find(teamfiler).FirstOrDefaultAsync().Result;
            var teamPlayerPro = new TeamPlayerProvider(team, _playerCollection, _teamPositionCollection);
            var chartProvider = new ChartProvider(team, _teamPositionCollection, teamPlayerPro);
            return chartProvider.GetFullDepthChart();
        }


        [HttpGet("GetBackups/{position}/{name}")]
        public TeamPlayer[] GetBackups(string position, string name)
        {
            if (string.IsNullOrWhiteSpace(position) || string.IsNullOrWhiteSpace(name)) return new TeamPlayer[] {};
            var teamfiler = Builders<Team>.Filter.Eq(x => x.Name, _defalutTeamName);
            var team = _teamCollection.Find(teamfiler).FirstOrDefaultAsync().Result;
            if (team != null)
            {
                var teamPlayerPro = new TeamPlayerProvider(team, _playerCollection, _teamPositionCollection);
                var positionTeamFilter = Builders<TeamPosition>.Filter.Eq(x => x.TeamId, team.TeamId);
                var positionNameFilter = Builders<TeamPosition>.Filter.Eq(x => x.Name, position);
                var positionFilter = Builders<TeamPosition>.Filter.And(positionTeamFilter, positionNameFilter);
                var teamPosition = _teamPositionCollection.Find(positionFilter).FirstOrDefaultAsync().Result;
                var teamPositionDic = teamPosition.PlayerPositions.ToDictionary(x => x.PlayerId, x => x.Order);
                var playerTeamFilter = Builders<Player>.Filter.Eq(x => x.CurrentTeamId, team.TeamId);
                var playerFilter = Builders<Player>.Filter.And(playerTeamFilter, playerTeamFilter);
                var allPlayers = _playerCollection.Find(playerFilter).ToList();
                var targetPlayer = allPlayers.FirstOrDefault(x => x.Name == name);
                if (targetPlayer != null)
                {
                    var depthOperPro = new DepthOperationProvider(teamPosition, allPlayers.ToArray());
                    var backUps = depthOperPro.GetBackupPlayers(targetPlayer);
                    var backUpTeamPlayers = backUps.Select(x => TeamPlayer.FromModel(x, team, teamPositionDic, teamPosition)).ToArray();
                    return backUpTeamPlayers;
                }
            }

            return new TeamPlayer[] { };
        }


        [HttpPost("AddPlayerToDepthChart/{position}/{name}/{depth?}")]
        public async Task<ActionResult> AddPlayerToDepthChart(string position, string name, int? depth = null)
        {
            if (string.IsNullOrWhiteSpace(position) || string.IsNullOrWhiteSpace(name)) return Ok("empty name or position");
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
                var player = teamPlayerPro.GetPlayerByNameAndTeam(name);
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
