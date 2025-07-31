extends TextureRect

func _get_drag_data(at_position: Vector2) -> Variant:
	var data = {}
	
	data["origin_texture"] = texture

	var drag_texture = TextureRect.new()
	drag_texture.expand_mode = self.expand_mode
	drag_texture.texture = self.texture
	drag_texture.size = self.size
	
	var control = Control.new()
	control.add_child(drag_texture)
	drag_texture.get_rect().position = -drag_texture.size/2
	set_drag_preview(control)
	
	return data
	
func _can_drop_data(at_position: Vector2, data: Variant) -> bool:
	print("Can drop data %s", at_position)
	return true
	
func _drop_data(at_position: Vector2, data: Variant) -> void:
	texture = data["origin_texture"]
