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
    def create(cls, feature_name: str):
        if feature_name in cls._registry:
            return cls._registry[feature_name]()
        else:
            raise ValueError(f"No feature class registered under name {feature_name}")

