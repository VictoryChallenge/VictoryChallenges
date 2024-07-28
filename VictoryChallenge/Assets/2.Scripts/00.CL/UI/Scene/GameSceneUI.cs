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
        // 사람인원수 나중에 카운트할때
        public int person;
        private int round;

        // 캐릭터 이동을 막기 위한 불값 
        private bool isMoving;
        private GameManagerCL gameManager;

        // 장애물
        private ObstacleManager _obstacleManager;
        private ConveyorBelt _conveyorBelt;
        private ConveyorBelt _conveyorBelt2;

        void Start()
        {
            Init();
            // 로그인안하고 씬에서 테스트만 하려면 아래코드 주석해제
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

            #region 장애물
            // 장애물
            _obstacleManager = FindObjectOfType<ObstacleManager>();

            // 오른쪽 트랙
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

            // 왼쪽 트랙
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
            // 옵션팝업 띄우기
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

            // 시간 가게하기
            if (time > 0)
            {
                int displayTime = Mathf.CeilToInt(time); // 시간을 올림하여 정수로 변환
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
                    GetTextMeshPro((int)TMPs.Time).text = "0"; // time을 정확히 0으로 설정
                }
            }
            // 인원수
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

            // ROUND 텍스트 코루틴
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

            // 2인용 맵에서는 안보이게
            if (SceneManager.GetActiveScene().buildIndex != 6)
            { 
                personText.gameObject.SetActive(true);
                clock.gameObject.SetActive(true);
            }

            text.text = "달려라~";
            yield return new WaitForSeconds(1f);

            text.gameObject.SetActive(false);

            // 장애물
            _obstacleManager = FindObjectOfType<ObstacleManager>();

            if (_obstacleManager != null)
            {
                _obstacleManager.obstaclespawn = true;
                StartCoroutine(_obstacleManager.SpawnObstacles());
            }

            if (_conveyorBelt != null)
            {
                _conveyorBelt.EnableConveyerBelt();
                Debug.Log("오른쪽 컨베이어 벨트");
            }

            if (_conveyorBelt2 != null)
            {
                _conveyorBelt2.EnableConveyerBelt();
                Debug.Log("왼쪽 컨베이어 벨트");
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

            float duration = 1.3f; // 텍스트가 가운데로 이동하는데 걸리는 시간
            float elapsedTime = 0f;

            // 왼쪽에서 가운데로 이동
            while (elapsedTime < duration)
            {
                rectTransform.localPosition = Vector3.Lerp(startPos, middlePos, Mathf.SmoothStep(0, 1, elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 미션 애니메이션
            StartCoroutine(FadeInMission());
            rectTransform.localPosition = middlePos;
            yield return new WaitForSeconds(2.5f); // 가운데에서 1초 머무름
            yield return StartCoroutine(FadeOutMission());

            elapsedTime = 0f;
            duration = 1.3f; // 텍스트가 오른쪽으로 이동하는데 걸리는 시간

            
            // 가운데에서 오른쪽으로 이동
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
                Debug.Log("오른쪽 컨베이어 벨트 disable");
            }

            if (_conveyorBelt2 != null)
            {
                _conveyorBelt2.DisableConveyerBelt();
                Debug.Log("왼쪽 컨베이어 벨트 disable");
            }

            Managers.Sound.Play("Whistle", Define.Sound.Effect);

            Text.gameObject.SetActive(true);
            Text.text = $"Finish";

            RectTransform rectTransform = Text.GetComponent<RectTransform>();

            Vector3 startPos = new Vector3(-Screen.width, rectTransform.localPosition.y, 0);
            Vector3 middlePos = new Vector3(0, rectTransform.localPosition.y, 0);
            Vector3 endPos = new Vector3(Screen.width, rectTransform.localPosition.y, 0);

            float duration = 1.2f; // 텍스트가 가운데로 이동하는데 걸리는 시간
            float elapsedTime = 0f;

            // 왼쪽에서 가운데로 이동
            while (elapsedTime < duration)
            {
                rectTransform.localPosition = Vector3.Lerp(startPos, middlePos, Mathf.SmoothStep(0, 1, elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // 미션 애니메이션
            rectTransform.localPosition = middlePos;
            yield return new WaitForSeconds(2.5f); // 가운데에서 1초 머무름

            elapsedTime = 0f;
            duration = 1.2f; // 텍스트가 오른쪽으로 이동하는데 걸리는 시간


            // 가운데에서 오른쪽으로 이동
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
