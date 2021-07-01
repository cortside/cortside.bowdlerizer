using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Cortside.Bowdlerizer {

    public class Bowdlerizer {
        private readonly List<BowdlerizerRule> rules = new List<BowdlerizerRule>();

        public Bowdlerizer() {
        }

        public Bowdlerizer(string[] v) {
            foreach (var s in v) {
                rules.Add(new BowdlerizerRule() { Path = s });
            }
        }

        public Bowdlerizer(List<BowdlerizerRule> paths) {
            this.rules.AddRange(paths);
        }

        public string BowdlerizeObject(Object o) {
            var token = JToken.FromObject(o);
            ObscureMatchingValues(token);

            return token.ToString(Newtonsoft.Json.Formatting.None);
        }

        public string BowdlerizeXml(string xml) {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            return BowdlerizeXml(doc);
        }
        public string BowdlerizeXml(XmlDocument doc) {
            var json = JsonConvert.SerializeXmlNode(doc);
            JToken token = JToken.Parse(json);
            ObscureMatchingValues(token);

            XNode node = JsonConvert.DeserializeXNode(token.ToString(Newtonsoft.Json.Formatting.None));
            return node.ToString(SaveOptions.DisableFormatting);
        }

        public string BowdlerizeDictionary(Dictionary<string, string> properties) {
            string json = JsonConvert.SerializeObject(properties, Newtonsoft.Json.Formatting.Indented);
            return BowdlerizeJson(json);
        }

        public string BowdlerizeJson(string json) {
            JToken token = JToken.Parse(json);
            ObscureMatchingValues(token);

            return token.ToString(Newtonsoft.Json.Formatting.None);
        }
        public string BowdlerizeJToken(JToken token) {
            ObscureMatchingValues(token);

            return token.ToString(Newtonsoft.Json.Formatting.None);
        }

        private void ObscureMatchingValues(JToken token) {
            foreach (var rule in rules) {
                foreach (JToken match in token.SelectTokens(rule.Path)) {
                    match.Replace(new JValue(rule.Bowdlerize(match.ToString())));
                }
            }
        }

        public string[] Paths() {
            return rules.Select(x => x.Path).ToArray();
        }
    }
}
