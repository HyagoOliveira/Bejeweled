using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using NUnit.Framework.Interfaces;
using UnityEditor.SceneManagement;

namespace Bejeweled.Tests
{
    public class LoadSceneAttribute : NUnitAttribute, IOuterUnityTestAction
    {
        private readonly string scene;

        public LoadSceneAttribute(string scene)
        {
            const string suffix = ".unity";

            this.scene = scene;

            var hasSufix = this.scene.EndsWith(suffix);
            if (!hasSufix) this.scene += suffix;
        }

        IEnumerator IOuterUnityTestAction.BeforeTest(ITest test)
        {
            yield return EditorSceneManager.LoadSceneAsyncInPlayMode(scene,
                new LoadSceneParameters(LoadSceneMode.Single));
        }

        IEnumerator IOuterUnityTestAction.AfterTest(ITest test)
        {
            yield return null;
        }
    }
}