using System.Collections.Generic;
using Interactions.Apps.ViewModels;
using Interactions.Domain;
using Interactions.Domain.Opponents;

namespace Interactions.Apps
{
    public class NonInteractiveBehavior : IOpponentBehavior
    {
        public void Configure(Opponent opponent, App app)
        {
            opponent.Bind(app.User, app.LeftGoal, app.RightGoal, app.OpponentMaximalPositionConstraint, false);
        }

        public void OnPassDetected(Opponent opponent, Ball ball)
        {
        }

        public IEnumerable<SettingDescriptor> GetSettings(App app)
        {
            return app.OpponentSettingsViewModel.GetDescriptors();
        }
    }
}
