using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

namespace codehunt.datarelease
{
    /// <summary>
    /// A level in the Code Hunt game. Each level corresponds to a single secret program.
    /// Levels are grouped into sectors. A player may not advance to the next sector
    /// until completing all but one level in the current sector.
    /// </summary>
    public class Level
    {
        private static readonly Regex levelNameRegex =
            new Regex(@"Sector(?<sector>\d)-Level(?<level>\d)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private readonly FileInfo challengeIdFile;
        private readonly string levelName;
        private readonly int sectorNum, levelInSector;

        private string challengeId = null, challengeText = null;

        public Level(FileInfo challengeIdFile)
        {
            this.challengeIdFile = challengeIdFile;
            string basename = Path.GetFileName(challengeIdFile.FullName);
            this.levelName = Path.GetFileNameWithoutExtension(basename);

            Match match = levelNameRegex.Match(this.levelName);          

            this.sectorNum = int.Parse(match.Groups["sector"].Value);
            this.levelInSector = int.Parse(match.Groups["level"].Value);
        }

        /// <summary>
        /// Enumerates all levels in the data set.
        /// </summary>
        /// <return>All levels.</returns>
        /// <param name="dataRele aseDir">Data release directory.</param>
        public static IEnumerable<Level> EnumerateAll(DirectoryInfo dataReleaseDir)
        {
            return dataReleaseDir
                  .EnumerateDirectories("solutions")
                  .Single()
                  .EnumerateFiles("*.challengeId")
                  .Select(f => new Level(f));
        }

        /// <summary>
        /// File containing the Code Hunt API challengeId for this level.
        /// </summary>
        /// <value>The challenge identifier file.</value>    
        public FileInfo ChallengeIdFile { get { return challengeIdFile; } }

        /// <summary>
        /// Path to file containing the reference solution for this level.
        /// </summary>
        /// <value>The challenge text file path.</value>
        public string ChallengeTextFilename
        {
            get
            {
                return Path.ChangeExtension(ChallengeIdFile.FullName, "cs");
            }
        }


        /// <summary>
        /// The name of this level used in the data release.
        /// </summary>
        /// <value>The name of the level.</value>
        public string LevelName { get { return levelName; } }

        /// <summary>
        /// Which sector this level appears in.
        /// </summary>
        /// <value>The sector number.</value>
        public int SectorNum { get { return this.sectorNum; } }

        /// <summary>
        /// Which level within the sector this level is.
        /// </summary>
        /// <value>The level number in the sector.</value>
        public int LevelInSector { get { return levelInSector; } }

        /// <summary>
        /// The challengeId used to identify this level in the Cod t API.
        /// </summary>
        /// <value>The challenge identifier.</value        		
        public string ChallengeId
        {
            get
            {
                if (challengeId == null)
                {
                    challengeId = File.ReadAllText(ChallengeIdFile.FullName);
                }
                return challengeId;
            }
        }

        /// <summary>
        /// The C# code of the reference solution.
        /// </summary>
        /// <value>The reference solution code.</value>
        public string ChallengeText
        {
            get
            {
                if (challengeText == null)
                {    		            
                    challengeText = File.ReadAllText(ChallengeTextFilename);
                }
                return challengeText;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            else if (obj is Level)
            {
                return ChallengeIdFile.Equals(((Level)obj).ChallengeIdFile);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return challengeIdFile.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("[Level: {0}]", LevelName);
        }
    }
}