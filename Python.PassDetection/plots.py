from pathlib import Path

from src.domain.inferences import InputData
from src.features.feature_registry import FeatureRegistry
from src.services.feature_engineer import FeatureEngineer
from src.services.ingester import DataIngester
from src.services.plotter import TimeSeriesPlot
from src.services.sampling.sample_generator import SampleGenerator

"""CONFIGURATION"""
data_path = Path("../Data/PassDetection")
plot_dir = Path("plots")
features_to_plot = ['VelocityMagnitudeDominantFoot', 'VelocityMagnitudeNonDominantFoot']

"""PIPELINE"""
recordings = DataIngester.ingest(data_path)
samples_iterator = SampleGenerator.generate(recordings)

for sample in samples_iterator:

    if not sample.contains_a_pass():
        print("Pass detected")
        continue

    plot = TimeSeriesPlot(plot_dir)
    plot.add_sample(sample)

    input_data = InputData(
        sample.recording.user_dominant_foot_positions,
        sample.recording.user_non_dominant_foot_positions,
        sample.recording.timestamps
    )

    for feature_name in features_to_plot:
        engineer = FeatureEngineer()
        feature_instance = FeatureRegistry.create(feature_name)
        engineer.add_feature(feature_instance)
        flattened_values = engineer.engineer(input_data)
        plot.add_feature(feature_name, flattened_values)

    plot.save()
