# """SAVE PREDICTIONS TO DATASET"""
# model = best_run.model
# engineer = FeatureEngineer()
# for feature in best_run.config.features:
#     feature_instance = FeatureRegistry.create(feature)
#     engineer.add_feature(feature_instance)

# dataset = PassDataset(repo, num_samples, engineer)
#
# with torch.no_grad():
#     for idx in range(num_samples):
#         input_tensor, label = dataset[idx]
#         input_tensor = input_tensor.unsqueeze(0).to(device)
#         output = model(input_tensor)
#         probability = output.item()
#         sample = repo.get(idx)
#         computed_features = engineer.engineer(sample.recording.input_data)
#         sample_with_prediction = replace(
#             sample,
#             inference=Inference(
#                 prediction=probability,
#                 split=sample.inference.split,
#                 computed_features=computed_features,
#                 label=sample.recording.contains_a_pass,
#             )
#         )
#         repo.add(sample_with_prediction)

# """SAVE DATASET"""
# samples = [repo.get(idx) for idx in test_indices]
# # Save some to folder
# with open('dataset.pkl', 'wb') as f:
#     pickle.dump(samples, f)
#
# with open("dataset.json", 'w') as f:
#     json.dump(samples, f, cls=CustomEncoder)
#
# """PLOT SAMPLES"""
# for idx, id in enumerate(samples):
#     if not save_plots:
#         break
#     plot_sample_with_features(id, plot_dir)