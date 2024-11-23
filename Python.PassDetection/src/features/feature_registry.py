import importlib
import os
from abc import ABC, ABCMeta


class FeatureRegistry(ABCMeta):
    _registry = {}

    def __new__(cls, name, bases, attrs):
        new_class = super().__new__(cls, name, bases, attrs)
        is_not_base_feature = not any(base is ABC or base is ABCMeta for base in bases)
        if is_not_base_feature:
            cls._registry[name] = new_class
        return new_class

    @classmethod
    def _load_features_from_this_module(cls):
        current_package = __name__.rsplit('.', 1)[0] if '.' in __name__ else __name__

        current_dir = os.path.dirname(__file__)
        for filename in os.listdir(current_dir):
            if filename.endswith('.py') and filename not in ('__init__.py', os.path.basename(__file__)):
                module_name = filename[:-3]
                full_module_name = f"{current_package}.{module_name}"
                importlib.import_module(full_module_name)

    @classmethod
    def create(cls, feature_name: str):
        if not cls._registry:
            cls._load_features_from_this_module()

        if feature_name in cls._registry:
            return cls._registry[feature_name]()
        else:
            raise ValueError(f"No feature class registered under name {feature_name}")
