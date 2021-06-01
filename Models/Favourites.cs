using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicFindAPI.Models
{
    public class Favourites
    {
        public string ChatId { get; set; }
        public List<string> Tracklist { get; set; }
    }
}
