extends VBoxContainer

@onready var completed_upgrade_template = preload(
    "res://scenes/UI/Pages/Upgrades/completed_upgrade.tscn"
    )
@onready var game_controller = $/root/GameController    
@onready var completed_upgrades_container = %CompletedUpgradesListContainer

var current_upgrades_list = []

func _ready():
    for n in completed_upgrades_container.get_children():
        completed_upgrades_container.remove_child(n)
        n.queue_free()

func _process(_delta):
    var upgrades_list = Array(Query.UpgradesUnlocked(game_controller.currentSettlementUuid))
    
    if upgrades_list == current_upgrades_list:
        return
    current_upgrades_list = upgrades_list
    
    for n in completed_upgrades_container.get_children():
        completed_upgrades_container.remove_child(n)
        n.queue_free()
    for upgrade_id in upgrades_list:
        var new_upgrade = completed_upgrade_template.instantiate()
        completed_upgrades_container.add_child(new_upgrade)
        new_upgrade.upgrade_id = upgrade_id
    
