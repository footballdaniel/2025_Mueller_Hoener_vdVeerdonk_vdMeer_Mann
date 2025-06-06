from pathlib import Path

from src.infra.tiny_db.tiny_db_repository import RepositoryWithInMemoryCache
from src.services.ingester import DataIngester
from src.services.sampling.sample_generator import SampleGenerator

recordings = DataIngester.ingest(Path("../Data/PassDetection"))
samples_iterator = SampleGenerator.generate(recordings)
repo = RepositoryWithInMemoryCache(samples_iterator)

samples_iterator = repo.get_all()
for sample in samples_iterator:
    print(sample)