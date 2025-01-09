using MonoMod.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace SUI;

public class SUiElement
{
    public RectTransform RectTransform;
    public GameObject Root;
    public SUiElement Parent;

    public string _id;
    
    public List<string> _classes = new();

    internal Dictionary<string, SUiElement> ChildIds = new();
    
    internal Dictionary<string, List<SUiElement>> ChildClasses = new();

    public SUiElement(GameObject root)
    {
        Root = root;
        RectTransform = GetOrAdd<RectTransform>();
    }

    public void SetParent(Transform parent)
    {
        Root.transform.SetParent(parent, false);
    }

    public void SetParent(SUiElement parent)
    {
        Root.transform.SetParent(parent.Root.transform, false);
    }
    
    public void RemoveFromParent()
    {
        Root.transform.SetParent(null, false);
    }

    public void Remove()
    {
        Object.Destroy(Root);
    }
    
    public T As<T>() where T : SUiElement
    {
        return this as T;
    }
    
    public List<SUiElement> GetClass(string className)
    {
        if(ChildClasses.TryGetValue(className, out var elements))
            return elements;
        
        return new List<SUiElement>();
    }

    protected T GetOrAdd<T>() where T : Component
    {
        var component = Root.GetComponent<T>();
        if (!component)
            component = Root.AddComponent<T>();
        return component;
    }
}

public class SUiElement<T> : SUiElement
{
    public TMP_Text TextObject;
    protected TooltipInfo TooltipInfo;
    
    protected Observable<bool> VisibilityObservable;
    protected bool InvertVisibility;

    public SUiElement(GameObject root) : base(root)
    { }
    
    protected virtual void VisibilityObservalbleChanged(bool value)
    {
        if (InvertVisibility)
            value = !value;
        
        if(value == Root.activeSelf)
            return;
        
        Root.SetActive(value);
    }
    
    /// <summary>
    /// Binds the visibility of the container to an observable boolean value.
    /// </summary>
    /// <param name="observable">The observable boolean value to bind to.</param>
    public T BindVisibility(Observable<bool> observable)
    {
        UnbindVisibility();

        InvertVisibility = false;
        VisibilityObservable = observable;
        observable.OnValueChanged += VisibilityObservalbleChanged;
        VisibilityObservalbleChanged(observable.Value);
        
        return (T)(object)this;
    }
    
    /// <summary>
    /// Binds the visibility of the container to an observable boolean value.
    /// </summary>
    /// <param name="observable">The observable boolean value to bind to.</param>
    public T BindVisibilityInverted(Observable<bool> observable)
    {
        UnbindVisibility();

        InvertVisibility = true;
        VisibilityObservable = observable;
        observable.OnValueChanged += VisibilityObservalbleChanged;
        VisibilityObservalbleChanged(observable.Value);
        
        return (T)(object)this;
    }
    
    /// <summary>
    /// Unbinds the visibility of the container from any previously bound observable.
    /// </summary>
    public T UnbindVisibility()
    {
        if (VisibilityObservable == null)
            return (T)(object)this;
        
        VisibilityObservable.OnValueChanged -= VisibilityObservalbleChanged;
        VisibilityObservable = null;

        return (T)(object)this;
    }
    
    public T Name(string name)
    {
        Root.name = name;
        return (T)(object)this;
    }
    
    public T Id(string id)
    {
        _id = id;
        return (T)(object)this;
    }
    
    public T Class(string classes)
    {
        _classes = classes.Split(' ').ToList();
        return (T)(object)this;
    }

    public virtual T Tooltip(string text)
    {
        if (!TextObject)
            return (T)(object)this;
        
        if (!TooltipInfo)
            TooltipInfo = TextObject.gameObject.AddComponent<TooltipInfo>();

        TooltipInfo.Text = text;
        
        return (T)(object)this;
    }
    
    /// <summary>
    /// Set the text of the main text object
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public virtual T Text(string text)
    {
        if (!TextObject)
            return (T)(object)this;
        
        TextObject.text = text;
        return (T)(object)this;
    }

    /// <summary>
    /// Set the text of the main text object as rich text
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public virtual T RichText(string text)
    {
        if (!TextObject)
            return (T)(object)this;

        TextObject.richText = true;
        TextObject.text = text;
        
        return (T)(object)this;
    }
    
    /// <summary>
    /// Set the font size of the main text object
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public virtual T FontSize(int size)
    {
        if (!TextObject)
            return (T)(object)this;
        
        TextObject.fontSize = size;
        return (T)(object)this;
    }
    
