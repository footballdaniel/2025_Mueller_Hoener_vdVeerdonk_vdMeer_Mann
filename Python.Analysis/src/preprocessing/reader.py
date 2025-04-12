from typing import List
import glob
from src.domain import Trial, TrialCollection
from src.preprocessing.manual_annotations import ingest


class TrialReader:
    @staticmethod
    def read_trials(data_path: str) -> TrialCollection:
        files = glob.glob(data_path, recursive=True)
        trials: List[Trial] = []
        
        for csv_file in files:
            json_file = csv_file.replace(".csv", ".json")
            trial = ingest(csv_file, json_file)
            trials.append(trial)
            
        return TrialCollection(trials) 