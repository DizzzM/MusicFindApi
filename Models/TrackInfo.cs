using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicFindAPI.Models
{
    public class TrackInfo
    {
        public string Id { get; set; }
        public string Artist { get; set; }
        public string ArtistId { get; set; }
        public string Name { get; set; }
        public string SpotifyURL { get; set; }
        public string ImageURL { get; set; }
        public string Lyrics { get; set; }
        public static int TrackComparison(TrackInfo X, TrackInfo Y)
        {
            return X.Name.CompareTo(Y.Name);
        }
    }
}
