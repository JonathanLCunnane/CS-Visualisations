public class BinaryTree {
    Node root;

    BinaryTree(int startingValue){
        root = new Node(startingValue);
    }

    BinaryTree(){
        root = null;
    }

    public void postOrderTraversal(Node n){
        if (n == null){
            return;
        }
        postOrderTraversal(n.left);
        postOrderTraversal(n.right);
        System.out.print(n.root + "-");
    }
}
