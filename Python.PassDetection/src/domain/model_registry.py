from torch import nn


class ModelRegistry(type):
    registry = {}

    def __new__(cls, name, bases, attrs):
        """Register a new model class. The name of the class is used as the key in the registry.
        The __new__ method is automatically called when the class is discovered."""
        new_class = super().__new__(cls, name, bases, attrs)
        if bases != (nn.Module,):  # Do not register the base class
            cls.registry[new_class.__name__] = new_class
        return new_class

    @staticmethod
    def create(model_name, *args, **kwargs):
        if model_name in ModelRegistry.registry:
            model_class = ModelRegistry.registry[model_name]
            return model_class(*args, **kwargs)
        else:
            raise ValueError(f"No model class registered under name {model_name}")
