
using UnityEngine;

namespace Live2D.Cubism.PortfolioPlugin.Editor
{
    public static class PortfolioGameObjectExtension
    {
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : MonoBehaviour
        {
            return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
        }

        public static T GetOrAddComponent<T>(this MonoBehaviour monoBehaviour) where T : MonoBehaviour
        {
            return monoBehaviour.GetComponent<T>() ?? monoBehaviour.gameObject.AddComponent<T>();
        }
    }
}
