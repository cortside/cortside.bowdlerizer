namespace Cortside.Bowdlerizer {
    public class BowdlerizerRule {
        public string Path { get; set; }
        public BowdlerizerStrategy Strategy { get; set; }

        public string Bowdlerize(string s) {
            if (Strategy == null) {
                Strategy = new BowdlerizerDefaultStrategy();
            }
            return Strategy.Bowdlerize(s);
        }
    }
}
