using Services.ObjectPool;
using UI;
using UnityEngine;

namespace Gameplay.GUIWidgets
{
    public class FloatingNumbersGUI : MonoSingleton<FloatingNumbersGUI>
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private FloatingNumberText _floatingNumberText;
        [SerializeField] private Transform _container;
        [SerializeField] private int _poolPrewarmCount;

        private ObjectPool _pool;

        private void Awake()
        {
            InitializeSingleton(false);

            _pool = new ObjectPool(_floatingNumberText.gameObject, Mathf.Max(0, _poolPrewarmCount), _container);
        }

        public void Spawn(Vector3 worldPosition, float amount, Color color)
        {
            var text = _pool.GetFreeElement<FloatingNumberText>();
            Camera camera = _camera != null ? _camera : Camera.main;
            if (camera != null)
            {
                text.transform.rotation = camera.transform.rotation;
            }

            text.Show(worldPosition, Mathf.CeilToInt(amount).ToString(), color);
        }
    }
}
