extends TextureRect

class_name ProgramTile

var command: ProgramCommands.Command = ProgramCommands.Command.NONE

func _get_drag_data(at_position: Vector2) -> Variant:
	var data = {
		type = "command",
		command = command,
		texture = self.texture
	}

	create_drag_texture()
	queue_free()
	
	return data

func create_drag_texture() -> void:
	var drag_texture = TextureRect.new()
	drag_texture.expand_mode = self.expand_mode
	drag_texture.texture = self.texture
	drag_texture.size = self.size
	
	var control = Control.new()
	control.add_child(drag_texture)
	drag_texture.get_rect().position = -drag_texture.size/2
	set_drag_preview(control)

func _can_drop_data(at_position: Vector2, data: Variant) -> bool:
	if data.get("type", "") == "command":
		return true
	return false
	
func _drop_data(at_position: Vector2, data: Variant) -> void:
	if data.get("type", "") != "command":
		return
	var new_button: ProgramTile = preload("res://prefabs/program_tile.tscn").instantiate()
	new_button.texture = data["texture"]
	new_button.command = data["command"]
	get_parent().add_child(new_button)
	
	var index = get_index()
	get_parent().move_child(new_button, index)
	
