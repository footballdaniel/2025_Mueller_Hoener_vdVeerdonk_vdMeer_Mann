using Interactions.Domain;
using Interactions.Domain.Opponents;

namespace Interactions.Apps
{
    /// <summary>Reactive opponent: picks its pressure spot and intercepts passes.</summary>
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
    }
}
