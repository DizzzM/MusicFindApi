using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicFindAPI.Models
{
    public class SpotifyAuthorization
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
    }
    public class SpotifyRelatedArtists
    {
        public List<SpotifyArtist> artists { get; set; }
    }

    public class ExternalUrls
    {
        public string spotify { get; set; }
    }

    public class SpotifyArtist
    {
        public ExternalUrls external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Image
    {
        public int height { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class Album
    {
        public string album_type { get; set; }
        public List<Artist> artists { get; set; }
        public List<string> available_markets { get; set; }
        public ExternalUrls external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public List<Image> images { get; set; }
        public string name { get; set; }
        public string release_date { get; set; }
        public string release_date_precision { get; set; }
        public int total_tracks { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class ExternalIds
    {
        public string isrc { get; set; }
    }

    public class Item
    {
        public Album album { get; set; }
        public List<SpotifyArtist> artists { get; set; }
        public List<string> available_markets { get; set; }
        public int disc_number { get; set; }
        public int duration_ms { get; set; }
        public bool @explicit { get; set; }
        public ExternalIds external_ids { get; set; }
        public ExternalUrls external_urls { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public bool is_local { get; set; }
        public string name { get; set; }
        public int popularity { get; set; }
        public string preview_url { get; set; }
        public int track_number { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Tracks
    {
        public string href { get; set; }
        public List<Item> items { get; set; }
        public int limit { get; set; }
        public string next { get; set; }
        public int offset { get; set; }
        public object previous { get; set; }
        public int total { get; set; }
    }

    public class SpotifyRoot
    {
        public Tracks tracks { get; set; }
    }



    public class PrimaryArtist
    {
        public string api_path { get; set; }
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Result
    {
        public string api_path { get; set; }
        public string full_title { get; set; }
        public string id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public PrimaryArtist primary_artist { get; set; }
    }

    public class Hit
    {
        public Result result { get; set; }
    }
    public class Song
    {
        public string api_path { get; set; }
        public string full_title { get; set; }
        public string id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public PrimaryArtist primary_artist { get; set; }
    }

    public class Response
    {
        public List<Hit> hits { get; set; }
        public string next_page { get; set; }
        public List<Song> songs { get; set; }
    }

    public class GeniusRoot
    {
        public Response response { get; set; }
        public Song song { get; set; }
    }
    public class Track
    {
        public string track_name { get; set; }
        public string track_share_url { get; set; }
        public string has_lyrics { get; set; }
        public string artist_name { get; set; }
    }

    public class TrackList
    {
        public Track track { get; set; }
    }

    public class Body
    {
        public List<TrackList> track_list { get; set; }
    }

    public class Message
    {
        public Body body { get; set; }
    }

    public class MusixRoot
    {
        public Message message { get; set; }
    }
}
