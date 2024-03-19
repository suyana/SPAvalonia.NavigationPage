using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using ReactiveUI;
using SPAvalonia.NavigationPage.Platform.Windows;
using Splat;

namespace SPAvalonia.NavigationPage;

public partial class NavigationPage : TemplatedControl {
    #region Enums

    public enum ScreenSizeType {
        Small,
        Medium,
        Large
    }
    #endregion

    #region Variables

    private StackContentView? _contentView;
    private NavigationBar? _navigationBar;
    private StackContentView? _modalView;
    private Rectangle? _modalBackground;

    private bool _loadedFlag = false;
    private bool _topLevelEventFlag = false;

    #endregion

    #region Properties

    public NavigationBar NavigationBar => _navigationBar;

    #region ScreenSize

    private ScreenSizeType _screenSize = ScreenSizeType.Large;

    public static readonly DirectProperty<NavigationPage, ScreenSizeType> ScreenSizeProperty =
        AvaloniaProperty.RegisterDirect<NavigationPage, ScreenSizeType>(
            nameof(ScreenSize),
            o => o.ScreenSize,
            (o, v) => o.ScreenSize = v);

    public ScreenSizeType ScreenSize {
        get => _screenSize;
        set {
            var oldValue = _screenSize;
            if (SetAndRaise(ScreenSizeProperty, ref _screenSize, value))
                UpdateScreenSize(oldValue, value);
        }
    }

    #endregion


    #region ClickAway

    public static DirectProperty<NavigationPage, BoxShadows> BoxShadowProperty =
        AvaloniaProperty.RegisterDirect<NavigationPage, BoxShadows>(
            nameof(BoxShadow), o => o.BoxShadow, (o, v) => o.BoxShadow = v);

    public BoxShadows BoxShadow { get; set; }

    #endregion

    #region ClickAway

    public static DirectProperty<NavigationPage, bool> CloseOnClickAwayProperty =
        AvaloniaProperty.RegisterDirect<NavigationPage, bool>(
            nameof(CloseOnClickAway), o => o.CloseOnClickAway, (o, v) => o.CloseOnClickAway = v);

    public bool CloseOnClickAway { get; set; } = false;

    #endregion

    #region IsEscBack(PC平台时，Esc是否能执行Back)

    public static DirectProperty<NavigationPage, bool> IsEscBackProperty =
        AvaloniaProperty.RegisterDirect<NavigationPage, bool>(
            nameof(IsEscBack), o => o.IsEscBack, (o, v) => o.IsEscBack = v);

    public bool IsEscBack { get; set; } = false;

    #endregion

    #region DefaultRoute

    public static DirectProperty<NavigationPage, string?> DefaultRouteProperty =
        AvaloniaProperty.RegisterDirect<NavigationPage, string?>(
            nameof(DefaultRoute), o => o.DefaultRoute, (o, v) => o.DefaultRoute = v);

    public string? DefaultRoute { get; set; }

    #endregion

    #region TopSafeSpace

    public static readonly StyledProperty<double> TopSafeSpaceProperty =
        AvaloniaProperty.Register<NavigationPage, double>(nameof(TopSafeSpace));

    public double TopSafeSpace {
        get => GetValue(TopSafeSpaceProperty);
        set => SetValue(TopSafeSpaceProperty, value);
    }

    #endregion

    #region BottomSafeSpace

    public static readonly StyledProperty<double> BottomSafeSpaceProperty =
        AvaloniaProperty.Register<NavigationPage, double>(nameof(BottomSafeSpace));

    public double BottomSafeSpace {
        get => GetValue(BottomSafeSpaceProperty);
        set => SetValue(BottomSafeSpaceProperty, value);
    }

    #endregion

    #region TopSafePadding

    public static readonly StyledProperty<Thickness> TopSafePaddingProperty =
        AvaloniaProperty.Register<NavigationPage, Thickness>(nameof(TopSafePadding));

    public Thickness TopSafePadding {
        get => GetValue(TopSafePaddingProperty);
        set => SetValue(TopSafePaddingProperty, value);
    }

    #endregion

    #region BottomSafePadding

    public static readonly StyledProperty<Thickness> BottomSafePaddingProperty =
        AvaloniaProperty.Register<NavigationPage, Thickness>(nameof(BottomSafePadding));

    public Thickness BottomSafePadding {
        get => GetValue(BottomSafePaddingProperty);
        set => SetValue(BottomSafePaddingProperty, value);
    }

    #endregion

    #region SafePadding

