﻿using ExampleAnalyzer1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Roslyn.SonarQube;
using System.Linq;
using Tests.Common;

namespace RuleGeneratorTests
{
    [TestClass]
    public class RulesGeneratorTests
    {
        public TestContext TestContext { get; set; }

        #region Tests

        [TestMethod]
        public void RuleGen_SimpleRules()
        {
            // Arrange
            TestLogger logger = new TestLogger();
            ConfigurableAnalyzer analyzer = new ConfigurableAnalyzer();
            var diagnostic1 = analyzer.RegisterDiagnostic(key: "DiagnosticID1", description: "Some description", helpLinkUri: "www.bing.com", tags: new[] { "unnecessary" });
            var diagnostic2 = analyzer.RegisterDiagnostic(key: "Diagnostic2", description: "");

            IRuleGenerator generator = new RuleGenerator(logger);

            // Act
            Rules rules = generator.GenerateRules(new[] { analyzer });

            // Assert
            AssertExpectedRuleCount(2, rules);

            Rule rule1 = rules.Single(r => r.Key == diagnostic1.Id);
            VerifyRule(diagnostic1, rule1);

            Assert.IsTrue(rule1.Description.Contains(diagnostic1.Description.ToString()), "Invalid rule description");
            Assert.IsTrue(rule1.Description.Contains(diagnostic1.HelpLinkUri), "Invalid rule description");
            Assert.IsFalse(rule1.Description.Trim().StartsWith("<![CDATA"), "Description should not be formatted as a CData section");

            Rule rule2 = rules.Single(r => r.Key == diagnostic2.Id);
            VerifyRule(diagnostic2, rule2);

            Assert.IsTrue(rule2.Description.Contains(Resources.NoDescription), "Invalid rule description");
        }

        #endregion Tests

        #region Checks

        private static void ValidateRule(Rules rules, string expectedKey, string[] expectedTags)
        {
            Rule rule = rules.SingleOrDefault(r => r.Key == expectedKey);
            Assert.IsNotNull(rule, "No rule found with the Key " + expectedKey);
            CollectionAssert.AreEquivalent(rule.Tags, expectedTags, "Mismatch in rule tags");
        }

        private static void AssertExpectedRuleCount(int expected, Rules rules)
        {
            Assert.IsNotNull(rules, "Generated rules list should not be null");
            Assert.AreEqual(expected, rules.Count, "Unexpected number of rules");
        }

        private static void VerifyRule(Microsoft.CodeAnalysis.DiagnosticDescriptor diagnostic, Rule rule)
        {
            Assert.AreEqual(diagnostic.Id, rule.Key, "Invalid rule key");
            Assert.AreEqual(diagnostic.Id, rule.InternalKey, "Invalid rule internal key");
            Assert.AreEqual(RuleGenerator.Cardinality, rule.Cardinality, "Invalid rule cardinality");
            Assert.AreEqual(RuleGenerator.Status, rule.Status, "Invalid rule status");

            Assert.AreEqual(diagnostic.Title.ToString(), rule.Name, "Invalid rule name");
            Assert.IsNull(rule.Tags, "No tags information should be derived from the diagnostics");
        }

        #endregion Checks
    }
}