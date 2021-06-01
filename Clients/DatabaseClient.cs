using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MusicFindAPI.Models;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System.Linq;

namespace MusicFindAPI.Clients
{
    public class DatabaseClient
    {
        static DataFromDb Data = new DataFromDb();
        static IFirebaseConfig _config = new FirebaseConfig
        {
            AuthSecret = Constants.FirebaseAuthSecret,
            BasePath = Constants.FirebaseBasePath
        };
        static IFirebaseClient _client;

        public static void InitializeClient()
        {
            _client = new FireSharp.FirebaseClient(_config);
        }
        public static async Task GetDataFromDatabase()
        {
            FirebaseResponse Response = await _client.GetAsync($"");
            Data = Response.ResultAs<DataFromDb>();
        }

        //SEARCH section

        public static async Task PostToDatabase(TrackInfo Track)
        {
            Artist _Artist = new Artist
            {
                Name = Track.Artist,
                Tracks = new List<TrackInfo>()
            };
            var Artist = new Artist();

            if (Data == null)
            {
                Data = new DataFromDb
                {
                    Artists = new List<Artist>(),
                    Favourites = new List<Favourites>(),
                    MaxId = GetId().ToString()
                };
                Artist = _Artist;
                Data.Artists.Add(Artist);
            }
            else
            {
                Artist = Data.FindArtist(Track.Artist);
                if (Artist == null)
                {
                    if (Data.Artists == null)
                    {
                        Data.Artists = new List<Artist>();
                    }
                    Artist = _Artist;
                    Data.Artists.Add(Artist);
                }
            }
            Data.Artists.Find(_artist => _artist.Name == Artist.Name).Tracks.Add(Track);
            await SortDB();
            Data.MaxId = GetId().ToString();
            var Response = await _client.SetAsync("", Data);
        }

        //FAVOURITE section
        public async static Task<HttpStatusCode> AddToFavourite(string ChatId, string TrackId)
        {
            await GetDataFromDatabase();
            var Favor = new Favourites();
            if (Data == null)
            {
                Data = new DataFromDb { Artists = new List<Artist>(), Favourites = new List<Favourites>(), MaxId = GetId().ToString() };
                Favor = new Favourites
                {
                    ChatId = ChatId,
                    Tracklist = new List<string>()
                };
                Data.Favourites.Add(Favor);
            }
            else
            {                
                Favor = Data.FindFavourite(ChatId);

                if (Favor == null)
                {                    
                    if (Data.Favourites == null)
                    {
                        Data.Favourites = new List<Favourites>();
                    }

                    Favor = new Favourites
                    {
                        ChatId = ChatId,
                        Tracklist = new List<string>()
                    };
                    Data.Favourites.Add(Favor);
                }
            }

            if (Favor.Tracklist == null)
            {
                Favor.Tracklist = new List<string>();
            }
            var Return = new HttpStatusCode();

            if (Favor.Tracklist.Contains(TrackId)) 
                Return = HttpStatusCode.NotFound;
            else
            {
                Favor.Tracklist.Add(TrackId);
                var ChatIndex = Data.Favourites.FindIndex(_favor => _favor.ChatId == Favor.ChatId);
                var Response = await _client.SetAsync($"Favourites/{ChatIndex}/", Favor);
                Return = HttpStatusCode.OK;
            }
            return Return;
        }
        public async static Task<HttpStatusCode> DeleteFromFavourite(string ChatId, string TrackId)
        {
            await GetDataFromDatabase();
            if (Data != null)
            {
                if (Data.Favourites != null)
                {
                    var favor = Data.FindFavourite(ChatId);
                    if (favor != null)
                    {
                        if (favor.Tracklist != null)
                        {
                            if (!favor.Tracklist.Contains(TrackId)) return HttpStatusCode.NotFound;
                            var Favor = Data.Favourites.Find(_favor => _favor.ChatId == ChatId);
                            var ChatIndex = Data.Favourites.FindIndex(_favor => _favor == Favor);
                            var TrackIndex = Favor.Tracklist.FindIndex(id => id == TrackId);
                            var Response = await _client.DeleteAsync($"Favourites/{ChatIndex}/Tracklist/{TrackIndex}");
                            Data.Favourites.Find(_favor => _favor.ChatId == ChatId).Tracklist.Remove(TrackId);
                            return HttpStatusCode.OK;
                        }
                        else return HttpStatusCode.NotFound;
                    }
                    else return HttpStatusCode.NotFound;
                }
                else return HttpStatusCode.NotFound;
            }
            else return HttpStatusCode.NotFound;
        }
        public async static Task<List<TrackInfo>> GetFavouriteList(string ChatId)
        {
            await GetDataFromDatabase();
            var Return = new List<TrackInfo>();
            if (Data==null)
            {
                Return = null;
            }
            else
            {
                if (Data.Favourites == null)
                {
                    Return = null;
                }
                else
                {
                    var Favor = new Favourites();
                    Favor = Data.FindFavourite(ChatId);
                    if (Favor != null)
                    {
                        if (Favor.Tracklist != null)
                        {
                            foreach (var Track in Favor.Tracklist)
                            {
                                var track = Data.FindTrack(Track);
                                if (track != null) Return.Add(track);
                            }
                        }
                        else Return = null;
                    }
                }
            }
            return Return;
        }

