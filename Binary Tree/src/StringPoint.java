public class StringPoint {
    Integer x;
    Integer y;
    String value;

    StringPoint(int x, int y, String value) {
        this.x = x;
        this.y = y;
        this.value = value;
    }

    @Override
    public String toString() {
        return "(" + x + "," + y + "," + value + ")";
    }
}
