using UnityEngine;
using TMPro;

namespace BuildingCrusher.Entities
{
    public class FloatingTextView : MonoBehaviour
    {
        TextMeshPro _tmp;
        public int Uid { get; set; }

        void Awake() => _tmp = GetComponent<TextMeshPro>();

        public void Setup(string text, bool isCrit, Vector3 pos)
        {
            if (_tmp == null) _tmp = GetComponent<TextMeshPro>();
            _tmp.text = text;
            _tmp.color = isCrit ? Color.red : Color.white;
            _tmp.fontSize = isCrit ? 5f : 3.5f;
            transform.position = pos;
        }

        public void UpdatePosition(Vector3 pos) => transform.position = pos;
    }
}
