// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using FMLab.Aspnet.URLShortener.Business.DependencyInjection;
using FMLab.Aspnet.URLShortener.Infrastructure.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace FMLab.Aspnet.URLShortener.Tests.Architecture;

public class ArchitectureTests
{
    private static readonly System.Reflection.Assembly BusinessAssembly = typeof(ApplicationModule).Assembly;

    private static readonly System.Reflection.Assembly InfrastructureAssembly = typeof(InfrastructureModule).Assembly;

    private static readonly System.Reflection.Assembly ApiAssembly = typeof(Program).Assembly;

    private static readonly IObjectProvider<IType> ApiLayer = Types().That().ResideInAssembly(ApiAssembly);
    private static readonly IObjectProvider<IType> BusinessLayer = Types().That().ResideInAssembly(BusinessAssembly);
    private static readonly IObjectProvider<IType> InfrastructureLayer = Types().That().ResideInAssembly(InfrastructureAssembly);

    private static readonly ArchUnitNET.Domain.Architecture Architecture = new ArchLoader()
    .LoadAssemblies(
        BusinessAssembly,
        InfrastructureAssembly,
        ApiAssembly)
    .Build();

    [Fact]
    public void BusinessLayer_ShouldNotDependOn_InfrastructureLayer() =>
        Types().That().Are(BusinessLayer).Should().NotDependOnAny(InfrastructureLayer)
               .Check(Architecture);

    [Fact]
    public void BusinessLayer_ShouldNotDependOn_ApiLayer() =>
        Types().That().Are(BusinessLayer).Should().NotDependOnAny(ApiLayer)
               .Check(Architecture);

    [Fact]
    public void InfrastructureLayer_ShouldNotDependOn_ApiLayer() =>
        Types().That().Are(InfrastructureLayer).Should().NotDependOnAny(ApiLayer)
               .Check(Architecture);

    [Fact]
    public void RepositoryInterfaces_ShouldResideIn_BusinessLayer() =>
        Interfaces().That().HaveNameMatching(".*Repository")
                    .Should().ResideInAssembly(BusinessAssembly)
                    .Because("Abstrasct repositories belong to the Business layer")
                    .Check(Architecture);

    [Fact]
    public void RepositoryImplementations_ShouldResideIn_InfrastructureLayer() =>
        Classes().That().HaveNameMatching(".*Repository")
                 .Should().ResideInAssembly(InfrastructureAssembly)
                 .Because("Concrete repositories belong to the Business layer")
                 .Check(Architecture);

    [Fact]
    public void Services_ShouldResideIn_BusinessLayer() =>
        Classes().That().HaveNameMatching(".*Service")
                 .Should().ResideInAssembly(BusinessAssembly)
                 .Because("Concrete Services belong to the Business layer")
                 .Check(Architecture);
}
