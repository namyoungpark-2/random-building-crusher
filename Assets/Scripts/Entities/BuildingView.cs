using UnityEngine;

namespace BuildingCrusher.Entities
{
    public class BuildingView : MonoBehaviour
    {
        SpriteRenderer _sr;
        public SpriteRenderer SR => _sr ??= GetComponent<SpriteRenderer>();
        public void SetColor(Color c) => SR.color = c;
        public void SetSize(Vector3 scale) => transform.localScale = scale;
    }
}
