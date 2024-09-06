using System;

internal static class GameEvents
{
    public static readonly Action TrialEnded = new Action(delegate { });
    public static readonly Action RecordingStarted = new Action(delegate { });
}