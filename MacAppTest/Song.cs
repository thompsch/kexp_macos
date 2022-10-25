using System;
using Realms;

namespace KEXP
{
    public class Song : RealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public DateTimeOffset AirTime { get; set; }
        public bool IsFavorite { get; set; }
    }
}

