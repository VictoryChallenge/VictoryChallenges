using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VictoryChallenge.Customize
{
    public class PlayerCharacterChangeMesh : MonoBehaviour
    {
        [SerializeField] private BodyPartData[] _bodyPartDataArray;

        public enum BodyPartType
        {
            Color,
            BodyParts,
            Eyes,
            Gloves,
            HeadParts,
            Mouth,
            Tails,
        }

        [System.Serializable]
        public class BodyPartData
        {
            public BodyPartType bodyPartType;
            public Mesh[] meshArray;
            public MeshFilter meshFilter;
        }

        public void ChangeBodyPart(BodyPartType bodyPartType)
        {
            BodyPartData bodyPartData = GetBodyPartData(bodyPartType);
            int meshIndex = System.Array.IndexOf(bodyPartData.meshArray, bodyPartData.meshFilter.sharedMesh);
            bodyPartData.meshFilter.sharedMesh = bodyPartData.meshArray[(meshIndex + 1) % bodyPartData.meshArray.Length];
        }

        private BodyPartData GetBodyPartData(BodyPartType bodyPartType)
        {
            foreach (BodyPartData bodyPartData in _bodyPartDataArray)
            {
                if (bodyPartData.bodyPartType == bodyPartType)
                {
                    return bodyPartData;
                }
            }
            return null;
        }
    }
}
