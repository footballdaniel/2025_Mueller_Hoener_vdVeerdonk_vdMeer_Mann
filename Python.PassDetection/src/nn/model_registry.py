import importlib
import os


class ModelRegistry(type):
    registry = {}

    def __new__(cls, name, bases, attrs):
        """Register a new model class."""
        new_class = super().__new__(cls, name, bases, attrs)
        if any(base.__name__ == "BaseModel" for base in bases) and name != "BaseModel":
            cls.registry[new_class.__name__] = new_class
        return new_class

    @classmethod
    def _load_all_modules(cls):
        current_package = __name__.rsplit('.', 1)[0] if '.' in __name__ else __name__
        current_dir = os.path.dirname(__file__)
        for filename in os.listdir(current_dir):
            if filename.endswith('.py') and filename not in ('__init__.py', os.path.basename(__file__)):
                module_name = filename[:-3]
                full_module_name = f"{current_package}.{module_name}"
                importlib.import_module(full_module_name)

    @classmethod
    def create(cls, model_name, *args, **kwargs):
        if not cls.registry:
            cls._load_all_modules()  # Load modules dynamically if registry is empty

        if model_name in cls.registry:
            model_class = cls.registry[model_name]
            return model_class(*args, **kwargs)
        else:
            raise ValueError(f"No model class registered under name {model_name}")
