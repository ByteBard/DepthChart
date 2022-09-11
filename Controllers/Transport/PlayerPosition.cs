
namespace DepthChart01.Controllers.Transport
{
    public class PlayerPosition
    {
        public string PlayerId { get; set; }

        public int Order { get; set; }

        public static PlayerPosition FromModel(Models.PlayerPosition playerPosition) => new PlayerPosition
        {
            PlayerId = playerPosition.PlayerId,
            Order = playerPosition.Order,
        };

        public static Models.PlayerPosition ToModel(PlayerPosition playerPosition) => new Models.PlayerPosition
        {
            PlayerId = playerPosition.PlayerId,
            Order = playerPosition.Order
        };
    }
}
