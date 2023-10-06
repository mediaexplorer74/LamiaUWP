extends VBoxContainer

@onready var resource_template = preload("res://scenes/UI/Global/resource.tscn")
@onready var game_controller = $/root/GameController

var current_resource_list = []

func _ready():
    for n in get_children():
        remove_child(n)
        n.queue_free()    

func _process(delta):
    var resource_list = Array(Query.SettlementInventory(game_controller.currentSettlementUuid))
    
    if resource_list == current_resource_list:
        return
    current_resource_list = resource_list
    
    for n in get_children():
        remove_child(n)
        n.queue_free()
    for resource_name in resource_list:
        var new_resource = resource_template.instantiate() as ResourceUI
        add_child(new_resource)
        new_resource.resource_name = resource_name
