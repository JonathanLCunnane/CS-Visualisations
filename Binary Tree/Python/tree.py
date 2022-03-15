class Tree:
    def __init__(self, value):
        self.left = None
        self.right = None
        self.value = value
        self.finalString = ""

    def getNodeValue(self):
        return self.value

    def insert(self, value):
        if self.left is None:
            self.left = value
        else:
            self.right = value
