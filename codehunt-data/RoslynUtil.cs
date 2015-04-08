using Microsoft.CodeAnalysis;

using codehunt.datarelease;

namespace codehunt
{
    /// <summary>
    /// TODO This doesn't work on Mono... does it work on Windows?
    /// </summary>
    public static class RoslynUtil
    {
        public static SyntaxTree Parse(string code)
        {
            return Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(code);
        }

        public static SyntaxTree Parse(this Attempt attempt)
        {
            return Parse(attempt.Text);
        }

        public static SyntaxTree Parse(this Level level)
        {
            return Parse(level.ChallengeText);
        }

        public static SyntaxNode RemoveComments(this SyntaxNode node)
        {
            var newNode = node.ReplaceTrivia(node.DescendantTrivia(),
                              (_, __) => default(SyntaxTrivia));
            return newNode.NormalizeWhitespace();
        }
    }
}

