namespace Cortside.Bowdlerizer {
    public class BowdlerizerHeadStrategy : BowdlerizerStrategy {
        private readonly int headLength;

        public BowdlerizerHeadStrategy(int headLength) {
            this.headLength = headLength;
        }

        public override string Bowdlerize(string s) {
            if (string.IsNullOrEmpty(s)) {
                return s;
            }

            var chars = headLength;
            if (s.Length <= chars) {
                chars = s.Length > 1 ? 1 : 0;
            }

            return s.Substring(0, chars) + "***";
        }
    };
}
