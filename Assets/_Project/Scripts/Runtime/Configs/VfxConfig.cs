using System;
using System.Collections.Generic;
using Project.Vfx;
using UnityEngine;

namespace Project.Configs
{
    [Serializable]
    public sealed class VfxEntry
    {
        [SerializeField] private string _id;
        [SerializeField] private Effect _prefab;
        [SerializeField, Min(1)] private int _poolSize = 8;

        public string Id => _id;
        public Effect Prefab => _prefab;
        public int PoolSize => _poolSize;
    }

    [CreateAssetMenu(fileName = "VfxConfig", menuName = "Game/Vfx Config")]
    public sealed class VfxConfig : ScriptableObject
    {
        [SerializeField] private List<VfxEntry> _effects = new();
        [SerializeField, Min(1)] private int _defaultPoolSize = 8;

        public int DefaultPoolSize => _defaultPoolSize;

        public VfxEntry Find(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;

            for (var i = 0; i < _effects.Count; i++)
            {
                var entry = _effects[i];
                if (entry != null && entry.Id == id)
                    return entry;
            }

            return null;
        }
    }
}
