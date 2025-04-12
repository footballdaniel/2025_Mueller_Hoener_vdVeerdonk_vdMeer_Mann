from typing import List
import numpy as np
from scipy.signal import butter, filtfilt
from scipy.interpolate import interp1d


class Filter:
    @staticmethod
    def low_pass_filter(
            signal: List[float],
            timestamps: List[float],
            cutoff: float = 1,
            target_fs: int = 100,
            order: int = 3
    ) -> List[float]:
        target_times = np.arange(timestamps[0], timestamps[-1], 1 / target_fs)
        interp_func = interp1d(timestamps, signal, kind="linear", fill_value="extrapolate")
        up_sampled_signal = interp_func(target_times)
        nyquist = 0.5 * target_fs
        normalized_cutoff = cutoff / nyquist
        b, a = butter(order, normalized_cutoff, btype='low')
        filtered_up_sampled = filtfilt(b, a, up_sampled_signal)
        return np.interp(timestamps, target_times, filtered_up_sampled).tolist()
