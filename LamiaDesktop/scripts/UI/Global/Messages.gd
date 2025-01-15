extends PanelContainer

@onready var message_history_list = %MessageHistoryList
@onready var message_list = %MessageList
const message_template = preload("res://scenes/UI/Global/message.tscn")
const message_history_template = preload("res://scenes/UI/Global/message_history.tscn")
var message_history = []
var unread_messages = []

func _ready():
    pass


func _process(_delta):
    var current_message_history = Array(Query.MessageHistory())
    if message_history != current_message_history:
        message_history = current_message_history
        for n in message_history_list.get_children():
            message_history_list.remove_child(n)
            n.queue_free()			
        for message in message_history:
            var new_message_child = message_history_template.instantiate() as Label
            new_message_child.text = message
            message_history_list.add_child(new_message_child)
    
    var new_unread_messages = Query.UnreadMessages()
    if new_unread_messages.size():
        unread_messages = new_unread_messages
        for n in message_list.get_children():
            message_list.remove_child(n)
            n.queue_free()			
        for message in unread_messages:
            var new_message_child = message_template.instantiate() as Label
            new_message_child.text = message
            message_list.add_child(new_message_child)

