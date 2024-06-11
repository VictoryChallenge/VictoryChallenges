using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VictoryChallenge.Customize
{
    public class PlayerCharacterCustomized : MonoBehaviour
    {
        private const string PLAYER_PREFS_SAVE = "PlayerCustomization";

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
            public SkinnedMeshRenderer skinnedMeshRenderer;
        }

        public void ChangeBodyPart(BodyPartType bodyPartType)
        {
            BodyPartData bodyPartData = GetBodyPartData(bodyPartType);
            int meshIndex = System.Array.IndexOf(bodyPartData.meshArray, bodyPartData.skinnedMeshRenderer.sharedMesh);
            bodyPartData.skinnedMeshRenderer.sharedMesh = bodyPartData.meshArray[(meshIndex + 1) % bodyPartData.meshArray.Length];
        }

        private BodyPartData GetBodyPartData(BodyPartType bodyPartType)
        {
            foreach(BodyPartData bodyPartData in _bodyPartDataArray)
            {
                if(bodyPartData.bodyPartType == bodyPartType)
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
                BodyPartData bodyPartData = GetBodyPartData(bodyPartType);
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
                BodyPartData bodyPartData = GetBodyPartData(bodyPartTypeIndex.bodyPartType);
                bodyPartData.skinnedMeshRenderer.sharedMesh = bodyPartData.meshArray[bodyPartTypeIndex.index];
            }
        }
    }
}
