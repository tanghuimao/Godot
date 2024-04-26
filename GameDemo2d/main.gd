extends Node

#导出变量到编辑器  包装场景为变量
@export var mob_scene: PackedScene

#声明分数
var score
#初始速度
var speed

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func new_game():
	#播放音乐
	$Music.play()
	#清空怪物 上一次游戏怪物
	get_tree().call_group("mobs", "queue_free")
	#初始化分数
	score = 0
	speed = 130
	#获取玩家节点，调用玩家节点start方法
	$Player.start($StartPosition.position)
	$StartTimer.start()
	$HUD.update_score(score)
	$HUD.show_message("Get Ready")


func game_over():
	$Music.stop()
	$DeathSound.play()
	
	#停止定时器
	$ScoreTimer.stop()
	$MobTimer.stop()
	$HUD.show_game_over()
	

#启动定时器
func _on_start_timer_timeout():
	$MobTimer.start()
	$ScoreTimer.start()

# 定时器执行  分数加
func _on_score_timer_timeout():
	#分数叠加
	score += 1
	#速度叠加
	speed += 1
	$HUD.update_score(score)

#生成怪物
func _on_mob_timer_timeout():
	#创建一个怪物场景实例
	var mob = mob_scene.instantiate()
	#创建一个随机位置  在路径上随机获取一个位置
	var mob_spawn_location = get_node("MobPath/MobSpawnLocation")
	mob_spawn_location.progress_ratio = randf()
	#设置怪物方向
	var direction = mob_spawn_location.rotation + PI / 2 
	#设置怪物随机位置
	mob.position = mob_spawn_location.position
	#添加一些方向随机性
	direction += randf_range(-PI / 5, PI / 5)
	#设置怪物方向 弧度
	mob.rotation = direction
	#为怪物选择速度 速度随时间变化增加
	var velocity = Vector2(speed, 0.0)
	mob.linear_velocity = velocity.rotated(direction)
	#将怪物添加到屏幕
	add_child(mob)
