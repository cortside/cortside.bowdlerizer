using System;

namespace Cortside.Bowdlerizer {
    public class BowdlerizerMaskStrategy : BowdlerizerStrategy {
        private readonly int length;
        private readonly char mask;

        public BowdlerizerMaskStrategy(int length, char mask) {
            this.length = length;
            this.mask = mask;
        }

        public override string Bowdlerize(string s) {
            if (string.IsNullOrEmpty(s)) {
                return s;
            }

            return new string(mask, length);
        }
    }
}
