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
        private float time = 300;
        private Image mission;
        private TextMeshProUGUI roundText;
        private TextMeshProUGUI personText;
        private CanvasGroup missionCanvasGroup;
        // 사람인원수 나중에 카운트할때
        public int winner;
        public int person;
        private int round;

        // 캐릭터 이동을 막기 위한 불값 
        private bool isMoving;

        void Start()
        {
            Init();
            StartCoroutine(GameStart());

            switch (SceneManager.GetActiveScene().buildIndex)
            {
                case 3:
                    round = 1;
                    person = 4;
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    break;
            }
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

            personText = GetTextMeshPro((int)TMPs.Person);
            personText.gameObject.SetActive(false);
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

            // 시간 가게하기
            if (isMoving == true && time > 0)
            { 
                time -= Time.deltaTime;
                int displayTime = Mathf.CeilToInt(time); // 시간을 올림하여 정수로 변환
                GetTextMeshPro((int)TMPs.Time).text = displayTime.ToString();
                
                if (time == 0)
                    GetTextMeshPro((int)TMPs.Finish).text = "GameOver";
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
            }

            // ROUND 텍스트 코루틴
            yield return StartCoroutine(AnimateRoundText());

            // 카메라 워킹
            //// 미션 애니메이션
            //yield return StartCoroutine(FadeInMission());
            //yield return new WaitForSeconds(1f);
            //yield return StartCoroutine(FadeOutMission());

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

            personText.gameObject.SetActive(true);

            text.text = "달려라~";
            yield return new WaitForSeconds(1f);

            text.gameObject.SetActive(false);
        }

        IEnumerator AnimateRoundText()
        {
            yield return new WaitForSeconds(0.5f);

            roundText.gameObject.SetActive(true);
            roundText.text = $"Round {round}";

            RectTransform rectTransform = roundText.GetComponent<RectTransform>();

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
            roundText.gameObject.SetActive(false);
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
