class CleanerApi:
    def __init__(self, execute_fn):
        self._execute = execute_fn

    def activate_cleaner(self, code):
        for command in code:
            self._execute(command)