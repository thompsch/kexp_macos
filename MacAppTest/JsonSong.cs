using System;
using System.Collections.Generic;

namespace KEXP
{
    public class JsonSong
    {
        public List<JsonSongResults> results { get; set; }
        public JsonSong() { this.results = new List<JsonSongResults>(); }
    }

    public class JsonSongResults
    {
        public string song { get; set; }
        public string artist { get; set; }
        public string airdate { get; set; }
    }
}

