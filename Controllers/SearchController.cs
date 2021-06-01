using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MusicFindAPI.Clients;
using MusicFindAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicFindAPI.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        private readonly ILogger<SearchController> _logger;
        private readonly LyricsClient _lyrics;
        private readonly DatabaseClient _database;
        private readonly SpotifyClient _spotify;
        public SearchController(ILogger<SearchController> logger, SpotifyClient spotify, LyricsClient lyrics, DatabaseClient database)
        {
            _logger = logger;
            _spotify = spotify;
            _lyrics = lyrics;
            _database = database;
            DatabaseClient.InitializeClient();
        }
        
        [HttpGet]
        [Route("tracks")]
        public async Task<List<TrackInfo>> GetTracks([FromQuery] string value)
        {
            return await SpotifyClient.TrackSearch(value);
        }

        [HttpPost]
        [Route("lyrics")]
        public async Task<TrackInfo> GetLyrics([FromBody] TrackInfo Track)
        {
            return await LyricsClient.LyricsSearch(Track);
        }

        [HttpGet]
        [Route("related")]
        public async Task<List<TrackInfo>> GetRelatedArtist([FromQuery] string id)
        {
            return await SpotifyClient.SpotifyRelatedArtist(id);
        }
    }
}
