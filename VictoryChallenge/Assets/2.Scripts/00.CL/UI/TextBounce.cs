using System.Collections;
using TMPro;
using UnityEngine;

namespace VictoryChallenge.Scripts.CL
{ 
    public class TextBounce : MonoBehaviour
    {
        public TextMeshProUGUI[] textLetters; // loading...�� �̷�� ��� ���ڵ�
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
            // 'loading...'�� �� ���� �ִϸ��̼� ����
            for (int i = 0; i < textLetters.Length; i++)
            {
                coroutines[i] = StartCoroutine(BounceLetter(textLetters[i], i * delayBetweenLetters));
                yield return new WaitForSeconds(delayBetweenLetters);
            }

            // ��� �ִϸ��̼��� ������ ��ٸ�
            yield return new WaitForSeconds(animationDuration * 2 + (textLetters.Length - 1) * delayBetweenLetters);

            // ��� �ִϸ��̼� �ڷ�ƾ�� ����
            for (int i = 0; i < textLetters.Length; i++)
            {
                if (coroutines[i] != null)
                {
                    StopCoroutine(coroutines[i]);
                    coroutines[i] = null;
                }
                // ������ ��ġ�� ������� �缳��
                textLetters[i].transform.localPosition = new Vector3(textLetters[i].transform.localPosition.x, 0, textLetters[i].transform.localPosition.z);
            }

            // ���� ������ �����ϱ� ���� ��� ���
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

            letter.transform.localPosition = targetPosition; // ��ġ ���� ����
            elapsedTime = 0f;

            while (elapsedTime < animationDuration)
            {
                letter.transform.localPosition = Vector3.Lerp(targetPosition, originalPosition, (elapsedTime / animationDuration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            letter.transform.localPosition = originalPosition; // ��ġ ���� ����
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            // ������ ��ġ�� ������� �缳��
            for (int i = 0; i < textLetters.Length; i++)
            {
                textLetters[i].transform.localPosition = new Vector3(textLetters[i].transform.localPosition.x, 0, textLetters[i].transform.localPosition.z);
            }
        }
    }
}
