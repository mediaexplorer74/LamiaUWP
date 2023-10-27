extends VBoxContainer

@onready var resource_category_template = preload("res://scenes/UI/Global/resource_category.tscn")
@onready var game_controller = $/root/GameController

var current_resource_category_list = []

func _ready():
    for n in get_children():
        remove_child(n)
        n.queue_free()    

func _process(delta):
    var resource_category_list = Array(Query.SettlementInventoryCategories(game_controller.currentSettlementUuid))
    
    if resource_category_list == current_resource_category_list:
        return
    current_resource_category_list = resource_category_list
    
    for n in get_children():
        remove_child(n)
        n.queue_free()
    for resource_category_name in resource_category_list:
        var new_category = resource_category_template.instantiate() as ResourceCategory
        add_child(new_category)
        new_category.resource_category_name = resource_category_name
