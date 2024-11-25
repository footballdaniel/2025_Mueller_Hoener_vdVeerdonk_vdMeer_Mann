import mlflow

mlflow.set_experiment("my_experiment")

with mlflow.start_run(run_name="root_experiment") as root_run:
    # Nested run
    for i in range(3):
        with mlflow.start_run(nested=True, run_name=f"nested_run_{i}") as nested_run:
            mlflow.log_param("nested_param", f"value_{i}")
            mlflow.log_metric("nested_metric", i * 0.1)

    # Back to the root run
    mlflow.log_param("new_root_param", "value2")
    mlflow.log_metric("new_root_metric", 0.99)
