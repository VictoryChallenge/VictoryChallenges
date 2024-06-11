using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VictoryChallenge.Customize
{
    public class CharacterCustomizationsUI : MonoBehaviour
    {
        [SerializeField] private Button _colorButton;
        [SerializeField] private Button _bodyPartsButton;
        [SerializeField] private Button _eyesButton;
        [SerializeField] private Button _glovesButton;
        [SerializeField] private Button _headPartsButton;
        [SerializeField] private Button _mouthAndNosesButton;
        [SerializeField] private Button _tailsButton;


        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _loadButton;

        [SerializeField] private PlayerCharacterCustomized _playerCharacterCustomized;


        private void Awake()
        {
            _colorButton.onClick.AddListener(() =>
            {
                Debug.Log("Color Button");
                _playerCharacterCustomized.ChangeBodyPart(PlayerCharacterCustomized.BodyPartType.Color);
            });

            _bodyPartsButton.onClick.AddListener(() =>
            {
                Debug.Log("BodyParts Button");
                _playerCharacterCustomized.ChangeBodyPart(PlayerCharacterCustomized.BodyPartType.BodyParts);
            });

            _eyesButton.onClick.AddListener(() =>
            {
                Debug.Log("eyes Button");
                _playerCharacterCustomized.ChangeBodyPart(PlayerCharacterCustomized.BodyPartType.Eyes);
            });

            _glovesButton.onClick.AddListener(() =>
            {
                Debug.Log("gloves Button");
                _playerCharacterCustomized.ChangeBodyPart(PlayerCharacterCustomized.BodyPartType.Gloves);
            });

            _headPartsButton.onClick.AddListener(() =>
            {
                Debug.Log("headParts Button");
                _playerCharacterCustomized.ChangeBodyPart(PlayerCharacterCustomized.BodyPartType.HeadParts);
            });

            _mouthAndNosesButton.onClick.AddListener(() =>
            {
                Debug.Log("Mouth Button");
                _playerCharacterCustomized.ChangeBodyPart(PlayerCharacterCustomized.BodyPartType.Mouth);
            });

            _tailsButton.onClick.AddListener(() =>
            {
                Debug.Log("tails Button");
                _playerCharacterCustomized.ChangeBodyPart(PlayerCharacterCustomized.BodyPartType.Tails);
            });

            _loadButton.onClick.AddListener(() =>
            {
                Debug.Log("Load");
                _playerCharacterCustomized.Load();
            });

            _saveButton.onClick.AddListener(() =>
            {
                Debug.Log("Save");
                _playerCharacterCustomized.Save();
            });
        }
    }

}
