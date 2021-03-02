using System.IO;
using UnityEditor;
using UnityEngine;

namespace Bejeweled.Tests
{
    /// <summary>
    /// Utility class for Testing.
    /// </summary>
    public static class TestUtility
    {
        /// <summary>
        /// Finds the prefab path inside Project using the given name.
        /// </summary>
        /// <param name="name">The prefab name to find.</param>
        /// <param name="throwException">If true, it'll throw a FileNotFoundException if prefab was not found.</param>
        /// <returns>A prefab GameObject or null if none was found.</returns>
        public static GameObject FindPrefab(string name, bool throwException = true)
        {
            var prefabPath = FindAssetPath(name, "Prefab", throwException);
            var hasPrefabPath = !string.IsNullOrEmpty(prefabPath);
            return hasPrefabPath ? PrefabUtility.LoadPrefabContents(prefabPath) : null;
        }

        /// <summary>
        /// Finds the asset path inside Project using the given name and type.
        /// </summary>
        /// <param name="name">The asset name to find.</param>
        /// <param name="type">The asset type to find.</param>
        /// <param name="throwException">If true, it'll throw a FileNotFoundException if asset was not found.</param>
        /// <returns>A valid asset path starting from 'Assets/' or empty if none was found.</returns>
        public static string FindAssetPath(string name, string type, bool throwException = true)
        {
            var filter = $"{name} t:{type}";
            var guids = AssetDatabase.FindAssets(filter);
            var hasPath = guids != null && guids.Length > 0;

            if (throwException && !hasPath)
            {
                var message = $"File '{name}' was not found on the Project. Make sure you have one.";
                throw new FileNotFoundException(message, name);
            }

            var guid = hasPath ? guids[0] : string.Empty;
            return AssetDatabase.GUIDToAssetPath(guid);
        }
    }
}
