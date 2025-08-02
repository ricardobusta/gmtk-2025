extends PanelContainer

@onready var layout = $HBoxContainer

func _can_drop_data(at_position: Vector2, data: Variant) -> bool:
	if data.get("type", "") == "command":
		return true
	return false
	
func _drop_data(at_position: Vector2, data: Variant) -> void:
	if data.get("type", "") != "command":
		return
	var new_button: TextureRect = preload("res://prefabs/program_tile.tscn").instantiate()
	new_button.texture = data["texture"]
	layout.add_child(new_button)
