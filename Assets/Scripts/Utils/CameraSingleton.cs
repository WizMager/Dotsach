using UnityEngine;

namespace Utils
{
    public class CameraSingleton : MonoBehaviour
    {
        public static CameraSingleton Instance;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                
                return;
            }

            Instance = this;
        }
    }
}