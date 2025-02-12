#pragma warning disable S1694

namespace Cortside.Bowdlerizer {
    public abstract class BowdlerizerStrategy {
        public abstract string Bowdlerize(string s);
    };
}
