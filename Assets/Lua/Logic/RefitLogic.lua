local RefitLogic = RequireSingleton("RefitLogic")
local Part_C = RequireConfig("Part_C")
local Model_C = RequireConfig("Model_C")
local String_C = RequireConfig("String_C") 
local Level_C=RequireConfig("Level_C")
local FairyGUI = FairyGUI

function RefitLogic:OnInitialize()
    self.Sell_QualityId = 0 -- 出售界面 筛选零件的类型 索引  
    self.partsTypeId = 0  -- 改装界面  筛选零件的类型  索引
    self.partByNameGetId = {  -- 按钮类型
        btnparts_zhuti = 1,
        btnparts_gongji = 2,
        btnparts_chelun = 3,
        btnparts_fuzhu = 4,
        btnparts_fuwen = 5,
        btnparts_zhuangshi = 6,
        btnparts_tiezhi = 7,
    }
    self.partByIdGetName = {
        [1] = "btnparts_zhuti" ,
        [2] = "btnparts_gongji" ,
        [3] = "btnparts_chelun" ,
        [4] = "btnparts_fuzhu" ,
        [5] = "btnparts_fuwen" ,
        [6] = "btnparts_zhuangshi",
        [7] = "btnparts_tiezhi" ,
    }

    -- 模型路径
    self.houseIdGetModelPath = {
        [1] = "Models/JiJiaPao01_cheshen",
        [2] = "Models/Feiji_01",
        [3] = "Models/JiQiRen_jishen",
    }

    -- 属性类型 图标
    self.AttTypeIcon = {
        [1] = "icon-shuxing-gongji",
        [2] = "icon-shuxing-chengzai",
        [3] = "icon-shuxing-xueliang",
        [4] = "icon-shuxing-sudu",
        [5] = "icon-shuxing-zhuangxiang",
    }
end

-- 获取道具类型
function RefitLogic:GetItemType( nItemId )
    local itemType = Part_C[nItemId].partType
    return itemType
end

-- 获取道具名称
function RefitLogic:GetItemName( nItemId )
    local itemName = Part_C[nItemId].name
    itemName=String_C[itemName].str
    return itemName
end

-- 获取道具品质
function RefitLogic:GetItemQuality( nItemId )
    local itemQuality = Part_C[nItemId].partQuality
    return itemQuality
end

-- 获取道具模型加载路径
function RefitLogic:GetItemModelPath( nItemId )
    local itemModelId = Part_C[nItemId].normalModel
    local modelPath=Model_C[itemModelId].strSkinName
    modelPath = "Models/"..modelPath
    return modelPath
end

--  加载道具品质星星         -- 星星数量 ，资源包名，UI视图名字  ， 层级名字   ，图片资源名字 ， 星星的父物体
function RefitLogic:LoadItemStar( nStarNum,sBagName,sViewName,sLayerName,sImageName ,gItem) 
    --local _star=FairyGUI.UIPackage.CreateObject(sBagName,sImageName).asImage  --星星如果不存在等级分布 就去掉俩参数
    for i=1,nStarNum do        
        local loader=FairyGUI.GLoader.New()
        loader.url = FairyGUI.UIPackage.GetItemURL(sBagName,sImageName)
        local _Layer=FairyGUI.GRoot.inst:GetChild(sLayerName):GetChild(sViewName)
        _Layer:AddChild(loader)
        gItem:AddChild(loader)
        local x = 30*(i-1)  --_star.width
        loader:SetXY(x,4)      
    end
    --_star.asGameobject
    --UnityEngine.Object.Destroy(_star.gameObject,0.001)
end

function RefitLogic:GetPartIcon(nItemId)
    local itemicon = Part_C[nItemId].strIcon
    return itemicon
end

--通过关卡ID获取关卡通关条件 怪物ID  体力消耗
function RefitLogic:GetMonsterModelid(id)
    local table={}
    local tCfg = Level_C[id]
    table[1]=tCfg.victoryCondition
    table[2]=tCfg.physical
    table[3]=tCfg.modelShow 
    return table
end
