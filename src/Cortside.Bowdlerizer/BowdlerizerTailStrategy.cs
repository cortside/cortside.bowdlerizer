namespace Cortside.Bowdlerizer {
    public class BowdlerizerTailStrategy : BowdlerizerStrategy {
        private readonly int tailLength;

        public BowdlerizerTailStrategy(int tailLength) {
            this.tailLength = tailLength;
        }

        public override string Bowdlerize(string s) {
            if (string.IsNullOrEmpty(s)) {
                return s;
            }

            var chars = tailLength;
            if (s.Length <= chars) {
                chars = s.Length > 1 ? 1 : 0;
            }

            return "***" + s.Substring(s.Length - chars, chars);
        }
    };
}
