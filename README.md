# 1151VR-HW6-412402141-陳怡珊

## 1. 專案截圖

<img width="865" height="516" alt="image" src="https://github.com/user-attachments/assets/e281078a-f27b-4801-8dca-995a6f1db5bd" />


<!-- 上傳截圖到 GitHub 後，可使用下列語法：
![遊戲畫面](screenshots/gameplay.png)
-->

## 2. GitHub 連結

https://github.com/Shan11111111/1151VR-HW6-412402141-chenyishan

## 3. YouTube 連結

https://youtu.be/xjqdaxQrn_k

## 4. 製作流程與相關操作

### 4.1 建立 Unity 3D 專案與遊戲場景

- 使用 Unity 6.3 LTS 建立 3D 專案，並以 `MainScene` 作為主要遊戲場景。
- 使用九個 Cube 建立 3×3 九宮格舞台，透過 Transform 設定各方塊的位置與大小，並將物件統一放入 `GridStage` 管理。
- 調整 Main Camera 的位置與拍攝角度，讓玩家能從斜上方觀察九宮格及掉落物。
- 使用 Directional Light 提供場景光源，並開啟 Soft Shadows，使物件能在舞台上產生陰影。
- 使用 Unity Material 設定物件顏色，並將攝影機背景改為 Solid Color。

### 4.2 製作籐籃模型與移動控制

- 使用 Cube 組合籐籃底部及四面牆，建立可接取物品的籐籃模型。
- 在 `Basket` 父物件加入 Box Collider，並勾選 Is Trigger，作為接取物品的判定區域。
- 加入 Rigidbody，關閉 Use Gravity 並啟用 Is Kinematic，避免籐籃受到重力影響。
- 撰寫 `BasketController.cs`，透過滑鼠或手機觸控取得螢幕座標，使用 `ScreenPointToRay()` 與 `Plane.Raycast()` 轉換為 3D 世界座標。
- 使用 `Mathf.Clamp()` 限制籐籃的 X、Z 移動範圍，避免籐籃超出九宮格舞台。
- 利用 `Rigidbody.MovePosition()` 控制籐籃移動，並透過 `OnTriggerEnter()` 判斷是否接住蘋果或炸彈。

### 4.3 建立 Apple 與 Bomb 掉落物

- 使用 Sphere 建立 Apple 與 Bomb 模型，分別套用紅色及黑色材質。
- 為兩種物品加入 Rigidbody 與 Sphere Collider，並分別設定 `Apple`、`Bomb` Tag。
- 將 Apple 和 Bomb 製作為 Prefab，供 Factory 隨機產生。
- 撰寫 `FallingItem.cs`，利用 `Rigidbody.linearVelocity` 控制物品由上往下掉落。
- 加入地面高度與碰撞判定，讓蘋果及炸彈接觸九宮格地板時，自動觸發碎裂或爆炸效果並銷毀。
- 使用布林變數避免同一物件重複觸發碎裂或銷毀事件。

### 4.4 製作 Factory 隨機生成系統

- 撰寫 `ItemFactory.cs`，透過 `Instantiate()` 產生 Apple 或 Bomb Prefab。
- 使用 `Random.Range()` 隨機決定物品生成位置，並由舞台上方往下掉落。
- 使用 `Random.value` 搭配 BombRate 決定炸彈生成機率。
- 在 Inspector 開放 Interval、Speed、BombRate 等參數，方便調整生成間隔、掉落速度及炸彈比例。
- 遊戲結束後停止 Factory，避免持續產生新物件。

### 4.5 建立分數、倒數計時與結局系統

- 使用 Canvas 和 TextMeshPro 建立 Score、Timer、遊戲開始畫面及結束畫面。
- 撰寫 `Director.cs`，集中管理遊戲狀態、分數、倒數時間與 UI 顯示。
- 設定遊戲時間為 60 秒，接到蘋果增加 10 分，接到炸彈扣除 20 分。
- 當分數小於 0 時立即結束遊戲；若分數未低於 0，則持續遊玩至倒數結束。
- 根據最終分數顯示四種不同結局：

| 最終分數 | 結局 |
|---|---|
| 小於 0 分 | GAME OVER |
| 0～29 分 | 再接再厲！ |
| 30～99 分 | 很棒～！ |
| 100 分以上 | 超級棒～！ |

