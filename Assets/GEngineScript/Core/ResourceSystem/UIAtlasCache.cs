using System.Collections.Generic;
using UnityEngine;

namespace GEX.Resource
{
    public class UIAtlasCache
    {
        private Dictionary<string, Sprite> atlas;

        private int _refCount = 0;

        public UIAtlasCache(Sprite[] sprites)
        {
            atlas = new Dictionary<string, Sprite>();

            for (int i = 0; i < sprites.Length; i++)
            {
                atlas[sprites[i].name] = sprites[i];
            }
        }


        public Sprite GetSprite(string spriteName)
        {
            Sprite spt = null;
            atlas.TryGetValue(spriteName, out spt);
            _refCount++;
            return spt;
        }


        public bool Contains(string spriteName)
        {
            return atlas.ContainsKey(spriteName);
        }

        public bool Unload()
        {
            _refCount--;
            return _refCount <= 0;
        }

        public void Load()
        {
            _refCount++;
        }
    }
}