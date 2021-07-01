using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Cortside.Bowdlerizer.Test.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Cortside.Bowdlerizer.Test {
    public class BowdlerizerTest {
        private readonly Bowdlerizer bowdlerizer;

        public BowdlerizerTest() {
            this.bowdlerizer = new Bowdlerizer(new List<BowdlerizerRule> {
                new BowdlerizerRule() { Path = "$..SocialSecurityNum", Strategy=new BowdlerizerTailStrategy(0) },
                new BowdlerizerRule() { Path = "$..SSN", Strategy=new BowdlerizerTailStrategy(4) },
                new BowdlerizerRule() { Path = "$..['SearchBy.SSN']", Strategy=new BowdlerizerTailStrategy(4) },
                new BowdlerizerRule() { Path = "$..PhoneNumber"},
                new BowdlerizerRule() { Path = "$..Phone"},
                new BowdlerizerRule() { Path = "$..MailingAddress.Address1", Strategy=new BowdlerizerHeadStrategy(4) },
                new BowdlerizerRule() { Path = "$..MailingAddress.Address2", Strategy=new BowdlerizerHeadStrategy(4) },
                new BowdlerizerRule() { Path = "$..BorrowerFName", Strategy=new BowdlerizerHeadStrategy(0) }
            });
        }

        [Fact]
        public void ShouldBowdlerizePerson() {
            var person = new Person() { BorrowerFName = "Chester", SocialSecurityNum = "324324324", PhoneNumber = "8015551212", MailingAddress = new Address() { Address1 = "1234 Main Street", Address2 = "Suite 300" } };
            var result = JsonConvert.DeserializeObject<Person>(bowdlerizer.BowdlerizeObject(person));
            Assert.Equal("***", result.SocialSecurityNum);
            Assert.Equal("8*******12", result.PhoneNumber);
            Assert.Equal("1234***", result.MailingAddress.Address1);
            Assert.Equal("Suit***", result.MailingAddress.Address2);
            Assert.Equal("***", result.BorrowerFName);
        }

        [Fact]
        public void ShouldBowdlerizeXml() {
            var xml = "<?xml version=\"1.0\" encoding=\"UTF - 8\"?><VerificationOfOccupancyResponseEx><response><Header><Status>0</Status></Header><Result><InputEcho><Name><First>ALYSON</First><Last>ABELEDA</Last></Name><Address><StreetAddress1>4400 HORNER ST UNIT 4</StreetAddress1><City>Union City</City><State>CA</State><Zip5>94587</Zip5></Address><Phone>7542298653</Phone><SSN>612527378</SSN></InputEcho><UniqueId>0</UniqueId><AttributeGroup><Name>VOOATTRV1</Name><Attributes><Attribute><Name>AddressReportingSourceIndex</Name></Attribute><Attribute><Name>AddressReportingHistoryIndex</Name></Attribute><Attribute><Name>AddressSearchHistoryIndex</Name></Attribute><Attribute><Name>AddressUtilityHistoryIndex</Name></Attribute><Attribute><Name>AddressOwnershipHistoryIndex</Name></Attribute><Attribute><Name>AddressPropertyTypeIndex</Name></Attribute><Attribute><Name>AddressValidityIndex</Name></Attribute><Attribute><Name>RelativesConfirmingAddressIndex</Name></Attribute><Attribute><Name>AddressOwnerMailingAddressIndex</Name></Attribute><Attribute><Name>PriorAddressMoveIndex</Name></Attribute><Attribute><Name>PriorResidentMoveIndex</Name></Attribute><Attribute><Name>AddressDateFirstSeen</Name></Attribute><Attribute><Name>AddressDateLastSeen</Name></Attribute><Attribute><Name>OccupancyOverride</Name></Attribute><Attribute><Name>InferredOwnershipTypeIndex</Name></Attribute><Attribute><Name>OtherOwnedPropertyProximity</Name></Attribute><Attribute><Name>VerificationOfOccupancyScore</Name></Attribute></Attributes></AttributeGroup></Result></response></VerificationOfOccupancyResponseEx>";
            var result = bowdlerizer.BowdlerizeXml(xml);
            Assert.DoesNotContain("612527378", result);
            Assert.DoesNotContain("7542298653", result);
        }

        [Fact]
        public void XmlToJsonToXml() {
            var xml = "<VerificationOfOccupancyResponseEx><response><Header><Status>0</Status></Header><Result><InputEcho><Name><First>ALYSON</First><Last>ABELEDA</Last></Name><Address><StreetAddress1>4400 HORNER ST UNIT 4</StreetAddress1><City>Union City</City><State>CA</State><Zip5>94587</Zip5></Address><Phone>7542298653</Phone><SSN>612527378</SSN></InputEcho><UniqueId>0</UniqueId><AttributeGroup><Name>VOOATTRV1</Name><Attributes><Attribute><Name>AddressReportingSourceIndex</Name></Attribute><Attribute><Name>AddressReportingHistoryIndex</Name></Attribute><Attribute><Name>AddressSearchHistoryIndex</Name></Attribute><Attribute><Name>AddressUtilityHistoryIndex</Name></Attribute><Attribute><Name>AddressOwnershipHistoryIndex</Name></Attribute><Attribute><Name>AddressPropertyTypeIndex</Name></Attribute><Attribute><Name>AddressValidityIndex</Name></Attribute><Attribute><Name>RelativesConfirmingAddressIndex</Name></Attribute><Attribute><Name>AddressOwnerMailingAddressIndex</Name></Attribute><Attribute><Name>PriorAddressMoveIndex</Name></Attribute><Attribute><Name>PriorResidentMoveIndex</Name></Attribute><Attribute><Name>AddressDateFirstSeen</Name></Attribute><Attribute><Name>AddressDateLastSeen</Name></Attribute><Attribute><Name>OccupancyOverride</Name></Attribute><Attribute><Name>InferredOwnershipTypeIndex</Name></Attribute><Attribute><Name>OtherOwnedPropertyProximity</Name></Attribute><Attribute><Name>VerificationOfOccupancyScore</Name></Attribute></Attributes></AttributeGroup></Result></response></VerificationOfOccupancyResponseEx>";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            var json = JsonConvert.SerializeXmlNode(doc);
            JToken token = JToken.Parse(json);
            var s = token.ToString(Newtonsoft.Json.Formatting.None);
            XNode node = JsonConvert.DeserializeXNode(s);

            Assert.Equal(xml, node.ToString(SaveOptions.DisableFormatting));
        }

        [Fact]
        public void ShouldRedactDictionary() {
            var dict = new Dictionary<string, string>();
            dict.Add("SearchBy.SSN", "123456789");
            var result = bowdlerizer.BowdlerizeDictionary(dict);

            Assert.DoesNotContain("123456789", result);
        }

        [Fact]
        public void DefaultStrategyShouldHandleNull() {
            var s = new BowdlerizerDefaultStrategy();
            Assert.Null(s.Bowdlerize(null));
        }

        [Fact]
        public void DefaultStrategyShouldHandleEmptyString() {
            var s = new BowdlerizerDefaultStrategy();
            Assert.Empty(s.Bowdlerize(string.Empty));
        }

        [Fact]
        public void HeadStrategyShouldHandleNull() {
            var s = new BowdlerizerHeadStrategy(4);
            Assert.Null(s.Bowdlerize(null));
        }

        [Fact]
        public void HeadStrategyShouldHandleEmptyString() {
            var s = new BowdlerizerHeadStrategy(4);
            Assert.Empty(s.Bowdlerize(string.Empty));
        }

        [Fact]
        public void HeadStrategyShouldHandleShort() {
            var s = new BowdlerizerHeadStrategy(4);
            Assert.Equal("a***", s.Bowdlerize("abc"));
        }

        [Fact]
        public void TailStrategyShouldHandleNull() {
            var s = new BowdlerizerTailStrategy(4);
            Assert.Null(s.Bowdlerize(null));
        }

        [Fact]
        public void TailStrategyShouldHandleEmptyString() {
            var s = new BowdlerizerTailStrategy(4);
            Assert.Empty(s.Bowdlerize(string.Empty));
        }

        [Fact]
        public void TailStrategyShouldHandleShort() {
            var s = new BowdlerizerTailStrategy(4);
            Assert.Equal("***c", s.Bowdlerize("abc"));
        }

    }
}
