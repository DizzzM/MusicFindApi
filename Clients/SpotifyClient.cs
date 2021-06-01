using MusicFindAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MusicFindAPI.Clients
{
    public class SpotifyClient
    {
        static string OAuthToken { get; set; }
        static async Task Authorize()
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://accounts.spotify.com/api/token"))
                {
                    request.Headers.TryAddWithoutValidation("Authorization", "Basic " + Constants.SpotifyToken);

                    request.Content = new StringContent("grant_type=client_credentials");
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

                    var response = await httpClient.SendAsync(request);
                    var Content = JsonConvert.DeserializeObject<SpotifyAuthorization>(await response.Content.ReadAsStringAsync());
                    OAuthToken = Content.access_token;
                }
            }
        }
        
        static async Task<SpotifyRoot> SpotifyTrackSearch(string value)
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), $"https://api.spotify.com/v1/search?q={value}&type=track"))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");
                    request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + OAuthToken);

                    var response = await httpClient.SendAsync(request);
                    var Content = JsonConvert.DeserializeObject<SpotifyRoot>(await response.Content.ReadAsStringAsync());
                    return Content;
                }
            }
        }
        
        public static async Task<List<TrackInfo>> TrackSearch(string Name)
        {
            await Authorize();
            var Result = new List<TrackInfo>();
            int Count = 0;
            var Response = await SpotifyTrackSearch(Name);
            foreach (var Song in Response.tracks.items)
            {
                if (Count < 10)
                {
                    var Track = new TrackInfo { Artist = Song.artists.First().name, ArtistId = Song.artists.First().id, Name = Song.name, SpotifyURL = Song.external_urls.spotify, ImageURL = Song.album.images.First().url, Lyrics = "", Id = "" };
                    Result.Add(Track);
                    Count++;
                }
                else break;
            }
            return Result;
        }
        
        public static async Task<List<TrackInfo>> SpotifyRelatedArtist(string id)
        {
            await Authorize();
            var Return = new List<TrackInfo>();
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), $"https://api.spotify.com/v1/artists/{id}/related-artists"))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");
                    request.Headers.TryAddWithoutValidation("Authorization", "Bearer " + OAuthToken);

                    var response = await httpClient.SendAsync(request);
                    var Content = JsonConvert.DeserializeObject<SpotifyRelatedArtists>(await response.Content.ReadAsStringAsync());
                    foreach (var artist in Content.artists)
                    {
                        Return.Add(new TrackInfo { Artist = artist.name });
                    }
                    return Return;
                }
            }
        }

    }
}
