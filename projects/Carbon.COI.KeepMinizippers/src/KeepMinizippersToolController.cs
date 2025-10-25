using System;

using Mafi;
using Mafi.Localization;
using Mafi.Unity.InputControl;
using Mafi.Unity.Ui.Hud;
using Mafi.Unity.UiStatic.Toolbar;
using Mafi.Unity.UiToolkit.Component;
using Mafi.Unity.UiToolkit.Library;

namespace Carbon.COI.KeepMinizippers;

[GlobalDependency(RegistrationMode.AsEverything)]
internal class KeepMinizippersToolController : IToolbarItemController
{
    /// <summary>
    /// Whether the controller can be activated and should be displayed in the menu.
    /// </summary>
    public bool IsVisible => true;

    /// <summary>
    /// Describes how the toolbar item interacts with the input system.
    /// </summary>
    public ControllerConfig Config => ControllerConfig.Mode;

    /// <summary>
    /// Invoked when <see cref="IToolbarItemController.IsVisible"/> changes. The event can be invoked from any thread.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/en-us/archive/blogs/trevor/c-warning-cs0067-the-event-event-is-never-used"/>
    public event Action<IToolbarItemController>? VisibilityChanged { add {} remove {} }


    [Mafi.Serialization.OnlyForSaveCompatibility]
    public bool DeactivateShortcutsIfNotVisible => false;


    private readonly KeepMinizippersModConfig modConfig;


    public KeepMinizippersToolController(KeepMinizippersModConfig modConfig, ToolbarHud toolbar)
    {
        this.modConfig = modConfig;

        LocStr toolName = Loc.Str(id: "Keep Minizippers", enUs: "Keep Minizippers", comment: "KeepMinizippersTool");
        Button toolButton = toolbar.AddToolButton(name: toolName, controller: this, iconAssetPath: "Assets/unity/generated/icons/layoutentity/MiniZip_IoPortShape_Pipe.png", order: 100f, shortcut: GetShortcut);
        toolButton.Selected(this.modConfig.IsEnabled);
    }


    private static KeyBindings GetShortcut(ShortcutsManager shortcutsManager)
    {
        return KeyBindings.FromPrimaryKeys(KbCategory.Tools, ShortcutMode.Game, UnityEngine.KeyCode.LeftAlt, UnityEngine.KeyCode.Z);
    }


    /// <summary>
    /// Called when this input controller is activated by the player. Invoked on the main thread.
    /// </summary>
    public void Activate() => this.modConfig.IsEnabled = true;


    /// <summary>
    /// Called when this input controller is deactivated by the player. Invoked on the main thread.
    /// </summary>
    public void Deactivate() => this.modConfig.IsEnabled = false;


    /// <summary>
    /// Called every frame when the controller is active. Invoked on the main thread.
    /// </summary>
    /// <returns>Whether input was processed and no other controllers should be updated.</returns>
    public bool InputUpdate() => false;
}
