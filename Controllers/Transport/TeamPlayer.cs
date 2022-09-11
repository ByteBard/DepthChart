using DepthChart01.Controllers.Models;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.RegularExpressions;

namespace DepthChart01.Controllers.Transport
{
    [Serializable]
    public class TeamPlayer
    {
        public string PlayerId { get; set; }

        public string Name { get; set; }

        public string Number { get; set; }
        public string Position { get; set; }
        public int? PositionDepth { get; set; }

        public static TeamPlayer FromModel(Player player, Team team, Dictionary<string, int> positionDic, TeamPosition position)
        {
            var number = string.Empty;
            var splits = Regex.Split(player.Name, @"(?<!^)(?=[A-Z])");
            var name = string.Join(" ", splits);
            var teamMatched = player.TeamPlayerNumbers.FirstOrDefault(x => x.TeamId == team.TeamId);
            var positionDepth = (positionDic != null && positionDic.ContainsKey(player.PlayerId)) ? positionDic[player.PlayerId] : (int?) null;
            if (teamMatched != null) number = teamMatched.Number;
            return new TeamPlayer
            {
                PlayerId = player.PlayerId,
                Name = name,
                Number = number,
                Position = position.Name,
                PositionDepth = positionDepth
            };
        }
    }
}
