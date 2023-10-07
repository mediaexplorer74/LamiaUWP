extends PanelContainer
class_name TaskPopulation

@export var color_normal: Color
@export var color_wait: Color

@onready var population_icon = %PopulationIcon
@onready var poplation_name_label = %PopulationNameLabel
@onready var task_list = %TaskList
@onready var assign_task_button = %AssignTaskButton
@onready var assign_task_holder = %AssignTaskHolder
@onready var current_action_progress = %CurrentActionProgress
@onready var current_action_label = %CurrentActionLabel
@onready var wait_message = %WaitMessage
@onready var inventory_progress = %InventoryProgress
@onready var hunger_progress = %HungerProgress
@onready var game_controller = $/root/GameController

@export var population_uuid = "xxxx":
    set(uuid):
        population_uuid = uuid
        poplation_name_label.text = Query.PopulationMemberName(game_controller.currentSettlementUuid, population_uuid)
        var species = Query.PopulationMemberSpecies(game_controller.currentSettlementUuid, population_uuid)
        population_icon.texture = load("res://media/icons/icon_species_%s.png" % species)


func _process(_delta):
    if not population_uuid:
        return
    # state
    if Query.PopulationMemberState(game_controller.currentSettlementUuid, population_uuid) == "wait" :
        wait_message.show()
        find_child("TooltipShower").tooltip_label_text = Query.PopulationMemberWaitMessage(game_controller.currentSettlementUuid, population_uuid)
        get_theme_stylebox("panel", "PanelContainer").border_color = color_wait
    else:
        wait_message.hide()
        get_theme_stylebox("panel", "PanelContainer").border_color = color_normal
    # Inventory 
    inventory_progress.set_value_no_signal(Query.PopulationMemberInventoryProgress(game_controller.currentSettlementUuid, population_uuid))
    # task
    var current_task = Query.PopulationMemberTask(game_controller.currentSettlementUuid, population_uuid)
    current_action_label.text = Query.PopulationMemberCurrentActionName(game_controller.currentSettlementUuid, population_uuid)
    var current_action = Query.PopulationMemberCurrentAction(game_controller.currentSettlementUuid, population_uuid)
    # task progress
    if current_action == "idle":
        current_action_progress.hide()
        return
    current_action_progress.show()
    current_action_progress.set_value_no_signal(Query.PopulationMemberCurrentActionProgress(game_controller.currentSettlementUuid, population_uuid))

func _on_assign_task_button_pressed():
    if assign_task_holder.visible:
        assign_task_holder.hide()
        return
    task_list.clear()
    for task_name in get_available_tasks():
        task_list.add_item(
            Query.SettlementTaskName(game_controller.currentSettlementUuid, task_name),
            load("res://media/icons/icon_task_%s.png" % task_name)
        )
    assign_task_holder.position = assign_task_button.global_position
    assign_task_holder.show()


func _on_assign_task_holder_mouse_exited():
    if assign_task_holder.visible:
        assign_task_holder.hide()


func _on_task_list_item_clicked(index, _at_position, _mouse_button_index):
    Action.PopulationAssignToTask(game_controller.currentSettlementUuid, population_uuid, get_available_tasks()[index])


func get_available_tasks():
    var current_task = Query.PopulationMemberTask(game_controller.currentSettlementUuid, population_uuid)
    var available_tasks = Array(Query.SettlementTasks(game_controller.currentSettlementUuid))
    available_tasks.remove_at(available_tasks.find(current_task))
    return available_tasks    
