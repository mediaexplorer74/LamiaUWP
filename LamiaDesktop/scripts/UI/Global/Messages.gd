extends PanelContainer

@onready var message_list = $MessageList
const message_template = preload("res://scenes/UI/Global/message.tscn")
var messages = []

func _ready():
    pass


func _process(_delta):
    var new_messages = Query.UnreadMessages()
    if new_messages.size():
        messages.append_array(new_messages)
        if messages.size() > Globals.MESSAGES_DISPLAY_NUM:
            messages = messages.slice(
                messages.size() - Globals.MESSAGES_DISPLAY_NUM
            )
        for n in message_list.get_children():
            message_list.remove_child(n)
            n.queue_free()			
        for message in messages:
            var new_message_child = message_template.instantiate() as Label
            new_message_child.text = message
            message_list.add_child(new_message_child)			


func _gui_input(event):
    if event is InputEventMouseMotion:
        if event.button_mask == 1 and event.pressure == 1.00:
            global_position += event.relative
