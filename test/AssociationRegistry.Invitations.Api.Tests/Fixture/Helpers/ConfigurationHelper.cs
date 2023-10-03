﻿using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace AssociationRegistry.Invitations.Api.Tests.Fixture.Helpers;

public class ConfigurationHelper
{
    public static IConfigurationRoot GetConfiguration()
        => new ConfigurationBuilder()
            .SetBasePath(GetRootDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true)
            .AddUserSecrets<Program>()
            .AddEnvironmentVariables()
            .Build();

    private static string GetRootDirectory()
    {
        var maybeRootDirectory = Directory
            .GetParent(typeof(Program).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;
        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");
        return rootDirectory;
    }
}
