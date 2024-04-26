extends Area2D

#自定义信号
signal hit
#导出变量速度到编辑器使用
@export var speed = 400
#声明屏幕大小
var screen_size


# Called when the node enters the scene tree for the first time.
func _ready():
	# 获取可视屏幕大小
	screen_size = get_viewport_rect().size
	# 设置隐藏玩家
	hide()
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	#设置移动初始速度
	var velocity = Vector2.ZERO
	#检测移动
	if Input.is_action_pressed("ui_up"):
		velocity.y -= 1
	if Input.is_action_pressed("ui_down"):
		velocity.y += 1
	if Input.is_action_pressed("ui_left"):
		velocity.x -= 1
	if Input.is_action_pressed("ui_right"):
		velocity.x += 1
	
	#计算移动
	if velocity.length() > 0:
		#计算实际移动向量  可能有夹角
		velocity = velocity.normalized() * speed
		#播放动画
		$AnimatedSprite2D.play()
	else:
		$AnimatedSprite2D.stop()
	#变更位置  速度*刷新帧率
	position += velocity * delta
	#限制玩家 移动范围在可视屏幕内
	position = position.clamp(Vector2.ZERO, screen_size)
	#根据移动选择动画
	if velocity.x != 0:
		#选择动画
		$AnimatedSprite2D.animation = "walk"
		#垂直反转
		$AnimatedSprite2D.flip_v = false
		#判断水平反转
		$AnimatedSprite2D.flip_h = velocity.x < 0
	elif velocity.y != 0:
		$AnimatedSprite2D.animation = "up"
		$AnimatedSprite2D.flip_v = velocity.y > 0
	#调整玩家回正身位
	if velocity.x < 0:
		$AnimatedSprite2D.flip_h = true
	else:
		$AnimatedSprite2D.flip_h = false
	if velocity.y > 0:
		$AnimatedSprite2D.flip_v = true
	else:
		$AnimatedSprite2D.flip_v = false

# 信号接收
func _on_body_entered(body):
	#隐藏玩家
	hide()
	#调用自定义碰撞信号
	hit.emit()
	#延迟 设置节点属性 disabled 不可用
	$CollisionShape2D.set_deferred("disabled", true)
	
func start(pos):
	#设置玩家位置
	position = pos
	#显示
	show()
	#设置节点可用
	$CollisionShape2D.disabled = false
