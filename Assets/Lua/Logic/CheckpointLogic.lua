local CheckpointLogic = RequireSingleton("CheckpointLogic") 
local Level_C = RequireConfig("Level_C")
local Chapter_C = RequireConfig("Chapter_C")

function CheckpointLogic:OnInitialize()
    self.sratImage = {
        [0]="icon-guanka-xingxing-weijihuo", --未通过
        [1]="icon-guanka-xingxing", --通过
    }

    self.checkLineImg = {
        [0] = "icon-gunaka-xuanzhong-xian",
        [1] = "icon-gunaka-jihuo-xian",
        [2] = "icon-gunaka-weijihuo-xian",
    }

    self.starnIndex = {
        [1]={x = 944,y = 518,scale = 1,alpha = 1}, --中间星球坐标
        [2]={x = 65,y = 564,scale = 0.52,alpha = 0.3},  -- 左1
        [3]={x = 535,y = 586,scale = 0.25,alpha = 0.1}, -- 左2
        [4]={x = 1354,y = 560,scale = 0.25,alpha = 0.1}, -- 右1
        [5]={x = 1770,y = 555,scale = 0.52,alpha = 0.3},-- 右2
    }  
    self.ModeId = 0  -- 模式选择标识
    self.MainRecord={} -- 主题关卡通关记录
    self.MainStarNum = {}  -- 主题界面 星星数量
    self.CheckRecord = {}  -- 关卡界面 通关记录 每关星星数量
    self.themeId = 0      --  
end

-- 获取主题关卡数量
function CheckpointLogic:GetCheckNum( nThemeId )
    self.CheckInfo = {}
   local _checkid = nThemeId*1000
    for i=_checkid,1015 do
        local _themeth=math.floor(i/1000)
        if _themeth == nThemeId then
            local _info=Level_C[i] 
            if _info~=nil then 
                table.insert( self.CheckInfo, i )
            end
        end 
    end
    -- for _checkid,_ in pairs(Level_C) do
    --     local _themeth=math.floor(_checkid/1000)
    --     if _themeth == nThemeId then 
    --         table.insert( self.CheckInfo, _checkid)
    --     end
    -- end    
end

--  获取主题章节数量
function CheckpointLogic:GetChapterInfo(  )
    self.ChapterInfo ={}
    self.Chapter ={}
    local _chapterNum =#Chapter_C    
    for i=1,_chapterNum do
        local _chapterInfo=Chapter_C[i]
        self.ChapterInfo[i] =_chapterInfo
        self.Chapter[i-1]=i
    end
end

-- 获取章节星星数量
function CheckpointLogic:GetThemeStarNum ( nThemeId )
    local starNum = 0
    self:GetCheckNum(nThemeId)
    for i=1,10 do
        local checkId=self.CheckInfo[i]
        local checkedId=CS_GetPlayerPassGameLevel()
        if checkedId>Level_C[checkId].preLevelId then 
            local _starNum = 3
            starNum = starNum + _starNum
            self.MainStarNum[nThemeId]=starNum
        else
            self.MainStarNum[nThemeId]=starNum
        end
    end        
end

-- 判断 是否通关全部关卡
function  CheckpointLogic:GetCheckMax( nThemeId )
    local checkId = 0
    self:GetCheckNum(nThemeId)
    local checkLength =#self.CheckInfo
    for i=1,checkLength do
        local _checkID=self.CheckInfo[i]
        if _checkID>checkId then 
            checkId =_checkID
        else            
        end 
    end
    return checkId
end