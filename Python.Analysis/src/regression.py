import os
from pathlib import Path
from typing import List

import arviz as az
import bambi as bmb
import pandas as pd


from src.domain import TrialCollection
from src.persistence import Persistence


def regression_1(trials: TrialCollection, model_path: Path, persistence: Persistence) -> None:
    df = pd.DataFrame({
        "participant_id": [trial.participant_id for trial in trials.trials],
        "duration": [trial.duration() for trial in trials.trials],
        "condition": [trial.condition.value for trial in trials.trials]
    })
    
    df["participant_id"] = pd.Categorical(df["participant_id"])
    df["condition_idx"] = pd.Categorical(df["condition"]).codes
    
    flat_prior = bmb.Prior("Uniform", lower=-3, upper=3)
    priors = {
        "condition_idx": flat_prior
    }
    
    formula = "duration ~ 0 + condition_idx + (1|participant_id)"
    
    model_bambi = bmb.Model(
        formula,
        df,
        family="gaussian",
        priors=priors,
    )
    
    # persistence.save_model(model_bambi.__str__(), Path("model_1.txt"))
    
    if not os.path.exists(str(model_path)):
        results = model_bambi.fit(
            draws=100,
            chains=4,
            idata_kwargs={"log_likelihood": True},
            random_seed=1991
        )
        
        model_bambi.predict(results, kind="response")
        
        az.to_netcdf(results, str(model_path)) 