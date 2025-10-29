/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;

namespace Meta.XR.Simulator.Editor.SyntheticEnvironments
{
    /// <summary>
    /// Registry of available <see cref="SyntheticEnvironment"/>s.
    /// </summary>
    /// <remarks>Call <see cref="Register"/> to add additional <see cref="SyntheticEnvironment"/>s
    /// to this registry.</remarks>
    /// <seealso cref="Register"/>
    /// <seealso cref="SyntheticEnvironment"/>
    [InitializeOnLoad]
    internal static class Registry
    {
        internal static string[] Names;
        internal static readonly List<SyntheticEnvironment> RegisteredEnvironments = new();
        private static bool _dirtyRegistry = true;

#if UNITY_EDITOR_OSX
        private static readonly Regex EnvScriptArgs = new(@"open -n (\S+) --args (\S+)");
        private const string ScriptExtension = "*.sh";
#else
        private static readonly Regex EnvScriptArgs = new(@"start (\S+) (\S+)");
        private const string ScriptExtension = "*.bat";
#endif

        public static event Action<SyntheticEnvironment> OnEnvironmentRegistered;

        /// <summary>
        /// Get a registered <see cref="SyntheticEnvironment"/> from its public name.
        /// </summary>
        /// <param name="name">The public name (shown in settings) of the <see cref="SyntheticEnvironment"/></param>
        /// <returns>The <see cref="SyntheticEnvironment"/> matching the name, or <code>null</code></returns>
        public static SyntheticEnvironment GetByName(string name)
            => RegisteredEnvironments.FirstOrDefault(environment => environment.Name == name);

        /// <summary>
        /// Get a registered <see cref="SyntheticEnvironment"/> from its internal name.
        /// </summary>
        /// <param name="internalName">The internal name (shown in settings) of the <see cref="SyntheticEnvironment"/></param>
        /// <returns>The <see cref="SyntheticEnvironment"/> matching the name, or <code>null</code></returns>
        public static SyntheticEnvironment GetByInternalName(string internalName)
            => RegisteredEnvironments.FirstOrDefault(room => room.InternalName == internalName);

        internal static SyntheticEnvironment GetByIndex(int index)
            => RegisteredEnvironments.ElementAt(index);

        /// <summary>
        /// Registers a <see cref="SyntheticEnvironment"/> to the registry.
        /// This will make it available in the settings.
        /// </summary>
        /// <param name="environment">The <see cref="SyntheticEnvironment"/> to register.</param>
        public static void Register(SyntheticEnvironment environment)
        {
            RegisteredEnvironments.Add(environment);
            _dirtyRegistry = true;
            OnEnvironmentRegistered?.Invoke(environment);
        }

        internal static void RefreshNames()
        {
            if (!_dirtyRegistry) return;

            Names = RegisteredEnvironments.Select(environment => environment.Name).ToArray();
            _dirtyRegistry = false;
        }

        static Registry()
        {
            if (Installer.IsInstalled)
            {
                RegisterSyntheticEnvironments();
            }
            else
            {
                Installer.OnInstalled += RegisterSyntheticEnvironments;
            }
        }

        private static SyntheticEnvironment ParseSynthEnvScript(string file)
        {
            StreamReader reader = File.OpenText(file);
            while (reader.ReadLine() is { } line)
            {
                var match = EnvScriptArgs.Match(line);
                if (!match.Success)
                {
                    continue;
                }

                return new SyntheticEnvironment
                {
                    Name = Path.GetFileNameWithoutExtension(file),
                    ServerBinaryPath = match.Groups[1].Value,
                    InternalName = match.Groups[2].Value,
                };
            }
            return null;
        }

        private static void RegisterSyntheticEnvironments()
        {
            var synthEnvs = new List<SyntheticEnvironment>();

            if (!Directory.Exists(XRSimConstants.XrSimDataFolderPath))
            {
                return;
            }

            // Find all scripts in the package and create a SyntheticEnvironment for each.
            foreach (var dir in Directory.EnumerateDirectories(XRSimConstants.XrSimDataFolderPath))
            {
                foreach (var file in Directory.EnumerateFiles(dir, ScriptExtension, SearchOption.AllDirectories))
                {
                    // Parse the file
                    var env = ParseSynthEnvScript(file);
                    if (env == null) { continue; }

                    env.ServerBinaryPath = Path.Join(dir, env.ServerBinaryPath);
                    synthEnvs.Add(env);
                }
            }

            synthEnvs.ForEach(Register);
        }

        public static void RefreshRegistryOnVersionChange()
        {
            RegisteredEnvironments.Clear();
            RegisterSyntheticEnvironments();
        }
    }
}
