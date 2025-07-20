extends PanelContainer

@onready var restart_game_confirm_dialog = %RestartGameConfirmDialog

func _on_main_menu_button_pressed():
    show()

func _on_back_to_game_button_pressed():
    hide()

func _on_quit_button_pressed():
    $/root/GameController.SaveGame()
    get_tree().quit()

func _on_restart_game_button_pressed():
    restart_game_confirm_dialog.visible = true

func _on_restart_game_confirm_dialog_canceled():
    restart_game_confirm_dialog.visible = false

func _on_restart_game_confirm_dialog_confirmed():
    $/root/GameController.RestartGame()
    get_tree().reload_current_scene()
