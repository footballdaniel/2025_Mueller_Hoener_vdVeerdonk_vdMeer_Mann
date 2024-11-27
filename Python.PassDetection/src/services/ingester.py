from pathlib import Path
from typing import List, Set

from src.domain.samples import IngestableRecording


class DataIngester:

    @staticmethod
    def ingest(path_raw_files: Path) -> List[IngestableRecording]:
        pattern = "**/*.*"
        all_files = list(path_raw_files.glob(pattern))
        ingest_files: Set[IngestableRecording] = set()

        for file in all_files:
            matching_ingest = None

            for ingest in ingest_files:
                if ingest.stem == file.stem:
                    matching_ingest = ingest
                    break

            if not matching_ingest:
                matching_ingest = IngestableRecording()
                ingest_files.add(matching_ingest)

            if file.suffix == ".csv":
                matching_ingest.add_event_file(file)
            elif file.suffix == ".json":
                matching_ingest.add_timeseries_file(file)

        return [ingest for ingest in ingest_files if ingest.both_files_present()]
