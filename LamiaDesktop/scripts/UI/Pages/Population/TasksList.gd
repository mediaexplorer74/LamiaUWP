extends ScrollContainer

@onready var task_template = preload(
    "res://scenes/UI/Pages/Population/task.tscn"
    )
@onready var game_controller = $/root/GameController
@onready var task_container = $TaskContainer

var current_task_list = []


func _process(_delta):
    var task_list = Array(Query.SettlementTasks(game_controller.currentSettlementUuid))
    
    if task_list == current_task_list:
        return
    current_task_list = task_list
    
    for n in task_container.get_children():
        task_container.remove_child(n)
        n.queue_free()
    for task_name in task_list:
        var new_task = task_template.instantiate() as Task
        task_container.add_child(new_task)
        new_task.task_name = task_name