    public static readonly StyledProperty<Thickness> SafePaddingProperty =
        AvaloniaProperty.Register<NavigationPage, Thickness>(nameof(SafePadding));

    public Thickness SafePadding {
        get => GetValue(SafePaddingProperty);
        set => SetValue(SafePaddingProperty, value);
    }

    #endregion

    #region ApplyTopSafePadding

    public static readonly StyledProperty<bool> ApplyTopSafePaddingProperty =
        AvaloniaProperty.Register<NavigationPage, bool>(nameof(ApplyTopSafePadding), defaultValue: true);

    public bool ApplyTopSafePadding {
        get => GetValue(ApplyTopSafePaddingProperty);
        set => SetValue(ApplyTopSafePaddingProperty, value);
    }

    #endregion

    #region ApplyBottomSafePadding

    public static readonly StyledProperty<bool> ApplyBottomSafePaddingProperty =
        AvaloniaProperty.Register<NavigationPage, bool>(nameof(ApplyBottomSafePadding), defaultValue: true);

    public bool ApplyBottomSafePadding {
        get => GetValue(ApplyBottomSafePaddingProperty);
        set => SetValue(ApplyBottomSafePaddingProperty, value);
    }

    #endregion

    #region ApplyBottomSafePadding

    /// <summary>
    /// Defines the <see cref="DefaultPageTransitionProperty"/> property.
    /// </summary>
    public static readonly StyledProperty<IPageTransition?> DefaultPageTransitionProperty =
        AvaloniaProperty.Register<NavigationPage, IPageTransition?>(
            nameof(DefaultPageTransition),
            defaultValue: new DrillInNavigationTransition());

    /// <summary>
    /// Gets or sets the animation played when content appears and disappears.
    /// </summary>
    public IPageTransition? DefaultPageTransition {
        get => GetValue(DefaultPageTransitionProperty);
        set => SetValue(DefaultPageTransitionProperty, value);
    }

    #endregion

    #endregion

    #region Attached properties

    #region EnableSafeAreaForTop

    public static readonly AttachedProperty<bool> EnableSafeAreaForTopProperty =
        AvaloniaProperty.RegisterAttached<NavigationBar, AvaloniaObject, bool>("EnableSafeAreaForTop",
            defaultValue: true);

    public static bool GetEnableSafeAreaForTop(AvaloniaObject element) =>
        element.GetValue(EnableSafeAreaForTopProperty);

    public static void SetEnableSafeAreaForTop(AvaloniaObject element, bool parameter) =>
        element.SetValue(EnableSafeAreaForTopProperty, parameter);

    #endregion

    #region EnableSafeAreaForBottom

    public static readonly AttachedProperty<bool> EnableSafeAreaForBottomProperty =
        AvaloniaProperty.RegisterAttached<NavigationBar, AvaloniaObject, bool>("EnableSafeAreaForBottom",
            defaultValue: true);

    public static bool GetEnableSafeAreaForBottom(AvaloniaObject element) =>
        element.GetValue(EnableSafeAreaForBottomProperty);

    public static void SetEnableSafeAreaForBottom(AvaloniaObject element, bool parameter) =>
        element.SetValue(EnableSafeAreaForBottomProperty, parameter);

    #endregion

    #endregion

    #region Ctor and loading

