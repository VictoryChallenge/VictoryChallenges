using ithappy;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VictoryChallenge.Controllers.Player;
using VictoryChallenge.KJ.Map;

namespace VictoryChallenge.Scripts.CL
{ 
    public class GameSceneUI : UI_Scene
    {
        enum TMPs
        { 
            countdown,
            Time,
            Finish,
            Round,
            Person,
        }

        enum Images
        { 
            Clock,
            Mission,
        }

        TMP_Text text;
        private float time;
        private Image mission;
        private Image clock;
        private TextMeshProUGUI roundText;
        private TextMeshProUGUI finishText;
        private TextMeshProUGUI personText;
        private CanvasGroup missionCanvasGroup;
        public int winner;
        // ����ο��� ���߿� ī��Ʈ�Ҷ�
        public int person;
        private int round;

        // ĳ���� �̵��� ���� ���� �Ұ� 
        private bool isMoving;
        private GameManagerCL gameManager;

        // ��ֹ�
        private ObstacleManager _obstacleManager;
        private ConveyorBelt _conveyorBelt;
        private ConveyorBelt _conveyorBelt2;

        void Start()
        {
            Init();
            // �α��ξ��ϰ� ������ �׽�Ʈ�� �Ϸ��� �Ʒ��ڵ� �ּ�����
            //StartCoroutine(GameStart());

            switch (SceneManager.GetActiveScene().buildIndex)
            {
                case 3:
                    round = 1;
                    person = 2;
                    break;
                case 4:
                    break;
                case 5:
                    round = 2;
                    person = 2;
                    break;
                case 6:
                    round = 2;
                    person = 1;
                    break;
                case 9:
                    round = 1;
                    person = 4;
                    break;
            }

            gameManager = GameObject.FindObjectOfType<GameManagerCL>();

            #region ��ֹ�
            // ��ֹ�
            _obstacleManager = FindObjectOfType<ObstacleManager>();

            // ������ Ʈ��
            GameObject redPlayerLine = GameObject.Find("Red_Player_Line");
            if (redPlayerLine != null)
            {
                Transform trackTransform = redPlayerLine.transform.Find("Track");
                if (trackTransform != null)
                {
                    _conveyorBelt = trackTransform.GetComponent<ConveyorBelt>();
                    if (_conveyorBelt != null)
                    {
                        _conveyorBelt.Initialize();
                    }
                }
            }

            // ���� Ʈ��
            GameObject bluePlayerLine = GameObject.Find("Blue_Player_Line");
            if (bluePlayerLine != null)
            {
                Transform track2Transform = bluePlayerLine.transform.Find("Track");
                if (track2Transform != null)
                {
                    _conveyorBelt2 = track2Transform.GetComponent<ConveyorBelt>();
                    if (_conveyorBelt2 != null)
                    {
                        _conveyorBelt2.Initialize();
                    }
                }
            }
            #endregion

        }

        public override void Init()
        {
            base.Init();
            Bind<TextMeshProUGUI>(typeof(TMPs));
            Bind<Image>(typeof(Images));
            Managers.Sound.Play("RunBGM1", Define.Sound.BGM);
            AudioSource _audioSource = GameObject.Find("BGM").GetComponent<AudioSource>();
            _audioSource.volume = 0.4f;

            text = GetTextMeshPro((int)TMPs.countdown);
            text.gameObject.SetActive(false);

            mission = GetImage((int)Images.Mission);
            missionCanvasGroup = mission.GetComponent<CanvasGroup>();
            if (missionCanvasGroup == null)
            {
                missionCanvasGroup = mission.gameObject.AddComponent<CanvasGroup>();
            }
            missionCanvasGroup.alpha = 0;

            roundText = GetTextMeshPro((int)TMPs.Round);
            roundText.gameObject.SetActive(false);

            finishText = GetTextMeshPro((int)TMPs.Finish);
            finishText.gameObject.SetActive(false);

            personText = GetTextMeshPro((int)TMPs.Person);
            personText.gameObject.SetActive(false);

            clock = GetImage((int)Images.Clock);
            clock.gameObject.SetActive(false);
        }

        void Update()
        {
            // �ɼ��˾� ����
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameObject go = GameObject.Find("GameSettingPopup");
                if (go != null)
                    return;
                else 
                    Managers.UI.ShowPopupUI<GameSettingPopup>();
            }

            time = gameManager.time;
            gameManager.isMoving = isMoving;

            // �ð� �����ϱ�
            if (time > 0)
            {
                int displayTime = Mathf.CeilToInt(time); // �ð��� �ø��Ͽ� ������ ��ȯ
                GetTextMeshPro((int)TMPs.Time).text = displayTime.ToString();
            }
            if (time <= 0 || winner == person)
            {
                if (isMoving == true)
                {
                    isMoving = false;
                    StartCoroutine(AnimateFinishText(finishText));
                }

                if(time <= 0)
                {
                    GetTextMeshPro((int)TMPs.Time).text = "0"; // time�� ��Ȯ�� 0���� ����
                }
            }
            // �ο���
            personText.text = $"{winner}/{person}";
        }

