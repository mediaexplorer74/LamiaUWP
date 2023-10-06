extends VBoxContainer
class_name ResourceUI

@onready var resource_name_label = %ResourceNameLabel
@onready var resource_amount_label = %ResourceAmountLabel
@onready var game_controller = $/root/GameController

@export var resource_name = "wooood": 
    set(val):
        resource_name = val
        find_child("TooltipShower").tooltip_label_text = Query.ResourceDescription(resource_name)

func _process(delta):
    resource_name_label.text = Query.ResourceName(resource_name)
    resource_amount_label.text = str(Query.SettlementInventoryResourceAmount(game_controller.currentSettlementUuid, resource_name))
