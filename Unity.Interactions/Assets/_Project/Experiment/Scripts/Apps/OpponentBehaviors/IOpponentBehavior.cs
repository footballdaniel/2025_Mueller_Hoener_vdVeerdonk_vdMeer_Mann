using System.Collections.Generic;
using Interactions.Apps.ViewModels;
using Interactions.Domain;
using Interactions.Domain.Opponents;

namespace Interactions.Apps
{
    /// <summary>
    /// Strategy describing how a laboratory trial's opponent is set up when it spawns and how it
    /// responds when the user passes. One implementation per experimental condition, injected into
    /// <see cref="States.LaboratoryTrial"/> so the trial logic itself carries no condition branching.
    /// </summary>
    public interface IOpponentBehavior
    {
        void Configure(Opponent opponent, App app);
        void OnPassDetected(Opponent opponent, Ball ball);

        // The settings sliders that apply to this opponent; shown in the dynamic settings panel.
        IEnumerable<SettingDescriptor> GetSettings(App app);
    }
}