    public NavigationPage() {
        Navigator = Locator.Current.GetService<INavigator>() ?? throw new ArgumentException("Cannot find INavigationService");
        Navigator.RegisterShell(this);

        var isMobile = OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();
        if(isMobile){
            Classes.Add("Mobile");
        }

        SizeChanged += OnSizeChanged;
    }

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);

        if (TopLevel.GetTopLevel(this) is { } topLevel && !_topLevelEventFlag) {
            topLevel.BackRequested += TopLevelOnBackRequested;
            topLevel.KeyUp += TopLevelOnKeyUp;
            _topLevelEventFlag = true;
        }

        if (!_loadedFlag) {
            _loadedFlag = true;
            if (string.IsNullOrEmpty(DefaultRoute)) {
                var route = Navigator.Registrar.GetFirstNodeRoute();
                _ = Navigator.NavigateAsync(route, CancellationToken.None);
            }
            else {
                _ = Navigator.NavigateAsync(DefaultRoute, CancellationToken.None);
            }
        }

        if (TopLevel.GetTopLevel(this) is { InsetsManager: { } insetsManager }) {
            insetsManager.DisplayEdgeToEdge = true;
        }

        OnSafeEdgeSetup();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _contentView = e.NameScope.Find<StackContentView>("PART_ContentView");
        _modalView = e.NameScope.Find<StackContentView>("PART_Modal");
        _navigationBar = e.NameScope.Find<NavigationBar>("PART_NavigationBar");
        _modalBackground = e.NameScope.Find<Rectangle>("PART_ModalBackground");

        SetupUi();

        if (CloseOnClickAway && _isModalBgTapEvent==false && _modalBackground != null ) {
            _isModalBgTapEvent = true;
            _modalBackground.Tapped += _modalBackground_Tapped;
        }
    }

    private bool _isModalBgTapEvent = false;
    private void _modalBackground_Tapped(object? sender, TappedEventArgs e) {
        Navigator.BackAsync();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);
        switch (change.Property.Name) {
            case nameof(SafePadding):
                OnSafeEdgeSetup();
                break;
        }
    }

    private void SetupUi() {
        if (_navigationBar != null) {
            _navigationBar.ShellView = this;
            _navigationBar.BackCommand = ReactiveCommand.CreateFromTask(BackActionAsync);
        }

        OnSafeEdgeSetup();
    }

    protected virtual void OnSafeEdgeSetup() {
        Dispatcher.UIThread.InvokeAsync(async () => {
            await Task.Delay(100);

            if (TopLevel.GetTopLevel(this) is { InsetsManager: { DisplayEdgeToEdge: true } insetsManager })
                SafePadding = insetsManager.SafeAreaPadding;

            TopSafeSpace = SafePadding.Top;
            TopSafePadding = new Thickness(0, SafePadding.Top, 0, 0);
            BottomSafeSpace = SafePadding.Bottom;
            BottomSafePadding = new Thickness(0, 0, 0, SafePadding.Bottom);
        });
    }

    #endregion

    #region Services and navigation

    public INavigator Navigator { get; }

    #endregion

    #region View Stack Manager

    public async Task PushViewAsync(object view,
        NavigateType navigateType,
        CancellationToken cancellationToken = default) {
        await (_contentView?.PushViewAsync(view, navigateType, cancellationToken) ?? Task.CompletedTask);
        UpdateBindings();
    }

    public async Task RemoveViewAsync(object view,
        NavigateType navigateType,
        CancellationToken cancellationToken = default) {
        await (_contentView?.RemoveViewAsync(view, navigateType, cancellationToken) ?? Task.CompletedTask);
        await (_modalView?.RemoveViewAsync(view, navigateType, cancellationToken) ?? Task.CompletedTask);
    }

    public async Task ClearStackAsync(CancellationToken cancellationToken) {
        await (_contentView?.ClearStackAsync(cancellationToken) ?? Task.CompletedTask);
        await (_modalView?.ClearStackAsync(cancellationToken) ?? Task.CompletedTask);
    }

    public Task ModalAsync(object instance, NavigateType navigateType, CancellationToken cancellationToken) =>
        _modalView?.PushViewAsync(instance, navigateType, cancellationToken) ?? Task.CompletedTask;

    private bool Back() {
        var result = Navigator.HasItemInStack();
        if (result) Navigator.BackAsync();

        return result;
    }

    #endregion

    #region Ui Events

    private void OnSizeChanged(object? sender, SizeChangedEventArgs e) {
        ScreenSize = e.NewSize.Width switch {
            <= 768 => ScreenSizeType.Small,
            <= 1024 => ScreenSizeType.Medium,
            _ => ScreenSizeType.Large,
        };
    }

    private void TopLevelOnBackRequested(object? sender, RoutedEventArgs e) {
        e.Handled = Back();
    }

    private void TopLevelOnKeyUp(object? sender, KeyEventArgs e) {
        if (e.Key == Key.Escape && IsEscBack) Back();
    }

    protected virtual Task BackActionAsync(CancellationToken cancellationToken) {
        return Navigator.BackAsync(cancellationToken);
    }

    #endregion

    #region Screen Actions

    private void UpdateScreenSize(ScreenSizeType old, ScreenSizeType newScreen) {
        Classes.Add(newScreen.ToString());
        Classes.Remove(old.ToString());
    }

    private void UpdateBindings() {
        var view = _contentView.CurrentView;
        if (view is StyledElement element) {
            this[!ApplyTopSafePaddingProperty] = element[!EnableSafeAreaForTopProperty];
            this[!ApplyBottomSafePaddingProperty] = element[!EnableSafeAreaForBottomProperty];
        }
        else {
            ApplyTopSafePadding = true;
            ApplyBottomSafePadding = true;
        }
    }

    #endregion
}