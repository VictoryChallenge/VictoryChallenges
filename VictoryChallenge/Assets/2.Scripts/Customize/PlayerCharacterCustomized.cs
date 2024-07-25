using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using static VictoryChallenge.Customize.PlayerCharacterCustomized;
using VictoryChallenge.KJ.Database;
using Firebase.Auth;
using VictoryChallenge.KJ.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using VictoryChallenge.Controllers.Player;
using UnityEngine.SceneManagement;
using Proyecto26;
using Unity.VisualScripting.Antlr3.Runtime;
using Newtonsoft.Json.Linq;

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

        private string _userId;
        private string _shortUID;
        private bool _isLocal;

        [SerializeField] private PhotonView _pv;

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

        private void Start()
        {
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(4) || SceneManager.GetActiveScene().buildIndex == 7 || SceneManager.GetActiveScene().buildIndex == 8)
            {
                _isLocal = true;
                LoadData();
            }
            //if(SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(2) || SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(3) || SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(5))
            else
            {
                if (_pv.IsMine)
                {
                    object[] data = _pv.InstantiationData;
                    _userId = (string)data[0];
                    _shortUID = UIDHelper.GenerateShortUID(_userId);
                    _isLocal = false;
                    LoadData();
                }
            }
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
            public int accessoryIndex;
            public int hatIndex;
        }

        public string Initialize()
        {
            List<BodyPartTypeIndex> bodyPartTypeIndexList = new List<BodyPartTypeIndex>();

            bodyPartTypeIndexList.Add(new BodyPartTypeIndex
            {
                bodyPartType = BodyPartType.Color,
                index = 0,
            });

            bodyPartTypeIndexList.Add(new BodyPartTypeIndex
            {
                bodyPartType = BodyPartType.BodyParts,
                index = 0,
            });

            bodyPartTypeIndexList.Add(new BodyPartTypeIndex
            {
                bodyPartType = BodyPartType.Eyes,
                index = 0,
            });

            bodyPartTypeIndexList.Add(new BodyPartTypeIndex
            {
                bodyPartType = BodyPartType.Gloves,
                index = 0,
            });

            bodyPartTypeIndexList.Add(new BodyPartTypeIndex
            {
                bodyPartType = BodyPartType.HeadParts,
                index = 0,
            });

            bodyPartTypeIndexList.Add(new BodyPartTypeIndex
            {
                bodyPartType = BodyPartType.Mouth,
                index = 0,
            });

            bodyPartTypeIndexList.Add(new BodyPartTypeIndex
            {
                bodyPartType = BodyPartType.Tails,
                index = 0,
            });

            SaveObject saveObject = new SaveObject
            {
                bodyPartTypeIndexList = bodyPartTypeIndexList,
                earIndex = 0,
                accessoryIndex = 0,
                hatIndex = 0,
            };

            string jsonData = JsonConvert.SerializeObject(saveObject);

            return jsonData;
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
                accessoryIndex = _accessoryIndex,
                hatIndex = _hatIndex,
            };

            //string jsonData = JsonUtility.ToJson(saveObject);
            //PlayerPrefs.SetString(PLAYER_PREFS_SAVE, jsonData);

            //FileStream stream = new FileStream(Application.persistentDataPath + "/customData.json", FileMode.Create);

            //byte[] data = Encoding.UTF8.GetBytes(customData);
            //stream.Write(data, 0, data.Length);
            //stream.Close();

            string customData = JsonConvert.SerializeObject(saveObject);

            // �� ���� ���̵� �´� Ŀ���� ������ ����
            string shortUID = UIDHelper.GenerateShortUID(RestAPIAuth.Instance.UserId);
            Debug.Log("shortUID : " + shortUID);
            User user = DBManager.Instance.gameData.users[shortUID];
            string userData = JsonUtility.ToJson(user);
            DBManager.Instance.WriteUserData(shortUID, userData, customData);
            DBManager.Instance.customData = customData;
            //Debug.Log("customData : " + customData);
        }

        public void Load()
        {
            FileStream stream = new FileStream(Application.persistentDataPath + "/customData.json", FileMode.Open);
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

            // Customizing �� �ε����� ���� ������Ʈ ���� ����

            foreach (BodyPartTypeIndex bodyPartTypeIndex in saveObject.bodyPartTypeIndexList)
            {
                SkinnedBodyPartData bodyPartData = GetSkinnedBodyPartData(bodyPartTypeIndex.bodyPartType);

                int childCount = bodyPartData.skinnedMeshRenderer.transform.parent.childCount;

                //Debug.Log("Partstype = " + bodyPartTypeIndex.bodyPartType + " " + bodyPartTypeIndex.index);

                for(int i = 0; i < childCount; i++)
                {
                    if(i != 0)
                    {
                        Destroy(bodyPartData.skinnedMeshRenderer.transform.parent.transform.GetChild(i).gameObject);
                    }
                }
            }

            _earMesh.transform.GetChild(saveObject.earIndex).gameObject.SetActive(true);
            _accessoryMesh.transform.GetChild(saveObject.accessoryIndex).gameObject.SetActive(true);
            _hatMesh.transform.GetChild(saveObject.hatIndex).gameObject.SetActive(true);
            
            int earMeshCount = _earMesh.transform.childCount;
            int accessoryMeshCount = _accessoryMesh.transform.childCount;
            int hatMeshCount = _hatMesh.transform.childCount;

            // Customizing �� �ε����� ���� ������Ʈ ���� ����
            for(int i = 0; i < earMeshCount; i++)
            {
                if(i != saveObject.earIndex)
                {
                    Destroy(_earMesh.transform.GetChild(i).gameObject);
                }
            }

            for (int i = 0; i < accessoryMeshCount; i++)
            {
                if (i != saveObject.accessoryIndex)
                {
                    Destroy(_accessoryMesh.transform.GetChild(i).gameObject);
                }
            }

            for (int i = 0; i < hatMeshCount; i++)
            {
                if (i != saveObject.hatIndex)
                {
                    Destroy(_hatMesh.transform.GetChild(i).gameObject);
                }
            }
        }

        public void LoadData()
        {
            string shortUID = UIDHelper.GenerateShortUID(RestAPIAuth.Instance.UserId);
            StartCoroutine(C_LoadjsonData(shortUID));
        }

        private IEnumerator C_LoadjsonData(string shortUID)
        {
            //string userData = "";
            //DatabaseReference db = FirebaseDatabase.DefaultInstance.GetReference("User");
            //db.GetValueAsync().ContinueWithOnMainThread(task =>
            //{
            //    if (task.IsFaulted)
            //    {
            //        Debug.LogError("ReadData is Faulted");
            //    }
            //    else if (task.IsCompleted)
            //    {
            //        DataSnapshot snapshot = task.Result;
            //        Debug.Log("ChilderenCount" + snapshot.ChildrenCount);

            //        foreach (var child in snapshot.Children)
            //        {
            //            if (child.Key == shortUID)
            //            {
            //                Debug.Log("child.Value.ToString() : " + child.ToString());
            //                userData = child.Child("customData").Value.ToString();
            //                DBManager.Instance.customData = userData;

            //                // RPC ���� �� �ִ� �������� Ȯ��
            //                if(!_isLocal)
            //                {
            //                    _pv.RPC("SendCustomData", RpcTarget.AllBuffered, userData);
            //                }
            //            }
            //        }
            //    }
            //});

            //yield return new WaitUntil(() => !string.IsNullOrEmpty(userData));
            string customData = "";

            var request = new RequestHelper
            {
                Uri = $"{RestAPIAuth.Instance.dbUrl}/User/{shortUID}.json?auth={RestAPIAuth.Instance.IdToken}",
                Method = "GET",
                EnableDebug = true
            };

            RestClient.Request(request, (err, res) =>
            {
                if (err != null || string.IsNullOrEmpty(res.Text) || res.Text == "null")
                {
                    Debug.LogError("Ŀ���� �����Ͱ� �������� �ʽ��ϴ�");
                    return;
                }

                var snapshot = JObject.Parse(res.Text);
                
                if (snapshot != null)
                {
                    customData = snapshot["customData"]?.ToString();
                    //Debug.Log("customData :" + customData);

                    DBManager.Instance.customData = customData;

                    // RPC ���� �� �ִ� �������� Ȯ��
                    if(!_isLocal)
                    {
                        _pv.RPC("SendCustomData", RpcTarget.AllBuffered, customData);
                    }

                    // ������ȭ
                    SaveObject saveObject = JsonConvert.DeserializeObject<SaveObject>(customData);

                    foreach (BodyPartTypeIndex bodyPartTypeIndex in saveObject.bodyPartTypeIndexList)
                    {
                        SkinnedBodyPartData bodyPartData = GetSkinnedBodyPartData(bodyPartTypeIndex.bodyPartType);

                        if (bodyPartTypeIndex.index > -1)
                        {
                            bodyPartData.skinnedMeshRenderer.sharedMesh = bodyPartData.meshArray[bodyPartTypeIndex.index];
                        }
                        else
                        {
                            bodyPartData.skinnedMeshRenderer.sharedMesh = null;
                        }
                    }

                    // Customizing �� �ε����� ���� ������Ʈ ���� ����

                    //foreach (BodyPartTypeIndex bodyPartTypeIndex in saveObject.bodyPartTypeIndexList)
                    //{
                    //    SkinnedBodyPartData bodyPartData = GetSkinnedBodyPartData(bodyPartTypeIndex.bodyPartType);
                    //
                    //    int childCount = bodyPartData.skinnedMeshRenderer.transform.parent.childCount;
                    //
                    //    //Debug.Log("Partstype = " + bodyPartTypeIndex.bodyPartType + " " + bodyPartTypeIndex.index);
                    //
                    //    for (int i = 0; i < childCount; i++)
                    //    {
                    //        if (i != 0)
                    //        {
                    //            Destroy(bodyPartData.skinnedMeshRenderer.transform.parent.transform.GetChild(i).gameObject);
                    //        }
                    //    }
                    //}

                    _earMesh.transform.GetChild(saveObject.earIndex).gameObject.SetActive(true);
                    _accessoryMesh.transform.GetChild(saveObject.accessoryIndex).gameObject.SetActive(true);
                    _hatMesh.transform.GetChild(saveObject.hatIndex).gameObject.SetActive(true);

                    int earMeshCount = _earMesh.transform.childCount;
                    int accessoryMeshCount = _accessoryMesh.transform.childCount;
                    int hatMeshCount = _hatMesh.transform.childCount;

                    // Customizing �� �ε����� ���� ������Ʈ ���� ����
                    for (int i = 0; i < earMeshCount; i++)
                    {
                        if (i != saveObject.earIndex)
                        {
                            _earMesh.transform.GetChild(i).gameObject.SetActive(false);
                        }
                    }

                    for (int i = 0; i < accessoryMeshCount; i++)
                    {
                        if (i != saveObject.accessoryIndex)
                        {
                            _accessoryMesh.transform.GetChild(i).gameObject.SetActive(false);
                        }
                    }

                    for (int i = 0; i < hatMeshCount; i++)
                    {
                        if (i != saveObject.hatIndex)
                        {
                            _hatMesh.transform.GetChild(i).gameObject.SetActive(false);
                        }
                    }
                }
            });

            yield return null;
        }

        [PunRPC]
        public void SendCustomData(string customData) 
        {
            SaveObject saveObject = JsonConvert.DeserializeObject<SaveObject>(customData);

            foreach (BodyPartTypeIndex bodyPartTypeIndex in saveObject.bodyPartTypeIndexList)
            {
                SkinnedBodyPartData bodyPartData = GetSkinnedBodyPartData(bodyPartTypeIndex.bodyPartType);

                if (bodyPartTypeIndex.index > -1)
                {
                    bodyPartData.skinnedMeshRenderer.sharedMesh = bodyPartData.meshArray[bodyPartTypeIndex.index];
                }
                else
                {
                    bodyPartData.skinnedMeshRenderer.sharedMesh = null;
                }
            }

            // Customizing �� �ε����� ���� ������Ʈ ���� ����

            foreach (BodyPartTypeIndex bodyPartTypeIndex in saveObject.bodyPartTypeIndexList)
            {
                SkinnedBodyPartData bodyPartData = GetSkinnedBodyPartData(bodyPartTypeIndex.bodyPartType);

                int childCount = bodyPartData.skinnedMeshRenderer.transform.parent.childCount;

                //Debug.Log("Partstype = " + bodyPartTypeIndex.bodyPartType + " " + bodyPartTypeIndex.index);

                for (int i = 0; i < childCount; i++)
                {
                    if (i != 0)
                    {
                        Destroy(bodyPartData.skinnedMeshRenderer.transform.parent.transform.GetChild(i).gameObject);
                    }
                }
            }

            _earMesh.transform.GetChild(saveObject.earIndex).gameObject.SetActive(true);
            _accessoryMesh.transform.GetChild(saveObject.accessoryIndex).gameObject.SetActive(true);
            _hatMesh.transform.GetChild(saveObject.hatIndex).gameObject.SetActive(true);
        }
    }
}
