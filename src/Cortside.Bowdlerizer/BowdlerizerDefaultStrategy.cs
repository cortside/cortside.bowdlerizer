using System;

namespace Cortside.Bowdlerizer {
    public class BowdlerizerDefaultStrategy : BowdlerizerStrategy {
        public override string Bowdlerize(string s) {
            if (string.IsNullOrEmpty(s)) {
                return s;
            }

            int len = s.Length;
            int leftLen = len > 4 ? 1 : 0;
            int rightLen = len > 6 ? Math.Min((len - 6) / 2, 4) : 0;
            return s.Substring(0, leftLen) +
                new string('*', len - leftLen - rightLen) +
                s.Substring(len - rightLen);
        }
    };
}
