
namespace DepthChart01.Controllers.Transport
{
    public class Player
    {
        public string PlayerId { get; set; }

        public string Name { get; set; }

        public static Player FromModel(Models.Player player) => new Player()
        {
            PlayerId = player.PlayerId,
            Name = player.Name,
        };

        public static Models.Player ToModel(Player player) => new Models.Player
        {
            PlayerId = player.PlayerId,
            Name = player.Name
        };
    }

}
