extends TileMapLayer

class_name LevelController

@export var start_position: Node2D

enum TileSide
{
	TOP,
	BOTTOM,
	LEFT,
	RIGHT
}

func _ready() -> void:
	var game: GameController = preload("res://scenes/game.tscn").instantiate()
	game.set_map(self)

func get_terrain_at_tile(position: Vector2i, side: TileSide) -> String:
	var tile_data := get_cell_tile_data(position)
	if !tile_data:
		return ""
		
	if is_cell_flipped_h(position):
		if side == TileSide.LEFT:
			side = TileSide.RIGHT
		if side == TileSide.RIGHT:
			side = TileSide.LEFT
			
	if is_cell_flipped_v(position):
		if side == TileSide.TOP:
			side = TileSide.BOTTOM
		if side == TileSide.BOTTOM:
			side = TileSide.TOP
			
	if is_cell_transposed(position):
		match side:
			TileSide.TOP:
				side = TileSide.LEFT
			TileSide.LEFT:
				side = TileSide.BOTTOM
			TileSide.BOTTOM:
				side = TileSide.RIGHT
			TileSide.RIGHT:
				side = TileSide.TOP
	
	var facing_side := map_facing_side(side)
	var facing_id := tile_data.get_terrain_peering_bit(facing_side)
	var facing_name := tile_set.get_terrain_name(0, facing_id)
	return facing_name

func map_facing_side(tile_side: TileSide) ->  TileSet.CellNeighbor:
	match tile_side:
		TileSide.TOP:
			return TileSet.CellNeighbor.CELL_NEIGHBOR_TOP_SIDE
		TileSide.BOTTOM:
			return TileSet.CellNeighbor.CELL_NEIGHBOR_BOTTOM_SIDE
		TileSide.LEFT:
			return TileSet.CellNeighbor.CELL_NEIGHBOR_LEFT_SIDE
		TileSide.RIGHT:
			return TileSet.CellNeighbor.CELL_NEIGHBOR_RIGHT_SIDE
	return 0