    public virtual T FontStyle(FontStyles style)
    {
        if (!TextObject)
            return (T)(object)this;
        
        TextObject.fontStyle = style;
        return (T)(object)this;
    }
    
    public T Font(TMP_FontAsset font)
    {
        if (!TextObject)
            return (T)(object)this;
        
        TextObject.font = font;
        return (T)(object)this;
    }

    public T Font(SUI.EFont font)
    {
        if (!TextObject)
            return (T)(object)this;
        
        TextObject.font = SUI.GetFont(font);
        return (T)(object)this;
    }
    
    public virtual T UpperCase()
    {
        if (!TextObject)
            return (T)(object)this;
        
        TextObject.fontStyle |= FontStyles.UpperCase;
        return (T)(object)this;
    }
    
    /// <summary>
    /// Toggle auto sizing of the main text object
    /// </summary>
    /// <param name="enabled"></param>
    /// <returns></returns>
    public T FontAutoSize(bool enabled)
    {
        if (!TextObject)
            return (T)(object)this;
        
        TextObject.enableAutoSizing = enabled;
        return (T)(object)this;
    }
    
    /// <summary>
    /// Set the auto size range of the text
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public T FontAutoSizeRange(int min, int max)
    {
        if (!TextObject)
            return (T)(object)this;
        
        TextObject.enableAutoSizing = true;
        TextObject.fontSizeMin = min;
        TextObject.fontSizeMax = max;
        return (T)(object)this;
    }
    
    /// <summary>
    /// Set the font color of the main text object
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public T FontColor(Color color)
    {
        if (!TextObject)
            return (T)(object)this;
        
        TextObject.color = color;
        return (T)(object)this;
    }
    
    public T FontColor(string color)
    {
        if (!TextObject)
            return (T)(object)this;
        
        TextObject.color = SUI.ColorFromString(color);
        return (T)(object)this;
    }
    
    /// <summary>
    /// Finds a child object and returns it as a SUiElement
    /// </summary>
    /// <param name="path"></param>
    /// <typeparam name="TObj"></typeparam>
    /// <returns></returns>
    public TObj Find<TObj> (string path) where TObj : SUiElement
    {
        return (TObj)Activator.CreateInstance(typeof(TObj), new GameObject[1] { Root.transform.Find(path).gameObject });
    }

    /// <summary>
    /// Set the minimum offset of the rect transform
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public T MinOffset(int? x = null, int? y = null)
    {
        var offset = RectTransform.offsetMin;
        RectTransform.offsetMin = new Vector2(x ?? offset.x, y ?? offset.y);
        return (T)(object)this;
    }
    
    /// <summary>
    /// Set the maximum offset of the rect transform
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public T MaxOffset(int? x = null, int? y = null)
    {
        var offset = RectTransform.offsetMax;
        RectTransform.offsetMax = new Vector2(x ?? offset.x, y ?? offset.y);
        return (T)(object)this;
    }
    
    /// <summary>
    /// Set the size delta of the rect transform
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public T Size(float? width = null, float? height = null)
    {
        var size = RectTransform.sizeDelta;
        RectTransform.sizeDelta = new Vector2(width ?? size.x, height ?? size.y);
        return (T)(object)this;
    }

    /// <summary>
    /// Set the size delta of the rect transform
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public T Size(Vector2 size)
    {
        RectTransform.sizeDelta = size;
        return (T)(object)this;
    }

    /// <summary>
    /// Set the anchor position of the rect transform
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public T Position(float? x, float? y = null)
    {
        var anchoredPosition = RectTransform.anchoredPosition;
        RectTransform.anchoredPosition  = new Vector2(x ?? anchoredPosition.x, y ?? anchoredPosition.y);
        return (T)(object)this;
    }

    /// <summary>
    /// Set the size delta width of the rect transform
    /// </summary>
    /// <param name="width"></param>
    /// <returns></returns>
    public T Width(float width)
    {
        RectTransform.sizeDelta = new Vector2(width, RectTransform.sizeDelta.y);
        return (T)(object)this;
    }

