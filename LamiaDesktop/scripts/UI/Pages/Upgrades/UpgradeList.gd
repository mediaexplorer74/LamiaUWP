extends VBoxContainer

@onready var upgrade_template = preload(
    "res://scenes/UI/Pages/Upgrades/upgrade.tscn"
    )
@onready var game_controller = $/root/GameController        
@onready var available_upgrades_container = %AvailableUpgradesListContainer

var current_upgrades_list = []

func _ready():
    for n in available_upgrades_container.get_children():
        available_upgrades_container.remove_child(n)
        n.queue_free()

func _process(_delta):
    var upgrades_list = Array(Query.UpgradesAvailable())
    
    if upgrades_list == current_upgrades_list:
        return
    current_upgrades_list = upgrades_list
    
    for n in available_upgrades_container.get_children():
        available_upgrades_container.remove_child(n)
        n.queue_free()
    for upgrade_id in upgrades_list:
        var new_upgrade = upgrade_template.instantiate()
        available_upgrades_container.add_child(new_upgrade)
        new_upgrade.upgrade_id = upgrade_id
    
