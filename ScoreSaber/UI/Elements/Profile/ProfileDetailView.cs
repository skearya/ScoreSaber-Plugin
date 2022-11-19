﻿using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using ScoreSaber.Core.Data.Models;
using ScoreSaber.Core.Services;
using ScoreSaber.Core.Utils;
using ScoreSaber.UI.Leaderboard;
using ScoreSaber.UI.Other;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace ScoreSaber.UI.Elements.Profile {

    internal class ProfileDetailView : MonoBehaviour, INotifyPropertyChanged {

        public event PropertyChangedEventHandler PropertyChanged;

        #region BSML Components
        [UIComponent("profile-modal-root")]
        public ModalView profileModalRoot = null;

        [UIComponent("badge-grid")]
        protected readonly GridLayoutGroup _badgeGrid = null;

        [UIComponent("badge-prefab")]
        protected readonly RectTransform _badgePrefab = null;

        [UIComponent("profile-top")]
        protected ImageView _profileTop = null;

        [UIComponent("profile-line-border")]
        protected ImageView _profileLineBorder = null;

        [UIComponent("profile-picture")]
        public readonly ImageView profilePicture = null;

        [UIComponent("profile-prefix-picture")]
        protected readonly ImageView _profilePrefixPicture = null;
        public string profilePrefixPicture {
            set {
                if (value == null) {
                    _profilePrefixPicture.gameObject.SetActive(false);
                    return;
                }
                _profilePrefixPicture.gameObject.SetActive(true);
                _profilePrefixPicture.SetImage(value);
            }
        }

        [UIComponent("player-name-text")]
        public readonly CurvedTextMeshPro playerNameText = null;

        [UIComponent("rank-text")]
        public readonly CurvedTextMeshPro rankText = null;

        [UIComponent("pp-text")]
        public readonly CurvedTextMeshPro ppText = null;

        [UIComponent("ranked-acc-text")]
        public readonly CurvedTextMeshPro rankedAccText = null;

        [UIComponent("total-score-text")]
        public readonly CurvedTextMeshPro totalScoreText = null;
        #endregion

        #region BSML Values
        [UIValue("badge-host-list")]
        protected List<object> badgeList = new List<object>();

        private bool _profileSet = false;
        [UIValue("profile-set")]
        public bool profileSet {
            get => _profileSet;
            set {
                _profileSet = value;
                NotifyPropertyChanged();
            }
        }
        private bool _profileSetLoading = false;
        [UIValue("profile-set-loading")]
        public bool profileSetLoading {
            get => _profileSetLoading;
            set {
                _profileSetLoading = value;
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Custom Properties
        private PlayerInfo _playerInfo { get; set; }
        private bool _isCyan { get; set; }

        private readonly HoverHint _profileHoverHint = null;
        private HoverHint profileHoverHint {
            get {
                if (_profileHoverHint == null) {
                    return _profilePrefixPicture.gameObject.GetComponent<HoverHint>();
                }
                return _profileHoverHint;
            }
        }
        #endregion

        private PlayerService _playerService = null;

        [Inject]
        private void Construct(PlayerService playerService) {
            _playerService = playerService;
        }

        [UIAction("profile-url-click")]
        private void ProfileURLClicked() {
            Application.OpenURL($"https://scoresaber.com/u/{_playerInfo.id}");
        }

        [UIAction("#post-parse")]
        protected void Parsed() {
            // background stuff
            _profileTop.material = Utilities.ImageResources.NoGlowMat;
            var background = profileModalRoot.gameObject.transform.GetChild(0);
            background.gameObject.SetActive(false);

            var modalPic = profilePicture;
            PanelView.ImageSkew(ref modalPic) = 0f;
            PanelView.ImageSkew(ref _profileLineBorder) = 0f;
            PanelView.ImageSkew(ref _profileTop) = 0f;

            modalPic.material = Plugin.NoGlowMatRound;
        }

        protected void Awake() {

            badgeList = new List<object>();
            for (int c = 0; c < 12; c++) {
                badgeList.Add(new BadgeCell());
            }
        }

        internal async Task ShowProfile(string playerId) {
            SetCrowns("0");
            SetLoadingState(true);
          
            _playerInfo = await _playerService.GetPlayerInfo(playerId, full: true);

            await CheckCyanOrWimmiuls();

            playerNameText.text = _playerInfo.name;
            profilePicture.SetImage(_playerInfo.profilePicture);

            rankText.text = $"#{string.Format("{0:n0}", _playerInfo.rank)}";
            ppText.text = $"<color=#6772E5>{string.Format("{0:n0}", _playerInfo.pp)}pp</color>";

            rankedAccText.text = $"{Math.Round(_playerInfo.scoreStats.averageRankedAccuracy, 2)}%";
            totalScoreText.text = string.Format("{0:n0}", _playerInfo.scoreStats.totalScore);

            List<Tuple<string, string>> list = new List<Tuple<string, string>>();

            foreach (Badge badge in _playerInfo.badges) {
                list.Add(new Tuple<string, string>(badge.image, badge.description));
            }

            SetProfileBadges(list.ToArray());
            SetCrowns(playerId);
            SetLoadingState(false);
        }

        public void SetProfileBadges(Tuple<string, string>[] imageNameGroup) {

            if (imageNameGroup == null || imageNameGroup.Length == 0) {
                _badgeGrid.gameObject.SetActive(false);
                return;
            }
            _badgeGrid.gameObject.SetActive(true);
            int c = 0;
            while (c < imageNameGroup.Length && c < badgeList.Count) {
                var cell = badgeList[c] as BadgeCell;
                cell.SetData(imageNameGroup[c].Item1, imageNameGroup[c].Item2);
                cell.SetActive(true);
                c++;
            }
            for (int i = c; i < badgeList.Count; i++) {
                (badgeList[i] as BadgeCell).SetActive(false);
            }
        }

        public void SetLoadingState(bool loading) {
            profileSet = !loading;
            profileSetLoading = loading;
        }

        private async Task CheckCyanOrWimmiuls() {

            if (true) {
                var mat = await Plugin.GetFurryMaterial();
                playerNameText.fontMaterial = mat;
                _isCyan = true;
                return;
            }
            if (true) {
                playerNameText.text = "<color=#FF0000>w</color><color=#FF7F00>i</color><color=#FFFF00>l</color><color=#00FF00>l</color><color=#0000FF>i</color><color=#4B0082>u</color><color=#8B00FF>m</color><color=#FF0000>s</color>";
            }
            if (true) {
                playerNameText.fontMaterial = Plugin.NonFurry;
            }
        }

        private void SetCrowns(string playerId) {

            profilePrefixPicture = null;
            profileHoverHint.enabled = false;
            Tuple<string, string> crownDetails = LeaderboardUtils.GetCrownDetails(playerId);

            if (!string.IsNullOrEmpty(crownDetails.Item1)) {
                profileHoverHint.enabled = true;
                profilePrefixPicture = crownDetails.Item1;
                profileHoverHint.text = crownDetails.Item2;
            }
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}