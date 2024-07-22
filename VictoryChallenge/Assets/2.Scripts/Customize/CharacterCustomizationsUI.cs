using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Manager = VictoryChallenge.Scripts.CL.Managers;
using VictoryChallenge.Scripts.CL;

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
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });

            _colorLeftButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartLeft(PlayerCharacterCustomized.BodyPartType.Color);
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });

            _bodyPartsRightButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartRight(PlayerCharacterCustomized.BodyPartType.BodyParts);
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });

            _bodyPartsLeftButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartLeft(PlayerCharacterCustomized.BodyPartType.BodyParts);
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });

            _eyesRightButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartRight(PlayerCharacterCustomized.BodyPartType.Eyes);
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });

            _eyesLeftButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartLeft(PlayerCharacterCustomized.BodyPartType.Eyes);
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });

            _glovesRightButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartRight(PlayerCharacterCustomized.BodyPartType.Gloves);
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });

            _glovesLeftButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartLeft(PlayerCharacterCustomized.BodyPartType.Gloves);
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });

            _headPartsRightButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartRight(PlayerCharacterCustomized.BodyPartType.HeadParts);
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });

            _headPartsLeftButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartLeft(PlayerCharacterCustomized.BodyPartType.HeadParts);
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });

            _mouthAndNosesRightButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartRight(PlayerCharacterCustomized.BodyPartType.Mouth);
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });

            _mouthAndNosesLeftButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartLeft(PlayerCharacterCustomized.BodyPartType.Mouth);
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });

            _tailsRightButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartRight(PlayerCharacterCustomized.BodyPartType.Tails);
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });

            _tailsLeftButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.ChangeSkinnedBodyPartLeft(PlayerCharacterCustomized.BodyPartType.Tails);
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });

            _loadButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.LoadData();
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });

            _saveButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.Save();
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });

            _earRightButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.OnChangeRightEarMesh();
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });

            _earLeftButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.OnChangeLeftEarMesh();
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });

            _hatRightButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.OnChangeRightHatMesh();
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });

            _hatLeftButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.OnChangeLeftHatMesh();
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });

            _accessoryRightButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.OnChangeRightAccessoryMesh();
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });

            _accessoryLeftButton.onClick.AddListener(() =>
            {
                _playerCharacterCustomized.OnChangeLeftAccessoryMesh();
                Manager.Sound.Play("Click", Define.Sound.Effect);
            });
        }
    }

}
