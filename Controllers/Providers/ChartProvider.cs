using DepthChart01.Controllers.Models;
using DepthChart01.Controllers.Transport;
using MongoDB.Driver;

namespace DepthChart01.Controllers.Providers
{
    public class ChartProvider
    {
        private Team _team;
        private readonly IMongoCollection<TeamPosition> _teamPositionCollection;
        private readonly TeamPlayerProvider _teamPlayerProvider;
        public ChartProvider(Team team,
            IMongoCollection<TeamPosition> teamPositionCollection,
            TeamPlayerProvider teamPlayerProvider)
        { 
            _team = team;
            _teamPositionCollection = teamPositionCollection;
            _teamPlayerProvider = teamPlayerProvider;
        }

        public IEnumerable<TeamPlayerPostition> GetAllPositionsFromModel()
        {
            var players = _teamPlayerProvider.GetAllPlayers().Result;
            var positionFilter = Builders<TeamPosition>.Filter.Eq(x => x.TeamId, _team.TeamId);
            var positions = _teamPositionCollection.Find(positionFilter).ToList();
            var playerDic = players.Value?.ToDictionary(x => x.PlayerId, x => x);
            return GetAllPositions(positions, playerDic);
        }

        public IEnumerable<TeamPlayerPostition> GetAllPositions(List<TeamPosition>? teamPositions, Dictionary<string, TeamPlayer>? teamPlayerDic)
        {
            if (teamPlayerDic != null && teamPositions != null)
            {
                var playerPositions = teamPositions.SelectMany(x => x.PlayerPositions).Select(x => TeamPlayerPostition.FromModel(x, teamPlayerDic)).ToArray();
                return playerPositions;
            }

            return new TeamPlayerPostition[] { };
        }
    }
}
