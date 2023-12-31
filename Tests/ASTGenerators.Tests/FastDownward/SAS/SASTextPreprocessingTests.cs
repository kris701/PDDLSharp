﻿using PDDLSharp;
using PDDLSharp.ASTGenerators;
using PDDLSharp.ASTGenerators.FastDownward.SAS;
using PDDLSharp.ASTGenerators.PDDL;
using PDDLSharp.ASTGenerators.Tests;
using PDDLSharp.ASTGenerators.Tests.FastDownward.SAS;
using PDDLSharp.ASTGenerators.Tests.PDDL;
using PDDLSharp.Models.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDDLSharp.ASTGenerators.Tests.FastDownward.SAS
{
    [TestClass]
    public class SASTextPreprocessingTests
    {
        [TestMethod]
        [DataRow("", "")]
        [DataRow("\r", " ")]
        [DataRow("\r \t \r", "     ")]
        [DataRow("\reyayaya\taaaa\r", " eyayaya aaaa ")]
        public void Can_ReplaceSpecialCharacters(string text, string expectedText)
        {
            // ARRANGE
            // ACT
            var result = SASTextPreprocessing.ReplaceSpecialCharacters(text);

            // ASSERT
            Assert.AreEqual(expectedText, result);
        }
    }
}
