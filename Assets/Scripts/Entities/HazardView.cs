using UnityEngine;

namespace BuildingCrusher.Entities
{
    public class HazardView : MonoBehaviour
    {
        SpriteRenderer _sr;
        public SpriteRenderer SR => _sr ??= GetComponent<SpriteRenderer>();
        public int Uid { get; set; }
    }
}
