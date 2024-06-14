using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using static VictoryChallenge.Customize.PlayerCharacterCustomized;

namespace VictoryChallenge.Customize
{
    public class PlayerCharacterCustomized : MonoBehaviour
    {
        private const string PLAYER_PREFS_SAVE = "PlayerCustomization";

        [SerializeField] private SkinnedBodyPartData[] _skinnedBodyPartDataArray;
        [SerializeField] private GameObject _earMesh;
        [SerializeField] private GameObject _hatMesh;
        [SerializeField] private GameObject _accessoryMesh;

        private int _earIndex = 0;
        private int _hatIndex = 0;
        private int _accessoryIndex = 0;

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

        public void ChangeSkinnedBodyPartRight(BodyPartType bodyPartType)
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

                if(bodyPartData.skinnedMeshRenderer.sharedMesh == null)
                {
                    bodyPartData.skinnedMeshRenderer.sharedMesh = bodyPartData.meshArray[bodyPartData.meshArray.Length - 2];
                }
                else
                {
                    bodyPartData.skinnedMeshRenderer.sharedMesh = bodyPartData.meshArray[Mathf.Abs(meshIndex - 1) % bodyPartData.meshArray.Length];
                }

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

        public void OnChangeRightEarMesh()
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

        public void OnChangeLeftEarMesh()
        {
            int childCount = _earMesh.transform.childCount;

            if (_earIndex > 0)
            {
                _earIndex--;
            }
            else
            {
                _earIndex = childCount - 1;
            }
            _earMesh.transform.GetChild(_earIndex).gameObject.SetActive(true);

            for (int i = 0; i < childCount; i++)
            {
                if (i != _earIndex)
                {
                    _earMesh.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        public void OnChangeRightAccessoryMesh()
        {
            int childCount = _accessoryMesh.transform.childCount;

            if (_accessoryIndex < childCount - 1)
            {
                _accessoryIndex++;
                _accessoryMesh.transform.GetChild(_accessoryIndex).gameObject.SetActive(true);
            }
            else
            {
                _accessoryIndex = 0;
            }

            for (int i = 0; i < childCount; i++)
            {
                if (i != _accessoryIndex)
                {
                    _accessoryMesh.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        public void OnChangeLeftAccessoryMesh()
        {
            int childCount = _accessoryMesh.transform.childCount;

            if (_accessoryIndex > 0)
            {
                _accessoryIndex--;
            }
            else
            {
                _accessoryIndex = childCount - 1;
            }
            _accessoryMesh.transform.GetChild(_accessoryIndex).gameObject.SetActive(true);

            for (int i = 0; i < childCount; i++)
            {
                if (i != _accessoryIndex)
                {
                    _accessoryMesh.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        public void OnChangeRightHatMesh()
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

        public void OnChangeLeftHatMesh()
        {
            int childCount = _hatMesh.transform.childCount;

            if (_hatIndex > 0)
            {
                _hatIndex--;
            }
            else
            {
                _hatIndex = childCount - 1;
            }
            _hatMesh.transform.GetChild(_hatIndex).gameObject.SetActive(true);

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
                eyeIndex = _accessoryIndex,
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
            _accessoryMesh.transform.GetChild(saveObject.eyeIndex).gameObject.SetActive(true);
            _hatMesh.transform.GetChild(saveObject.hatIndex).gameObject.SetActive(true);
        }
    }
}