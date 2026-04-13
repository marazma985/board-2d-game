using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DopamineButton : MonoBehaviour
{
    [Header("Цвета градиента")]
    [SerializeField] private Color gradientStart = new Color(1f, 0.4f, 0.7f);
    [SerializeField] private Color gradientEnd = new Color(1f, 0.8f, 0.2f);

    [Header("Настройки")]
    [SerializeField] private float cornerRadius = 20f;
    [SerializeField] private float shadowDistance = 4f;
    [SerializeField] private Color shadowColor = new Color(0f, 0f, 0f, 0.3f);
    [SerializeField] private Color borderColor = Color.white;
    [SerializeField] private float borderWidth = 3f;

    [Header("Анимация")]
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float pressScale = 0.95f;
    [SerializeField] private float animationSpeed = 0.1f;

    private Image buttonImage;
    private Button button;
    private Vector3 originalScale;
    private Vector3 targetScale;

    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        originalScale = transform.localScale;
        targetScale = originalScale;

        SetupButton();
        button.onClick.AddListener(OnButtonClick);
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime / animationSpeed);
    }

    void SetupButton()
    {
        // Создаем спрайт с закругленными углами
        CreateRoundedGradientSprite();

        CreateShadow();
        CreateBorder();
        SetupText();
    }

    void CreateRoundedGradientSprite()
    {
        int width = 64;
        int height = 64;
        int cornerSize = Mathf.RoundToInt(cornerRadius * (width / 100f));

        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color[] pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Проверяем углы для скругления
                bool inCorner = false;

                // Левый нижний угол
                if (x < cornerSize && y < cornerSize)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cornerSize, cornerSize));
                    if (dist > cornerSize) inCorner = true;
                }
                // Правый нижний угол
                else if (x >= width - cornerSize && y < cornerSize)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(width - 1 - cornerSize, cornerSize));
                    if (dist > cornerSize) inCorner = true;
                }
                // Левый верхний угол
                else if (x < cornerSize && y >= height - cornerSize)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(cornerSize, height - 1 - cornerSize));
                    if (dist > cornerSize) inCorner = true;
                }
                // Правый верхний угол
                else if (x >= width - cornerSize && y >= height - cornerSize)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(width - 1 - cornerSize, height - 1 - cornerSize));
                    if (dist > cornerSize) inCorner = true;
                }

                if (inCorner)
                {
                    pixels[y * width + x] = Color.clear;
                }
                else
                {
                    // Создаем градиент
                    float t = (float)y / (height - 1);
                    pixels[y * width + x] = Color.Lerp(gradientStart, gradientEnd, t);
                }
            }
        }

        texture.SetPixels(pixels);
        texture.Apply();
        texture.filterMode = FilterMode.Bilinear;

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, width, height), Vector2.one * 0.5f, 100, 0, SpriteMeshType.FullRect, Vector4.one * cornerRadius);
        buttonImage.sprite = sprite;
        buttonImage.type = Image.Type.Sliced;
    }

    void CreateShadow()
    {
        GameObject shadowObj = new GameObject("Shadow");
        shadowObj.transform.SetParent(transform, false);
        shadowObj.transform.SetAsFirstSibling();

        RectTransform shadowRect = shadowObj.AddComponent<RectTransform>();
        shadowRect.anchorMin = Vector2.zero;
        shadowRect.anchorMax = Vector2.one;
        shadowRect.sizeDelta = Vector2.zero;
        shadowRect.anchoredPosition = new Vector2(shadowDistance, -shadowDistance);

        Image shadowImage = shadowObj.AddComponent<Image>();
        shadowImage.color = shadowColor;
        shadowImage.sprite = buttonImage.sprite;
        shadowImage.type = Image.Type.Sliced;
    }

    void CreateBorder()
    {
        GameObject borderObj = new GameObject("Border");
        borderObj.transform.SetParent(transform, false);
        borderObj.transform.SetAsLastSibling();

        RectTransform borderRect = borderObj.AddComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.sizeDelta = Vector2.zero;
        borderRect.anchoredPosition = Vector2.zero;

        Image borderImage = borderObj.AddComponent<Image>();
        borderImage.color = borderColor;

        Texture2D borderTexture = new Texture2D(1, 1);
        borderTexture.SetPixel(0, 0, Color.white);
        borderTexture.Apply();

        Sprite borderSprite = Sprite.Create(borderTexture, new Rect(0, 0, 1, 1), Vector2.one * 0.5f, 100);
        borderImage.sprite = borderSprite;
        borderImage.type = Image.Type.Sliced;
        borderRect.sizeDelta = new Vector2(-borderWidth * 2, -borderWidth * 2);
    }

    void SetupText()
    {
        TextMeshProUGUI text = GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.fontSize = 24;
            text.fontStyle = FontStyles.Bold;
            text.color = Color.white;
            text.enableWordWrapping = false;
            text.outlineWidth = 0.1f;
            text.outlineColor = new Color(0f, 0f, 0f, 0.5f);
        }
    }

    public void OnPointerEnter()
    {
        targetScale = originalScale * hoverScale;
    }

    public void OnPointerExit()
    {
        targetScale = originalScale;
    }

    public void OnPointerDown()
    {
        targetScale = originalScale * pressScale;
    }

    public void OnPointerUp()
    {
        targetScale = originalScale * hoverScale;
    }

    void OnButtonClick()
    {
        targetScale = originalScale;
    }
}