/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using CardGameDef.Unity;
using Cgs.CardGameView;
using UnityEngine;
using UnityEngine.UI;

namespace Cgs.Play.Drawer
{
    public class CardDrawer : MonoBehaviour
    {
        public static readonly Vector2 ShownPosition = Vector2.zero;

        public static Vector2 HiddenPosition =>
            new Vector2(0, -(CardGameManager.PixelsPerInch * CardGameManager.Current.CardSize.Y) - 10);

        public StackViewer viewer;
        public Button downButton;
        public Button upButton;
        public RectTransform panelRectTransform;

        private void OnEnable()
        {
            CardGameManager.Instance.OnSceneActions.Add(Resize);
        }

        private void Update()
        {
            if (CardGameManager.Instance.ModalCanvas != null)
                return;

            if (!Inputs.IsSort)
                return;

            if (upButton.interactable)
                Show();
            else
                Hide();
        }

        private void Resize()
        {
            viewer.Resize();
        }

        public void Show()
        {
            panelRectTransform.anchoredPosition = ShownPosition;
            downButton.interactable = true;
            upButton.interactable = false;
        }

        public void AddCard(UnityCard card)
        {
            viewer.AddCard(card);
        }

        public void Clear()
        {
            viewer.Clear();
        }

        public void Hide()
        {
            panelRectTransform.anchoredPosition = HiddenPosition;
            downButton.interactable = false;
            upButton.interactable = true;
        }

    }
}
