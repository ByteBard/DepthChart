using Player = DepthChart01.Controllers.Models.Player;
using PlayerPosition = DepthChart01.Controllers.Models.PlayerPosition;
using TeamPosition = DepthChart01.Controllers.Models.TeamPosition;

namespace DepthChart01.Controllers.Providers
{
    public class DepthOperationProvider
    {
        private TeamPosition _teamPosition;
        private Player[] _allPlayers;
        public TeamPosition TeamPosition { get => _teamPosition; set => _teamPosition = value; }
        public Player[] AllPlayers { get => _allPlayers; set => _allPlayers = value; }

        public DepthOperationProvider(TeamPosition teamPosition, Player[] players)
        {
            _teamPosition = teamPosition;
            _allPlayers = players;
        }

        //Add TeamPlayer To Depth Chart
        public TeamPosition? AddPlayerToDepthChart(Player player, int? targetOrder = null)
        {
            if (
                _teamPosition == null || 
                _teamPosition.PlayerPositions == null || 
                //TeamPlayer already exist in the Position! Skip...
                _teamPosition.PlayerPositions.Any(x => x.PlayerId == player.PlayerId)
                ) return null;

            if (!targetOrder.HasValue)
            {
                if (!_teamPosition.PlayerPositions.Any())
                {
                    //if nothing, it would be th first
                    var newPosition = new PlayerPosition
                    {
                        PlayerId = player.PlayerId,
                        Order = 1
                    };

                    var playerPositionList = _teamPosition.PlayerPositions.ToList();
                    playerPositionList.Add(newPosition);
                    _teamPosition.PlayerPositions = playerPositionList.ToArray();

                    return _teamPosition;
                }
                else 
                {
                    //if not empty, the order should be the last order + 1
                    var lastPosition = _teamPosition.PlayerPositions.OrderBy(x => x.Order).Last();
                    var newPosition = new PlayerPosition
                    {
                        PlayerId = player.PlayerId,
                        Order = lastPosition.Order + 1
                    };

                    var playerPositionList = _teamPosition.PlayerPositions.ToList();
                    playerPositionList.Add(newPosition);
                    _teamPosition.PlayerPositions = playerPositionList.ToArray();

                    return _teamPosition;
                }
            }
            else 
            {
                //if positions is empty
                if (!_teamPosition.PlayerPositions.Any())
                {
                    var newPosition = new PlayerPosition
                    {
                        PlayerId = player.PlayerId,
                        Order = targetOrder.Value
                    };

                    var playerPositionList = _teamPosition.PlayerPositions.ToList();
                    playerPositionList.Add(newPosition);
                    _teamPosition.PlayerPositions = playerPositionList.ToArray();

                    return _teamPosition;
                }
                else 
                {
                    //if positions already has some data 
                    var orderedList = _teamPosition.PlayerPositions.OrderBy(x => x.Order).ToList();
                    var newPosition = new PlayerPosition
                    {
                        PlayerId = player.PlayerId,
                        Order = targetOrder.Value
                    };

                    for (int i = 0; i < orderedList.Count; i++)
                    {
                        var currentOrder = orderedList[i].Order;
                        if (currentOrder < targetOrder) continue;
                        if (currentOrder == targetOrder)
                        {
                            orderedList[i].Order++;
                            continue;
                        }

                        if (i != 0 && currentOrder == orderedList[i - 1].Order)
                        {
                            orderedList[i].Order++;
                        }
                    }

                    orderedList.Add(newPosition);
                    _teamPosition.PlayerPositions = orderedList.OrderBy(x => x.Order).ToArray();
                    return _teamPosition;
                }
            }
        }

        //Remove TeamPlayer From Depth Chart
        public static Player[] GetRemovedPlayerFromPosition(Player player, List<PlayerPosition> playerPositions)
        {
            if (player == null || playerPositions == null || !playerPositions.Any()) return new Player[] { };
            var targetPlayerPosition = playerPositions.FirstOrDefault(x => x.PlayerId == player.PlayerId);
            if (targetPlayerPosition != null)
            {
                playerPositions.Remove(targetPlayerPosition);
                return new Player[] { player };
            }
            return new Player[] { };
        }

        // Get Backups for a player
        public Player[] GetBackupPlayers(Player player)
        {
            if (
                player == null ||
                _teamPosition == null ||
                _teamPosition.PlayerPositions == null ||
                !_teamPosition.PlayerPositions.Any() ||
                !_teamPosition.PlayerPositions.Any(x => x.PlayerId == player.PlayerId)
                ) return new Player[] { };

            var matchedPosition = _teamPosition.PlayerPositions.FirstOrDefault(x => x.PlayerId == player.PlayerId);
            if(matchedPosition == null) return new Player[] { };
            var playerIds = _teamPosition.PlayerPositions.Where(x => x.Order > matchedPosition.Order).Select(x => x.PlayerId).ToArray();
            return _allPlayers.Where(x => playerIds.Contains(x.PlayerId)).ToArray();
        }

    }
}
