using System;
using System.Collections.Generic;
using ClassicRoguelikeCourse.Component;
using ClassicRoguelikeCourse.Entites;
using ClassicRoguelikeCourse.Entites.Characters.Player;
using ClassicRoguelikeCourse.Entities.Characters.Enemies;
using ClassicRoguelikeCourse.Managers.CombatManager;
using ClassicRoguelikeCourse.Managers.MapManager;
using ClassicRoguelikeCourse.Managers.SaveLoadManager;
using ClassicRoguelikeCourse.Resources.CharacterData;
using Godot;

namespace ClassicRoguelikeCourse.Entities.Characters;

/// <summary>
/// 角色抽象父类接口
/// </summary>
public partial class Character : Node2D, IEntity, ISavable, ILoadable
{
    //角色数据
    [Export] protected CharacterData _characterData;
    public  CharacterData CharacterData => _characterData;
    //组件集合
    protected List<IComponent> Components = new ();
    //地图管理器
    protected MapManager _mapManager;
    //战斗管理器
    protected CombatManager _combatManager;
    //是否死亡
    protected bool _isDead;
    public bool IsDead => _isDead;
    
    // 存档管理器
    protected  SaveLoadManager _saveLoadManager;
    
    public virtual void Initialize()
    {
        _mapManager = GetTree().CurrentScene.GetNode<MapManager>("%MapManager");
        _combatManager = GetTree().CurrentScene.GetNode<CombatManager>("%CombatManager");
        _saveLoadManager = GetTree().CurrentScene.GetNode<SaveLoadManager>("%SaveLoadManager");
        //获取所有子节点
        foreach (var child in GetChildren())
        {
            if (child is not IComponent) continue;
            var component = child as IComponent;
            component.Initialize();
            Components.Add(component);
        }
        //初始化战斗属性
        if (!InitializeByLoadData())
        {
            InitializeCombatAttributes();
        }
        //订阅事件
        _combatManager.CharacterDead += OnCharacterDead;
    }

    public virtual void Update(double delta)
    {
        //遍历组件 调用更新方法
        foreach (var component in Components)
        {
            component.Update(delta);
        }
    }
    /// <summary>
    /// 获取保存数据
    /// </summary>
    /// <returns></returns>
    public virtual Godot.Collections.Dictionary<string, Variant> GetDataForSave()
    {
        return new Godot.Collections.Dictionary<string, Variant>()
        {
            { "Name", _characterData.Name },
            { "Sight", _characterData.Sight },
            { "Strength", _characterData.Strength },
            { "constitution", _characterData.Constitution },
            { "Agility", _characterData.Agility },
            { "StrengthIncrementEffects", _characterData.StrengthIncrementEffects },
            { "ConstitutionIncrementEffects", _characterData.ConstitutionIncrementEffects },
            { "AgilityIncrementEffects", _characterData.AgilityIncrementEffects },
            { "Health", _characterData.Health },
            { "MaxHealth", _characterData.MaxHealth },
            { "Attack", _characterData.Attack },
            { "Defend", _characterData.Defend },
            { "Dodge", _characterData.Dodge },
            { "CriticalChance", _characterData.CriticalChance },
        };
    }
    
    /// <summary>
    /// 初始化战斗属性
    /// </summary>
    /// <param name="characterLoadData"></param>
    protected void InitializeByCharacterLoadData(Godot.Collections.Dictionary<string,Variant> characterLoadData)
    {
        _characterData.Name = characterLoadData["Name"].AsString();
        _characterData.Sight = characterLoadData["Sight"].AsInt32();
        _characterData.Strength = characterLoadData["Strength"].AsInt32();
        _characterData.Constitution = characterLoadData["constitution"].AsInt32();
        _characterData.Agility = characterLoadData["Agility"].AsInt32();
        _characterData.StrengthIncrementEffects = characterLoadData["StrengthIncrementEffects"].AsGodotDictionary<string, float>();
        _characterData.ConstitutionIncrementEffects = characterLoadData["ConstitutionIncrementEffects"].AsGodotDictionary<string, float>();
        _characterData.AgilityIncrementEffects = characterLoadData["AgilityIncrementEffects"].AsGodotDictionary<string, float>();
        _characterData.Health = characterLoadData["Health"].AsSingle();
        _characterData.MaxHealth = characterLoadData["MaxHealth"].AsSingle();
        _characterData.Attack = characterLoadData["Attack"].AsSingle();
        _characterData.Defend = characterLoadData["Defend"].AsSingle();
        _characterData.Dodge = characterLoadData["Dodge"].AsSingle();
        _characterData.CriticalChance = characterLoadData["CriticalChance"].AsSingle();
    }
    
