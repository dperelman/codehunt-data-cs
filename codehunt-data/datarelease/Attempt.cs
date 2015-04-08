using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace codehunt.datarelease
{
    /// <summary>
    /// A single code submission for a level.
    /// </summary>
    public class Attempt : IExplorable
    {
        //Format: attemptNNN-YYYYMMDD-HHMMSS[-winningR].(java|cs)
        private static readonly Regex attemptFilenameRegex =
            new Regex(@"attempt(?<attemptNum>[0-9]{3})-"
                + "(?<year>[0-9]{4})(?<month>[0-9]{2})(?<day>[0-9]{2})-"
                + "(?<hour>[0-9]{2})(?<minute>[0-9]{2})(?<second>[0-9]{2})"
                + "(-winning(?<rating>[1-3]))?.(?<ext>java|cs)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);


        private readonly User user;
        private readonly Level level;
        private readonly FileInfo attemptFile;

        private readonly int attemptNum;
        private readonly DateTime timestamp;
        private readonly int? rating;

        private readonly Language language;

        private string text;

        public Attempt(User user, Level level, FileInfo attemptFile)
        {
            this.user = user;
            this.level = level;
            this.attemptFile = attemptFile;

            Match match = attemptFilenameRegex.Match(Path.GetFileName(attemptFile.FullName));

            this.attemptNum = int.Parse(match.Groups["attemptNum"].Value);

            int year = int.Parse(match.Groups["year"].Value);
            int month = int.Parse(match.Groups["month"].Value);
            int day = int.Parse(match.Groups["day"].Value);
            int hour = int.Parse(match.Groups["hour"].Value);
            int minute = int.Parse(match.Groups["minute"].Value);
            int second = int.Parse(match.Groups["second"].Value);

            this.timestamp = new DateTime(year: year, month: month, day: day,
                hour: hour, minute: minute, second: second,
                kind: DateTimeKind.Utc);

            if (match.Groups["rating"].Success)
            {
                this.rating = int.Parse(match.Groups["rating"].Value);
            }
            else
            {
                this.rating = null;
            }
            
            this.language = LanguageUtil.FromExtension(match.Groups["ext"].Value);
        }

        /// <summary>
        /// Gets the code for this attempt. See the Language property for
        /// whether this code is in Java or C#.
        /// </summary>
        /// <value>The code.</value>
        public string Text
        {
            get
            {
                if (this.text == null)
                {
                    this.text = File.ReadAllText(this.attemptFile.FullName);
                }
                return this.text;
            }
        }

        /// <summary>
        /// Language this submission is in. Java or CSharp.
        /// </summary>
        /// <value>The language.</value>
        public Language Language { get { return this.language; } }

        /// <summary>
        /// Time this attempt was submitted.
        /// </summary>
        /// <value>The timestamp.</value>
        public DateTime Timestamp { get { return this.timestamp; } }

        /// <summary>
        /// User that submitted this attempt.
        /// </summary>
        /// <value>The user.</value>
        public User User { get { return this.user; } }

        /// <summary>
        /// Level this attempt was submitted for.
        /// </summary>
        /// <value>The level.</value>
        public Level Level { get { return this.level; } }

        public string ChallengeId { get { return this.Level.ChallengeId; } }

        /// <summary>
        /// File storing the code for this attempt.
        /// </summary>
        /// <value>The attempt file.</value>
        public FileInfo AttemptFile { get { return this.attemptFile; } }

        /// <summary>
        /// Gets the attempt number where 1 is the first attempt.
        /// </summary>
        /// <value>The attempt number.</value>
        public int AttemptNum { get { return this.attemptNum; } }

        /// <summary>
        /// Gets a value indicating whether this attempt won the level.
        /// </summary>
        /// <value><c>true</c> if won; otherwise, <c>false</c>.</value>
        public bool Won { get { return this.rating.HasValue; } }

        /// <summary>
        /// Gets the rating (1-3) of a winning attempt or null if not a winning attempt.
        /// </summary>
        /// <value>The rating.</value>
        public int? Rating { get { return this.rating; } }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            else if (obj is Attempt)
            {
                return ((Attempt)obj).AttemptFile.Equals(AttemptFile);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return AttemptFile.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[Attempt {0} {1} {2}]", User, Level, Path.GetFileName(AttemptFile.FullName));
        }
    }
}

