using System;
using System.Linq;
using System.Collections.Generic;

namespace codehunt.rest
{
    public struct CompilerError
    {
        public string errorNumber;
        // e.g. "CS0123"
        public string errorDocumentationUrl;
        // specific help page for that errorNumber
        public string errorText;
        public int line;
        public int column;

        public override string ToString()
        {
            return line + ":" + column + "::" + errorNumber + ": " + errorText;
        }
    }

    public struct TestCase
    {
        public string status;
        // one of the following: "Failure", "Inconclusive", "Success"
        public string[] values;
        // pretty-printed strings representing argument values,
        // corresponding to parameter names, if any
        public string summary;
        public bool anyExceptionOrPathBoundsExceeded;
        public string message;
        public string code;
        // test case code, if at explored function has at least one parameter
        public string exception;
        // exception message, if any
        public string stackTrace;
        // stack trace, if any
    }

    public struct ExplorationJson
    {
        public string id;
        public string programId;
        public string challengeId;
        public string kind;
        public bool isComplete;

        // if (kind == "InternalError")
        public string exception;

        // if (kind == "CompilationError")
        public string documentationUrl;
        public CompilerError[] errors;

        // if (kind == "BadPuzzle")
        public string description;

        // if (kind == "BadDependency")
        public string[] referencedTypes;

        // if (kind == "TestCases")
        public bool hasWon;
        public string[] names;
        public TestCase[] testCases;
    }

    public class Exploration
    {
        private readonly IExplorable attempt;
        private readonly ExplorationJson exp;
        private readonly IReadOnlyList<string> errors;

        public Exploration(IExplorable attempt, ExplorationJson exp)
        {
            if (exp.id == null)
            {
                throw new ArgumentNullException("exp");
            }

            this.attempt = attempt;
            this.exp = exp;

            switch (exp.kind)
            {
                case "InternalError":            
                // Should not happen
                    this.errors = new string[] { exp.exception };
                    break;
                case "CompilationError":
                    this.errors = exp.errors.Select(e => e.ToString()).ToList();
                    break;
                case "BadPuzzle":
                    this.errors = new string[] { exp.description };
                    break;
                case "BadDependency":
                    this.errors = exp.referencedTypes;
                    break;
                default:
                    this.errors = null;
                    break;
            }
        }

        public IExplorable Attempt { get { return attempt; } }

        public ExplorationJson Json { get { return exp; } }

        public string Kind { get { return this.Json.kind; } }

        public bool AttemptCompiles { get { return exp.kind == "TestCases"; } }

        public bool HasWon
        {
            get
            {
                return AttemptCompiles && this.Json.hasWon; 
            }
        }

        public IReadOnlyList<TestCase> TestCases
        {
            get
            {
                return exp.testCases;
            }
        }

        public IReadOnlyList<CompilerError> CompilationErrors
        {
            get
            {
                return exp.errors;
            }
        }

        public IReadOnlyList<string> Errors
        {
            get
            {
                return errors;
            }
        }
    }
}