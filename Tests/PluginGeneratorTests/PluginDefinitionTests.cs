﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PluginGenerator;
using System.IO;
using Tests.Common;

namespace PluginGeneratorTests
{
    [TestClass]
    public class PluginDefinitionTests
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Defn_Serialization()
        {
            string testDir = TestUtils.CreateTestDirectory(this.TestContext);
            string filePath = Path.Combine(testDir, "defn1.txt");

            PluginDefinition originalDefn = new PluginDefinition()
            {
                Name = "name",
                Class = "class",
                Description = "description",
                Key = "key"
            };

            originalDefn.Save(filePath);
            Assert.IsTrue(File.Exists(filePath), "File was not created: {0}", filePath);

            PluginDefinition reloadedDefn = PluginDefinition.Load(filePath);

            Assert.IsNotNull(reloadedDefn, "Reloaded object should not be null");
            Assert.AreEqual("name", reloadedDefn.Name, "Unexpected name");
            Assert.AreEqual("class", reloadedDefn.Class, "Unexpected class");
            Assert.AreEqual("description", reloadedDefn.Description, "Unexpected description");
            Assert.AreEqual("key", reloadedDefn.Key, "Unexpected key");

            Assert.AreEqual(null, reloadedDefn.IssueTrackerUrl, "Unexpected issue tracker url");
            Assert.AreEqual(null, reloadedDefn.License, "Unexpected license");

        }
    }
}