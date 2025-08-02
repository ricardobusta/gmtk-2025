extends Node

class_name GameController

@onready var play_button: BaseButton = $"CanvasLayer/UI/PlayButton"
@onready var stop_button: BaseButton = $"CanvasLayer/UI/StopButton"
@onready var robot: CharacterBody2D = $"World/CharacterBody2D"
@onready var program_container: Container = $"CanvasLayer/UI/ProgramScrollContainer/PanelContainer/HBoxContainer"
@onready var world: Node2D = $World

const tile_size := 16
const move_time := 0.5
const flip_time := move_time/2
const jump_time := move_time

const TILE_EMPTY := "empty"
const TILE_SOLID := "solid"
const TILE_SPIKE := "spike"

var is_interrupted := false
var is_playing := false

var start_position: Vector2i
var map: LevelController

func _ready() -> void:
	play_button.button_down.connect(on_play_button_clicked)
	stop_button.button_down.connect(on_stop_button_clicked)
	
	if not map:
		printerr("Need to boot from a map")
		return
		
	start_position = map.map_to_local(map.local_to_map(map.start_position.position))
	robot.global_position = start_position
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
	robot.global_position = start_position
	robot.scale = Vector2.ONE
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
		return

	if side_terrain == TILE_SPIKE:
		stop_execution("Hit spike wall!")
		return

	var ground_terrain := map.get_terrain_at_tile(target_grid_pos + Vector2i.DOWN,
		 	LevelController.TileSide.TOP)
	
	if ground_terrain == TILE_SPIKE:
		stop_execution("Stepped on spikes!")
		
	if ground_terrain == TILE_EMPTY:
		stop_execution("Hole!")

	# move into position
	var target_pos = map.map_to_local(target_grid_pos)
	await animate_to_position(target_pos, 0.2)

func execute_command_flip() -> void:
	await get_tree().create_timer(flip_time).timeout
	robot.scale.x = -robot.scale.x
	await get_tree().create_timer(flip_time).timeout

func execute_command_jump() -> void:
	await execute_command_move()

func stop_execution(reason: String) -> void:
	self.is_interrupted = true
	print("Execution Interrupted: ", reason)

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