        IEnumerator GameStart()
        {
            Controllers.Player.CharacterController[] cc = GameObject.FindObjectsOfType<Controllers.Player.CharacterController>();
            foreach (Controllers.Player.CharacterController c in cc)
            {
                c.isKeyActive = false;
                yield return c.StartCoroutine(c.C_IntroCutSceneStart());
            }

            // ROUND �ؽ�Ʈ �ڷ�ƾ
            yield return StartCoroutine(AnimateRoundText(roundText));

            text.gameObject.SetActive(true);
            Managers.Sound.Play("Countdown", Define.Sound.Effect);

            text.text = "3";
            yield return new WaitForSeconds(1f);

            text.text = "2";
            yield return new WaitForSeconds(1f);

            text.text = "1";     
            
            Rnd_Animation[] objs = FindObjectsOfType<Rnd_Animation>();
            foreach(var obj in objs)
            {
                obj.Active();
            }

            yield return new WaitForSeconds(1f);

            foreach (Controllers.Player.CharacterController c in cc)
            {
                c.isKeyActive = true;
            }

            isMoving = true;

            // 2�ο� �ʿ����� �Ⱥ��̰�
            if (SceneManager.GetActiveScene().buildIndex != 6)
            { 
                personText.gameObject.SetActive(true);
                clock.gameObject.SetActive(true);
            }

            text.text = "�޷���~";
            yield return new WaitForSeconds(1f);

            text.gameObject.SetActive(false);

            // ��ֹ�
            _obstacleManager = FindObjectOfType<ObstacleManager>();

            if (_obstacleManager != null)
            {
                _obstacleManager.obstaclespawn = true;
                StartCoroutine(_obstacleManager.SpawnObstacles());
            }

            if (_conveyorBelt != null)
            {
                _conveyorBelt.EnableConveyerBelt();
                Debug.Log("������ �����̾� ��Ʈ");
            }

            if (_conveyorBelt2 != null)
            {
                _conveyorBelt2.EnableConveyerBelt();
                Debug.Log("���� �����̾� ��Ʈ");
            }

        }

        IEnumerator AnimateRoundText(TextMeshProUGUI Text)
        {
            yield return new WaitForSeconds(0.5f);

            Text.gameObject.SetActive(true);
            Text.text = $"Round {round}";

            RectTransform rectTransform = Text.GetComponent<RectTransform>();

            Vector3 startPos = new Vector3(-Screen.width, rectTransform.localPosition.y, 0);
            Vector3 middlePos = new Vector3(0, rectTransform.localPosition.y, 0);
            Vector3 endPos = new Vector3(Screen.width, rectTransform.localPosition.y, 0);

            float duration = 1.3f; // �ؽ�Ʈ�� ����� �̵��ϴµ� �ɸ��� �ð�
            float elapsedTime = 0f;

            // ���ʿ��� ����� �̵�
            while (elapsedTime < duration)
            {
                rectTransform.localPosition = Vector3.Lerp(startPos, middlePos, Mathf.SmoothStep(0, 1, elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // �̼� �ִϸ��̼�
            StartCoroutine(FadeInMission());
            rectTransform.localPosition = middlePos;
            yield return new WaitForSeconds(2.5f); // ������� 1�� �ӹ���
            yield return StartCoroutine(FadeOutMission());

            elapsedTime = 0f;
            duration = 1.3f; // �ؽ�Ʈ�� ���������� �̵��ϴµ� �ɸ��� �ð�

            
            // ������� ���������� �̵�
            while (elapsedTime < duration)
            {
                rectTransform.localPosition = Vector3.Lerp(middlePos, endPos, Mathf.SmoothStep(0, 1, elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            rectTransform.localPosition = endPos;
            Text.gameObject.SetActive(false);
        }

        public IEnumerator AnimateFinishText(TextMeshProUGUI Text)
        {
            Controllers.Player.CharacterController[] cc = GameObject.FindObjectsOfType<Controllers.Player.CharacterController>();
            foreach (Controllers.Player.CharacterController c in cc)
            {
                c.isKeyActive = false;
            }

            if (_obstacleManager != null)
            {
                _obstacleManager.obstaclespawn = false;
                StopCoroutine(_obstacleManager.SpawnObstacles());
            }

            if (_conveyorBelt != null)
            {
                _conveyorBelt.DisableConveyerBelt();
                Debug.Log("������ �����̾� ��Ʈ disable");
            }

            if (_conveyorBelt2 != null)
            {
                _conveyorBelt2.DisableConveyerBelt();
                Debug.Log("���� �����̾� ��Ʈ disable");
            }

            Managers.Sound.Play("Whistle", Define.Sound.Effect);

            Text.gameObject.SetActive(true);
            Text.text = $"Finish";

            RectTransform rectTransform = Text.GetComponent<RectTransform>();

            Vector3 startPos = new Vector3(-Screen.width, rectTransform.localPosition.y, 0);
            Vector3 middlePos = new Vector3(0, rectTransform.localPosition.y, 0);
            Vector3 endPos = new Vector3(Screen.width, rectTransform.localPosition.y, 0);

            float duration = 1.2f; // �ؽ�Ʈ�� ����� �̵��ϴµ� �ɸ��� �ð�
            float elapsedTime = 0f;

            // ���ʿ��� ����� �̵�
            while (elapsedTime < duration)
            {
                rectTransform.localPosition = Vector3.Lerp(startPos, middlePos, Mathf.SmoothStep(0, 1, elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // �̼� �ִϸ��̼�
            rectTransform.localPosition = middlePos;
            yield return new WaitForSeconds(2.5f); // ������� 1�� �ӹ���

            elapsedTime = 0f;
            duration = 1.2f; // �ؽ�Ʈ�� ���������� �̵��ϴµ� �ɸ��� �ð�


            // ������� ���������� �̵�
            while (elapsedTime < duration)
            {
                rectTransform.localPosition = Vector3.Lerp(middlePos, endPos, Mathf.SmoothStep(0, 1, elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            rectTransform.localPosition = endPos;
            Text.gameObject.SetActive(false);
        }

        IEnumerator FadeInMission()
        {
            mission.gameObject.SetActive(true);
            float duration = 1f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                missionCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            missionCanvasGroup.alpha = 1;
        }

        IEnumerator FadeOutMission()
        {
            float duration = 1f;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                missionCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            missionCanvasGroup.alpha = 0;
            mission.gameObject.SetActive(false);
        }
    }
}