        //PLAYLISTS section

        public async static Task<HttpStatusCode> AddToPlaylist(string ChatId, string Name, string TrackId)
        {
            await GetDataFromDatabase();
            
            var Playlist = new Playlist();
            if (Data == null)
            {
                return HttpStatusCode.BadRequest;
            }
            else
            {
                Playlist = Data.FindPlaylist(ChatId, Name);

                if (Playlist == null)
                {
                    return HttpStatusCode.BadRequest;
                }
            }
            if (Playlist.Tracks == null)
            {
                Playlist.Tracks = new List<string>();
            }
            foreach (var playlist in Data.Playlists)
            {
                if (playlist == null)
                    Data.Playlists.Remove(playlist);
            }
            if (Playlist.Tracks.Contains(TrackId)) 
                return HttpStatusCode.NotFound;
            else
            {
                Playlist.Tracks.Add(TrackId);
                var ChatIndex = Data.Playlists.FindIndex(playlist => playlist.ChatId == Playlist.ChatId && playlist.Name == Playlist.Name);
                var Response = await _client.SetAsync($"Playlists/{ChatIndex}/", Playlist);
                return HttpStatusCode.OK;
            }
        }
        public async static Task<HttpStatusCode> DeleteFromPlaylist(string ChatId, string Name, string TrackId)
        {
            await GetDataFromDatabase();
            if (Data != null)
            {
                if (Data.Playlists != null)
                {
                    var playlist = Data.FindPlaylist(ChatId, Name);
                    if (playlist != null)
                    {
                        if (playlist.Tracks != null)
                        {
                            if (!playlist.Tracks.Contains(TrackId)) return HttpStatusCode.NotFound;
                            var Playlist = Data.Playlists.Find(playlist => playlist.ChatId == ChatId && playlist.Name == Name);
                            var PlaylistIndex = Data.Playlists.FindIndex(playlist => playlist == Playlist);
                            var TrackIndex = Playlist.Tracks.FindIndex(id => id == TrackId);
                            var Response = await _client.DeleteAsync($"Playlists/{PlaylistIndex}/Tracks/{TrackIndex}");
                            Data.Playlists.Find(playlist => playlist.ChatId == ChatId && playlist.Name == Name).Tracks.Remove(TrackId);
                            return HttpStatusCode.OK;
                        }
                        else return HttpStatusCode.NotFound;
                    }
                    else return HttpStatusCode.BadRequest;
                }
                else return HttpStatusCode.BadRequest;
            }
            else return HttpStatusCode.BadRequest;
        }
        public async static Task<List<TrackInfo>> GetPlaylist(string ChatId, string Name)
        {
            await GetDataFromDatabase();
            var Return = new List<TrackInfo>();
            if (Data == null)
            {
                Return = null;
            }
            else
            {
                if (Data.Playlists == null)
                {
                    Return = null;
                }
                else
                {
                    var Playlist = new Playlist();
                    Playlist = Data.FindPlaylist(ChatId, Name);
                    if (Playlist != null)
                    {
                        if (Playlist.Tracks != null)
                        {
                            foreach (var Track in Playlist.Tracks)
                            {
                                var track = Data.FindTrack(Track);
                                if (track != null && track.Id!=null) Return.Add(track);
                            }
                        }
                        else Return = null;
                    }
                }
            }
            return Return;
        }
        public async static Task<List<string>> GetPlaylists(string ChatId)
        {
            var Return = new List<string>();
            await GetDataFromDatabase();
            if (Data != null)
            {
                if (Data.Playlists!=null)
                {
                    foreach(var playlist in Data.Playlists)
                    {
                        if (playlist!=null)
                            if (playlist.ChatId == ChatId)
                                Return.Add(playlist.Name);
                    }
                }
                else
                {
                    Return = null;
                }
            }
            else
            {
                Return = null;
            }
            return Return;
        }
        public async static Task<HttpStatusCode> AddPlaylist(string ChatId, string Name)
        {
            var idle = new Playlist
            {
                ChatId = ChatId,
                Name = Name,
                Tracks = new List<string>()
            };
            await GetDataFromDatabase();
            var Playlist = new Playlist();
            var List = new List<Playlist>();
            if (Data == null)
            {
                Data = new DataFromDb
                {
                    Playlists = new List<Playlist>()
                };
            }
            else
            {
                if (Data.Playlists != null)
                {
                    Playlist = Data.FindPlaylist(ChatId, Name);
                }
                else
                {
                    Data.Playlists = new List<Playlist>();
                }
            }
            if (Playlist == null)
            {
                List = Data.Playlists;
                List.Add(idle);
                var Response = await _client.SetAsync("Playlists", List);
                return HttpStatusCode.OK;
            }
            else
            {
                if (Playlist.Name == Name && Playlist.ChatId == ChatId)
                    return HttpStatusCode.NotFound;
                else
                {
                    List = Data.Playlists;
                    List.Add(idle);
                    var Response = await _client.SetAsync("Playlists", List);
                    return HttpStatusCode.OK;
                }
            }
        }
        public async static Task<HttpStatusCode> DeletePlaylist(string ChatId, string Name)
        {
            await GetDataFromDatabase();
            if (Data != null)
            {
                if (Data.Playlists != null)
                {
                    var Playlist = Data.FindPlaylist(ChatId, Name);
                    if (Playlist!=null)
                    {
                            var PlaylistIndex = Data.Playlists.FindIndex(playlist => playlist == Playlist);
                            var Response = await _client.DeleteAsync($"Playlists/{PlaylistIndex}");
                            Data.Playlists.Remove(Playlist);
                            return HttpStatusCode.OK;
                        
                    }
                        else return HttpStatusCode.NotFound;
                }
                else return HttpStatusCode.NotFound;
            }
            else return HttpStatusCode.NotFound;
        }

