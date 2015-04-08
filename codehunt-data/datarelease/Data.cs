using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace codehunt.datarelease
{
    public class Data
    {
        private IReadOnlyDictionary<string, Level> levels;
        private IReadOnlyDictionary<int, User> users;

        public Data(DirectoryInfo directory)
        {
            levels = Level.EnumerateAll(directory).ToDictionary(l => l.LevelName);
            users = User.EnumerateAll(directory).ToDictionary(u => u.Num);
        }

        public IReadOnlyDictionary<string, Level> LevelsByName { get { return levels; } }

        public IReadOnlyDictionary<int, User> UsersByNumber { get { return users; } }

        public IEnumerable<Level> Levels { get { return levels.Values.OrderBy(l => l.LevelName); } }

        public IEnumerable<User> Users { get { return users.Values.OrderBy(u => u.Num); } }
    }
}

