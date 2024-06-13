using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VictoryChallenge.Customize
{
    public class CharacterCustomizationsUI : MonoBehaviour
    {
        // Skinned
        [SerializeField] private Button _colorButton;
        [SerializeField] private Button _bodyPartsButton;
        [SerializeField] private Button _eyesButton;
        [SerializeField] private Button _glovesButton;
        [SerializeField] private Button _headPartsButton;
        [SerializeField] private Button _mouthAndNosesButton;
        [SerializeField] private Button _tailsButton;
        [SerializeField] private Button _changeButton;

        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _loadButton;

        // Mesh
        [SerializeField] private Button _earButton;
        [SerializeField] private Button _eyes2Button;
        [SerializeField] private Button _hatButton;

        [SerializeField] private PlayerCharacterCustomized _playerCharacterCustomized;




        private void Awake()
        {
            _colorButton.onClick.AddListener(() =>
            {
                Debug.Log("Color Button");
                _playerCharacterCustomized.ChangeSkinnedBodyPart(PlayerCharacterCustomized.BodyPartType.Color);
            });

            _changeButton.onClick.AddListener(() =>
            {
                Debug.Log("Color Button");
                _playerCharacterCustomized.ChangeSkinnedBodyPartLeft(PlayerCharacterCustomized.BodyPartType.Color);
            });

            _bodyPartsButton.onClick.AddListener(() =>
            {
                Debug.Log("BodyParts Button");
                _playerCharacterCustomized.ChangeSkinnedBodyPart(PlayerCharacterCustomized.BodyPartType.BodyParts);
            });

            _eyesButton.onClick.AddListener(() =>
            {
                Debug.Log("eyes Button");
                _playerCharacterCustomized.ChangeSkinnedBodyPart(PlayerCharacterCustomized.BodyPartType.Eyes);
            });

            _glovesButton.onClick.AddListener(() =>
            {
                Debug.Log("gloves Button");
                _playerCharacterCustomized.ChangeSkinnedBodyPart(PlayerCharacterCustomized.BodyPartType.Gloves);
            });

            _headPartsButton.onClick.AddListener(() =>
            {
                Debug.Log("headParts Button");
                _playerCharacterCustomized.ChangeSkinnedBodyPart(PlayerCharacterCustomized.BodyPartType.HeadParts);
            });

            _mouthAndNosesButton.onClick.AddListener(() =>
            {
                Debug.Log("Mouth Button");
                _playerCharacterCustomized.ChangeSkinnedBodyPart(PlayerCharacterCustomized.BodyPartType.Mouth);
            });

            _tailsButton.onClick.AddListener(() =>
            {
                Debug.Log("tails Button");
                _playerCharacterCustomized.ChangeSkinnedBodyPart(PlayerCharacterCustomized.BodyPartType.Tails);
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

            _earButton.onClick.AddListener(() =>
            {
                //Debug.Log("ear");
                _playerCharacterCustomized.OnChangeEarMesh();
            });

            _eyes2Button.onClick.AddListener(() =>
            {
                Debug.Log("eyes2");
                _playerCharacterCustomized.OnChangeEyeMesh();
            });

            _hatButton.onClick.AddListener(() =>
            {
                Debug.Log("hat");
                _playerCharacterCustomized.OnChangeHatMesh();
            });
        }
    }

}
