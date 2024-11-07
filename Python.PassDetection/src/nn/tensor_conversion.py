import torch


def position_list_to_tensor(position_list):
    # Convert a list of Position objects to a tensor of shape (sequence_length, 3)
    return torch.tensor([[pos.x, pos.y, pos.z] for pos in position_list], dtype=torch.float32)


def feature_to_input_tensor(feature):
    # Combine all feature sequences into a single input tensor
    zeroed_positions = position_list_to_tensor(feature.zeroed_position_dominant_foot)
    offsets = position_list_to_tensor(feature.offset_dominant_foot_to_non_dominant_foot)
    velocities_dominant = position_list_to_tensor(feature.velocities_dominant_foot)
    velocities_non_dominant = position_list_to_tensor(feature.velocities_non_dominant_foot)

    # Concatenate along the feature dimension
    # Each has shape (sequence_length, 3), so concatenated features will have shape (sequence_length, 12)
    input_tensor = torch.cat([zeroed_positions, offsets, velocities_dominant, velocities_non_dominant], dim=1)
    return input_tensor  # Shape: (sequence_length, 12)