    /// <summary>
    /// Set the size delta height of the rect transform
    /// </summary>
    /// <param name="height"></param>
    /// <returns></returns>
    public T Height(float height)
    {
        RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, height);
        return (T)(object)this;
    }

    /// <summary>
    /// Set the preferred height of the layout element
    /// </summary>
    /// <param name="height"></param>
    /// <returns></returns>
    public T PHeight(float height)
    {
        var layoutElement = GetOrAdd<LayoutElement>();
        layoutElement.preferredHeight = height;
        return (T)(object)this;
    }
    
    /// <summary>
    /// Set the preferred width of the layout element
    /// </summary>
    /// <param name="width"></param>
    /// <returns></returns>
    public T PWidth(float width)
    {
        var layoutElement = GetOrAdd<LayoutElement>();
        layoutElement.preferredWidth = width;
        return (T)(object)this;
    }
    
    /// <summary>
    /// Set the minimum height of the layout element
    /// </summary>
    /// <param name="height"></param>
    /// <returns></returns>
    public T MHeight(float height)
    {
        var layoutElement = GetOrAdd<LayoutElement>();
        layoutElement.minHeight = height;
        return (T)(object)this;
    }
    
    /// <summary>
    /// Set the minimum width of the layout element
    /// </summary>
    /// <param name="width"></param>
    /// <returns></returns>
    public T MWidth(float width)
    {
        var layoutElement = GetOrAdd<LayoutElement>();
        layoutElement.minWidth = width;
        return (T)(object)this;
    }
    
    public T FlexHeight(float height)
    {
        var layoutElement = GetOrAdd<LayoutElement>();
        layoutElement.flexibleHeight = height;
        return (T)(object)this;
    }
    
    public T FlexWidth(float width)
    {
        var layoutElement = GetOrAdd<LayoutElement>();
        layoutElement.flexibleWidth = width;
        return (T)(object)this;
    }
    
    /// <summary>
    /// Set the offsets of the rect transform
    /// </summary>
    /// <param name="top"></param>
    /// <param name="right"></param>
    /// <param name="bottom"></param>
    /// <param name="left"></param>
    /// <returns></returns>
    public T Margin(float left, float right, float top, float bottom)
    {
        RectTransform.offsetMin = new Vector2(left, bottom);
        RectTransform.offsetMax = new Vector2(-right, -top);
        return (T)(object)this;
    }
    
    /// <summary>
    /// Set the offsets of the rect transform
    /// </summary>
    /// <param name="rightLeft"></param>
    /// <param name="topBottom"></param>
    /// <returns></returns>
    public T Margin(float rightLeft, float topBottom)
    {
        RectTransform.offsetMin = new Vector2(rightLeft, topBottom);
        RectTransform.offsetMax = new Vector2(-rightLeft, -topBottom);
        return (T)(object)this;
    }
    
    /// <summary>
    /// Set the offsets of the rect transform
    /// </summary>
    /// <param name="padding"></param>
    /// <returns></returns>
    public T Margin(float padding)
    {
        RectTransform.offsetMin = new Vector2(padding, padding);
        RectTransform.offsetMax = new Vector2(-padding, -padding);
        return (T)(object)this;
    }

    /// <summary>
    /// Sets the anchor and offsets to fill horizontally
    /// </summary>
    /// <returns></returns>
    public T HFill()
    {
        RectTransform.anchorMin = new Vector2(0, RectTransform.anchorMin.y);
        RectTransform.anchorMax = new Vector2(1, RectTransform.anchorMax.y);
        RectTransform.offsetMin = new Vector2(0, RectTransform.offsetMin.y);
        RectTransform.offsetMax = new Vector2(0, RectTransform.offsetMax.y);
        return (T)(object)this;
    }
    
    /// <summary>
    /// Sets the anchor and offsets to fill vertically
    /// </summary>
    /// <returns></returns>
    public T VFill()
    {
        RectTransform.anchorMin = new Vector2(RectTransform.anchorMin.x, 0);
        RectTransform.anchorMax = new Vector2(RectTransform.anchorMax.x, 1);
        RectTransform.offsetMin = new Vector2(RectTransform.offsetMin.x, 0);
        RectTransform.offsetMax = new Vector2(RectTransform.offsetMax.x, 0);
        return (T)(object)this;
    }
    
    /// <summary>
    /// Set the horizontal rect offset
    /// </summary>
    /// <param name="top"></param>
    /// <param name="bottom"></param>
    /// <returns></returns>
    public T VOffset(float top, float bottom)
    {
        RectTransform.offsetMin = new Vector2(RectTransform.offsetMin.x, bottom);
        RectTransform.offsetMax = new Vector2(RectTransform.offsetMax.x, -top);
        return (T)(object)this;
    }
    
    /// <summary>
    /// Set the vertical rect offset
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public T HOffset(float left, float right)
    {
        RectTransform.offsetMin = new Vector2(left, RectTransform.offsetMin.y);
        RectTransform.offsetMax = new Vector2(-right, RectTransform.offsetMax.y);
        return (T)(object)this;
    }
    
    /// <summary>
    /// Set the rect pivot
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public T Pivot(float? x = null, float? y = null)
    {
        var pivot = RectTransform.pivot;
        pivot = new Vector2(x ?? pivot.x, y ?? pivot.y);
        RectTransform.pivot = pivot;
        return (T)(object)this;
    }

    /// <summary>
    /// A shortcut to set the pivot to (0,1) and anchor to top left.
    /// </summary>
    /// <returns></returns>
    public T TopLeft()
    {
        RectTransform.anchorMin = new Vector2(0, 1);
        RectTransform.anchorMax = new Vector2(0, 1);
        RectTransform.pivot = new Vector2(0, 1);
        return (T)(object)this;
    }
    
    /// <summary>
    /// A shortcut to set the pivot to (0,1) and anchor to top left.
    /// Additionally sets the position (with the y value negated).
    /// This might be easier for users coming from web dev or frameworks like Imgui.
    /// </summary>
    /// <returns></returns>
    public T TopLeft(float x, float y)
    {
        TopLeft();
        Position(x, -y);
        return (T)(object)this;
    }
    
    /// <summary>
    /// Sets the aspect ratio mode for the objects's aspect ratio fitter.
    /// Adds an aspect ratio fitter if none is present.
    /// </summary>
    /// <param name="mode">The aspect ratio mode to apply.</param>
    public T AspectRatio(AspectRatioFitter.AspectMode mode)
    {
        var layout = GetOrAdd<AspectRatioFitter>();
        layout.aspectMode = mode;
        return (T)(object)this;
    }

    /// <summary>
    /// Set the rect anchor
    /// </summary>
    /// <param name="anchorType"></param>
    /// <returns></returns>
    public T Anchor(AnchorType anchorType)
    {
        if (RectTransform != null)
        {
            switch (anchorType)
            {
                case AnchorType.Fill:
                    RectTransform.anchorMin = Vector2.zero;
                    RectTransform.anchorMax = Vector2.one;
                    RectTransform.offsetMin = RectTransform.offsetMax = Vector2.zero;
                    break;

                case AnchorType.FillHorizontal:
                    RectTransform.anchorMin = new Vector2(0, RectTransform.anchorMin.y);
                    RectTransform.anchorMax = new Vector2(1, RectTransform.anchorMax.y);
                    RectTransform.offsetMin = new Vector2(0, RectTransform.offsetMin.y);
                    RectTransform.offsetMax = new Vector2(0, RectTransform.offsetMax.y);
                    break;

                case AnchorType.FillVertical:
                    RectTransform.anchorMin = new Vector2(RectTransform.anchorMin.x, 0);
                    RectTransform.anchorMax = new Vector2(RectTransform.anchorMax.x, 1);
                    RectTransform.offsetMin = new Vector2(RectTransform.offsetMin.x, 0);
                    RectTransform.offsetMax = new Vector2(RectTransform.offsetMax.x, 0);
                    break;

                case AnchorType.TopLeft:
                    RectTransform.anchorMin = RectTransform.anchorMax = new Vector2(0, 1);
                    break;

                case AnchorType.TopCenter:
                    RectTransform.anchorMin = RectTransform.anchorMax = new Vector2(0.5f, 1);
                    break;

                case AnchorType.TopRight:
                    RectTransform.anchorMin = RectTransform.anchorMax = new Vector2(1, 1);
                    break;

                case AnchorType.MiddleLeft:
                    RectTransform.anchorMin = RectTransform.anchorMax = new Vector2(0, 0.5f);
                    break;

                case AnchorType.MiddleCenter:
                    RectTransform.anchorMin = RectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    break;

                case AnchorType.MiddleRight:
                    RectTransform.anchorMin = RectTransform.anchorMax = new Vector2(1, 0.5f);
                    break;

                case AnchorType.BottomLeft:
                    RectTransform.anchorMin = RectTransform.anchorMax = new Vector2(0, 0);
                    break;

                case AnchorType.BottomCenter:
                    RectTransform.anchorMin = RectTransform.anchorMax = new Vector2(0.5f, 0);
                    break;

                case AnchorType.BottomRight:
                    RectTransform.anchorMin = RectTransform.anchorMax = new Vector2(1, 0);
                    break;
            }

            RectTransform.offsetMin = RectTransform.offsetMax = Vector2.zero;
        }

        return (T)(object)this;
    }
    
    /// <summary>
    /// An abstraction of the anchor and offset settings
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public T Dock(EDockType type)
    {
        switch (type)
        {
            case EDockType.Top:
                Anchor(AnchorType.TopCenter);
                HFill();
                Pivot(0.5f, 1);
                break;
            case EDockType.Bottom:
                Anchor(AnchorType.BottomCenter);
                HFill();
                Pivot(0.5f, 0);
                break;
            case EDockType.Left:
                Anchor(AnchorType.MiddleLeft);
                VFill();
                Pivot(0, 0.5f);
                break;
            case EDockType.Right:
                Anchor(AnchorType.MiddleRight);
                VFill();
                Pivot(1, 0.5f);
                break;
            case EDockType.Center:
                Anchor(AnchorType.MiddleCenter);
                Pivot(0.5f, 0.5f);
                break;
            case EDockType.Fill:
                Anchor(AnchorType.Fill);
                break;
        }
        return (T)(object)this;
    }

    /// <summary>
    /// Sets the pixels per unit multiplier for the object's image.
    /// </summary>
    /// <param name="ppu"></param>
    /// <returns></returns>
    public T Ppu(float ppu)
    {
        var image = Root.GetComponent<Image>();
        if (image == null)
        {
            return (T)(object)this;
        }

        image.pixelsPerUnitMultiplier = ppu;
        return (T)(object)this;
    }
    
    /// <summary>
    /// Set the visibility of the element (by setting the alpha and blocking raycasts)
    /// </summary>
    /// <param name="visible"></param>
    /// <returns></returns>
    public T Visible(bool visible)
    {
        var canvasGroup = GetOrAdd<CanvasGroup>();
        canvasGroup.alpha = visible ? 1 : 0;
        canvasGroup.blocksRaycasts = visible;
        return (T)(object)this;
    }
    
    /// <summary>
    /// Set the root gameobject to active or inactive
    /// </summary>
    /// <param name="active"></param>
    /// <returns></returns>
    public T Active(bool active)
    {
        Root.SetActive(active);
        return (T)(object)this;
    }
    
    /// <summary>
    /// Toggle the root gameobject
    /// </summary>
    /// <returns></returns>
    public T Toggle()
    {
        Root.SetActive(!Root.activeSelf);
        return (T)(object)this;
    }
    
    /// <summary>
    /// Fire an action when the container is clicked (FOR BUTTONS USE .Notify() INSTEAD)
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public T OnClick(Action action)
    {
        if (action == null)
            return (T)(object)this;
        
        var button = GetOrAdd<Button>();
        button.onClick.AddListener(action);
        return (T)(object)this;
    }

    public virtual T Add(SUiElement element)
    {
        element.SetParent(this);
        
        // add ids from child
        ChildIds.AddRange(element.ChildIds);
        if (!string.IsNullOrEmpty(element._id))
            ChildIds[element._id] = element;
        
        // add classes from child
        foreach (var (cls, elements) in element.ChildClasses)
        {
            if (!ChildClasses.ContainsKey(cls))
                ChildClasses[cls] = new List<SUiElement>();
            ChildClasses[cls].AddRange(elements);
        }
        
        foreach (var cls in element._classes)
        {
            if (!ChildClasses.ContainsKey(cls))
                ChildClasses[cls] = new List<SUiElement>();
            ChildClasses[cls].Add(element);
        }

        return (T)(object)this;
    }

    public SUiElement this[string id]
    {
        get
        {
            if (ChildIds.TryGetValue(id, out var item))
                return item;
            return null;
        }
    }
}

public class SUiElement<T, TData> : SUiElement<T>
{
    protected Observable<TData> Observable;
    private EObservableMode _observableMode;
    
    public SUiElement(GameObject root) : base(root)
    { }
    
    /// <summary>
    /// Bind an observable to the element value
    /// </summary>
    /// <param name="observable"></param>
    /// <returns></returns>
    public T Bind(Observable<TData> observable)
    {
        Unbind();
        
        Observable = observable;
        _observableMode = global::SUI.Observable.GetModeFromString("w");
        Observable.OnValueChanged += OnObservaleChanged;
        RegisterObservable(Observable);
        return (T)(object)this;
    }
    
    /// <summary>
    /// Unbind the current observable from the element value
    /// </summary>
    /// <returns></returns>
    public T Unbind()
    {
        if(Observable == null)
            return (T)(object)this;
        
        Observable.OnValueChanged -= OnObservaleChanged;
        UnregisterObservable(Observable);
        Observable = null;
        return (T)(object)this;
    }
    
    protected virtual void RegisterObservable(Observable<TData> observable)
    {
    }
    
    protected virtual void UnregisterObservable(Observable<TData> observable)
    {
    }
    
    protected virtual void OnObservaleChanged(TData value)
    { }
}
