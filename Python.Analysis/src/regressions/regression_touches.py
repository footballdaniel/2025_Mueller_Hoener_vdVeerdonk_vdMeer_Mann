import os
from pathlib import Path

import arviz as az
import bambi as bmb
import pandas as pd

from ..domain import TrialCollection, Condition
from ..persistence import Persistence
from ..services import MovementCalculator


def regression_touches(trials: TrialCollection, model_path: Path, model_description_path: Path, persistence: Persistence, n_draws: int = 100, n_tune: int = 100) -> None:
    df = pd.DataFrame({
        "participant_id": [trial.participant_id for trial in trials],
        "touches": [MovementCalculator.number_of_touches(trial) for trial in trials],
        "condition": [trial.condition.value for trial in trials]
    })
    
    df["participant_id"] = pd.Categorical(df["participant_id"])
    
    # Define conditions in desired order
    condition_order = [c.value for c in Condition]
    condition_order.reverse()
    df["condition"] = pd.Categorical(df["condition"], categories=condition_order, ordered=True)
    df["condition_idx"] = df["condition"].cat.codes
    
    # Priors for fixed effects
    numeric_prior = bmb.Prior("Uniform", lower=0, upper=10)
    
    # Priors for random effects with partial pooling
    participant_prior = bmb.Prior(
        "Normal",
        mu=0,
        sigma=bmb.Prior("HalfNormal", sigma=1.0)
    )
    
    # Prior for observation noise
    observation_noise = bmb.Prior("HalfNormal", sigma=2.5)

    priors = {
        "C(condition)": numeric_prior,
        "sigma": observation_noise,
        "1|participant_id": participant_prior
    }
    formula = "touches ~ 0 + C(condition) + (1|participant_id)"
    
    model_bambi = bmb.Model(
        formula,
        df,
        family="gaussian",
        priors=priors,
    )
    
    persistence.save_model(model_bambi.__str__(), model_description_path)
    
    if not os.path.exists(str(model_path)):
        results = model_bambi.fit(
            draws=n_draws,
            tune=n_tune,
            chains=4,
            idata_kwargs={"log_likelihood": True},
            random_seed=1991
        )
        
        model_bambi.predict(results, kind="response")
        
        az.to_netcdf(results, str(model_path)) 