/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Cgs.Menu
{
    public delegate IEnumerator DownloadCoroutineDelegate(string url);

    public class DownloadMenu : Modal
    {
        public Text labelText;
        public InputField urlInputField;
        public Button downloadButton;

        private DownloadCoroutineDelegate _downloadCoroutine;

        private void Update()
        {
            if (!IsFocused || urlInputField.isFocused)
                return;

            if ((Inputs.IsSubmit || Inputs.IsNew) && downloadButton.interactable)
                StartDownload();
            else if ((Inputs.IsSort || Inputs.IsLoad) && urlInputField.interactable)
                Clear();
            else if ((Inputs.IsFilter || Inputs.IsSave) && urlInputField.interactable)
                Paste();
            else if (Inputs.IsFocus && urlInputField.interactable)
                urlInputField.ActivateInputField();
            else if (Inputs.IsCancel || Inputs.IsOption)
                Hide();
        }

        public void Show(string label, string prompt, DownloadCoroutineDelegate downloadCoroutine)
        {
            Show();

            labelText.text = label;
            ((Text) urlInputField.placeholder).text = prompt;
            _downloadCoroutine = downloadCoroutine;
        }

        [UsedImplicitly]
        public void Clear()
        {
            urlInputField.text = string.Empty;
        }

        [UsedImplicitly]
        public void Paste()
        {
            if (urlInputField.interactable)
                urlInputField.text = UniClipboard.GetText();
        }

        [UsedImplicitly]
        public void CheckDownloadUrl(string url)
        {
            downloadButton.interactable = Uri.IsWellFormedUriString(url.Trim(), UriKind.Absolute);
        }

        [UsedImplicitly]
        public void StartDownload()
        {
            CardGameManager.Instance.StartCoroutine(Download());
        }

        private IEnumerator Download()
        {
            string url = urlInputField.text.Trim();

            urlInputField.text = string.Empty;
            urlInputField.interactable = false;

            Debug.Log("DownloadMenu: Download start");
            yield return _downloadCoroutine(url);
            Debug.Log("DownloadMenu: Download end");

            // ReSharper disable once Unity.InefficientPropertyAccess
            urlInputField.interactable = true;
            Hide();
        }
    }
}
