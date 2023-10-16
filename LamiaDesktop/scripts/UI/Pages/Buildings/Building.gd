extends PanelContainer
class_name Building

@onready var building_resource_cost_template = preload(
    "res://scenes/UI/Pages/Buildings/building_resource_cost.tscn"
    )
@onready var name_text = %Name
@onready var amount_text = %Amount
@onready var info_button = %Info
@onready var game_controller = $/root/GameController
@onready var resource_cost_container = %ResourceCostContainer
@onready var open_cost_button = %OpenCostButton
@onready var resource_cost_list_container = %ResourceCostListContainer
@onready var build_button = %BuildButton

@export var building_id = "thing"

var resource_cost_open = false
var resource_cost_dirty = true
var current_resource_cost_list = []


func _process(_delta):
    if not building_id:
        return
    name_text.text = Query.SettlementBuildingDisplayName(game_controller.currentSettlementUuid, building_id)
    amount_text.text = str(
        Query.SettlementBuildingsAmount(
            game_controller.currentSettlementUuid, building_id
        )
    )
    find_child("Info").get_node("TooltipShower").tooltip_label_text = "\n".join(Query.SettlementBuildingDescription(game_controller.currentSettlementUuid, building_id))

    build_button.set_disabled(not Query.SettlementBuildingCanAfford(game_controller.currentSettlementUuid, building_id))

    if resource_cost_open:
        resource_cost_container.show()
    else:
        resource_cost_container.hide()

    var resource_cost_list = Array(Query.SettlementBuildingResourceList(game_controller.currentSettlementUuid, building_id))
    
    if resource_cost_list != current_resource_cost_list:
        current_resource_cost_list = resource_cost_list
        resource_cost_dirty = true
        
    if resource_cost_dirty:
        for n in resource_cost_list_container.get_children():
            resource_cost_list_container.remove_child(n)
            n.queue_free()
        for resource_id in current_resource_cost_list:
            var new_resource = building_resource_cost_template.instantiate()
            resource_cost_list_container.add_child(new_resource)
            new_resource.get_node("ResourceNameLabel").text = Query.ResourceName(resource_id)
            new_resource.get_node("AmountLabel").text = str(Query.SettlementBuildingSingleResourceCost(game_controller.currentSettlementUuid, building_id, resource_id))
        resource_cost_dirty = false

func _on_open_cost_button_pressed():
    resource_cost_open = not resource_cost_open
    if resource_cost_open:
        open_cost_button.text = "V"
    else:
        open_cost_button.text = ">"


func _on_build_button_pressed():
    Action.SettlementPurchaseBuilding(game_controller.currentSettlementUuid, building_id)
    resource_cost_dirty = true
