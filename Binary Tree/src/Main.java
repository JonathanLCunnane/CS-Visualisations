import java.util.Scanner;

public class Main {

    public static void main(String[] args){
        Scanner input = new Scanner(System.in);
        System.out.println("Enter depth of tree between 1 - 4");
        int depth = input.nextInt();
        if (depth == 1) {
            depth1();
        } else if (depth == 2) {
            depth2();
        } else if (depth == 3) {
            depth3();
        } else if (depth == 4) {
            depth4();
        } else {
            errorMsg();
        }
    }
    private static void depth1 (){
        BinaryTree tree = new BinaryTree(9);
        new TreePrinter(tree);
    }
    private static void depth2 (){
        BinaryTree tree = new BinaryTree(9);
        tree.root.right = new Node(10);
        tree.root.left = new Node(20);
        new TreePrinter(tree);
    }
    private static void depth3 (){
        BinaryTree tree = new BinaryTree(9);
        tree.root.right = new Node(10);
        tree.root.left = new Node(20);
        tree.root.left.left = new Node(25);
        tree.root.left.right = new Node(31);
        tree.root.right.right = new Node(69);
        new TreePrinter(tree);
    }

    private static void depth4 (){
        BinaryTree tree = new BinaryTree(9);
        tree.root.right = new Node(10);
        tree.root.left = new Node(20);
        tree.root.left.left = new Node(25);
        tree.root.left.right = new Node(31);
        tree.root.right.right = new Node(69);
        tree.root.left.left.left = new Node(690);
        tree.root.left.left.right = new Node(16);
        tree.root.right.right.left = new Node(45);
        tree.root.left.right.right = new Node(34);
        tree.root.right.right.right = new Node(89);
        new TreePrinter(tree);
    }

    private static void errorMsg(){
        System.out.println("Error, invalid input, try again");
        main(new String[0]);
    }
}
