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

        public Player GetPlayerByName(string name)
        {
            var playerFilter = Builders<Player>.Filter.Eq(x => x.Name, name);
            var player = _playerCollection.Find(playerFilter).FirstOrDefault();
            return player;
        }

        public async Task<ActionResult<IEnumerable<TeamPlayer>>> GetAllPlayers()
        {
            var positionFilter = Builders<TeamPosition>.Filter.Eq(x => x.TeamId, _team.TeamId);
            var positions = await _teamPositionCollection.Find(positionFilter).ToListAsync();
            var playerIds = positions.SelectMany(x => x.PlayerPositions).Select(x => x.PlayerId).ToArray();
            var playerFilter = Builders<Player>.Filter.In(x => x.PlayerId, playerIds);
            var players = await _playerCollection.Find(playerFilter).ToListAsync();
            var result = players.Select(x => TeamPlayer.FromModel(x, _team)).ToArray();
            return result;
        }

    }
}
