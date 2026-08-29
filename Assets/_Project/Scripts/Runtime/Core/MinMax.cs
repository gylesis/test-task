using System;
using UnityEngine;

namespace Project.Core
{
    [Serializable]
    public struct MinMax
    {
        [SerializeField] private float _min;
        [SerializeField] private float _max;

        public MinMax(float min, float max)
        {
            _min = min;
            _max = max;
        }

        public float Min => _min;
        public float Max => Mathf.Max(_min, _max);
        public float Average => (Min + Max) * 0.5f;

        public float Roll()
        {
            return Max <= Min ? Min : UnityEngine.Random.Range(Min, Max);
        }

        public float Lerp(float t)
        {
            return Mathf.Lerp(Min, Max, t);
        }

        public float Clamp(float value)
        {
            return Mathf.Clamp(value, Min, Max);
        }

        public bool Contains(float value)
        {
            return value >= Min && value <= Max;
        }
    }
}
