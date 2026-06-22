using System.Collections.Generic;
using Interactions.Apps.ViewModels;
using Interactions.Domain;
using Interactions.Domain.Opponents;

namespace Interactions.Apps
{
    /// <summary>Virtual opponent: picks its spot but does not react to passes.</summary>
    public class NonInteractiveBehavior : IOpponentBehavior
    {
        public void Configure(Opponent opponent, App app)
        {
            opponent.Bind(app.User, app.LeftGoal, app.RightGoal, app.OpponentMaximalPositionConstraint, false);
        }

        public void OnPassDetected(Opponent opponent, Ball ball)
        {
            // Non-interactive opponent ignores the pass.
        }

        public IEnumerable<SettingDescriptor> GetSettings(App app)
        {
            return app.OpponentSettingsViewModel.GetDescriptors();
        }
    }
}
