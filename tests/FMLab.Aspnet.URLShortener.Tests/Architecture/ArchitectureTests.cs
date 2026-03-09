// API - URL Shortener
// Copyright (c) 2026 Fagner Marinho 
// Licensed under the MIT License. See LICENSE file in the project root for details.

using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using ArchUnitNET.xUnit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace FMLab.Aspnet.URLShortener.Tests.Architecture;

public class ArchitectureTests
{
    private static readonly ArchUnitNET.Domain.Architecture _architecture = new ArchLoader()
        .LoadAssemblies(
            typeof(Business.Services.URL.UrlService).Assembly,
            typeof(Infrastructure.Persistence.Repositories.UrlRepository).Assembly,
            typeof(Api.Endpoints.URL.UrlEndpoints).Assembly
        )
        .Build();

    private static readonly IObjectProvider<IType> ApiLayer =
        Types().That().ResideInNamespace("FMLab.Aspnet.URLShortener.Api")
               .As("Api Layer");

    private static readonly IObjectProvider<IType> BusinessLayer =
        Types().That().ResideInNamespace("FMLab.Aspnet.URLShortener.Business")
               .As("Business Layer");

    private static readonly IObjectProvider<IType> InfrastructureLayer =
        Types().That().ResideInNamespace("FMLab.Aspnet.URLShortener.Infrastructure")
               .As("Infrastructure Layer");

    [Fact]
    public void BusinessLayer_ShouldNotDependOn_InfrastructureLayer() =>
        Types().That().Are(BusinessLayer).Should().NotDependOnAny(InfrastructureLayer)
               .Check(_architecture);

    [Fact]
    public void BusinessLayer_ShouldNotDependOn_ApiLayer() =>
        Types().That().Are(BusinessLayer).Should().NotDependOnAny(ApiLayer)
               .Check(_architecture);

    [Fact]
    public void InfrastructureLayer_ShouldNotDependOn_ApiLayer() =>
        Types().That().Are(InfrastructureLayer).Should().NotDependOnAny(ApiLayer)
               .Check(_architecture);

    [Fact]
    public void RepositoryInterfaces_ShouldResideIn_BusinessLayer() =>
        Interfaces().That().HaveNameEndingWith("Repository")
                    .Should().ResideInNamespace("FMLab.Aspnet.URLShortener.Business")
                    .Check(_architecture);

    [Fact]
    public void RepositoryImplementations_ShouldResideIn_InfrastructureLayer() =>
        Classes().That().HaveNameEndingWith("Repository")
                 .Should().ResideInNamespace("FMLab.Aspnet.URLShortener.Infrastructure")
                 .Check(_architecture);

    [Fact]
    public void QueryInterfaces_ShouldResideIn_BusinessLayer() =>
        Interfaces().That().HaveNameEndingWith("Query")
                    .Should().ResideInNamespace("FMLab.Aspnet.URLShortener.Business")
                    .Check(_architecture);

    [Fact]
    public void QueryImplementations_ShouldResideIn_InfrastructureLayer() =>
        Classes().That().HaveNameEndingWith("Query")
                 .Should().ResideInNamespace("FMLab.Aspnet.URLShortener.Infrastructure")
                 .Check(_architecture);

    [Fact]
    public void Services_ShouldResideIn_BusinessLayer() =>
        Classes().That().HaveNameEndingWith("Service")
                 .Should().ResideInNamespace("FMLab.Aspnet.URLShortener.Business")
                 .Check(_architecture);
}
