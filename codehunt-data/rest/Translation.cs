using System;

namespace codehunt.rest
{
    public struct TranslationJson
    {
        public string kind;
        public CodeHuntClient.Program program;
        public CompilerError[] errors;
    }

    public class Translation : IExplorable
    {
        private readonly IExplorable attempt;
        private readonly TranslationJson translation;

        public Translation(IExplorable attempt, TranslationJson translation)
        {
            this.attempt = attempt;
            this.translation = translation;
        }

        public IExplorable Attempt { get { return attempt; } }

        public TranslationJson Json { get { return translation; } }

        public bool Success { get { return translation.kind == "Translated"; } }

        public string Text { get { return translation.program.text; } }

        public Language Language { get { return Language.CSharp; } }

        public string ChallengeId { get { return attempt.ChallengeId; } }

        public CompilerError[] Errors { get { return translation.errors; } }
    }
}

