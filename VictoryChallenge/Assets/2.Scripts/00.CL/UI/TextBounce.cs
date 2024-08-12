using System.Collections;
using TMPro;
using UnityEngine;

namespace VictoryChallenge.Scripts.CL
{ 
    public class TextBounce : MonoBehaviour
    {
        public TextMeshProUGUI[] textLetters; // loading...을 이루는 모든 글자들
        float bounceHeight = 20f;
        float animationDuration = 0.2f;
        float delayBetweenLetters = 0.07f;
        Coroutine[] coroutines;

        void OnEnable()
        {
            coroutines = new Coroutine[textLetters.Length];
            StartCoroutine(BounceText());
        }

        IEnumerator BounceText()
        {
            // 'loading...'의 각 글자 애니메이션 시작
            for (int i = 0; i < textLetters.Length; i++)
            {
                coroutines[i] = StartCoroutine(BounceLetter(textLetters[i], i * delayBetweenLetters));
                yield return new WaitForSeconds(delayBetweenLetters);
            }

            // 모든 애니메이션이 끝나길 기다림
            yield return new WaitForSeconds(animationDuration * 2 + (textLetters.Length - 1) * delayBetweenLetters);

            // 모든 애니메이션 코루틴을 중지
            for (int i = 0; i < textLetters.Length; i++)
            {
                if (coroutines[i] != null)
                {
                    StopCoroutine(coroutines[i]);
                    coroutines[i] = null;
                }
                // 글자의 위치를 원래대로 재설정
                textLetters[i].transform.localPosition = new Vector3(textLetters[i].transform.localPosition.x, 0, textLetters[i].transform.localPosition.z);
            }

            // 다음 루프를 시작하기 전에 잠시 대기
            yield return new WaitForSeconds(0.05f);

            gameObject.SetActive(false);
        }

        IEnumerator BounceLetter(TextMeshProUGUI letter, float delay)
        {
            yield return new WaitForSeconds(delay);

            Vector3 originalPosition = letter.transform.localPosition;
            Vector3 targetPosition = originalPosition + new Vector3(0, bounceHeight, 0);
            float elapsedTime = 0f;

            while (elapsedTime < animationDuration)
            {
                letter.transform.localPosition = Vector3.Lerp(originalPosition, targetPosition, (elapsedTime / animationDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            letter.transform.localPosition = targetPosition; // 위치 강제 설정
            elapsedTime = 0f;

            while (elapsedTime < animationDuration)
            {
                letter.transform.localPosition = Vector3.Lerp(targetPosition, originalPosition, (elapsedTime / animationDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            letter.transform.localPosition = originalPosition; // 위치 강제 설정
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            // 글자의 위치를 원래대로 재설정
            for (int i = 0; i < textLetters.Length; i++)
            {
                textLetters[i].transform.localPosition = new Vector3(textLetters[i].transform.localPosition.x, 0, textLetters[i].transform.localPosition.z);
            }
        }
    }
}
