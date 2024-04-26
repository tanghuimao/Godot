extends Node

#导出怪物场景
@export var mob_scene: PackedScene

# Called when the node enters the scene tree for the first time.
func _ready():
	$UserInterface/Retry.hide()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_mob_timer_timeout():
	#创建一个怪物场景
	var mob = mob_scene.instantiate()
	#创建一个随机位置  在路径上随机获取一个位置
	var mob_spawn_location = get_node("SpawnPath/SpawnLocation")
	mob_spawn_location.progress_ratio  = randf()
	#获取玩家位置
	var player_position = $Player.position
	#调用mob脚本函数
	mob.initialize(mob_spawn_location.position, player_position)
	#添加节点到场景
	add_child(mob)
	#链接信号
	mob.squashed.connect($UserInterface/ScoreLabel._on_mob_squashed.bind())


func _on_player_hit():
	$MobTimer.stop()
	$UserInterface/Retry.show()

func _unhandled_input(event):
	if event.is_action_pressed("ui_accept") && $UserInterface/Retry.visible:
		#重置场景
		get_tree().reload_current_scene()
