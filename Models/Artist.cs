using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicFindAPI.Models
{
    public class Artist
    { 
        public string Name { get; set; }
        public List<TrackInfo> Tracks { get; set; }
        public static int ArtistComparison(Artist X, Artist Y)
        {
            return X.Name.CompareTo(Y.Name);
        }
    }
}
