﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.FSharp.Compiler.SourceCodeServices;

namespace MirrorSharp.FSharp.Advanced {
    /// <summary>Provides Roslyn-like extensions that allow simple updates to <see cref="FSharpProjectOptions" />.</summary>
    public static class FSharpProjectOptionsExtensions {
        /// <summary>
        /// Returns a new instance of <see cref="FSharpProjectOptions" /> with 
        /// <see cref="FSharpProjectOptions.OtherOptions"/> <c>--debug</c> option set to the provided value; if 
        /// it already matches the provided value, returns <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to use as a base for the returned value.</param>
        /// <param name="debug">The new value for the <c>--debug</c> option; if <c>null</c>, means option should be removed.</param>
        /// <returns>
        /// Either a new instance of <see cref="FSharpProjectOptions" /> with the option set; or <paramref name="options" />
        /// if it already matches the provided value.
        /// </returns>
        [NotNull]
        public static FSharpProjectOptions WithOtherOptionDebug([NotNull] this FSharpProjectOptions options, [CanBeNull] bool? debug) {
            return options.WithOtherOptions(
                options.OtherOptions.WithSwitch("--debug", debug)
            );
        }

        /// <summary>
        /// Returns a new instance of <see cref="FSharpProjectOptions" /> with 
        /// <see cref="FSharpProjectOptions.OtherOptions"/> <c>--optimize</c> option set to the provided value; if 
        /// it already matches the provided value, returns <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to use as a base for the returned value.</param>
        /// <param name="optimize">The new value for the <c>--optimize</c> option; if <c>null</c>, means option should be removed.</param>
        /// <returns>
        /// Either a new instance of <see cref="FSharpProjectOptions" /> with the option set; or <paramref name="options" />
        /// if it already matches the provided value.
        /// </returns>
        [NotNull]
        public static FSharpProjectOptions WithOtherOptionOptimize([NotNull] this FSharpProjectOptions options, [CanBeNull] bool? optimize) {
            return options.WithOtherOptions(
                options.OtherOptions.WithSwitch("--optimize", optimize)
            );
        }

        /// <summary>
        /// Returns a new instance of <see cref="FSharpProjectOptions" /> with 
        /// <see cref="FSharpProjectOptions.OtherOptions"/> <c>--target</c> option set to the provided value; if 
        /// it already matches the provided value, returns <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to use as a base for the returned value.</param>
        /// <param name="target">The new value for the <c>--target</c> option; if <c>null</c>, means option should be removed.</param>
        /// <returns>
        /// Either a new instance of <see cref="FSharpProjectOptions" /> with the option set; or <paramref name="options" />
        /// if it already matches the provided value.
        /// </returns>
        [NotNull]
        public static FSharpProjectOptions WithOtherOptionTarget([NotNull] this FSharpProjectOptions options, [CanBeNull] string target) {
            return options.WithOtherOptions(
                options.OtherOptions.With("--target:", target)
            );
        }

        /// <summary>
        /// Returns a new instance of <see cref="FSharpProjectOptions" /> with 
        /// <see cref="FSharpProjectOptions.OtherOptions"/> set to the provided value; if 
        /// it is already the same as the provided value, returns <paramref name="options" />.
        /// </summary>
        /// <param name="options">The options to use as a base for the returned value.</param>
        /// <param name="otherOptions">The new value for <see cref="FSharpProjectOptions.OtherOptions" />.</param>
        /// <returns>
        /// Either a new instance of <see cref="FSharpProjectOptions" /> with <see cref="FSharpProjectOptions.OtherOptions" /> set;
        /// or <paramref name="options" /> if it already includes the provided value.
        /// </returns>
        [NotNull]
        public static FSharpProjectOptions WithOtherOptions([NotNull] this FSharpProjectOptions options, [NotNull] string[] otherOptions) {
            Argument.NotNull(nameof(options), options);
            Argument.NotNull(nameof(otherOptions), otherOptions);

            if (options.OtherOptions == otherOptions)
                return options;

            return new FSharpProjectOptions(
                options.ProjectFileName,
                options.ProjectFileNames,
                otherOptions,
                options.ReferencedProjects,
                options.IsIncompleteTypeCheckEnvironment,
                options.UseScriptResolutionRules,
                options.LoadTime,
                options.UnresolvedReferences,
                options.OriginalLoadReferences,
                options.ExtraProjectInfo
            );
        }

        private static string[] WithSwitch(this string[] otherOptions, string prefix, bool? value) {
            var valueString = value != null ? (value.Value ? "+" : "-") : null;
            return otherOptions.With(prefix, valueString);
        }

        private static string[] With(this string[] otherOptions, string prefix, string value) {
            // generally options are not changed often, so it's OK to use LINQ here
            if (value == null) {
                // need to remove the item if it exists
                if (!Array.Exists(otherOptions, o => o.StartsWith(prefix)))
                    return otherOptions;
                return Array.FindAll(otherOptions, o => !o.StartsWith(prefix));
            }

            var newOption = prefix + value;
            if (Array.IndexOf(otherOptions, newOption) > -1)
                return otherOptions;

            var newOptions = new List<string>();
            var newAdded = false;
            foreach (var option in otherOptions) {
                if (option.StartsWith(prefix)) {
                    newOptions.Add(newOption);
                    newAdded = true;
                    continue;
                }
                newOptions.Add(option);
            }
            if (!newAdded)
                newOptions.Add(newOption);
            return newOptions.ToArray();
        }
    }
}