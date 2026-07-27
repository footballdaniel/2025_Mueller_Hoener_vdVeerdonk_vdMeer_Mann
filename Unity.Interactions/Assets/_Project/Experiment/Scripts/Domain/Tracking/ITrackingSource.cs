using System;

namespace Interactions.Domain.Tracking
{
	public interface ITrackingSource
	{
		/// Clamped to the recorded range, returns the nearest recorded sample.
		/// Never null, never throws.
		Frame GetFrameAt(TimeSpan time);
	}
}
