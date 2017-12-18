﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardInfoViewer : MonoBehaviour, IPointerDownHandler, ISelectHandler, IDeselectHandler
{
    public const string CardInfoViewerTag = "CardInfoViewer";
    public const string SetLabel = "Set";
    public const float VisibleYMin = 0.625f;
    public const float VisibleYMax = 1;
    public const float HiddenYmin = 1.025f;
    public const float HiddenYMax = 1.4f;
    public const float AnimationSpeed = 5.0f;

    public RectTransform infoPanel;
    public RectTransform zoomPanel;
    public Image cardImage;
    public Text nameText;
    public Text idText;
    public Dropdown propertySelection;
    public Text labelText;
    public Text contentText;

    public List<Dropdown.OptionData> PropertyOptions { get; } = new List<Dropdown.OptionData>();

    private static CardInfoViewer _instance;
    private CardModel _selectedCardModel;
    private int _selectedPropertyIndex;
    private bool _isVisible;

    public void Reset()
    {
        cardImage.gameObject.GetOrAddComponent<AspectRatioFitter>().aspectRatio = CardGameManager.Current.AspectRatio;
        zoomPanel.GetChild(0).gameObject.GetOrAddComponent<AspectRatioFitter>().aspectRatio = CardGameManager.Current.AspectRatio;

        SelectedPropertyIndex = 0;
        PropertyOptions.Clear();
        PropertyOptions.Add(new Dropdown.OptionData() { text = SetLabel });
        foreach (PropertyDef propDef in CardGameManager.Current.CardProperties) {
            PropertyOptions.Add(new Dropdown.OptionData() { text = propDef.Name });
            if (propDef.Name.Equals(CardGameManager.Current.CardPrimaryProperty))
                SelectedPropertyIndex = PropertyOptions.Count - 1;
        }
        propertySelection.options = PropertyOptions;
        propertySelection.value = SelectedPropertyIndex;
        propertySelection.onValueChanged.Invoke(SelectedPropertyIndex);
    }

    public void SetContentText()
    {
        contentText.text = string.Empty;
        if (SelectedCardModel != null)
            contentText.text = SelectedPropertyIndex != 0 ? SelectedCardModel.Value.GetPropertyValueString(SelectedPropertyName) : CardGameManager.Current.Sets [SelectedCardModel.Value.SetCode].ToString();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        EventSystem.current.SetSelectedGameObject(gameObject, eventData);
    }

    public void OnSelect(BaseEventData eventData)
    {
        IsVisible = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        IsVisible = false;
    }

    public void ShowCardZoomed()
    {
        zoomPanel.gameObject.SetActive(true);
        zoomPanel.GetChild(0).GetComponent<Image>().sprite = cardImage.sprite;
    }

    public void HideCardZoomed()
    {
        zoomPanel.gameObject.SetActive(false);
    }

    void Update()
    {
        if (SelectedCardModel == null)
            IsVisible = false;
        
        infoPanel.anchorMin = IsVisible ? 
            new Vector2(infoPanel.anchorMin.x, Mathf.Lerp(infoPanel.anchorMin.y, VisibleYMin, AnimationSpeed * Time.deltaTime)) :
            new Vector2(infoPanel.anchorMin.x, Mathf.Lerp(infoPanel.anchorMin.y, HiddenYmin, AnimationSpeed * Time.deltaTime));
        infoPanel.anchorMax = IsVisible ? 
            new Vector2(infoPanel.anchorMax.x, Mathf.Lerp(infoPanel.anchorMax.y, VisibleYMax, AnimationSpeed * Time.deltaTime)) :
            new Vector2(infoPanel.anchorMax.x, Mathf.Lerp(infoPanel.anchorMax.y, HiddenYMax, AnimationSpeed * Time.deltaTime));
    }

    public static CardInfoViewer Instance {
        get {
            if (_instance != null) return _instance;
            GameObject cardInfoViewer = GameObject.FindWithTag(CardInfoViewerTag);
            if (cardInfoViewer != null)
                _instance = cardInfoViewer.GetOrAddComponent<CardInfoViewer>();
            return _instance;
        }
    }

    public string SelectedPropertyName {
        get {
            string selectedName = SetLabel;
            if (SelectedPropertyIndex >= 1 && SelectedPropertyIndex < PropertyOptions.Count)
                selectedName = PropertyOptions[SelectedPropertyIndex].text;
            return selectedName;
        }
    }

    public int SelectedPropertyIndex {
        get { return _selectedPropertyIndex; }
        set {
            if (value < 0 || value >= PropertyOptions.Count) return;
            _selectedPropertyIndex = value;
            labelText.text = SelectedPropertyName;
            SetContentText();
        }
    }

    public CardModel SelectedCardModel {
        get { return _selectedCardModel; }
        set {
            if (_selectedCardModel != null)
                _selectedCardModel.HideHighlight();
            
            _selectedCardModel = value;

            if (_selectedCardModel == null) {
                IsVisible = false;
                return;
            }
            cardImage.sprite = _selectedCardModel.GetComponent<Image>().sprite;
            nameText.text = _selectedCardModel.Value.Name;
            idText.text = _selectedCardModel.Value.Id;
            SetContentText();

            IsVisible = true;
        }
    }

    public bool IsVisible {
        get { return _isVisible; }
        set {
            _isVisible = value;
            if (!_isVisible && zoomPanel != null)
                zoomPanel.gameObject.SetActive(false);

            if (SelectedCardModel == null)
                return;

            if (_isVisible)
                SelectedCardModel.ShowHighlight();
            else
                SelectedCardModel.HideHighlight();
        }
    }
}
