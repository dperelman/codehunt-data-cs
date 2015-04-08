using System;
using System.Collections.Generic;

namespace codehunt
{
    public enum Language
    {
        CSharp = 1,
        Java,
    }

    public static class LanguageUtil
    {
        private static readonly IDictionary<string,Language> extDictionary =
            new Dictionary<string,Language>
            {
                { "java", Language.Java },
                { "cs", Language.CSharp },
            };

        public static Language FromExtension(string ext)
        {
            return extDictionary[ext];
        }

        public static string ToCodeHuntAPIString(this Language language)
        {
            switch (language)
            {
                case Language.CSharp:
                    return "CSharp";
                case Language.Java:
                    return "Java";
                default:
                    throw new ArgumentException("Not a Language enum value: " + language,
                        paramName: "language");
            }
        }
    }
}

