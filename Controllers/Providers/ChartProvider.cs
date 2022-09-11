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

        public TeamPlayer[] GetFullDepthChart()
        {
            var res = new List<TeamPlayer>();
            var players = _teamPlayerProvider.GetAllTeamPlayers().Result.Value;
            var positionFilter = Builders<TeamPosition>.Filter.Eq(x => x.TeamId, _team.TeamId);
            var positions = _teamPositionCollection.Find(positionFilter).ToList();
            var playerDic = players?.ToDictionary(x => x.PlayerId, x => x);
            if (playerDic != null && positions != null)
            {
                foreach (TeamPosition position in positions)
                {
                    var teamPositionDic = position.PlayerPositions.ToDictionary(x => x.PlayerId, x => x.Order);
                    var positionPlayers = players?.Where(x => teamPositionDic.ContainsKey(x.PlayerId)).ToArray();
                    if (positionPlayers != null && positionPlayers.Any())
                    {
                        foreach (Player player in positionPlayers)
                        {
                            var teamPlayer = TeamPlayer.FromModel(player, _team, teamPositionDic, position);
                            res.Add(teamPlayer);
                        }
                    }
                }
            }

            return res.OrderBy(x => x.Position).ThenBy(x => x.PositionDepth).ToArray();
        }
    }
}
