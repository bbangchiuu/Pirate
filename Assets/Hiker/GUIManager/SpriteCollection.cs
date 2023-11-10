using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Hiker.GUI
{
    [CreateAssetMenu(fileName = "SpritesCollection", menuName = "Hiker/Sprite Collection")]
    public class SpriteCollection : ScriptableObject
    {
        public Sprite[] sprites;

        public Sprite GetSprite(string spriteName)
        {
            if (sprites == null || sprites.Length == 0) return null;
            return System.Array.Find(sprites, e => e != null && e.name == spriteName);
        }
    }
}


