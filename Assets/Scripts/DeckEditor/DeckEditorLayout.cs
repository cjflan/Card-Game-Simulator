using UnityEngine;

public class DeckEditorLayout : MonoBehaviour
{
    public const float WidthCheck = 1000f;
    public static readonly Vector2 DeckButtonsLandscapePosition = new Vector2(-650f, 0f);

    public static readonly Vector2 SearchNamePortraitPosition = new Vector2(150f, 450f);
    public static readonly Vector2 SearchNameLandscapePosition = new Vector2(150f, 367.5f);

    public Vector2 DeckButtonsPortraitPosition => new Vector2(0f, -(GetComponent<RectTransform>().rect.height + 87.5f));

    public RectTransform filterButton;
    public RectTransform sortButton;
    public RectTransform deckButtons;
    public RectTransform searchName;
    public RectTransform deckSelectorButtons;
    public RectTransform resultsSelectorButtons;
    public SearchResults searchResults;

    void Start()
    {
        if (GetComponent<RectTransform>().rect.width < WidthCheck)
            deckButtons.anchoredPosition = DeckButtonsPortraitPosition;
    }

    void OnRectTransformDimensionsChange()
    {
        if (!gameObject.activeInHierarchy)
            return;

        deckButtons.anchoredPosition = GetComponent<RectTransform>().rect.width < WidthCheck ? DeckButtonsPortraitPosition : DeckButtonsLandscapePosition;

        searchName.anchoredPosition = GetComponent<RectTransform>().rect.width < WidthCheck ? SearchNamePortraitPosition : SearchNameLandscapePosition;
        searchResults.CurrentPageIndex = 0;
        searchResults.UpdateSearchResultsPanel();

        CardInfoViewer.Instance.IsVisible = false;
    }
}
