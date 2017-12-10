using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RandomSteamGame
{
    public class Game {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Exe { get; set; }
    }

    public class GameConfigs {
        [XmlElement("Game")]
        public List<Game> Games { get; set; }
    }
}
