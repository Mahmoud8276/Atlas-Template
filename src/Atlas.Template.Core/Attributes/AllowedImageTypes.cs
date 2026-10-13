using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace Pharmacy.System.Core.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter)]
public sealed class AllowedImageTypesAttribute : ValidationAttribute
{
    private readonly HashSet<string> _allowedExtensions;

    public AllowedImageTypesAttribute(params string[] extensions)
    {
        _allowedExtensions = extensions
            .Select(ext => ext.StartsWith('.')
                ? ext.ToLowerInvariant()
                : $".{ext.ToLowerInvariant()}")
            .ToHashSet();

        if (_allowedExtensions.Count == 0)
            throw new ArgumentException(
                "At least one image extension must be specified.",
                nameof(extensions));
    }

    public override bool IsValid(object? value)
    {
        // Optional image: null is valid.
        if (value is null)
            return true;

        if (value is not IFormFile file)
            return false;

        // An empty upload is treated as no file.
        if (file.Length == 0)
            return true;

        var extension = Path.GetExtension(file.FileName);

        return !string.IsNullOrWhiteSpace(extension)
            && _allowedExtensions.Contains(extension.ToLowerInvariant());
    }

    public override string FormatErrorMessage(string name)
    {
        var extensions = string.Join(", ", _allowedExtensions.Order());

        return $"{name} must be an image with one of these extensions: {extensions}.";
    }
}