﻿using Xunit;
using Xunit.Abstractions;

namespace SharpGen.E2ETests
{
    public class RenameTests : TestBase
    {
        public RenameTests(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public void MappingNameRuleRenamesStruct()
        {
            var testDirectory = GenerateTestDirectory();
            var config = new Config.ConfigFile
            {
                Namespace = nameof(MappingNameRuleRenamesStruct),
                Assembly = nameof(MappingNameRuleRenamesStruct),
                IncludeDirs = { GetTestFileIncludeRule() },
                Includes =
                {
                    CreateCppFile("simpleStruct", @"
                        struct Test {
                            int field;
                        };
                    ")
                },
                Bindings =
                {
                    new Config.BindRule("int", "System.Int32")
                },
                Mappings =
                {
                    new Config.MappingRule
                    {
                        Struct = "Test",
                        MappingName = "MyStruct"
                    }
                }
            };
            var result = RunWithConfig(config);
            AssertRanSuccessfully(result.success, result.output);

            var compilation = GetCompilationForGeneratedCode();
            var structType = compilation.GetTypeByMetadataName($"{nameof(MappingNameRuleRenamesStruct)}.MyStruct");
            Assert.NotNull(structType);
        }

        [Fact]
        public void MappingNameRuleRenamesStructMember()
        {
            var testDirectory = GenerateTestDirectory();
            var config = new Config.ConfigFile
            {
                Namespace = nameof(MappingNameRuleRenamesStructMember),
                Assembly = nameof(MappingNameRuleRenamesStructMember),
                IncludeDirs = { GetTestFileIncludeRule() },
                Includes =
                {
                    CreateCppFile("simpleStruct", @"
                        struct Test {
                            int field;
                        };
                    ")
                },
                Bindings =
                {
                    new Config.BindRule("int", "System.Int32")
                },
                Mappings =
                {
                    new Config.MappingRule
                    {
                        Field = "Test::field",
                        MappingName = "MyField"
                    }
                }
            };
            var result = RunWithConfig(config);
            AssertRanSuccessfully(result.success, result.output);

            var compilation = GetCompilationForGeneratedCode();
            var structType = compilation.GetTypeByMetadataName($"{nameof(MappingNameRuleRenamesStructMember)}.Test");
            Assert.Single(structType.GetMembers("MyField"));
        }

        [Fact]
        public void MarkingMappingFinalPreservesUnderscore()
        {
            var testDirectory = GenerateTestDirectory();
            var config = new Config.ConfigFile
            {
                Namespace = nameof(MarkingMappingFinalPreservesUnderscore),
                Assembly = nameof(MarkingMappingFinalPreservesUnderscore),
                IncludeDirs = { GetTestFileIncludeRule() },
                Includes =
                {
                    CreateCppFile("simpleStruct", @"
                        struct Test {
                            int _testfield;
                        };
                    ")
                },
                Bindings =
                {
                    new Config.BindRule("int", "System.Int32")
                },
                Mappings =
                {
                    new Config.MappingRule
                    {
                        Field = "Test::_testfield",
                        MappingName = "_field",
                        IsFinalMappingName = true
                    }
                }
            };
            var result = RunWithConfig(config);
            AssertRanSuccessfully(result.success, result.output);

            var compilation = GetCompilationForGeneratedCode();
            var structType = compilation.GetTypeByMetadataName($"{nameof(MarkingMappingFinalPreservesUnderscore)}.Test");
            Assert.Single(structType.GetMembers("_field"));
        }
    }
}
