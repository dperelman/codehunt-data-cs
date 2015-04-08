using System;

namespace codehunt
{
    public interface IExplorable
    {
        string Text { get; }
        Language Language { get; }
        string ChallengeId { get; }
    }
}

