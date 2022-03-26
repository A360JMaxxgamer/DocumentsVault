﻿using System;
using System.Threading.Tasks;
using Files.Service.GraphQL;
using Files.Service.Handlers;
using Files.Service.Models;
using Moq;
using UnitTestHelper.Extensions;
using Xunit;

namespace Files.Service.Tests.GraphQL;

public class QueryTest
{
    [Fact]
    public async Task GetDocumentDownloadUrl_Should_Return_PreSignedUrl()
    {
        // Arrange
        var query = new Query();
        var fileUrlProvider = new Mock<IFileUrlProvider>();
        fileUrlProvider
            .SetupReturn(f => f.GetPreSignedDownloadUrl(It.IsAny<Guid>()),
                Task.FromResult(new PreSignedUrl()));

        // Act
        var result = await query.GetDocumentDownloadUrl(fileUrlProvider.Object, Guid.NewGuid());

        // Assert
        Assert.NotNull(result);
    }
}