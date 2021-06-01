using MusicFindAPI.Models;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace MusicFindAPI.Clients
{
    public class LyricsClient
    {
        public static async Task<TrackInfo> LyricsSearch(TrackInfo Track)
        {
            await DatabaseClient.GetDataFromDatabase();
            var Return = DatabaseClient.FindTrack(Track.Artist, Track.Name);
            if (Return == null)
            {
                Return = new TrackInfo { Artist = Track.Artist, ArtistId = Track.ArtistId, Name = Track.Name, SpotifyURL = Track.SpotifyURL, ImageURL = Track.ImageURL, Lyrics = await GetLyrics(Track.Artist, Track.Name), Id = (DatabaseClient.GetId()+1).ToString()};
                await DatabaseClient.PostToDatabase(Return);
            }
            return Return;
        }

        static async Task<string> GetLyrics(string Artist, string Track)
        {
            string Lyrics = "";
            Lyrics += await GetLyricsGenius(Artist, Track);
            if (Lyrics == "") Lyrics += await GetLyricsMusixmatch(Artist, Track);
            if (Lyrics == "") Lyrics += "Not found";
            return Lyrics;
        }
        static async Task<string> GetLyricsGenius(string Artist, string Track)
        {
            var Lyrics = "";
            try
            {
                using (HttpClient Client = new HttpClient())
                {
                    Client.BaseAddress = new Uri("https://api.genius.com/");
                    Client.DefaultRequestHeaders.Clear();
                    Client.DefaultRequestHeaders.Add("User-Agent", "CompuServe Classic/1.22");
                    Client.DefaultRequestHeaders.Add("Accept", "application/json");
                    Client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Constants.GeniusBearer}");
                    using (var SearchRequest = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Get, Client.BaseAddress + $"search?q={Artist}")))
                    {
                        var _flag = false;
                        var Page = new GeniusRoot();
                        var Result = JsonConvert.DeserializeObject<GeniusRoot>(await SearchRequest.Content.ReadAsStringAsync());
                        int i = 1;
                        do
                        {
                            using (var ArtistRequest = await Client.SendAsync(new HttpRequestMessage(HttpMethod.Get, Client.BaseAddress + $"artists/{Result.response.hits.First().result.primary_artist.id}/songs?per_page=20&page={i}")))
                            {
                                Page = JsonConvert.DeserializeObject<GeniusRoot>(await ArtistRequest.Content.ReadAsStringAsync());
                                foreach (var Song in Page.response.songs)
                                {
                                    if (Song.title.ToLower() == Track.ToLower())
                                    {
                                        Lyrics += Song.url;
                                        _flag = true;
                                        break;
                                    }
                                }
                            }

                            i = int.Parse(Page.response.next_page);
                        } while (!_flag && Page.response.next_page != "" && int.Parse(Page.response.next_page) < 100);
                    }
                }
            }
            catch
            {

            }
            return Lyrics;
        }
        static async Task<string> GetLyricsMusixmatch(string Artist, string Track)
        {
            var Lyrics = "";
            try
            {
                using (HttpClient Client = new HttpClient())
                {
                    string Url = $"https://api.musixmatch.com/ws/1.1/track.search?format=jsonp&callback=callback&q_track={Track}&q_artist={Artist}&quorum_factor=1&apikey={Constants.MusixmatchKey}";
                    using (HttpResponseMessage Response = await Client.GetAsync(Url))
                    {
                        var Root = JsonConvert.DeserializeObject<MusixRoot>((await Response.Content.ReadAsStringAsync()).Replace("callback(", "").Replace(");", ""));
                        foreach (var Song in Root.message.body.track_list)
                        {
                            if (int.Parse(Song.track.has_lyrics) > 0)
                            {
                                if (Song.track.track_name == Track)
                                {
                                    Lyrics += Song.track.track_share_url;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            catch
            {

            }
            return Lyrics;
        }
    }
}
