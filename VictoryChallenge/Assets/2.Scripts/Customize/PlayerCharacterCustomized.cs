using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static VictoryChallenge.Customize.PlayerCharacterChangeMesh;

namespace VictoryChallenge.Customize
{
    public class PlayerCharacterCustomized : MonoBehaviour
    {
        private const string PLAYER_PREFS_SAVE = "PlayerCustomization";

        [SerializeField] private SkinnedBodyPartData[] _skinnedBodyPartDataArray;
        [SerializeField] private MeshBodyPartData[] _meshBodyPartDataArray;

        public enum BodyPartType
        {
            Color,
            BodyParts,
            Eyes,
            Gloves,
            HeadParts,
            Mouth,
            Tails,
            Ear,
            Eyes2,
            Hat,
        }

        [System.Serializable]
        public class SkinnedBodyPartData
        {
            public BodyPartType bodyPartType;
            public Mesh[] meshArray;
            public SkinnedMeshRenderer skinnedMeshRenderer;
        }

        [System.Serializable]
        public class MeshBodyPartData
        {
            public BodyPartType bodyPartType;
            public Mesh[] meshArray;
            public MeshFilter meshFilter;
            public Transform[] transformArray;
        }

        public void ChangeSkinnedBodyPart(BodyPartType bodyPartType)
        {
            SkinnedBodyPartData bodyPartData = GetSkinnedBodyPartData(bodyPartType);
            int meshIndex = System.Array.IndexOf(bodyPartData.meshArray, bodyPartData.skinnedMeshRenderer.sharedMesh);
            bodyPartData.skinnedMeshRenderer.sharedMesh = bodyPartData.meshArray[(meshIndex + 1) % bodyPartData.meshArray.Length];
        }

        public void ChangeMeshBodyPart(BodyPartType bodyPartType)
        {
            MeshBodyPartData bodyPartData = GetMeshBodyPartData(bodyPartType);
            int meshIndex = System.Array.IndexOf(bodyPartData.meshArray, bodyPartData.meshFilter.sharedMesh);
            bodyPartData.meshFilter.sharedMesh = bodyPartData.meshArray[(meshIndex + 1) % bodyPartData.meshArray.Length];
            bodyPartData.meshFilter.transform.position = bodyPartData.transformArray[(meshIndex + 1) % bodyPartData.transformArray.Length].position;
            bodyPartData.meshFilter.transform.rotation = bodyPartData.transformArray[(meshIndex + 1) % bodyPartData.transformArray.Length].rotation;
            Debug.Log("index" + (meshIndex + 1) % bodyPartData.meshArray.Length);
        }

        private SkinnedBodyPartData GetSkinnedBodyPartData(BodyPartType bodyPartType)
        {
            foreach(SkinnedBodyPartData bodyPartData in _skinnedBodyPartDataArray)
            {
                if(bodyPartData.bodyPartType == bodyPartType)
                {
                    return bodyPartData;
                }
            }
            return null;
        }

        private MeshBodyPartData GetMeshBodyPartData(BodyPartType bodyPartType)
        {
            foreach (MeshBodyPartData bodyPartData in _meshBodyPartDataArray)
            {
                if (bodyPartData.bodyPartType == bodyPartType)
                {
                    return bodyPartData;
                }
            }
            return null;
        }

        [Serializable]
        public class BodyPartTypeIndex
        {
            public BodyPartType bodyPartType;
            public int index;
        }

        public class SaveObject
        {
            public List<BodyPartTypeIndex> bodyPartTypeIndexList;
        }

        public void Save()
        {
            List<BodyPartTypeIndex> bodyPartTypeIndexList = new List<BodyPartTypeIndex>();

            foreach(BodyPartType bodyPartType in Enum.GetValues(typeof(BodyPartType)))
            {
                SkinnedBodyPartData bodyPartData = GetSkinnedBodyPartData(bodyPartType);
                int meshIndex = Array.IndexOf(bodyPartData.meshArray, bodyPartData.skinnedMeshRenderer.sharedMesh);

                bodyPartTypeIndexList.Add(new BodyPartTypeIndex
                {
                    bodyPartType = bodyPartType,
                    index = meshIndex,
                });
            }

            SaveObject saveObject = new SaveObject
            {
                bodyPartTypeIndexList = bodyPartTypeIndexList,
            };

            string jsonData = JsonUtility.ToJson(saveObject);
            Debug.Log(jsonData);
            PlayerPrefs.SetString(PLAYER_PREFS_SAVE, jsonData);
        }

        public void Load()
        {
            string jsonData = PlayerPrefs.GetString(PLAYER_PREFS_SAVE);
            SaveObject saveObject = JsonUtility.FromJson<SaveObject>(jsonData);

            foreach(BodyPartTypeIndex bodyPartTypeIndex in saveObject.bodyPartTypeIndexList)
            {
                SkinnedBodyPartData bodyPartData = GetSkinnedBodyPartData(bodyPartTypeIndex.bodyPartType);
                bodyPartData.skinnedMeshRenderer.sharedMesh = bodyPartData.meshArray[bodyPartTypeIndex.index];
            }
        }
    }
}
