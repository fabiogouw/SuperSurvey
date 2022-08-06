using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;
using Assembly = System.Reflection.Assembly;

namespace SuperSurvey.Architecture.Tests
{
    public class CleanArchitectureTest
    {
        private static readonly Assembly _domainAssembly = Assembly.Load("SuperSurvey.Domain");
        private static readonly Assembly _useCaseAssembly = Assembly.Load("SuperSurvey.UseCases");
        private static readonly Assembly _adaptersAssembly = Assembly.Load("SuperSurvey.Adapters");
        private static readonly Assembly _webAppAssembly = Assembly.Load("SuperSurvey.WebApp");

        private static readonly ArchUnitNET.Domain.Architecture _architecture =
            new ArchLoader().LoadAssemblies(_domainAssembly, _useCaseAssembly, _adaptersAssembly, _webAppAssembly).Build();

        [Fact]
        [Trait("Category", "Architecture")]
        public void AllLayers_Should_BelongToTheirOwnAssembly()
        {
            var assemblyRule = 
                Types().That().ResideInNamespace("^SuperSurvey.Domain", true)
                    .Should().ResideInAssembly(_domainAssembly)
                .And()
                    .Types().That().ResideInNamespace("^SuperSurvey.UseCases", true).Should()
                        .ResideInAssembly(_useCaseAssembly)
                .And()
                    .Types().That().ResideInNamespace("^SuperSurvey.Adapters", true).Should()
                        .ResideInAssembly(_adaptersAssembly)
                .And()
                    .Types().That().ResideInNamespace("^SuperSurvey.WebApp", true).Should()
                        .ResideInAssembly(_webAppAssembly)
                .Because("each layer belongs to its own assembly");
            assemblyRule.Check(_architecture);
        }

        [Fact]
        [Trait("Category", "Architecture")]
        public void DomainLayerTypes_Should_DependOnlyOnCLRTypesAndOwnNamespaces()
        {
            var domainReferencesRule =
                Types().That().ResideInAssembly(_domainAssembly).Should()
                    .OnlyDependOn(Types().That().ResideInNamespace("^System", true)
                        .Or().ResideInNamespace("^SuperSurvey.Domain", true))
                    .Because("domain objects and entities must depend only on the CLR types and their own namespace types");
            domainReferencesRule.Check(_architecture);
        }

        [Fact]
        [Trait("Category", "Architecture")]
        public void UseCaseLayerTypes_Should_DependOnlyOnCLRTypesAndOwnNamespacesAndDomainLayer()
        {
            var useCaseReferencesRule =
                Types().That().ResideInAssembly(_useCaseAssembly).Should()
                    .OnlyDependOn(Types().That().ResideInNamespace("^System", true)
                        .Or().ResideInNamespace("^SuperSurvey.Domain", true)
                        .Or().ResideInNamespace("^SuperSurvey.UseCases", true))
                    .Because("use cases types must depend only on the CLR types, their own namespace types and domain types");
            useCaseReferencesRule.Check(_architecture);
        }

        [Fact]
        [Trait("Category", "Architecture")]
        public void AdapterLayerTypes_Should_DependOnlyOnCLRTypesAndOwnNamespacesAndDomainLayerAndUseCaseLayer()
        {
            var adapterReferencesRule =
                Types().That().ResideInAssembly(_useCaseAssembly).Should()
                    .OnlyDependOn(Types().That().ResideInNamespace("^System", true)
                        .Or().ResideInNamespace("^SuperSurvey.Domain", true)
                        .Or().ResideInNamespace("^SuperSurvey.UseCases", true)
                        .Or().ResideInNamespace("^SuperSurvey.Adapter", true))
                    .Because("adapters must depend only on the CLR types, their own namespace types and domain and use cases types");
            adapterReferencesRule.Check(_architecture);
        }

        [Fact]
        [Trait("Category", "Architecture")]
        public void UseCaseClasses_Should_BelongToUseCaseAssembly()
        {
            IArchRule useCasesClassesInUseCaseNamespace =
                Classes().That().HaveNameContaining("UseCase")
                    .Should().ResideInAssembly(_useCaseAssembly)
                    .Because("UseCase implementations should belong to the SuperSurvey.UseCases project");
            useCasesClassesInUseCaseNamespace.Check(_architecture);
        }
    }
}