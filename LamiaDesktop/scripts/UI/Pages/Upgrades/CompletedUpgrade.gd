extends PanelContainer

@onready var game_controller = $/root/GameController    
@onready var name_text = %Name
@onready var info_button = %Info

@export var upgrade_id = "thing"

func _process(_delta):
    if not upgrade_id:
        return
    name_text.text = Query.UpgradeDisplayName(game_controller.currentSettlementUuid, upgrade_id)
    find_child("Info").get_node("TooltipShower").tooltip_label_text = Query.UpgradeDescription(game_controller.currentSettlementUuid, upgrade_id)
