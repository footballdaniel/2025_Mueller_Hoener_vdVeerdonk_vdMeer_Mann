import unittest

import numpy as np

from src.preprocessing.filtering import Filter


class TestLowPassFilter(unittest.TestCase):

    def test_constant_signal(self):
        signal = [5.0] * 10
        timestamps = np.linspace(0, 0.9, 10).tolist()
        result = Filter.low_pass_filter(signal, timestamps)
        self.assertTrue(np.allclose(result, signal, atol=1e-6))

    def test_high_freq_sine_is_filtered(self):
        fs = 10
        timestamps = np.linspace(0, 1, fs, endpoint=False).tolist()
        signal = np.sin(2 * np.pi * 5 * np.array(timestamps)).tolist()  # 5 Hz sine
        result = Filter.low_pass_filter(signal, timestamps, cutoff=2, target_fs=100)
        self.assertLess(np.std(result), np.std(signal))

    def test_output_length_matches_input(self):
        signal = [0, 1, 0, -1, 0]
        timestamps = np.linspace(0, 0.4, 5).tolist()
        result = Filter.low_pass_filter(signal, timestamps)
        self.assertEqual(len(result), len(signal))

