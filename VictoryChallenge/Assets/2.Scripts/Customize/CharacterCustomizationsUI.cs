using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace VictoryChallenge.Customize
{
    public class CharacterCustomizationsUI : MonoBehaviour
    {
        // Skinned

        // Color
        [SerializeField] private Button _colorRightButton;
        [SerializeField] private Button _colorLeftButton;

        // BodyPart
        [SerializeField] private Button _bodyPartsRightButton;
        [SerializeField] private Button _bodyPartsLeftButton;

        // Eye
        [SerializeField] private Button _eyesRightButton;
        [SerializeField] private Button _eyesLeftButton;

        // Glove
        [SerializeField] private Button _glovesRightButton;
        [SerializeField] private Button _glovesLeftButton;

        // HeadPart
        [SerializeField] private Button _headPartsRightButton;
        [SerializeField] private Button _headPartsLeftButton;

        // MouseAndNoses
        [SerializeField] private Button _mouthAndNosesRightButton;
        [SerializeField] private Button _mouthAndNosesLeftButton;

        // Tails
        [SerializeField] private Button _tailsRightButton;
        [SerializeField] private Button _tailsLeftButton;

        // Save & Load
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _loadButton;

        // Mesh
        // Ear
        [SerializeField] private Button _earRightButton;
        [SerializeField] private Button _earLeftButton;

        // Accessory
        [SerializeField] private Button _accessoryRightButton;
        [SerializeField] private Button _accessoryLeftButton;

        // Hat
        [SerializeField] private Button _hatRightButton;
        [SerializeField] private Button _hatLeftButton;

        [SerializeField] private PlayerCharacterCustomized _playerCharacterCustomized;


        private void Awake()
        {
            _colorRightButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartRight(PlayerCharacterCustomized.BodyPartType.Color);
            });

            _colorLeftButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartLeft(PlayerCharacterCustomized.BodyPartType.Color);
            });

            _bodyPartsRightButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartRight(PlayerCharacterCustomized.BodyPartType.BodyParts);
            });

            _bodyPartsLeftButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartLeft(PlayerCharacterCustomized.BodyPartType.BodyParts);
            });

            _eyesRightButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartRight(PlayerCharacterCustomized.BodyPartType.Eyes);
            });

            _eyesLeftButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartLeft(PlayerCharacterCustomized.BodyPartType.Eyes);
            });

            _glovesRightButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartRight(PlayerCharacterCustomized.BodyPartType.Gloves);
            });

            _glovesLeftButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartLeft(PlayerCharacterCustomized.BodyPartType.Gloves);
            });

            _headPartsRightButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartRight(PlayerCharacterCustomized.BodyPartType.HeadParts);
            });

            _headPartsLeftButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartLeft(PlayerCharacterCustomized.BodyPartType.HeadParts);
            });

            _mouthAndNosesRightButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartRight(PlayerCharacterCustomized.BodyPartType.Mouth);
            });

            _mouthAndNosesLeftButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartLeft(PlayerCharacterCustomized.BodyPartType.Mouth);
            });

            _tailsRightButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartRight(PlayerCharacterCustomized.BodyPartType.Tails);
            });

            _tailsLeftButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartLeft(PlayerCharacterCustomized.BodyPartType.Tails);
            });

            _loadButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.Load();
            });

            _saveButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.Save();
            });

            _earRightButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.OnChangeRightEarMesh();
            });

            _earLeftButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.OnChangeLeftEarMesh();
            });

            _hatRightButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.OnChangeRightHatMesh();
            });

            _hatLeftButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.OnChangeLeftHatMesh();
            });

            _accessoryRightButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.OnChangeRightAccessoryMesh();
            });

            _accessoryLeftButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.OnChangeLeftAccessoryMesh();
            });
        }
    }

}
