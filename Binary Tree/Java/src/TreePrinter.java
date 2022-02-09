import java.util.Arrays;
import java.util.HashMap;
import java.util.Objects;
import java.util.ArrayList;
import java.util.List;
import java.util.TreeMap;

public class TreePrinter {
    BinaryTree tree;
    String stringTree = "";
    int startingIndex = 0;
    HashMap<String, String> map = new HashMap<>();

    TreePrinter(BinaryTree input){
        this.tree = input;
        this.postOrderTraversal(this.tree.root, startingIndex, tree.root);
        System.out.println("\n### As a map ###");
        this.interpretStringTree();
        System.out.println("\n### As a tree ###");
        printTree(this.tree.root);
    }

    private void postOrderTraversal(Node n, int c, Node parent){
        if (n != null) {
            stringTree += "|"+ c +"#" + n.nodeVal + ">" + parent.nodeVal;
            postOrderTraversal(n.left, c + 1, n);
            postOrderTraversal(n.right, c + 1, n);
        }
    }

    private void interpretStringTree(){
        String[] elements = stringTree.split("\\|");
        elements = Arrays.copyOfRange(elements, 1, elements.length);
        for (String element : elements) {
            String[] temp = element.split("#");
            String temp2 = map.get(temp[0]);
            temp2 = (temp2 == null) ? "" : temp2;
            temp2 += "|" + temp[1];
            map.put(temp[0], temp2);
        }
        this.printTreeMap();
    }

    private void printTreeMap(){
        System.out.println("Node" + " | " + "Parent");
        for (String row: map.keySet()) {
            String value = map.get(row);
            String[] temp = value.split("\\|");
            String[] treeModule = new String[3];
            int lastInsert = 0;
            for (String v: temp){
                String[] temp2 = v.split(">");
                if (temp2.length != 1) {
                    System.out.println(temp2[0] + " | " + temp2[1]);

                    // will make work later
//                    treeModule[lastInsert] = temp2[(lastInsert != 1) ? 0 : 1];
//                    lastInsert ++;
//                    if (Objects.equals(treeModule[1], temp2[1])) {
//                        treeModule[2] = temp2[0];
//                    }
                }
            }
            // System.out.println(treeModule[0] + "<-" + treeModule[1] + "->" + treeModule[2]);
        }
    }

    // Below is stolen code from http://www.dsalgo.com/2016/01/draw-binary-tree-with-ascii.html
    public static void printTree (Node root) {
        List< StringPoint > result = getStrings((getWidth(root) + 1) / 2, 0, root);
        TreeMap< Integer, List< StringPoint > > lines = new TreeMap<  >();
        for (StringPoint s : result) {
            if (lines.get(s.y) != null) {
                lines.get(s.y).add(s);
            } else {
                List< StringPoint > l = new ArrayList<  >();
                l.add(s);
                lines.put(s.y, l);
            }
        }
        for (List< StringPoint > l : lines.values()) {
            System.out.println(flatten(l));
        }
    }

    private static String flatten(List< StringPoint > l) {
        int x = 0;
        StringBuilder sb = new StringBuilder();
        for (StringPoint s : l) {
            sb.append(new String(new char[Math.max((s.x - x), 0)]).replace('\0', ' '));
            sb.append(s.value);
            x = sb.length();
        }
        return sb.toString();
    }

    private static int getWidth(Node root) {
        int width = 0;
        if (root.left != null) {
            width += getWidth(root.left);
        }
        if (root.right != null) {
            width += getWidth(root.right);
        }
        width += ("" + root.nodeVal).length();
        return width;
    }

    private static List< StringPoint > getStrings(int x, int y, Node root) {
        List< StringPoint > result = new ArrayList<>();
        result.add(new StringPoint(x - ("" + root.nodeVal).length() / 2, y, ""
                + root.nodeVal));
        if (root.left != null) {
            int width = getWidth(root.left);
            int i = 0;
            for (; i <  (width + 1) / 2; ++i)
                result.add(new StringPoint(x - i - 1, y + i + 1, "/"));
            result.addAll(getStrings(x - i - 1, y + i + 1, root.left));
        }
        if (root.right != null) {
            int width = getWidth(root.right);
            int i = 0;
            for (; i <  (width + 1) / 2; ++i)
                result.add(new StringPoint(x + i + 1, y + i + 1, "\\"));
            result.addAll(getStrings(x + i + 1, y + i + 1, root.right));
        }
        return result;
    }
}