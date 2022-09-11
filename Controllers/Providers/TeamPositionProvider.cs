using DepthChart01.Controllers.Models;
using DepthChart01.Controllers.Transport;
using Player = DepthChart01.Controllers.Transport.Player;
using PlayerPosition = DepthChart01.Controllers.Transport.PlayerPosition;
using TeamPosition = DepthChart01.Controllers.Transport.TeamPosition;

namespace DepthChart01.Controllers.Providers
{
    public class TeamPositionProvider
    {
        private TeamPosition _teamPosition;
        private Player[] _allPlayers;
        public TeamPosition TeamPosition { get => _teamPosition; set => _teamPosition = value; }
        public Player[] AllPlayers { get => _allPlayers; set => _allPlayers = value; }

        public TeamPositionProvider(TeamPosition teamPosition, Player[] players)
        {
            _teamPosition = teamPosition;
            _allPlayers = players;
        }

        //Add Player To Depth Chart
        public TeamPosition? UpsertTeamPosition(Player player, int targetOrder)
        {
            if (_teamPosition == null || _teamPosition.PlayerPositions == null || _teamPosition.PlayerPositions.Any()) return null;

            if (!_teamPosition.PlayerPositions.Any())
            {
                var newPosition = new PlayerPosition
                {
                    PlayerId = player.PlayerId,
                    Order = targetOrder
                };

                var playerPositionList = _teamPosition.PlayerPositions.ToList();
                playerPositionList.Add(newPosition);
                _teamPosition.PlayerPositions = playerPositionList.ToArray();

                return _teamPosition;
            }

            var orderedList = _teamPosition.PlayerPositions.OrderBy(x => x.Order).ToList();

                var samePlayerPosition = orderedList.FirstOrDefault(x => x.PlayerId == player.PlayerId);
                if (samePlayerPosition != null)
                {
                    // remove the duplicated position
                    orderedList.Remove(samePlayerPosition);

                    // update position for the player and add to the list
                    var newPosition = new PlayerPosition
                    {
                        PlayerId = player.PlayerId,
                        Order = targetOrder
                    };

                    for (int i = 0; i < orderedList?.Count; i++)
                    {
                        var currentOrder = orderedList[i].Order;
                        if (currentOrder < targetOrder) continue;
                        if (currentOrder == targetOrder) orderedList[i].Order++;
                        if (i != 0 && currentOrder == orderedList[i - 1].Order)
                        {
                            orderedList[i].Order++;
                        }
                    }

                    orderedList.Add(newPosition);
                    _teamPosition.PlayerPositions = orderedList.OrderBy(x => x.Order).ToArray();
                    return _teamPosition;

                }
                else
                {
                    // Add new postiion to the list
                    var newPosition = new PlayerPosition
                    {
                        PlayerId = player.PlayerId,
                        Order = targetOrder
                    };

                    for (int i = 0; i < orderedList?.Count; i++)
                    {
                        var currentOrder = orderedList[i].Order;
                        if (currentOrder < targetOrder) continue;
                        if (currentOrder == targetOrder) orderedList[i].Order++;
                        if (i != 0 && currentOrder > targetOrder && currentOrder == orderedList[i - 1].Order)
                        {
                            orderedList[i].Order++;
                        }
                    }

                    orderedList.Add(newPosition);
                    _teamPosition.PlayerPositions = orderedList.OrderBy(x => x.Order).ToArray();
                    return _teamPosition;
                }
        }

        //Remove Player From Depth Chart
        public static Player[] GetRemovedPlayerFromPosition(Player player, List<PlayerPosition> playerPositions)
        {
            if (playerPositions == null || playerPositions == null || playerPositions.Any()) return new Player[] { };
            var targetPlayerPosition = playerPositions.FirstOrDefault(x => x.PlayerId == player.PlayerId);
            if (targetPlayerPosition != null)
            {
                playerPositions.Remove(targetPlayerPosition);
            }
            return new Player[] { player };
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
