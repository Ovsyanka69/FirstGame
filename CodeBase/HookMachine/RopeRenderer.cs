using System;
using UnityEngine;

namespace CodeBase.HookMachine
{
    public class RopeRenderer : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer;
        [SerializeField] private int _segments = 20;
        

        private void Start()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        public void DrawLine(Vector3 a, Vector3 b, float length)
        {
            _lineRenderer.enabled = true;

            float interpolant = Vector3.Distance(a, b) / length;
            float offset = Mathf.Lerp(length / 2f, 0f, interpolant);

            Vector3 aDown = a + Vector3.down * offset;
            Vector3 bDown = b + Vector3.down * offset;

            _lineRenderer.positionCount = _segments + 1;
            for (int i = 0; i < _segments + 1; i++)
            {
                _lineRenderer.SetPosition(i, Bezier.GetPoint(a, aDown,bDown, b, (float)i/_segments));
            }

        }

        public void Hide()
        {
            _lineRenderer.enabled = false;
        }
    }
}
