using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static VictoryChallenge.Customize.PlayerCharacterChangeMesh;
using static VictoryChallenge.Customize.PlayerCharacterCustomized;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace VictoryChallenge.Customize
{
    public class PlayerCharacterCustomized : MonoBehaviour
    {
        private const string PLAYER_PREFS_SAVE = "PlayerCustomization";
        private const string PLAYER_MESH_PREFS_SAVE = "PlayerMeshCustomization";

        [SerializeField] private SkinnedBodyPartData[] _skinnedBodyPartDataArray;
        private GameObject _earMesh;
        private GameObject _eyeMesh;
        private GameObject _hatMesh;
        
        private int _earIndex = 0;
        private int _eyeIndex = 0;
        private int _hatIndex = 0;

        private string _jsonDataPath;

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

            if (_earIndex < childCount)
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
        }

        public class SaveMeshObject
        {
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
            };

            SaveMeshObject saveMeshObject = new SaveMeshObject
            {
                earIndex = _earIndex,
                eyeIndex = _eyeIndex,
                hatIndex = _hatIndex,
            };

            //string jsonData = JsonUtility.ToJson(saveObject);
            //string jsonData2 = JsonUtility.ToJson(saveMeshObject);
            //Debug.Log(jsonData);
            //Debug.Log(jsonData2);
            FileStream stream = new FileStream(Application.dataPath + "/test.json", FileMode.OpenOrCreate);

            string jsonData = JsonConvert.SerializeObject(saveObject);
            //string jsonData2 = JsonConvert.SerializeObject(saveMeshObject);

            byte[] data = Encoding.UTF8.GetBytes(jsonData);
            //byte[] data2 = Encoding.UTF8.GetBytes(jsonData2);
            stream.Write(data, 0, data.Length);
            //stream.Write(data2, 0, data2.Length);
            stream.Close();

            //PlayerPrefs.SetString(PLAYER_PREFS_SAVE, jsonData);
            //PlayerPrefs.SetString(PLAYER_MESH_PREFS_SAVE, jsonData2);
        }

        public void Load()
        {
            FileStream stream = new FileStream(Application.dataPath + "/test.json", FileMode.Open);
            byte[] data = new byte[stream.Length];
            //byte[] data2 = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            //stream.Read(data2, 0, data2.Length);
            stream.Close();
            string jsonData = Encoding.UTF8.GetString(data);
            //string jsonData2 = Encoding.UTF8.GetString(data2);
            SaveObject saveObject = JsonConvert.DeserializeObject<SaveObject>(jsonData);
            //SaveMeshObject saveMeshObject = JsonConvert.DeserializeObject<SaveMeshObject>(jsonData2);

            //string jsonData = PlayerPrefs.GetString(PLAYER_PREFS_SAVE);
            //string jsonData2 = PlayerPrefs.GetString(PLAYER_MESH_PREFS_SAVE);
            //SaveObject saveObject = JsonUtility.FromJson<SaveObject>(jsonData);
            //SaveMeshObject saveMeshObject = JsonUtility.FromJson<SaveMeshObject>(jsonData2);

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

            //_earMesh.transform.GetChild(saveMeshObject.earIndex).gameObject.SetActive(true);
            //_eyeMesh.transform.GetChild(saveMeshObject.eyeIndex).gameObject.SetActive(true);
            //_hatMesh.transform.GetChild(saveMeshObject.hatIndex).gameObject.SetActive(true);
        }
    }
}
