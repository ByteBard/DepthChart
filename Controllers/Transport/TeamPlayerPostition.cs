//using DepthChart01.Controllers.Models;

//namespace DepthChart01.Controllers.Transport
//{
//    public class TeamPlayerPostition
//    {
//        public TeamPlayer TeamPlayer { get; set; }
//        public int Order { get; set; }

//        public static TeamPlayerPostition FromModel(PlayerPosition postition, Dictionary<string, TeamPlayer> teamPlayerDic)
//        {
//            if (teamPlayerDic != null && teamPlayerDic.ContainsKey(postition.PlayerId)) 
//            {
//                var teamPlayer = teamPlayerDic[postition.PlayerId];
//                return new TeamPlayerPostition
//                {
//                    TeamPlayer = teamPlayer,
//                    Order = postition.Order
//                };
//            }
//            return null;
//        }
//    }
//}
