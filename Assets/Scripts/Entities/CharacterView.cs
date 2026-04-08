using UnityEngine;

namespace BuildingCrusher.Entities
{
    public class CharacterView : MonoBehaviour
    {
        SpriteRenderer _sr;
        public SpriteRenderer SR => _sr ??= GetComponent<SpriteRenderer>();
    }
}
