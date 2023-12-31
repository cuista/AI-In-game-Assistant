﻿using System;

namespace NugetForUnity.Models
{
    /// <summary>
    ///     Interface for a versioned NuGet package.
    /// </summary>
    public interface INugetPackageIdentifier : IEquatable<INugetPackageIdentifier>, IComparable<INugetPackageIdentifier>
    {
        /// <summary>
        ///     Gets or sets a value indicating whether this package was installed manually or just as a dependency.
        /// </summary>
        bool IsManuallyInstalled { get; set; }

        /// <summary>
        ///     Gets the ID of the NuGet package.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///     Gets the normalized version number of the NuGet package.
        ///     This is the normalized version number without build-metadata e.g. <b>1.0.0+b3a8</b> is normalized to <b>1.0.0</b>.
        /// </summary>
        string Version { get; }

        /// <summary>
        ///     Gets a value indicating whether the version number specified is a range of values.
        /// </summary>
        bool HasVersionRange { get; }

        /// <summary>
        ///     Gets a value indicating whether this is a prerelease package or an official release package.
        /// </summary>
        bool IsPrerelease { get; }

        /// <summary>
        ///     Gets or sets the typed version number of the NuGet package.
        /// </summary>
        NugetPackageVersion PackageVersion { get; set; }

        /// <summary>
        ///     Gets the name of the '.nupkg' file that contains the whole package content as a ZIP.
        /// </summary>
        string PackageFileName { get; }

        /// <summary>
        ///     Gets the name of the '.nuspec' file that contains metadata of this NuGet package's.
        /// </summary>
        string SpecificationFileName { get; }

        /// <summary>
        ///     Determines if the given <see cref="INugetPackageIdentifier" />'s version <paramref name="otherPackage" /> is in the version range of this
        ///     <see cref="INugetPackageIdentifier" />.
        ///     See here: https://docs.nuget.org/ndocs/create-packages/dependency-versions.
        /// </summary>
        /// <param name="otherPackage">The <see cref="INugetPackageIdentifier" /> whose version to check if is in the range.</param>
        /// <returns>True if the given version is in the range, otherwise false.</returns>
        bool InRange(INugetPackageIdentifier otherPackage);

        /// <summary>
        ///     Determines if the given <paramref name="otherVersion" /> is in the version range of this <see cref="INugetPackageIdentifier" />.
        ///     See here: https://docs.nuget.org/ndocs/create-packages/dependency-versions.
        /// </summary>
        /// <param name="otherVersion">The <see cref="NugetPackageVersion" /> to check if is in the range.</param>
        /// <returns>True if the given version is in the range, otherwise false.</returns>
        bool InRange(NugetPackageVersion otherVersion);
    }
}
