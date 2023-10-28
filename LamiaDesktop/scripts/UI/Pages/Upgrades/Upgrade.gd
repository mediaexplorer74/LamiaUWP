extends PanelContainer

@onready var upgrade_resource_cost_template = preload(
    "res://scenes/UI/Pages/Upgrades/upgrade_resource_cost.tscn"
    )
@onready var game_controller = $/root/GameController    
@onready var name_text = %Name
@onready var info_button = %Info
@onready var resource_cost_container = %ResourceCostContainer
@onready var open_cost_button = %OpenCostButton
@onready var resource_cost_list_container = %ResourceCostListContainer
@onready var unlock_button = %UnlockButton

@export var upgrade_id = "thing"

var resource_cost_open = false
var resource_cost_dirty = true
var current_resource_cost_list = []


func _process(_delta):
    if not upgrade_id:
        return
    name_text.text = Query.UpgradeDisplayName(game_controller.currentSettlementUuid, upgrade_id)
    find_child("Info").get_node("TooltipShower").tooltip_label_text = Query.UpgradeDescription(game_controller.currentSettlementUuid, upgrade_id)

    unlock_button.set_disabled(not Query.UpgradeCanAfford(game_controller.currentSettlementUuid, upgrade_id))

    if resource_cost_open:
        resource_cost_container.show()
    else:
        resource_cost_container.hide()

    var resource_cost_list = Array(Query.UpgradeResourceList(game_controller.currentSettlementUuid, upgrade_id))
    
    if resource_cost_list != current_resource_cost_list:
        current_resource_cost_list = resource_cost_list
        resource_cost_dirty = true
        
    if resource_cost_dirty:
        for n in resource_cost_list_container.get_children():
            resource_cost_list_container.remove_child(n)
            n.queue_free()
        for resource_id in current_resource_cost_list:
            var new_resource = upgrade_resource_cost_template.instantiate()
            resource_cost_list_container.add_child(new_resource)
            new_resource.get_node("ResourceNameLabel").text = Query.ResourceName(resource_id)
            var amount = Query.UpgradeSingleResourceCost(game_controller.currentSettlementUuid, upgrade_id, resource_id)
            var format = "%.2f" if fmod(amount, 1.0) > 0 else "%d"
            new_resource.get_node("AmountLabel").text = format % amount
        resource_cost_dirty = false

func _on_open_cost_button_pressed():
    resource_cost_open = not resource_cost_open
    if resource_cost_open:
        open_cost_button.text = "V"
    else:
        open_cost_button.text = ">"


func _on_unlock_button_pressed():
    Action.UnlockUpgrade(game_controller.currentSettlementUuid, upgrade_id)
    resource_cost_dirty = true
