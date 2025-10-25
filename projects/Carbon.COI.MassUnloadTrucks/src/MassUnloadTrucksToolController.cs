using Mafi;
using Mafi.Collections;
using Mafi.Collections.ImmutableCollections;
using Mafi.Collections.ReadonlyCollections;
using Mafi.Core;
using Mafi.Core.Entities;
using Mafi.Core.Factory.Transports;
using Mafi.Core.Prototypes;
using Mafi.Core.Terrain;
using Mafi.Core.Vehicles.Commands;
using Mafi.Core.Vehicles.Trucks;
using Mafi.Localization;
using Mafi.Unity.Entities;
using Mafi.Unity.InputControl;
using Mafi.Unity.InputControl.AreaTool;
using Mafi.Unity.InputControl.Factory;
using Mafi.Unity.Terrain;
using Mafi.Unity.Ui;
using Mafi.Unity.Ui.Controllers.Tools;
using Mafi.Unity.Ui.Hud;
using Mafi.Unity.Ui.Library;
using Mafi.Unity.UiStatic;
using Mafi.Unity.UiStatic.Cursors;

namespace Carbon.COI.MassUnloadTrucks;


[GlobalDependency(RegistrationMode.AsEverything, onlyInDebug: false, onlyInDevOnly: false)]
internal class MassUnloadTrucksToolController : BaseEntityCursorInputController<Truck>
{
    private static readonly Proto.ID s_requiredTech = IdsCore.Technology.UnityTool;
    private static readonly CursorStyle s_cursorStyle = CursorsStyles.Upoints;
    private static readonly string s_successSoundAssetPath = "Assets/Unity/UserInterface/Audio/AssignClick.prefab";


    private readonly CursorMessageUpoints upointsCostLabel;


    private bool shouldUpdateCostPopup;
    private int selectedEntitiesCount;


    public MassUnloadTrucksToolController(
        ToolbarHud                                toolbar,
        UiContext                                 context,
        CursorPickingManager                      cursorPickingManager,
        CursorManager                             cursorManager,
        AreaSelectionToolFactory                  areaSelectionToolFactory,
        NewInstanceOf<TerrainAreaOutlineRenderer> terrainOutlineRenderer,
        IEntitiesManager                          entitiesManager,
        NewInstanceOf<EntityHighlighter>          highlighter,
        NewInstanceOf<CursorMessageUpoints>       upointsCostLabel)
    : base(toolbar, context, cursorPickingManager, cursorManager, areaSelectionToolFactory, terrainOutlineRenderer, entitiesManager, highlighter, Option.None, s_requiredTech, s_cursorStyle, s_successSoundAssetPath, Option.None)
    {
        this.upointsCostLabel = upointsCostLabel.Instance;

        LocStr toolName = Loc.Str(id: "Mass Unload Trucks", enUs: "Mass Unload Trucks", comment: "MassUnloadTrucksTool");
        // toolbar.AddToolButton(name: toolName, controller: this, iconAssetPath: "Assets/unity/generated/icons/vehicle/TruckT1.png", order: 100f);
        toolbar.AddToolButton(name: toolName, controller: this, iconAssetPath: "Assets/Unity/UserInterface/General/Trash128.png", order: 100f);
    }


    public override void Activate()
    {
        base.Activate();
        this.selectedEntitiesCount = 0;
        this.shouldUpdateCostPopup = true;
        this.Context.GameLoopEvents.SyncUpdate.AddNonSaveable(this, this.OnSync);
    }


    public override void Deactivate()
    {
        base.Deactivate();
        this.selectedEntitiesCount = 0;
        this.Context.GameLoopEvents.SyncUpdate.RemoveNonSaveable(this, this.OnSync);
        this.Context.TerrainRenderer.DisableSurfaceHighlight();
        this.upointsCostLabel.Hide();
    }


    private void OnSync(GameTime gameTime)
    {
        if (this.shouldUpdateCostPopup is false)
            return;
        if (base.HasCommandsInProgress)
            return;
        this.shouldUpdateCostPopup = false;

        Upoints discardCargoCost = VehicleCommandsProcessor.COST_TO_DISCARD_CARGO * this.selectedEntitiesCount;
        if (discardCargoCost.IsPositive)
            this.upointsCostLabel.SetUpointsCost(discardCargoCost, Tr.Cargo__DiscardTooltip);
        else
            this.upointsCostLabel.Hide();
    }


    // Return true if the entity is a valid selection target.
    protected override bool Matches(Truck entity, bool isAreaSelection, bool isLeftClick)
    {
        return entity.Cargo.IsNotEmpty;
    }


    // Not sure what this is used for. It's called by InputUpdate() on the first frame the tool is active.
    protected override bool OnFirstActivated(Truck hoveredEntity, Lyst<Truck> selectedEntities, Lyst<SubTransport> selectedPartialTransports)
    {
        return false;
    }


    // Called when the set of matched entities change. Also called after OnEntitiesSelected().
    protected override void OnHoverChanged(IIndexable<Truck> hoveredEntities, IIndexable<SubTransport> hoveredPartialTransports, IIndexable<TileSurfaceCopyPasteData> selectedSurfaces, RectangleTerrainArea2i? area, bool isLeftClick)
    {
        this.selectedEntitiesCount = hoveredEntities.Count;
        this.shouldUpdateCostPopup = true;
    }


    // Called when the user has confirmed the selection of the entities to process.
    protected override void OnEntitiesSelected(IIndexable<Truck> selectedEntities, IIndexable<SubTransport> selectedPartialTransports, ImmutableArray<TileSurfaceCopyPasteData> selectedSurfaces, ImmutableArray<TileSurfaceCopyPasteData> selectedDecals, bool isAreaSelection, bool isLeftMouse, RectangleTerrainArea2i? area)
    {
        foreach (Truck truck in selectedEntities)
        {
            DiscardVehicleCargoCmd discardVehicleCargoCmd = new DiscardVehicleCargoCmd(truck.Id);
            this.Context.InputScheduler.ScheduleInputCmd(discardVehicleCargoCmd);
            this.RegisterPendingCommand(discardVehicleCargoCmd);
        }
        this.Context.TerrainRenderer.DisableSurfaceHighlight();
    }
}
