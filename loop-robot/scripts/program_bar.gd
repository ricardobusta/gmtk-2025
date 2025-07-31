extends Node

func _get_drag_data(at_position: Vector2) -> Variant:
	var data = {}
	
	return data
	
func _can_drop_data(at_position: Vector2, data: Variant) -> bool:
	print("Can drop data %s", at_position)
	return true
	
func _drop_data(at_position: Vector2, data: Variant) -> void:
	pass
