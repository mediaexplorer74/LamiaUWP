extends VBoxContainer
class_name ResourceCategory

@onready var resource_template = preload("res://scenes/UI/Global/resource.tscn")
@onready var game_controller = $/root/GameController
@onready var resource_list_container = %ResourcesList
@onready var category_name_label = %ResourceCategoryName
@onready var category_name_container = %CategoryNameContainer

var current_resource_list = []
var resource_category_name = "catname"

func _ready():
    for n in resource_list_container.get_children():
        resource_list_container.remove_child(n)
        n.queue_free()    

func _process(delta):
    if resource_category_name == "special":
        category_name_container.hide()
    else:
        category_name_label.text = Query.ResourceCategoryName(resource_category_name)
        find_child("TooltipShower").tooltip_label_text = Query.ResourceCategoryDescription(resource_category_name)
        
    var resource_list = Array(Query.SettlementInventoryResources(game_controller.currentSettlementUuid, resource_category_name))    
    if resource_list == current_resource_list:
        return
    current_resource_list = resource_list
    
    for n in resource_list_container.get_children():
        resource_list_container.remove_child(n)
        n.queue_free()
    for resource_name in resource_list:
        var new_resource = resource_template.instantiate() as ResourceUI
        resource_list_container.add_child(new_resource)
        new_resource.resource_name = resource_name
