extends PanelContainer
class_name Task

@onready var header_icon = %HeaderIcon
@onready var header_label = %HeaderLabel
@onready var population_container = %PopulationContainer
@onready var game_controller = $/root/GameController
@onready var task_population_template = preload("res://scenes/UI/Pages/Population/task_population.tscn")

@export var task_name = "thing"

var current_population_assigned_list = []


func _ready():
    for n in population_container.get_children():
        population_container.remove_child(n)
        n.queue_free()
            

func _process(_delta):
    if not task_name:
        return
    header_label.text = Query.SettlementTaskName(game_controller.currentSettlementUuid, task_name)
    header_icon.texture = load("res://media/icons/icon_task_%s.png" % task_name)
    find_child("Info").get_node("TooltipShower").tooltip_label_text = "\n".join(Query.SettlementTaskDescription(game_controller.currentSettlementUuid, task_name))
    
    var population_assigned_list = Array(Query.SettlementTaskAssignments(game_controller.currentSettlementUuid, task_name))
    
    if population_assigned_list == current_population_assigned_list:
        return
    current_population_assigned_list = population_assigned_list
    
    for n in population_container.get_children():
        population_container.remove_child(n)
        n.queue_free()
    for population_uuid in current_population_assigned_list:
        var new_pop = task_population_template.instantiate() as TaskPopulation
        population_container.add_child(new_pop)
        new_pop.population_uuid = population_uuid
