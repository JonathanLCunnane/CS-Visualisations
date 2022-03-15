class TreePrinter:
    def __init__(self, root):
        self.finalString = ""
        self.root = root

    def postOrderTraversal(self, node):
        if node is not None:
            self.finalString += "|" + str(node.value)
            self.postOrderTraversal(node.left)
            self.postOrderTraversal(node.right)

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
            self.finalString = ""
            self.postOrderTraversal(self.root)
            return self.finalString
        else:
            return "Dumbass"
