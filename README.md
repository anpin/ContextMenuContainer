# ContextMenuContainer
[![NuGet](https://img.shields.io/nuget/v/ContextMenuContainer.svg?style=flat)](https://www.nuget.org/packages/ContextMenuContainer/)
[![Package nuget](https://github.com/anpin/ContextMenuContainer/actions/workflows/main.yml/badge.svg?branch=main)](https://github.com/anpin/ContextMenuContainer/actions/workflows/main.yml)
[![Paypal](https://img.shields.io/badge/Donate%20with-PayPal-blue)](https://paypal.me/APEngineeringLLC?locale.x=en_US)

# Native Context Menu for .NET MAUI

Add native context menus to any view in your .NET MAUI application. Supports all major platforms:
- Windows
- Android
- iOS
- macOS

## Platform Preview

iOS | Android | macOS | Windows
:-------------------------:|:-------------------------:|:-------------------------:|:-------------------------:
![iOS](img/ios.gif) | ![Android](img/android.gif) | ![Mac](img/macos.gif) | ![UWP](img/uwp.png)

## Installation

```
dotnet add package ContextMenuContainer
```

Or via NuGet Package Manager.

## Setup

1. Initialize the component in your `MauiProgram.cs` by adding the extension method:
```csharp
using APES.MAUI;

var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    // Add this line to configure the context menu container
    .ConfigureContextMenuContainer();
```

2. Add the namespace to your XAML file:
```xml
xmlns:apes="http://apes.ge"
```

## Basic Usage

Wrap any view with the `ContextMenuContainer` and define your context menu items:

### Inline menu items:

```xml
<apes:ContextMenuContainer>
    <apes:ContextMenuContainer.MenuItems>
        <apes:ContextMenuItem 
            Text="Copy" 
            Command="{Binding CopyCommand}" 
            CommandParameter="{Binding .}" />
        <apes:ContextMenuItem 
            Text="Delete" 
            Command="{Binding DeleteCommand}" 
            CommandParameter="{Binding .}" 
            IsDestructive="True" 
            Icon="{Binding DeleteIconSource}"/>
    </apes:ContextMenuContainer.MenuItems>
    <apes:ContextMenuContainer.Content>
        <Label Text="Long press or right-click me!"/>
    </apes:ContextMenuContainer.Content>
</apes:ContextMenuContainer>
```

### Binding menu items from ViewModel:

```xml
<apes:ContextMenuContainer MenuItems="{Binding ContextMenuItems}">
    <apes:ContextMenuContainer.Content>
        <Frame>
            <Image Source="{Binding ImageSource}"/>
        </Frame>
    </apes:ContextMenuContainer.Content>
</apes:ContextMenuContainer>
```

## ContextMenuItem Properties

| Property | Type | Description |
|----------|------|-------------|
| `Text` | string | The text label for the menu item |
| `Command` | ICommand | Command to execute when the item is tapped |
| `CommandParameter` | object | Parameter to pass to the command |
| `IsEnabled` | bool | Whether the item is enabled (default: true) |
| `IsDestructive` | bool | Marks the item as destructive (red text) |
| `Icon` | FileImageSource | Icon for the menu item |

## Icons

To use icons in your context menu items:

1. Add platform-specific image assets to the appropriate folders
2. Bind to a `FileImageSource` from your ViewModel

SVG format is recommended for best cross-platform support.

## Known Issues

- With Xamarin.Forms when used in a `CollectionView` or `ListView` item template, there may be conflicts with the collection's own tap/selection events. Consider using `TapGestureRecognizer` directly in these scenarios.

## Roadmap

- [x] Configure build scripts
- [x] MAUI migration and support
- [ ] Add visibility property
- [ ] Add highlight property
- [ ] Add support for keyboard shortcuts
- [ ] Improve accessibility features
- [ ] Add support for submenus and separators
- [ ] Add font icon support
- [x] Add comprehensive unit and UI tests

## Development & Contribution

### Versioning and Releases

This project uses [GitVersion](https://gitversion.net/) for automatic versioning based on Git history and tags. 

#### How to bump version for a new release:

1. **Tag-based versioning**: Create a Git tag with the format `v{major}.{minor}.{patch}` to specify the exact version
   ```bash
   git tag v1.2.0
   git push origin v1.2.0
   ```

2. **Commit message-based versioning**: Include specific keywords in your commit message to trigger version increments
   - Major version bump: `+semver:breaking` or `+semver:major`
   - Minor version bump: `+semver:feature` or `+semver:minor`
   - Patch version bump: `+semver:fix` or `+semver:patch`

3. **Branch-based versioning**: The versioning behavior depends on the branch:
   - `main` branch: Patch increments, creates release versions
   - `develop` branch: Minor increments with `alpha` suffix
   - `release/*` branch: No increment with `beta` suffix
   - `feature/*` branch: Inherits version with branch name suffix
   - `hotfix/*` branch: Patch increment with `beta` suffix

### Contribution Process

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes with appropriate version bump keywords if needed
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## Support Development

If this plugin saves you development time, please consider supporting its continued improvement:

[![Paypal](https://img.shields.io/badge/Donate%20with-PayPal-blue)](https://www.paypal.com/paypalme/PavelAnpin)
