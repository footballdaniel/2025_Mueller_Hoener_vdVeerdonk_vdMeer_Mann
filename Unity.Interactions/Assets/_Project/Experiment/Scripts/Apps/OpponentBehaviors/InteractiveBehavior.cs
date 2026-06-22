using System.Collections.Generic;
using Interactions.Apps.ViewModels;
using Interactions.Domain;
using Interactions.Domain.Opponents;

namespace Interactions.Apps
{
    public class InteractiveBehavior : IOpponentBehavior
    {
        public virtual void Configure(Opponent opponent, App app)
        {
            opponent.Bind(app.User, app.LeftGoal, app.RightGoal, app.OpponentMaximalPositionConstraint, true);
        }

        public void OnPassDetected(Opponent opponent, Ball ball)
        {
            opponent.Intercept(ball);
        }

        public virtual IEnumerable<SettingDescriptor> GetSettings(App app)
        {
            return app.OpponentSettingsViewModel.GetDescriptors();
        }
    }
}
