using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicFindAPI.Models
{
    public class DataFromDb
    {
        public string MaxId { get; set; }
        public List<Artist> Artists { get; set; }
        public List<Favourites> Favourites { get; set; }
        public List<Playlist> Playlists { get; set; }
        public Artist FindArtist(string Name)
        {
            var Artist = new Artist();
            var _flag = false;
            if (this != null)
            {
                if (this.Artists != null)
                    foreach (var _artist in this.Artists)
                    {
                        if (_artist.Name == Name)
                        {
                            Artist = _artist;
                            _flag = true;
                        }
                    }
            }
            if (!_flag) Artist = null;
            return Artist;
        }
        public Favourites FindFavourite(string ChatId)
        {
            var Favor = new Favourites();
            var _flag = false;
            if (this != null)
            {
                if (this.Favourites != null)
                    foreach (var _favor in this.Favourites)
                    {
                        if (_favor.ChatId == ChatId)
                        {
                            Favor = _favor;
                            _flag = true;
                        }
                    }
            }
            if (!_flag) Favor = null;
            return Favor;
        }
        public Playlist FindPlaylist(string ChatId, string Name)
        {
            var Playlist = new Playlist();
            var _flag = false;
            if (this != null)
            {
                if (this.Playlists != null)
                    foreach (var playlist in this.Playlists)
                    {
                        if (playlist != null)
                        {
                            if (playlist.ChatId == ChatId && playlist.Name == Name)
                            {
                                Playlist = playlist;
                                _flag = true;
                            }
                        }
                    }
            }
            if (!_flag) Playlist = null;
            return Playlist;
        }
        public TrackInfo FindTrack(string Id)
        {
            var Track = new TrackInfo();
            bool _flag = false;
            foreach(var Artist in Artists)
            {
                foreach (var track in Artist.Tracks)
                {
                    if (track.Id == Id)
                    {
                        Track = track;
                        _flag = true;
                    }
                }
            }
            if (!_flag) Track = null;
            return Track;
        }
        
    }

}