        //UTILITY section

        public static TrackInfo FindTrack(string Artist, string Track)
        {
            var _flag = false;
            var Return = new TrackInfo();
            if (Data != null)
            {
                if (Data.Artists != null)
                {
                    var _artist = Data.FindArtist(Artist);
                    if (_artist != null)
                        foreach (var _track in _artist.Tracks)
                        {
                            if (_track.Artist == Artist && _track.Name == Track)
                            {
                                Return = _track;
                                _flag = true;
                                break;
                            }
                        }
                }
            }
            if (!_flag) Return = null;
            return Return;
        }
        public static int GetId()
        {
            int Id = 0;
            if (Data != null)
                foreach (var Artist in Data.Artists)
                {
                    foreach (var Track in Artist.Tracks)
                    {
                        Id++;
                    }
                }
            return Id;
        }
        static async Task SortDB()
        {
            await DeleteDuplicates();
            Data.Artists.Sort(Artist.ArtistComparison);
            foreach (var _artist in Data.Artists)
            {
                _artist.Tracks.Sort(TrackInfo.TrackComparison);
            }
        }
        static async Task DeleteDuplicates()
        {
            if (Data != null)
            {
                if (Data.Artists != null)
                {
                    foreach (var Artist in Data.Artists)
                    {
                        if (Artist.Tracks.Count > 1)
                        {
                            var prevTrack = new TrackInfo();
                            prevTrack = null;
                            foreach (var Track in Artist.Tracks)
                            {
                                if (prevTrack == null)
                                {
                                    prevTrack = Track;
                                }
                                else
                                {
                                    if (prevTrack.SpotifyURL == Track.SpotifyURL && prevTrack.Lyrics == Track.Lyrics && prevTrack.Name == Track.Name)
                                    {
                                        await GetDataFromDatabase();
                                        var ArtistIndex = Data.Artists.FindIndex(artist => artist == Artist);
                                        var TrackIndex = Artist.Tracks.FindLastIndex(track => track == Track);
                                        var Response = await _client.DeleteAsync($"Artists/{ArtistIndex}/Tracks/{TrackIndex}");
                                        Artist.Tracks.Remove(Track);
                                    }
                                    else
                                    {
                                        prevTrack = Track;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}