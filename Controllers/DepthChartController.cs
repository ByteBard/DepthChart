using DepthChart01.Controllers.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace DepthChart01.Controllers
{
    [Route("api/DepthChart")]
    [ApiController]
    public class DepthChartController : ControllerBase
    {
        private readonly IMongoCollection<Player> _playerCollection;
        private readonly IMongoCollection<Position> _positionCollection;
        private readonly IMongoCollection<Team> _teamCollection;
        private readonly IMongoCollection<TeamPosition> _teamPositionCollection;
        public DepthChartController()
        {
            var dbHost = "localhost";
            var dbName = "depth_chart";
            var connectionString = $"mongodb://{dbHost}:27017/{dbName}";

            var mongoUrl = MongoUrl.Create(connectionString);
            var mongoClient = new MongoClient(mongoUrl);
            var database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
            _playerCollection = database.GetCollection<Player>("player");
            _positionCollection = database.GetCollection<Position>("position");
            _teamCollection = database.GetCollection<Team>("team");
            _teamPositionCollection = database.GetCollection<TeamPosition>("team_position");

        }
    }
}
