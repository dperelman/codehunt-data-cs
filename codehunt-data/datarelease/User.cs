using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace codehunt.datarelease
{
    /// <summary>
    /// A user in the data set. Corresponds to one real person playing
    /// the game. Includes all of the attempts submitted by that user.
    /// </summary>
    public class User
    {
        private readonly DirectoryInfo userDirectory;
        private readonly int num;

        private int? experience = null;

        public User(DirectoryInfo userDirectory)
        {
            this.userDirectory = userDirectory;
            this.num = int.Parse(userDirectory.FullName.Substring(userDirectory.FullName.Length - 3));
        }

        /// <summary>
        /// Directory storing information on the user.
        /// </summary>
        /// <value>The user directory.</value>
        public DirectoryInfo Directory { get { return this.userDirectory; } }

        /// <summary>
        /// Number identifying the user within this data set.
        /// </summary>
        /// <value>The user number.</value>
        public int Num { get { return this.num; } }

        /// <summary>
        /// Path to file containing the user's self-reported experience level.
        /// </summary>
        /// <value>The experience level file path.</value>
        public string ExperienceFilename
        {
            get
            {
                return Path.Combine(userDirectory.FullName, "experience");
            }
        }

        /// <summary>
        /// User-reported experience level, 1-3:
        /// 1="Beginner", 2="Intermediate", 3="Advanced"
        /// </summary>
        /// <value>The user's self-reported experience level.</value>
        public int Experience
        {
            get
            {
                if (!experience.HasValue)
                {
                    experience = int.Parse(File.ReadAllText(ExperienceFilename));
                }
                return experience.Value;
            }
        }

        public IEnumerable<Attempt> EnumerateAttemptsFor(Level level)
        {
            DirectoryInfo attemptDirectory = userDirectory
                                            .EnumerateDirectories(level.LevelName)
                                            .SingleOrDefault();
            if (attemptDirectory == null)
            {
                return null;
            }
            else
            {
                return attemptDirectory
                      .EnumerateFiles()
                      .Select(f => new Attempt(this, level, f));
            }
        }

        /// <summary>
        /// Enumerates all users.
        /// </summary>
        /// <returns>All users.</returns>
        /// <param name="dataReleaseDir">Data release directory.</param>
        public static IEnumerable<User> EnumerateAll(DirectoryInfo dataReleaseDir)
        {
            return dataReleaseDir
                  .EnumerateDirectories("users")
                  .Single()
                  .EnumerateDirectories()
                  .Select(d => new User(d));
        }

        public override string ToString()
        {
            return string.Format("[User: Num={0}]", Num);
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            else if (obj is User)
            {
                return ((User)obj).userDirectory.Equals(userDirectory);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return userDirectory.GetHashCode();
        }
    }
}

