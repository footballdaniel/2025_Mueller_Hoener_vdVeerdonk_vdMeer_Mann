from torch import nn

from src.domain.model_registry import ModelRegistry


class BaseNNModel(nn.Module, metaclass=ModelRegistry):
    pass
