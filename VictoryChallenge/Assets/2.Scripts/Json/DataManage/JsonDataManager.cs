using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;
using UnityEngine.UI;

/// �����͸� �����ϴ� ���
/// 1. ������ �����Ͱ� ����
/// 2. �����͸� ���̽����� ��ȯ
/// 3. ���̽��� �ܺο� ����

/// �����͸� �ҷ����� ���
/// 1. �ܺο� ����� ���̽��� ������
/// 2. ���̽��� ������ ���·� ��ȯ
/// 3. �ҷ��� �����͸� ���

namespace VictoryChallenge.Json.DataManage
{
    public class PlayerData
    {
        // �̸�, ����, ����, ���� ������
        public string name;
        public int level;
        public int coin;
        public int item;
    }

    [System.Serializable]
    public class MeshData
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uv;
    }

    [System.Serializable]
    public class SkinnedMeshData
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uv;
        public Matrix4x4[] bindposes;
        public BoneData[] bones;
    }

    [System.Serializable]
    public class BoneData
    {
        public string boneName;
        public string parentName;
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;
    }



    public class JsonDataManager : MonoBehaviour
    {
        public static JsonDataManager instance;

        PlayerData playerData = new PlayerData();

        string path;
        string filename = "save";

        MeshData meshData = new MeshData();
        public SkinnedMeshRenderer skinnedMeshToSave; // ������ SkinnedMeshRenderer
        public Material meshMaterial; // �޽ø� �������� �� ����� Material
        public Mesh testMesh;

        private void Awake()
        {
            // �̱���
            if (instance == null)
            {
                instance = this;
            }
            else if(instance != this)
            {
                Destroy(instance.gameObject);
            }

            DontDestroyOnLoad(this.gameObject);

            // Unity�� �������ִ� ���
            path = Application.persistentDataPath + "/";
        }

        // Start is called before the first frame update
        void Start()
        {
            MeshToMeshData(testMesh);
            SaveMeshToJson(testMesh, path + filename);
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        /// <summary>
        /// Json Data�� ����
        /// </summary>
        public void SaveData()
        {
            // Json Data�� ��ȯ
            string data = JsonUtility.ToJson(playerData);
            
            // print(path);

            // using System.IO
            // WriteAllTest(���, ����)
            File.WriteAllText(path + filename, data);
        }

        /// <summary>
        /// Json Data�� �ҷ�����
        /// </summary>
        public void LoadData()
        {
            string data = File.ReadAllText(path + filename);
            playerData = JsonUtility.FromJson<PlayerData>(data);
        }

        #region Mesh

        public void SaveMeshToJsonButton()
        {
            SaveMeshToJson(testMesh, path + filename);
        }

        public void LoadMeshFromJsonButton()
        {
            Mesh loadedMesh = LoadMeshFromJson(path + filename);
            // �ҷ��� Mesh�� ����ϴ� ���� �߰� (��: meshToSave = loadedMesh)
            CreateMeshObject(loadedMesh);
            //loadedMesh.GetComponent<Transform>().position = Vector3.zero;
        }

        private void CreateMeshObject(Mesh mesh)
        {
            GameObject meshObject = new GameObject("LoadedMesh");
            MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();

            meshFilter.mesh = mesh;
            meshRenderer.material = meshMaterial; // �ʿ��� Material�� �Ҵ�

            // �ʿ�� meshObject�� Transform ���� (��ġ, ȸ��, ������)
            meshObject.transform.position = Vector3.zero;
            meshObject.transform.rotation = Quaternion.Euler(-90, 0, 0);
            meshObject.transform.localScale = Vector3.one;
        }

        public MeshData MeshToMeshData(Mesh mesh)
        {
            MeshData meshData = new MeshData();
            meshData.vertices = mesh.vertices;
            meshData.triangles = mesh.triangles;
            meshData.uv = mesh.uv;
            return meshData;
        }

        public void SaveMeshToJson(Mesh mesh, string path)
        {
            MeshData meshData = MeshToMeshData(mesh);
            string json = JsonUtility.ToJson(meshData);
            File.WriteAllText(path, json);
        }

        public Mesh LoadMeshFromJson(string path)
        {
            MeshData meshData = JsonToMeshData(path);
            return MeshDataToMesh(meshData);
        }

        public MeshData JsonToMeshData(string path)
        {
            string json = File.ReadAllText(path);
            MeshData meshData = JsonUtility.FromJson<MeshData>(json);
            return meshData;
        }

        public Mesh MeshDataToMesh(MeshData meshData)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = meshData.vertices;
            mesh.triangles = meshData.triangles;
            mesh.uv = meshData.uv;
            mesh.RecalculateNormals(); // �ʿ�� Normals ����
            return mesh;
        }
        #endregion

        #region SkinnedMesh

        public void SaveSkinnedMeshToJsonButton()
        {
            SaveSkinnedMeshToJson(skinnedMeshToSave, path + filename);
        }

        public void LoadSkinnedMeshFromJsonButton()
        {
            SkinnedMeshRenderer loadedSkinnedMeshRenderer = LoadSkinnedMeshFromJson(path + filename);
            loadedSkinnedMeshRenderer.sharedMaterial = meshMaterial; // �ʿ��� Material�� �Ҵ�
        }

        public void SaveSkinnedMeshToJson(SkinnedMeshRenderer skinnedMeshRenderer, string path)
        {
            SkinnedMeshData skinnedMeshData = SkinnedMeshToSkinnedMeshData(skinnedMeshRenderer);
            string json = JsonUtility.ToJson(skinnedMeshData);
            File.WriteAllText(path, json);
        }

        public SkinnedMeshRenderer LoadSkinnedMeshFromJson(string path)
        {
            SkinnedMeshData skinnedMeshData = JsonToSkinnedMeshData(path);
            return SkinnedMeshDataToSkinnedMesh(skinnedMeshData);
        }

        // Skinned Mesh -> Json

        public SkinnedMeshData SkinnedMeshToSkinnedMeshData(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            SkinnedMeshData skinnedMeshData = new SkinnedMeshData();
            Mesh mesh = skinnedMeshRenderer.sharedMesh;

            skinnedMeshData.vertices = mesh.vertices;
            skinnedMeshData.triangles = mesh.triangles;
            skinnedMeshData.uv = mesh.uv;
            skinnedMeshData.bindposes = mesh.bindposes;

            Transform[] bones = skinnedMeshRenderer.bones;
            skinnedMeshData.bones = new BoneData[bones.Length];
            for (int i = 0; i < bones.Length; i++)
            {
                BoneData boneData = new BoneData
                {
                    boneName = bones[i].name,
                    parentName = bones[i].parent != null ? bones[i].parent.name : null,
                    localPosition = bones[i].localPosition,
                    localRotation = bones[i].localRotation,
                    localScale = bones[i].localScale
                };
                skinnedMeshData.bones[i] = boneData;
            }

            return skinnedMeshData;
        }

        // Json -> Skinned Mesh

        public SkinnedMeshData JsonToSkinnedMeshData(string path)
        {
            string json = File.ReadAllText(path);
            SkinnedMeshData skinnedMeshData = JsonUtility.FromJson<SkinnedMeshData>(json);
            return skinnedMeshData;
        }

        public SkinnedMeshRenderer SkinnedMeshDataToSkinnedMesh(SkinnedMeshData skinnedMeshData)
        {
            GameObject meshObject = new GameObject("LoadedSkinnedMesh");
            SkinnedMeshRenderer skinnedMeshRenderer = meshObject.AddComponent<SkinnedMeshRenderer>();

            Mesh mesh = new Mesh();
            mesh.vertices = skinnedMeshData.vertices;
            mesh.triangles = skinnedMeshData.triangles;
            mesh.uv = skinnedMeshData.uv;
            mesh.bindposes = skinnedMeshData.bindposes;
            mesh.RecalculateNormals();

            Transform[] bones = new Transform[skinnedMeshData.bones.Length];
            for (int i = 0; i < skinnedMeshData.bones.Length; i++)
            {
                BoneData boneData = skinnedMeshData.bones[i];
                GameObject boneObject = new GameObject(boneData.boneName);
                Transform boneTransform = boneObject.transform;

                boneTransform.localPosition = boneData.localPosition;
                boneTransform.localRotation = boneData.localRotation;
                boneTransform.localScale = boneData.localScale;

                if (!string.IsNullOrEmpty(boneData.parentName))
                {
                    Transform parentTransform = meshObject.transform.Find(boneData.parentName);
                    if (parentTransform != null)
                    {
                        boneTransform.parent = parentTransform;
                    }
                    else
                    {
                        boneTransform.parent = meshObject.transform;
                    }
                }
                else
                {
                    boneTransform.parent = meshObject.transform;
                }

                bones[i] = boneTransform;
            }

            skinnedMeshRenderer.sharedMesh = mesh;
            skinnedMeshRenderer.bones = bones;

            return skinnedMeshRenderer;
        }

        #endregion
    }
}
