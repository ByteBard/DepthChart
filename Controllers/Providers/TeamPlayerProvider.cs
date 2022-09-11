using DepthChart01.Controllers.Models;
using DepthChart01.Controllers.Transport;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace DepthChart01.Controllers.Providers
{
    public class TeamPlayerProvider
    {
        private Team _team;
        private readonly IMongoCollection<Player> _playerCollection;
        private readonly IMongoCollection<TeamPosition> _teamPositionCollection;
        public Team Team { get => _team; set => _team = value; }
        public TeamPlayerProvider(
            Team team, 
            IMongoCollection<Player> playerCollection, 
            IMongoCollection<TeamPosition> teamPositionCollection)
        {
            _team = team;
            _playerCollection = playerCollection;
            _teamPositionCollection = teamPositionCollection;
        }

        public Player GetPlayerByNameAndTeam(string name)
        {
            var playerFilterName = Builders<Player>.Filter.Eq(x => x.Name, name);
            var playerFilterTeam = Builders<Player>.Filter.Eq(x => x.CurrentTeamId, Team.TeamId);
            var filter = Builders<Player>.Filter.And(playerFilterName, playerFilterTeam);
            var player = _playerCollection.Find(filter).FirstOrDefault();
            return player;
        }

        public async Task<ActionResult<IEnumerable<Player>>> GetAllTeamPlayers()
        {
            var positionFilter = Builders<TeamPosition>.Filter.Eq(x => x.TeamId, _team.TeamId);
            var positions = await _teamPositionCollection.Find(positionFilter).ToListAsync();
            var playerIds = positions.SelectMany(x => x.PlayerPositions).Select(x => x.PlayerId).ToArray();
            var playerFilter = Builders<Player>.Filter.In(x => x.PlayerId, playerIds);
            var result = await _playerCollection.Find(playerFilter).ToListAsync();
            return result;
        }

    }
}
