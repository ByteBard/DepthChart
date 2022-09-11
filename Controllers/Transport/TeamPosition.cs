

namespace DepthChart01.Controllers.Transport
{
    public class TeamPosition
    {
        public string TeamPositionId { get; set; }

        public string TeamId { get; set; }
        
        public string Name { get; set; }
        
        public PlayerPosition[] PlayerPositions { get; set; }

        public static TeamPosition FromModel(Models.TeamPosition teamPosition) => new TeamPosition
        {
            TeamPositionId = teamPosition.TeamPositionId,
            TeamId = teamPosition.TeamId,
            Name = teamPosition.Name,
            PlayerPositions = teamPosition.PlayerPositions.Select(PlayerPosition.FromModel).ToArray()
        };

        public static Models.TeamPosition ToModel(TeamPosition teamPosition) => new Models.TeamPosition
        {
            TeamPositionId = teamPosition.TeamPositionId,
            TeamId = teamPosition.TeamId,
            Name = teamPosition.Name,
            PlayerPositions = teamPosition.PlayerPositions.Select(PlayerPosition.ToModel).ToArray()
        };
    }
}
