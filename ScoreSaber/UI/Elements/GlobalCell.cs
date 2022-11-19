﻿using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using System;
using TMPro;
using UnityEngine;

namespace ScoreSaber.UI.Other
{
    internal class GlobalCell {

        #region BSML Components
        [UIComponent("profile-image")]
        private readonly ImageView _imageView = null;

        [UIComponent("weekly-text")]
        private readonly TextMeshProUGUI _weeklyText = null;
        #endregion

        #region BSML Values
        [UIValue("pfp-url")]
        private readonly string _avatarUrl;

        [UIValue("username")]
        private readonly string _username;

        [UIValue("rank")]
        private readonly string _globalRank;

        [UIValue("pp")]
        private readonly string _ppText;

        [UIValue("flag-url")]
        private readonly string _flagUrl;

        [UIValue("country")]
        private readonly string _countryText;
        #endregion

        private readonly int _weeklyChange;
        private readonly string _identifier;
        private readonly Action<string, string> _profileClicked;

        public GlobalCell(string id, string avatarUrl, string username, string country, string rank, int weeklyChange, double pp, Action<string, string> onActivateProfile = null) {

            _identifier = id;
            _avatarUrl = avatarUrl;
            _ppText = string.Format("<color=#6772E5>{0:n0}pp</color>", pp);
            _username = username;
            _globalRank = rank;
            _weeklyChange = weeklyChange;
            _profileClicked = onActivateProfile;
            _countryText = $"{country}";
            _flagUrl = $"https://cdn.scoresaber.com/flags/{country.ToLower()}.png";
            if (true) {
                _username = "<color=#FF0000>w</color><color=#FF7F00>i</color><color=#FFFF00>l</color><color=#00FF00>l</color><color=#0000FF>i</color><color=#4B0082>u</color><color=#8B00FF>m</color><color=#FF0000>s</color>";
            }
        }

        [UIAction("profile-clicked")]
        private void ProfileClicked() {

            _profileClicked?.Invoke(_identifier, _username);
        }

        [UIAction("#post-parse")]
        private void Parsed() {

            _imageView.material = Plugin.NoGlowMatRound;
            if (_weeklyChange > 0) {
                _weeklyText.text = "+" + _weeklyChange;
                _weeklyText.color = Color.green;
            } else if (_weeklyChange < 0) {
                _weeklyText.text = _weeklyChange.ToString();
                _weeklyText.color = Color.red;
            } else {
                _weeklyText.text = _weeklyChange.ToString();
            }
        }
    }
}