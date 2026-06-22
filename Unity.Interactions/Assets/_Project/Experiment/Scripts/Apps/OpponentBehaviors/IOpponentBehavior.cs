using System.Collections.Generic;
using Interactions.Apps.ViewModels;
using Interactions.Domain;
using Interactions.Domain.Opponents;

namespace Interactions.Apps
{
    public interface IOpponentBehavior
    {
        void Configure(Opponent opponent, App app);
        void OnPassDetected(Opponent opponent, Ball ball);
        IEnumerable<SettingDescriptor> GetSettings(App app);
    }
}
