using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static VictoryChallenge.Customize.PlayerCharacterCustomized;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace VictoryChallenge.Customize
{
    public class PlayerCharacterCustomized : MonoBehaviour
    {
        private const string PLAYER_PREFS_SAVE = "PlayerCustomization";

        [SerializeField] private SkinnedBodyPartData[] _skinnedBodyPartDataArray;
        [SerializeField] private GameObject _earMesh;
        [SerializeField] private GameObject _eyeMesh;
        [SerializeField] private GameObject _hatMesh;

        private int _earIndex = 0;
        private int _eyeIndex = 0;
        private int _hatIndex = 0;

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
        public class SkinnedBodyPartData
        {
            public BodyPartType bodyPartType;
            public Mesh[] meshArray;
            public SkinnedMeshRenderer skinnedMeshRenderer;
        }

        private void Start()
        {
            _earMesh = transform.Find("root").
                       transform.Find("pelvis").
                       transform.Find("spine_01").
                       transform.Find("spine_02").
                       transform.Find("spine_03").
                       transform.Find("neck_01").
                       transform.Find("head").
                       transform.Find("Ear").gameObject;

            _eyeMesh = transform.Find("root").
                       transform.Find("pelvis").
                       transform.Find("spine_01").
                       transform.Find("spine_02").
                       transform.Find("spine_03").
                       transform.Find("neck_01").
                       transform.Find("head").
                       transform.Find("Eye").gameObject;

            _hatMesh = transform.Find("root").
                       transform.Find("pelvis").
                       transform.Find("spine_01").
                       transform.Find("spine_02").
                       transform.Find("spine_03").
                       transform.Find("neck_01").
                       transform.Find("head").
                       transform.Find("Hat").gameObject;
        }

        public void ChangeSkinnedBodyPart(BodyPartType bodyPartType)
        {
            SkinnedBodyPartData bodyPartData = GetSkinnedBodyPartData(bodyPartType);
            int meshIndex = System.Array.IndexOf(bodyPartData.meshArray, bodyPartData.skinnedMeshRenderer.sharedMesh);
            bodyPartData.skinnedMeshRenderer.sharedMesh = bodyPartData.meshArray[(meshIndex + 1) % bodyPartData.meshArray.Length];
        }

        public void ChangeSkinnedBodyPartLeft(BodyPartType bodyPartType)
        {
            SkinnedBodyPartData bodyPartData = GetSkinnedBodyPartData(bodyPartType);
            int meshIndex = System.Array.IndexOf(bodyPartData.meshArray, bodyPartData.skinnedMeshRenderer.sharedMesh);
            if(meshIndex > 0)
            {
                bodyPartData.skinnedMeshRenderer.sharedMesh = bodyPartData.meshArray[Mathf.Abs(meshIndex - 1) % bodyPartData.meshArray.Length];
            }
            else if (meshIndex <= 0)
            {
                meshIndex = bodyPartData.meshArray.Length;
                bodyPartData.skinnedMeshRenderer.sharedMesh = bodyPartData.meshArray[Mathf.Abs(meshIndex - 1) % bodyPartData.meshArray.Length];
            }
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

        public void OnChangeEarMesh()
        {
            int childCount = _earMesh.transform.childCount;

            if (_earIndex < childCount - 1)
            {
                _earIndex++;
                _earMesh.transform.GetChild(_earIndex).gameObject.SetActive(true);
            }
            else
            {
                _earIndex = 0;
            }

            for(int i = 0; i < childCount; i++)
            {
                if(i != _earIndex)
                {
                    _earMesh.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        public void OnChangeEyeMesh()
        {
            int childCount = _eyeMesh.transform.childCount;

            if (_eyeIndex < childCount - 1)
            {
                _eyeIndex++;
                _eyeMesh.transform.GetChild(_eyeIndex).gameObject.SetActive(true);
            }
            else
            {
                _eyeIndex = 0;
            }

            for (int i = 0; i < childCount; i++)
            {
                if (i != _eyeIndex)
                {
                    _eyeMesh.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        public void OnChangeHatMesh()
        {
            int childCount = _hatMesh.transform.childCount;

            if (_hatIndex < childCount - 1)
            {
                _hatIndex++;
                _hatMesh.transform.GetChild(_hatIndex).gameObject.SetActive(true);
            }
            else
            {
                _hatIndex = 0;
            }

            for (int i = 0; i < childCount; i++)
            {
                if (i != _hatIndex)
                {
                    _hatMesh.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
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
            public int earIndex;
            public int eyeIndex;
            public int hatIndex;
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
                earIndex = _earIndex,
                eyeIndex = _eyeIndex,
                hatIndex = _hatIndex,
            };

            //string jsonData = JsonUtility.ToJson(saveObject);
            //PlayerPrefs.SetString(PLAYER_PREFS_SAVE, jsonData);

            FileStream stream = new FileStream(Application.dataPath + "/Resources/customData.json", FileMode.Create);

            string jsonData = JsonConvert.SerializeObject(saveObject);

            byte[] data = Encoding.UTF8.GetBytes(jsonData);
            stream.Write(data, 0, data.Length);
            stream.Close();
        }

        public void Load()
        {
            FileStream stream = new FileStream(Application.dataPath + "/Resources/customData.json", FileMode.Open);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            stream.Close();
            string jsonData = Encoding.UTF8.GetString(data);
            SaveObject saveObject = JsonConvert.DeserializeObject<SaveObject>(jsonData);

            //string jsonData = PlayerPrefs.GetString(PLAYER_PREFS_SAVE);
            //SaveObject saveObject = JsonUtility.FromJson<SaveObject>(jsonData);

            foreach (BodyPartTypeIndex bodyPartTypeIndex in saveObject.bodyPartTypeIndexList)
            {
                SkinnedBodyPartData bodyPartData = GetSkinnedBodyPartData(bodyPartTypeIndex.bodyPartType);

                if(bodyPartTypeIndex.index > -1)
                {
                    bodyPartData.skinnedMeshRenderer.sharedMesh = bodyPartData.meshArray[bodyPartTypeIndex.index];
                }
                else
                {
                    bodyPartData.skinnedMeshRenderer.sharedMesh = null;
                }
            }

            _earMesh.transform.GetChild(saveObject.earIndex).gameObject.SetActive(true);
            _eyeMesh.transform.GetChild(saveObject.eyeIndex).gameObject.SetActive(true);
            _hatMesh.transform.GetChild(saveObject.hatIndex).gameObject.SetActive(true);
        }
    }
}
