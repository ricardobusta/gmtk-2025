extends Node

class_name GameController

@onready var play_button: BaseButton = $"CanvasLayer/UI/PlayButton"
@onready var stop_button: BaseButton = $"CanvasLayer/UI/StopButton"
@onready var robot: CharacterBody2D = $"World/CharacterBody2D"
@onready var program_container: Container = $"CanvasLayer/UI/ProgramScrollContainer/PanelContainer/HBoxContainer"
@onready var world: Node2D = $World

const tile_size := 16
const move_time := 0.5
const flip_time := 0.2
const jump_time := 0.5

const TILE_SOLID := "solid"
const TILE_SPIKE := "spike"

var is_interrupted := false
var is_playing := false

var map: LevelController

func _ready() -> void:
	play_button.button_down.connect(on_play_button_clicked)
	stop_button.button_down.connect(on_stop_button_clicked)
	
	if not map:
		printerr("Need to boot from a map")
		return
		
	var start_pos = map.local_to_map(map.start_position.position)
	robot.global_position = map.map_to_local(start_pos)
	map.start_position.queue_free()

func set_map(tile_map: TileMapLayer) -> void:
	map = tile_map
	map.add_child(self)

func on_play_button_clicked() -> void:
	if is_playing:
		return
		
	print("Play")
	is_playing = true
	is_interrupted = false
	var commands := get_program_commands()
	
	if len(commands) == 0:
		return
	
	while not is_interrupted:
		await execute_commands(commands)

func execute_commands(commands: Array[ProgramCommands.Command]):
	for c in commands:
		if self.is_interrupted:
			break
		print(ProgramCommands.Command.keys()[c])
		match c:
			ProgramCommands.Command.MOVE:
				await execute_command_move()
			ProgramCommands.Command.FLIP:
				await execute_command_flip()
			ProgramCommands.Command.JUMP:
				await execute_command_jump()
				
func on_stop_button_clicked() -> void:
	if not is_playing:
		return;
		
	print("Stop")
	is_playing = false
	stop_execution("Stop button clicked")

func get_program_commands() -> Array[ProgramCommands.Command]:
	var commands : Array[ProgramCommands.Command] = []

	for child in program_container.get_children():
		if child is ProgramTile:
			var tile:= child
			commands.append(tile.command)

	return commands
	
func execute_command_move() -> void:
	var direction = Vector2i(robot.scale.x, 0)
	var current_grid_pos = map.local_to_map(robot.position)
	var target_grid_pos = current_grid_pos + direction

	var side_terrain = map.get_terrain_at_tile(target_grid_pos,
	 	LevelController.TileSide.LEFT 
		if direction.x < 0 
		else LevelController.TileSide.RIGHT)
#
	if side_terrain == TILE_SOLID:
		stop_execution("Hit wall in front")

	if side_terrain == TILE_SPIKE:
		stop_execution("Hit spike wall!")

	var ground_terrain := map.get_terrain_at_tile(target_grid_pos - Vector2i.UP,
		 	LevelController.TileSide.TOP)
	
	if ground_terrain == TILE_SPIKE:
		stop_execution("Stepped on spikes!")
		
	if ground_terrain == "":
		stop_execution("Hole!")

	# move into position
	var target_pos = map.map_to_local(target_grid_pos)
	await animate_to_position(target_pos, 0.2)

func execute_command_flip() -> void:
	await get_tree().create_timer(flip_time).timeout
	robot.scale.x = -robot.scale.x
	await get_tree().create_timer(flip_time).timeout

func execute_command_jump() -> void:
	var start_pos:= robot.position
	var end_pos:= robot.position + Vector2(tile_size * (2 * robot.scale.x), 0)
	var jump_height:= tile_size
	await animate_jump_arc(start_pos, end_pos, jump_height, jump_time)

func stop_execution(reason: String) -> void:
	self.is_interrupted = true
	print("Execution Interrupted: ", reason)

func is_tile_blocking(grid_pos: Vector2i) -> bool:
	var tile_data = map.get_cell_tile_data(grid_pos)
	if tile_data == null:
		return false

	return false

func animate_to_position(target: Vector2, duration: float) -> void:
	var elapsed := 0.0
	var start := robot.global_position
	while elapsed < duration:
		elapsed += get_process_delta_time()
		robot.global_position = start.lerp(target, elapsed / duration)
		await get_tree().process_frame

func animate_jump_arc(start: Vector2, end: Vector2, height: float, duration: float) -> void:
	var elapsed := 0.0
	while elapsed < duration:
		elapsed += get_process_delta_time()
		var t := elapsed / duration
		var arc_y := -4 * height * t * (1 - t)
		var pos := start.lerp(end, t)
		pos.y += arc_y  # Apply arc vertically
		robot.position = pos
		await get_tree().process_frame

func tile_get_terrain_bit_from_direction(tile: TileData, neighbor: TileSet.CellNeighbor) -> int:
	var rotated := neighbor
	
	print("transpose", tile.flip_h)
	if tile.flip_h:
		if rotated == TileSet.CellNeighbor.CELL_NEIGHBOR_LEFT_SIDE:
			rotated = TileSet.CellNeighbor.CELL_NEIGHBOR_RIGHT_SIDE
		elif rotated == TileSet.CellNeighbor.CELL_NEIGHBOR_RIGHT_SIDE:
			rotated = TileSet.CellNeighbor.CELL_NEIGHBOR_LEFT_SIDE

	print("transpose", tile.flip_v)
	if tile.flip_v:
		if rotated == TileSet.CellNeighbor.CELL_NEIGHBOR_TOP_SIDE:
			rotated = TileSet.CellNeighbor.CELL_NEIGHBOR_BOTTOM_SIDE
		elif rotated == TileSet.CellNeighbor.CELL_NEIGHBOR_BOTTOM_SIDE:
			rotated = TileSet.CellNeighbor.CELL_NEIGHBOR_TOP_SIDE

	print("transpose", tile.transpose)
	if tile.transpose:
		var transpose_map = {
			TileSet.CellNeighbor.CELL_NEIGHBOR_TOP_SIDE: TileSet.CellNeighbor.CELL_NEIGHBOR_LEFT_SIDE,
			TileSet.CellNeighbor.CELL_NEIGHBOR_BOTTOM_SIDE: TileSet.CellNeighbor.CELL_NEIGHBOR_RIGHT_SIDE,
			TileSet.CellNeighbor.CELL_NEIGHBOR_LEFT_SIDE: TileSet.CellNeighbor.CELL_NEIGHBOR_TOP_SIDE,
			TileSet.CellNeighbor.CELL_NEIGHBOR_RIGHT_SIDE: TileSet.CellNeighbor.CELL_NEIGHBOR_BOTTOM_SIDE
		}
		rotated = transpose_map[rotated]
	return tile.get_terrain_peering_bit(rotated)
