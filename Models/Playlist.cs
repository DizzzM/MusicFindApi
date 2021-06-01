using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicFindAPI.Models
{
    public class Playlist
    {
        public string ChatId { get; set; }
        public string Name { get; set; }
        public List<string> Tracks { get; set; }
    }
}