- 依據不同結局切換自行繪製的背景插畫，並顯示最終分數。
- 加入開始與重新遊玩按鈕，重新遊玩時透過 `SceneManager.LoadScene()` 載入 `MainScene`。
- 遊戲開始前及結束後隱藏 Score、Timer，讓不同遊戲階段的畫面更加明確。

### 4.6 關卡難度調整

為了避免遊戲從開始到結束都維持相同難度，將 60 秒遊戲設計成三個階段，依據經過時間調整物品生成速度、掉落速度及炸彈比例。

| 遊戲時間 | Interval | Speed | BombRate |
|---|---:|---:|---:|
| 0～20 秒 | 1.2 秒 | 3 | 20% |
| 20～40 秒 | 0.9 秒 | 4.5 | 25% |
| 40～60 秒 | 0.6 秒 | 6 | 35% |

**設計原因：**

- 前 20 秒降低物品產生頻率與炸彈比例，讓玩家先熟悉籐籃移動和接取方式。
- 中間 20 秒提高掉落速度、縮短生成間隔，增加玩家需要處理的物品數量。
- 最後 20 秒進一步提高掉落速度與炸彈比例，使玩家必須更快速判斷物品種類及移動位置。
- 透過三階段難度變化，讓遊戲從熟悉操作逐漸轉為反應速度與判斷能力的挑戰。

### 4.7 加入粒子特效與音效

- 使用 Unity Particle System 製作 `AppleBurstEffect` 與 `BombExplosionEffect`。
- 蘋果被接住或碰到地面時，產生向外散開的紅色粒子效果。
- 炸彈被接住或碰到地面時，產生向外擴散的爆炸粒子效果。
- 使用 AudioSource 播放背景音樂，並依照遊戲開始、進行中及結束階段控制音樂播放。
- 為接到蘋果、接到炸彈、物品落地及按鈕點擊設定不同音效。
- 物品銷毀時使用共用碎裂方法觸發特效，並在落地時額外播放對應聲音，避免音效重複觸發。

### 4.8 製作開始畫面與自訂結局插畫

- 建立 `StartPanel`，顯示遊戲名稱、玩法說明與開始按鈕。
- 建立 `HUDPanel`，在遊戲正式開始後顯示分數與倒數時間。
- 建立 `ResultPanel`，於遊戲結束時顯示結局標題、最終分數及重新遊玩按鈕。
- 自行繪製四張不同結局的背景插畫，並由 `Director.cs` 根據最終分數自動切換。
- 使用 TextMeshPro 的 SDF 字型顯示繁體中文，調整字型 Atlas 設定，修正部分中文字無法顯示的問題。

### 4.9 Android 平台設定與匯出

- 在 Unity Hub 安裝 Android Build Support、Android SDK & NDK Tools 及 OpenJDK。
- 使用 File → Build Profiles 將目標平台切換為 Android，並將 `MainScene` 加入 Scene List。
- 將遊戲設定為橫向顯示，配合原本的橫向九宮格遊戲介面。
- 因 `BasketController.cs` 使用舊版 Input API，將 Active Input Handling 設為 Input Manager (Old)，並將 EventSystem 的輸入模組改為 Standalone Input Module。
- 重新設定 TextMeshPro 字型來源，解決平台切換後中文字出現方框的問題。
- 使用 Build Profiles 匯出 Android APK 安裝檔，供 Android 裝置安裝與測試。

## 5. 額外實作功能

除了老師指定的基本要求，本次另外增加以下功能：

1. **三階段動態難度**：依遊戲經過時間自動調整 Interval、Speed、BombRate。
2. **四種成績結局**：依分數顯示不同結局標題與自製背景插畫。
3. **負分立即結束**：增加遊戲挑戰性，不必等到倒數結束。
4. **粒子碎裂與爆炸**：蘋果、炸彈在被接取或落地時產生不同視覺效果。
5. **多種音效與背景音樂**：包含開始、接取、落地、結束和重新遊玩等互動聲音。
6. **開始及重新遊玩介面**：提供玩法說明與完整的重玩流程。
7. **Android APK 匯出**：讓遊戲除了 Unity 編輯器之外，也能部署到 Android 平台。