    /// <summary>
    /// 存档加载数据
    /// </summary>
    /// <returns></returns>
    public bool InitializeByLoadData()
    {
        if (_saveLoadManager.LoadedData == null ||
            _saveLoadManager.LoadedData.Count == 0 ||
            !_saveLoadManager.LoadedData.ContainsKey("Maps")) return false;
        //敌人
        if (this is Enemy)
        {
            if (!_saveLoadManager.LoadedData.ContainsKey("Maps")) return false;
            
            var maps = _saveLoadManager.LoadedData["Maps"].AsGodotArray<Godot.Collections.Dictionary<string, Variant>>();
            for (int i = 0; i < maps.Count; i++)
            {
                var map = maps[i];
                //名称 和 当前保存场景相同
                if (map["SceneName"].AsString() != GetTree().CurrentScene.Name) continue;
                //获取存档敌人集合
                var enemies = map["Enemies"].AsGodotArray<Godot.Collections.Dictionary<string, Variant>>();
                
                for (int j = 0; j < enemies.Count; j++)
                {
                    var enemy = enemies[j];
                    if (enemy["Index"].AsInt32() == GetIndex())
                    {
                        //初始化
                        InitializeByCharacterLoadData(enemy);
                        return true;
                    }
                }
                
            }
        }
        //玩家
        if (this is Player.Player)
        {
            if (!_saveLoadManager.LoadedData.ContainsKey("Player")) return false;
            
            var player = _saveLoadManager.LoadedData["Player"].AsGodotDictionary<string, Variant>();
            //初始化
            InitializeByCharacterLoadData(player);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 初始化战斗属性
    /// </summary>
    private void InitializeCombatAttributes()
    {
        //生命值
        _characterData.Health = _characterData.Constitution * _characterData.ConstitutionIncrementEffects["Health"];
        _characterData.MaxHealth = _characterData.Constitution * _characterData.ConstitutionIncrementEffects["MaxHealth"];
        //攻击力
        _characterData.Attack = _characterData.Strength * _characterData.StrengthIncrementEffects["Attack"];
        //防御力
        _characterData.Defend = _characterData.Strength * _characterData.StrengthIncrementEffects["Defend"];
        //闪避率
        _characterData.Dodge = _characterData.Agility * _characterData.AgilityIncrementEffects["Dodge"];
        //暴击率
        _characterData.CriticalChance = _characterData.Agility * _characterData.AgilityIncrementEffects["CriticalChance"];
    }
    
    /// <summary>
    /// 改变战斗属性
    /// </summary>
    public void ChangeCombatAttributes()
    {
        //最大生命值
        _characterData.MaxHealth = _characterData.Constitution * _characterData.ConstitutionIncrementEffects["MaxHealth"];
        //攻击力
        _characterData.Attack = _characterData.Strength * _characterData.StrengthIncrementEffects["Attack"];
        //防御力
        _characterData.Defend = _characterData.Strength * _characterData.StrengthIncrementEffects["Defend"];
        //闪避率
        _characterData.Dodge = _characterData.Agility * _characterData.AgilityIncrementEffects["Dodge"];
        //暴击率
        _characterData.CriticalChance = _characterData.Agility * _characterData.AgilityIncrementEffects["CriticalChance"];
    }
    

    /// <summary>
    /// 获取当前角色到目标单元格距离
    /// </summary>
    /// <param name="targetCell"></param>
    /// <returns></returns>
    public int GetDistanceTo(Vector2I targetCell)
    {
        //获取当前角色到目标单元格距离
        var startCell = (Vector2I)(GlobalPosition - _mapManager.MapData.CellSize / 2) / _mapManager.MapData.CellSize;
        //计算距离
        var distanceX = Mathf.Abs(startCell.X - targetCell.X);
        var distanceY = Mathf.Abs(startCell.Y - targetCell.Y);
        //返回距离
        return Mathf.Max(distanceX, distanceY);
    }
    
    /// <summary>
    /// 死亡事件  子对象重写使用
    /// </summary>
    /// <param name="character"></param>
    /// <exception cref="NotImplementedException"></exception>
    protected virtual void OnCharacterDead(Character character)
    {
        throw new Exception("不可直接调用基类方法");
    }
}