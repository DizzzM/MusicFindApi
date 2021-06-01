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
    [Route("api/playlists")]
    public class PlaylistsController : ControllerBase
    {
        private readonly ILogger<PlaylistsController> _logger;
        private readonly LyricsClient _lyrics;
        private readonly DatabaseClient _database;
        private readonly SpotifyClient _spotify;
        public PlaylistsController(ILogger<PlaylistsController> logger, SpotifyClient spotify, LyricsClient lyrics, DatabaseClient database)
        {
            _logger = logger;
            _spotify = spotify;
            _lyrics = lyrics;
            _database = database;
            DatabaseClient.InitializeClient();
        }

        [HttpPost]
        [Route("add/track")]
        public async Task<HttpStatusCode>  AddToPlaylist([FromQuery] string ChatId, string Name, string TrackId)
        {
            return await DatabaseClient.AddToPlaylist(ChatId, Name, TrackId);
        }
        [HttpDelete]
        [Route("delete/track")]
        public async Task<HttpStatusCode> DeleteFromPlaylist([FromQuery] string ChatId, string Name, string TrackId)
        {
            return await DatabaseClient.DeleteFromPlaylist(ChatId, Name, TrackId);
        }
        [HttpGet]
        [Route("get/playlist")]
        public async Task<List<TrackInfo>> GetPlaylist([FromQuery] string ChatId, string Name)
        {
            return await DatabaseClient.GetPlaylist(ChatId, Name);
        }
        [HttpPost]
        [Route("add/playlist")]
        public async Task<HttpStatusCode> AddPlaylist([FromQuery] string ChatId, string Name)
        {
            return await DatabaseClient.AddPlaylist(ChatId, Name);
        }
        [HttpDelete]
        [Route("delete/playlist")]
        public async Task<HttpStatusCode> DeletePlaylist([FromQuery] string ChatId, string Name)
        {
            return await DatabaseClient.DeletePlaylist(ChatId, Name);
        }
        [HttpGet]
        [Route("get/all")]
        public async Task<List<string>> GetPlaylists([FromQuery] string ChatId)
        {
            return await DatabaseClient.GetPlaylists(ChatId);
        }
    }
}
