from tree import Tree
from tree_printer import TreePrinter

root = Tree(10)
root.insert(Tree(3))
printer = TreePrinter(root)
print(printer.printTree('p'))
