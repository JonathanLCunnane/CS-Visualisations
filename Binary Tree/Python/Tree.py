class Tree:
    def __init__(self, value):
        self.left = None
        self.right = None
        self.value = value

    def getNodeValue(self):
        return self.value

    def insert(self, value):
        if self.left is None:
            self.left = value
        else:
            self.right = value

    def printTree(self, method):
        """
        Method parameter has three input options:
        'B' = Pre-order Traversal
        'I' = Inorder Traversal
        'P' = Post-order Traversal
        Why 'B'? Cause 'B'efore is a distant synonym of pre-order.
        """
        method = method.lower()
        if method == 'b':
            pass
        elif method == 'i':
            pass
        elif method == 'p':
            pass
        else:
            return "Dumbass"
