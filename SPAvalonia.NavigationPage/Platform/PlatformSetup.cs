using System;
using Avalonia.Animation;
using SPAvalonia.NavigationPage.Platform.Android;
using SPAvalonia.NavigationPage.Platform.Ios;
using SPAvalonia.NavigationPage.Platform.Windows;

namespace SPAvalonia.NavigationPage.Platform;

public class PlatformSetup {
    public static IPageTransition TransitionForPage {
        get {
            if (OperatingSystem.IsAndroid())
                return AndroidDefaultPageSlide.Instance;
            if (OperatingSystem.IsIOS())
                return DefaultIosPageSlide.Instance;
            if (OperatingSystem.IsWindows())
                return EntranceNavigationTransition.Instance;

            //Default for the moment
            return DrillInNavigationTransition.Instance;
        }
    }

    public static IPageTransition TransitionForList {
        get {
            if (OperatingSystem.IsAndroid())
                return MaterialListPageSlide.Instance;
            if (OperatingSystem.IsIOS())
                return DefaultIosPageSlide.Instance;

            //Default for the moment
            return ListSlideNavigationTransition.Instance;
        }
    }

    public static IPageTransition? TransitionForTab {
        get {
            if (OperatingSystem.IsIOS()) return null;
            if (OperatingSystem.IsMacOS()) return null;
            if (OperatingSystem.IsMacCatalyst()) return null;

            //Default for the moment
            return MaterialListPageSlide.Instance;
        }
    }
}