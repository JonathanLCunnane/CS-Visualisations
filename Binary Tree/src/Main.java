public class Main {
    public static void main(String[] args){
        BinaryTree tree = new BinaryTree();
        tree.root = new Node(9);
        tree.root.right = new Node(10);
        tree.root.left = new Node(20);
    }
}
