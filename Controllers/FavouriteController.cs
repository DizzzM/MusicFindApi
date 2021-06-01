using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MusicFindAPI.Clients;
using MusicFindAPI.Models;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace MusicFindAPI.Controllers
{
    [ApiController]
    [Route("api/favourite")]
    public class FavouriteController : ControllerBase
    {
        private readonly ILogger<SearchController> _logger;
        private readonly LyricsClient _lyrics;
        private readonly DatabaseClient _database;
        private readonly SpotifyClient _spotify;
        public FavouriteController(ILogger<SearchController> logger, SpotifyClient spotify, LyricsClient lyrics, DatabaseClient database)
        {
            _logger = logger;
            _spotify = spotify;
            _lyrics = lyrics;
            _database = database;
            DatabaseClient.InitializeClient();
        }

        [HttpPost]
        [Route("add")]
        public async Task<HttpStatusCode> AddToFavourite([FromQuery] string ChatId, string TrackId)
        {
            return await DatabaseClient.AddToFavourite(ChatId, TrackId);
        }

        [HttpDelete]
        [Route("delete")]
        public async Task<HttpStatusCode> DeleteFromFavourite([FromQuery] string ChatId, string TrackId)
        {
            return await DatabaseClient.DeleteFromFavourite(ChatId, TrackId);
        }

        [HttpGet]
        [Route("get")]
        public async Task<List<TrackInfo>> GetFavouriteList([FromQuery] string ChatId)
        {
            return await DatabaseClient.GetFavouriteList(ChatId);
        }
    }
}
